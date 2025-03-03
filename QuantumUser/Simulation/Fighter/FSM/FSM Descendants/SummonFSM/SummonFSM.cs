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
        public EntityRef playerOwnerEntity;
        
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
                .Permit(SummonTrigger.Summoned, SummonState.Unpooled)
                .OnEntry(OnPooled);

            // Fsm.Configure(SummonState.Unpooled)
            //     .OnEntry(Test);
            
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

        }
        
        public override EntityRef GetPlayer()
        {
            return playerOwnerEntity;
        }

        public void OnPooled(TriggerParams? triggerParams)
        {
            Debug.Log("OnPooled");
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            var f = frameParam.f;
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Position = new FPVector2(0, -20).XYO;
        }
    }

}