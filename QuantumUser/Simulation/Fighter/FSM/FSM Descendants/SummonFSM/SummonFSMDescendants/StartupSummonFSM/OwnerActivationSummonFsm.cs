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
        
        protected Dictionary<(int, int), int> OwnerActivationFrameTriggers;
        protected Dictionary<int, (int, int)> OwnerActivationMaxFrameTriggers;

        public OwnerActivationSummonFsm()
        {
            OwnerActivationFrameTriggers = new Dictionary<(int, int), int>();
            OwnerActivationMaxFrameTriggers = new Dictionary<int, (int, int)>();
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
            var state = ownerPlayerFsm.Fsm.State();
            var framesInCurrentState = ownerPlayerFsm.FramesInCurrentState(f);
            var key = (state, framesInCurrentState);
            
            if (OwnerActivationFrameTriggers.TryGetValue(key, out var t1))
            {
                Fsm.Fire(t1, new FrameParam() { f = f, EntityRef = EntityRef});
            }
            if (OwnerActivationMaxFrameTriggers.TryGetValue(state, out var t2))
            {
                if (framesInCurrentState <= t2.Item1)
                    Fsm.Fire(t2.Item2, new FrameParam() { f = f, EntityRef = EntityRef});
            }
            
            base.HandleSummonFSMTriggers(f);

        }
    }
}


// var framesInCurrentState = ownerPlayerFsm.FramesInCurrentState(f);
//
// foreach (var kvp in OwnerActivationTriggers)
// {
//     if (framesInCurrentState >= kvp.Key.Item2)
//         Fsm.Fire(kvp.Value, new FrameParam() { f = f, EntityRef = EntityRef});
// }
//

        
