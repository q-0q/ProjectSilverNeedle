using Photon.Deterministic;
using UnityEngine;

namespace Quantum.Types
{
    public unsafe class RisingTrajectoryAnimation : TrajectoryAnimation
    {
        public override void SetSpriteForFsm(Frame f, PlayerFSM fsm)
        {
            FP percentage = GetPercentage(f, fsm, TrajectoryType.Rise);
            int frame = SectionGroup.GetItemFromPercentage(percentage);
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame + SpriteSheetOffset;
        }
    }
}