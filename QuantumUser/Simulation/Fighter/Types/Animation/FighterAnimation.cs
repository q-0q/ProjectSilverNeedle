using System;
using Photon.Deterministic.Protocol;
using UnityEngine;

namespace Quantum.Types
{
    public unsafe class FighterAnimation
    {
        public SectionGroup<int> SectionGroup;
        public string Path;

        public virtual void SetAnimationFrameForFsm(Frame f, FSM fsm)
        {
            int frame;
            try
            {
                frame = SectionGroup.GetCurrentItem(f, fsm);
            }
            catch (Exception e)
            {
                Debug.LogError("SetAnimation failed for state: " + InheritableEnum.InheritableEnum.GetFieldNameByValue(fsm.Fsm.State(), fsm.StateType));
                throw;
            }
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame;
        }
    }
}