using System;
using Photon.Deterministic.Protocol;
using UnityEngine;

namespace Quantum.Types
{
    public unsafe class FighterAnimation
    {
        public SectionGroup<int> SectionGroup;
        public int Path;

        public virtual void SetAnimationPathForFsm(Frame f, FSM fsm)
        {
            int frame = SectionGroup.GetCurrentItem(f, fsm);
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame;
            // Debug.Log(Path);
            animationData->path = Path;
        }
    }
}