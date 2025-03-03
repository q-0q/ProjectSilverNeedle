using System;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMSReportSystem : SystemMainThreadFilter<PlayerFSMSReportSystem.Filter>
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
            
            fsm.UpdateDirection(f);
            fsm.TrajectoryArc(f);
            fsm.Animation(f);
            fsm.ReportFrameMeterType(f);
            
            fsm.IncrementClock(f, filter.Entity);
            Util.WritebackFsm(f, filter.Entity);
        }
        





        
    }
}