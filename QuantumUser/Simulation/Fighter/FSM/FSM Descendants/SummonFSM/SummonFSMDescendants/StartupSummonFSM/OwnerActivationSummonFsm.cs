using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public abstract class OwnerActivationSummonFsm : SummonFSM
    {
        public class OwnerActivationSummonState : SummonState
        {

        }
        
        public class OwnerActivationSummonTrigger : SummonTrigger
        {
            
        }
        
        protected Dictionary<(int, int), int> OwnerActivationTriggers;

        public OwnerActivationSummonFsm()
        {
            OwnerActivationTriggers = new Dictionary<(int, int), int>();
        }
        
        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
        }

        public override void SetupMachine()
        {
            base.SetupMachine();



        }
        
        public override void HandleSummonFSMTriggers(Frame f)
        {

            var ownerPlayerFsm = FsmLoader.FSMs[playerOwnerEntity];
            var key = (ownerPlayerFsm.Fsm.State(), ownerPlayerFsm.FramesInCurrentState(f));
            
            if (OwnerActivationTriggers.TryGetValue(key, out var frame))
            {
                Fsm.Fire(frame, new FrameParam() { f = f, EntityRef = EntityRef});
            }
            
            base.HandleSummonFSMTriggers(f);

        }
    }
}



        
