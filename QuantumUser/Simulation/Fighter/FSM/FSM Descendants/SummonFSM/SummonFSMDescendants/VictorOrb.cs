using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public class VictorOrbFsm : SummonFSM
    {
        public class VictorOrbState : SummonState
        {
            public static int Visible;
            public static int Invisible;
        }
        
        public class VictorOrbTrigger : SummonTrigger
        {
            public static int Hide;
            public static int Show;
        }
        
        public VictorOrbFsm()
        {
            Name = "VictorOrb";
            StateType = typeof(VictorOrbState);
            KinematicAttachPointOffset = FPVector2.Zero;
            SummonPositionOffset = new FPVector2(FP.FromString("3.5"), FP.FromString("4"));
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
            
            var visibleAnimation = new FighterAnimation()
            {
                Path = "Visible",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(visibleAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[VictorOrbState.Visible] = visibleAnimation;
            
            var invisibleAnimation = new FighterAnimation()
            {
                Path = "Invisible",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(invisibleAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[VictorOrbState.Invisible] = invisibleAnimation;
            
        }

        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, VictorOrbState.Visible);

            Fsm.Configure(VictorOrbState.Invisible)
                .SubstateOf(SummonState.Unpooled)
                .Permit(VictorOrbTrigger.Show, VictorOrbState.Visible);

            
            Fsm.Configure(VictorOrbState.Visible)
                .SubstateOf(SummonState.Unpooled)
                .Permit(VictorOrbTrigger.Hide, VictorOrbState.Invisible);
            
        }

        public override void HandleSummonFSMTriggers(Frame f)
        {
            if (FsmLoader.FSMs[playerOwnerEntity] is not StickTwoFSM stickTwoFsm)
            {
                Debug.LogError("VictorOrb owner is not StickTwoFsm");
                return;
            }
            
            Fsm.Fire(stickTwoFsm.Fsm.IsInState(StickTwoFSM.StickTwoState.Rekka1) 
                     || stickTwoFsm.Fsm.IsInState(StickTwoFSM.StickTwoState.Rekka2A) ? VictorOrbTrigger.Hide : VictorOrbTrigger.Show);
        }
        
        protected override void SummonMove(Frame f)
        {
            
        }
    }
}



        
