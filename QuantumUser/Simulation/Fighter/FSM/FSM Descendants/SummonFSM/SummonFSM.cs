using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public abstract unsafe partial class SummonFSM : FSM
    {
        public class SummonState : FSMState
        {
            public static int Pooled;
            public static int Unpooled;

        }
        
        public class SummonTrigger : Trigger
        {
            public static int Summoned;
            public static int HitTarget;
            public static int GotBlocked;
            public static int Offscreen;
        }
        
        public SummonFSM()
        {
            int currentState = SummonState.Pooled;
            Fsm = new Machine<int, int>(currentState);
        }
        
        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, SummonState.Unpooled);

        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

        }
    }

}