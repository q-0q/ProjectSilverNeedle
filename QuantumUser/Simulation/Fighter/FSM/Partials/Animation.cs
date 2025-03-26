using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class FSM
    {
        public void Animation(Frame f)
        {
            FighterAnimation currentAnimation = StateMapConfig.FighterAnimation.Get(this);
            if (currentAnimation is null) return;
            currentAnimation.SetAnimationFrameForFsm(f, this);
        }
    }
}