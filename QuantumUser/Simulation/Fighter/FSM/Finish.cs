using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class FSM
    {
        public void DoFinish(Frame f)
        {
            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            var frameParam = new FrameParam()
            {
                f = f,
                EntityRef = EntityRef,
            };
            int duration = character.Duration.Get(this, frameParam);
            if (FramesInCurrentState(f) >= duration)
            {
                var param = new CollisionHitParams() { f = f, EntityRef = EntityRef };
                Fsm.Fire(PlayerFSM.PlayerTrigger.Finish, param);
            }
        }
        
        // public void CheckForOpponentThrowTech(Frame f)
        // {
        //     EntityRef otherPlayerEntityRef = Util.GetOtherPlayer(f, EntityRef);
        //     var opponentFsm = Util.GetPlayerFSM(f, otherPlayerEntityRef);
        //     if (opponentFsm is null) return;
        //
        //     if (opponentFsm.Fsm.IsInState(State.ThrowTech))
        //     {
        //         var frameParam = new FrameParam() { f = f, EntityRef = EntityRef };
        //         Fsm.Fire(Trigger.ThrowTech, frameParam);
        //     }
        //
        // }
    }
}