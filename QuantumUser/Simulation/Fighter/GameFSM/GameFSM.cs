using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe class GameFSM
    {
        public enum State
        {
            Waiting,
            Loading,
            Ready,
            Countdown,
            Playing,
            RoundEnd,
            RoundResetting,
            GameEnd,
            CanUpdateCharacter,
        }

        public enum Trigger
        {
            PlayerJoin,
            Started,
            Finish,
            PlayerDeath,
            NewCharacterSelected,
        }
        
        public EntityRef EntityRef;
        public Machine<State, Trigger> Fsm;

        public GameFSM(int currentStateInt, EntityRef entityRef)
        {
            State currentState = (State)currentStateInt;
            EntityRef = entityRef;
            Fsm = new Machine<State, Trigger>(currentState);
            Fsm.OnTransitionCompleted(OnStateChanged);


            Fsm.Configure(State.CanUpdateCharacter)
                .Permit(Trigger.NewCharacterSelected, State.Loading);

            Fsm.Configure(State.Waiting)
                .PermitIf(Trigger.PlayerJoin, State.Loading, GameHasTwoPlayers);
            
            Fsm.Configure(State.Loading)
                .Permit(Trigger.Finish, State.Ready)
                .OnEntry(GameFSMSystem.OnLoading);

            Fsm.Configure(State.Ready)
                .Permit(Trigger.Started, State.Countdown) // Change to destination: State.Countdown
                .SubstateOf(State.CanUpdateCharacter);

            Fsm.Configure(State.Countdown)
                .Permit(Trigger.Finish, State.Playing)
                .OnEntry(GameFSMSystem.OnCountdown)
                .SubstateOf(State.CanUpdateCharacter);
            
            Fsm.Configure(State.Playing)
                .Permit(Trigger.PlayerDeath, State.RoundEnd)
                .SubstateOf(State.CanUpdateCharacter);

            Fsm.Configure(State.RoundEnd)
                .Permit(Trigger.Finish, State.RoundResetting)
                .OnEntry(GameFSMSystem.OnRoundEnd)
                .SubstateOf(State.CanUpdateCharacter);

            Fsm.Configure(State.RoundResetting)
                .Permit(Trigger.Finish, State.Countdown)
                .OnEntry(GameFSMSystem.ResetAllFsmData)
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
            
            GameFsmLoader.WritebackGameFSM(param.f);
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