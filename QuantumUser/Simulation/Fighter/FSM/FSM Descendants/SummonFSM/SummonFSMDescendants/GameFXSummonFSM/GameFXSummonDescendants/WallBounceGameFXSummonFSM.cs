

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
    public unsafe partial class WallBounceGameFXSummonFSM : GameFXSummonFSM
    {
        
        public WallBounceGameFXSummonFSM()
        {
            SummonPositionOffset = new FPVector2(FP.FromString("-0.5"), FP.FromString("1.75"));
            Name = "GameFXWallBounce";
            IgnoreHitstop = true;
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
                Path = "WallBounce",
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