using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public abstract class StartupSummonFSM : SummonFSM
    {
        public class StartupSummonState : SummonState
        {
            public static int Startup;
            public static int Alive;
        }
        
        public class StartupSummonTrigger : SummonTrigger
        {
            public static int OwnerActivated;
        }
        
        public static int OwnerActivationFrame = -1;
        
        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
        }

        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, StartupSummonState.Startup);
            
            Fsm.Configure(StartupSummonState.Startup)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, SummonState.Pooled)
                .Permit(StartupSummonTrigger.OwnerActivated, StartupSummonState.Alive);

            Fsm.Configure(StartupSummonState.Alive)
                .SubstateOf(SummonState.Unpooled);

        }
        
        protected override void SummonMove(Frame f)
        {
            if (Fsm.IsInState(StartupSummonState.Startup)) SnapToOwnerPosWithOffset(f);
        }
        
        public override void HandleSummonFSMTriggers(Frame f)
        {

            var ownerPlayerFsm = FsmLoader.FSMs[playerOwnerEntity];
            if (ownerPlayerFsm.FramesInCurrentState(f) == OwnerActivationFrame)
            {
                Fsm.Fire(StartupSummonTrigger.OwnerActivated, new FrameParam() { f = f, EntityRef = EntityRef});
            }
            
            base.HandleSummonFSMTriggers(f);

        }
    }
}



        
