using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe class GameFSM
    {
        public enum State
        {
            Waiting,
            Ready,
            Countdown,
            Playing,
            RoundEnd,
            RoundResetting,
            GameEnd
        }

        public enum Trigger
        {
            PlayerJoin,
            Started,
            Finish,
            PlayerDeath,
        }
        
        public EntityRef EntityRef;
        public Machine<State, Trigger> Fsm;

        public GameFSM(int currentStateInt, EntityRef entityRef)
        {
            State currentState = (State)currentStateInt;
            EntityRef = entityRef;
            Fsm = new Machine<State, Trigger>(currentState);
            Fsm.OnTransitioned(OnStateChanged);

            Fsm.Configure(State.Waiting)
                .PermitIf(Trigger.PlayerJoin, State.Ready, GameHasTwoPlayers);

            Fsm.Configure(State.Ready)
                .Permit(Trigger.Started, State.Countdown) // Change to destination: State.Countdown
                .OnEntry(GameFSMSystem.OnReady);

            Fsm.Configure(State.Countdown)
                .Permit(Trigger.Finish, State.Playing)
                .OnEntry(GameFSMSystem.OnCountdown);
            
            Fsm.Configure(State.Playing)
                .Permit(Trigger.PlayerDeath, State.RoundEnd);

            Fsm.Configure(State.RoundEnd)
                .Permit(Trigger.Finish, State.RoundResetting)
                .OnEntry(GameFSMSystem.OnRoundEnd);

            Fsm.Configure(State.RoundResetting)
                .Permit(Trigger.Finish, State.Countdown)
                .OnEntry(GameFSMSystem.ResetPlayers)
                .OnEntry(GameFSMSystem.ResetUnityView);
        }

        private bool GameHasTwoPlayers(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            var frameParam = (FrameParam)triggerParams;
            var count = 0;
            foreach (var _ in frameParam.f.GetComponentIterator<PlayerLink>())
            {
                count++;
            }
            return count == 2;
        }
        
        private void OnStateChanged(TriggerParams? triggerParams)
        {
            if (triggerParams == null)
            {
                return;
            }
            var param = (FrameParam)triggerParams;
            param.f.Unsafe.TryGetPointer<GameFSMData>(EntityRef, out var gameFsmData);
            gameFsmData->framesInState = 0;
        }

        public int FramesInState(Frame f)
        {
            f.Unsafe.TryGetPointer <GameFSMData>(EntityRef, out var gameFsmData);
            return gameFsmData->framesInState;
        }
    }
}