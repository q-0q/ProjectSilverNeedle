using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        public void Animation(Frame f)
        {
            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            FighterAnimation currentAnimation = character.FighterAnimation.Get(this);
            if (currentAnimation is null) return;
            currentAnimation.SetAnimationPathForFsm(f, this);
        }
    }
}