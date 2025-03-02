using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.StateMap;
using Quantum.Types;
using Wasp;

namespace Quantum
{
    public abstract unsafe partial class FSM
    {
        public class FSMState : InheritableEnum.InheritableEnum { }
        public class Trigger : InheritableEnum.InheritableEnum { }
        
        public Type AnimationPathsEnum;
        public FPVector2 KinematicAttachPointOffset;
        public string Name;

        
        public EntityRef EntityRef;
        public Machine<int, int> Fsm;
        public StateMapConfig StateMapConfig;
        public Type StateType;
        
        public Dictionary<int, Cutscene> Cutscenes;


        public virtual void SetupStateMaps()
        {
        }

        public virtual void SetupMachine()
        {
        }

        protected void ResetStateEnteredFrame(Frame f)
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