using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMSCollisionBoxSystem : SystemMainThreadFilter<PlayerFSMSCollisionBoxSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public FSMData* PlayerFsmData;

        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            AdvanceFsm(f, filter);
        }

        private static void AdvanceFsm(Frame f, Filter filter)
        {
            var fsm = Util.GetFSM(f, filter.Entity);
            if (fsm is null) return;
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            if (fsm.IsOnFirstFrameOfHit(f))
            {
                Debug.Log("clearing hit entities f: " + f.Number);
                fsm.ClearHitEntities(f);
            }
            
            // Write collision state
            filter.PlayerFsmData->currentCollisionState = fsm.Fsm.State();
            filter.PlayerFsmData->collisionFramesInState = fsm.FramesInCurrentState(f);
            
            Util.WritebackFsm(f, filter.Entity);
        }


        
    }
}