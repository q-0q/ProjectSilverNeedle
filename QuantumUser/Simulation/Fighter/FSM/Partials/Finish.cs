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
            GameFSM gameFsm = GameFsmLoader.LoadGameFSM(f);
            if (!gameFsm.Fsm.IsInState(GameFSM.State.Playing) && !gameFsm.Fsm.IsInState(GameFSM.State.RoundEnd)) return;
                
            var frameParam = new FrameParam()
            {
                f = f,
                EntityRef = EntityRef,
            };
            int duration = StateMapConfig.Duration.Get(this, frameParam);
            
            // if (duration <= 0)
            // {
            //     Debug.LogError("State " + InheritableEnum.InheritableEnum.GetFieldNameByValue(Fsm.State(), 
            //         StateType) + " has duration " + duration);
            // }
            
            if (FramesInCurrentState(f) >= duration)
            {
                var param = new CollisionHitParams() { f = f, EntityRef = EntityRef };
                
                Fsm.Fire(PlayerFSM.PlayerTrigger.Finish, param);
            }
        }
    }
}