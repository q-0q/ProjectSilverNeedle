

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public unsafe partial class JumpGameFXSummonFSM : GameFXSummonFSM
    {
        
        public JumpGameFXSummonFSM()
        {
            SummonPositionOffset = new FPVector2(0, -2);
            Name = "GameFXJump";
        }
        
        public override void SetupMachine()
        {
            base.SetupMachine();
            
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            var animation = new FighterAnimation()
            {
                Path = "Jump",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 3,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(animation, this);

            StateMapConfig.FighterAnimation.Dictionary[SummonState.Unpooled] = animation;
            StateMapConfig.Duration.Dictionary[SummonState.Unpooled] = animation.SectionGroup.Duration();
        }
  

    }
}