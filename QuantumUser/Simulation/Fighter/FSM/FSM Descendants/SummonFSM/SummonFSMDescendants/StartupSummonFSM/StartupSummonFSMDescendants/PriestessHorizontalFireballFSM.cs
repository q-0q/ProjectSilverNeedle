
using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe class PriestessHorizontalFireballFSM : OwnerActivationSummonFsm
    {
        
        public class PriestessHorizontalFireballState : OwnerActivationSummonState
        {
            public static int Startup;
            public static int Active;
            public static int Destroy;
        }
        
        public class PriestessHorizontalFireballTrigger : OwnerActivationSummonTrigger
        {
            public static int OwnerStartupComplete;
        }

        private int ReturnStartup = 16;
        
        public PriestessHorizontalFireballFSM()
        {
            Name = "PriestessHorizontalFireball";
            StateType = typeof(PriestessHorizontalFireballState);
            KinematicAttachPointOffset = FPVector2.Zero;
            SummonPositionOffset = new FPVector2(FP.FromString("4.25"), FP.FromString("3.5"));

            OwnerActivationFrameTriggers[(PriestessFSM.PriestessState._5H, 21)] =
                PriestessHorizontalFireballTrigger.OwnerStartupComplete;
        }
        

        
        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            const int lifeSpan = 110;
            
            StateMapConfig.HitSectionGroup.Dictionary[PriestessHorizontalFireballState.Active] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new (1, null),
                    new(3, new Hit()
                    {
                        // Launches = true,
                        Level = 0,
                        Projectile = true,
                        HitPushback = 0,
                        BlockPushback = 0,
                        // GroundBounce = true,
                        TrajectoryHeight = 1,
                        TrajectoryXVelocity = -1,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new(3, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowWidth = false,
                                            GrowHeight = false,
                                            PosX = 0,
                                            PosY = 1,
                                            Height = 3,
                                            Width = 3,
                                        }
                                    }
                                })
                            }
                        }
                    }),
                }
            };
            


            
            
            var startupAnimation = new FighterAnimation()
            {
                Path = "Startup",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 9,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(startupAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessHorizontalFireballState.Startup] = startupAnimation;
            
            
            
            var activeAnimation = new FighterAnimation()
            {
                Path = "Active",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 3,
                    AutoFromAnimationPath = true
                }
            };
            
            var activeMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(5, FP.FromString("5.5")),
                    new(10, FP.FromString("4.5"))
                }
            };
            
            Util.AutoSetupFromAnimationPath(activeAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessHorizontalFireballState.Active] = activeAnimation;
            StateMapConfig.MovementSectionGroup.Dictionary[PriestessHorizontalFireballState.Active] = activeMovement;
            StateMapConfig.Duration.Dictionary[PriestessHorizontalFireballState.Active] = 30;
            
            
            var destroyAnimation = new FighterAnimation()
            {
                Path = "Destroy",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 6,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(destroyAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessHorizontalFireballState.Destroy] = destroyAnimation;
            StateMapConfig.Duration.Dictionary[PriestessHorizontalFireballState.Destroy] = 12;

            

        }

        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, PriestessHorizontalFireballState.Startup);
                
            Fsm.Configure(PriestessHorizontalFireballState.Startup)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, PriestessHorizontalFireballState.Destroy)
                .PermitIf(SummonTrigger.OwnerCollided, SummonState.Pooled, IsOwnerInCorrectStateToSnuff)
                .Permit(PriestessHorizontalFireballTrigger.OwnerStartupComplete, PriestessHorizontalFireballState.Active);

            Fsm.Configure(PriestessHorizontalFireballState.Active)
                .SubstateOf(SummonState.Unpooled)
                .Permit(Trigger.Finish, PriestessHorizontalFireballState.Destroy)
                .PermitIf(SummonTrigger.OwnerCollided, SummonState.Pooled, IsOwnerInCorrectStateToSnuff)
                .Permit(SummonTrigger.Collided, PriestessHorizontalFireballState.Destroy);
            
            Fsm.Configure(PriestessHorizontalFireballState.Destroy)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.Summoned, PriestessHorizontalFireballState.Startup)
                .Permit(Trigger.Finish, SummonState.Pooled);

        }

        private bool IsOwnerInCorrectStateToSnuff(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam frameParam) return false;
            if (GetPlayerFsm() is not PriestessFSM priestessFsm) return false;
            return priestessFsm.IsNotWhiffedFromHit(frameParam.f, priestessFsm._5HHit.LookupId);
        }
        
//         protected override void SummonMove(Frame f)
//         {
// base.S
//         }

        
        
    }
}



        
