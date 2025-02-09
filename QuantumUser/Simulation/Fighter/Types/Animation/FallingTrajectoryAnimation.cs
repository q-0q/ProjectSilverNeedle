using Photon.Deterministic;
using UnityEngine;

namespace Quantum.Types
{
    public unsafe class FallingTrajectoryAnimation : TrajectoryAnimation
    {
        
        public override void SetAnimationPathForFsm(Frame f, PlayerFSM fsm)
        {
            int frame = fsm.FramesInCurrentState(f);
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame;
            animationData->path = Path;
        }
    }
}