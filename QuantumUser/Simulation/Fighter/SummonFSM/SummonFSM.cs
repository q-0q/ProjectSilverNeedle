using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public unsafe partial class SummonFSM
    {
        
        public class State : InheritableEnum.InheritableEnum
        {
            public static int Pooled;
        }

        // When adding a new trigger
        // 1. Add as an enum here
        // 2. Create a TriggerWithParameter<Frame> field and initialize in ctor
        public enum Trigger
        {
            Summoned,
            HitSomething,
            GotBlocked,
            ButtonAndDirection,
            Finish,
            Offscreen,
        }

        public EntityRef EntityRef;
        public Machine<int, Trigger> Fsm;
        
        
        public SummonFSM()
        {
            int currentState = State.Pooled;
            Fsm = new Machine<int, Trigger>(currentState);
            ConfigureBaseFsm(Fsm);
        }

        
        public void ConfigureBaseFsm(Machine<int, Trigger> machine)
        {
            machine.OnTransitionCompleted(OnStateChanged);
            
            
            
        }
        
        
        private void OnStateChanged(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            ResetStateEnteredFrame(param.f);
            PlayerDirectionSystem.ForceUpdatePlayerDirection(param.f, EntityRef);
            
            Util.WritebackFsm(param.f, EntityRef);
        }
        

        private void ResetStateEnteredFrame(Frame f)
        {
            f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var playerFsmData);
            playerFsmData->framesInState = 0;
            playerFsmData->virtualTimeInState = 0;
        }

        public int FramesInCurrentState(Frame f)
        {
            f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var playerFsmData);
            return Util.FramesFromVirtualTime(playerFsmData->virtualTimeInState);
        }
        
        public static int FramesInCurrentState(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<FSMData>(entityRef, out var playerFsmData);
            return Util.FramesFromVirtualTime(playerFsmData->virtualTimeInState);
        }
        
        
    }
}