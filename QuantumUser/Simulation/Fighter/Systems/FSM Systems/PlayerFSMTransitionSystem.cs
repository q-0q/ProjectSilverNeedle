using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMTransitionSystem : SystemMainThreadFilter<PlayerFSMTransitionSystem.Filter>
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
            FSM fsm = Util.GetFSM(f, filter.Entity);
            if (fsm is null) return;
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            // Handle necessary state transitions
            fsm.DoFinish(f);
            fsm.CheckForLand(f);
            // fsm.CheckForOpponentThrowTech(f);
            InputSystem.FireFsmFromInput(f, fsm);
            
            Util.WritebackFsm(f, filter.Entity);
        }
        
    }
}