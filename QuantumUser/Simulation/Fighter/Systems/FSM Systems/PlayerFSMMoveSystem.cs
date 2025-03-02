using System;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMSMoveSystem : SystemMainThreadFilter<PlayerFSMSMoveSystem.Filter>
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
            PlayerFSM fsm = Util.GetFSM(f, filter.Entity);
            if (fsm is null) return;
            
            
            fsm.Move(f);
            
            Util.WritebackFsm(f, filter.Entity);
        }
        
    }
}