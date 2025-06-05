

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
    public unsafe partial class LandGameFXSummonFSM : GameFXSummonFSM
    {
        
        public LandGameFXSummonFSM()
        {
            SummonPositionOffset = new FPVector2(0, 0);
            Name = "GameFXLand";
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
                Path = "Land",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 5,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(animation, this);

            StateMapConfig.FighterAnimation.Dictionary[SummonState.Unpooled] = animation;
            StateMapConfig.Duration.Dictionary[SummonState.Unpooled] = animation.SectionGroup.Duration();
        }
  

    }
}