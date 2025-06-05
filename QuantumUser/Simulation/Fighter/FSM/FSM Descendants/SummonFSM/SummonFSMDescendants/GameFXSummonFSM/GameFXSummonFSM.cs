
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
    public abstract unsafe partial class GameFXSummonFSM : SummonFSM
    {
        
        
        public GameFXSummonFSM()
        {
            StateType = typeof(SummonState);
            sendToBack = true;
        }
        
        
        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, SummonState.Unpooled);
            
            Fsm.Configure(SummonState.Unpooled)
                .Permit(Trigger.Finish, SummonState.Pooled);
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
            
        }
  

    }
}