using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class FSMSystemB : SystemMainThreadFilter<FSMSystemB.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public FSMData* FsmData;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            FsmLoader.ReadAllFSMsFromNetwork(f);
            FSM fsm = FsmLoader.FSMs[filter.Entity];
            if (fsm is null) return;
            
            if (!fsm.IsTimeStopped(f))
            {
                // Receive collisions
                fsm.HitboxHurtboxCollide(f);

                // Animation
                fsm.Animation(f);
            }
            
            // Report frame meter
            if (!HitstopSystem.IsHitstopActive(f))
                fsm.ReportFrameMeterType(f);
            
            
            if (!fsm.IsTimeStopped(f))
            {
                // unpool summons
                fsm.UnpoolSummon(f);
                
                // Clock
                FP virtualTimeIncrement = Util.FrameLengthInSeconds * fsm.GetSlowdownMod(f, filter.Entity);
                fsm.IncrementClockByAmount(f, filter.Entity, virtualTimeIncrement);
            }
            
            // Done!
            FsmLoader.WriteAllFSMsToNetwork(f);
            
        }
        
    }
}