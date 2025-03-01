using System;
using Wasp;

namespace Quantum
{
    public abstract unsafe partial class FSM
    {
        public class FSMState : InheritableEnum.InheritableEnum { }
        public class Trigger : InheritableEnum.InheritableEnum { }
        
        public EntityRef EntityRef;
        public Machine<int, int> Fsm;
        
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