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
            int path = f.Get<AnimationData>(fsm.EntityRef).path;
            var pathEnum = fsm.AnimationPathsEnum;
            string stringPath = Enum.ToObject(pathEnum, path).ToString();            
            Debug.Log(stringPath);
            int frame = SectionGroup.GetCurrentItem(f, fsm);
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame;
            animationData->path = Path;
        }
    }
}