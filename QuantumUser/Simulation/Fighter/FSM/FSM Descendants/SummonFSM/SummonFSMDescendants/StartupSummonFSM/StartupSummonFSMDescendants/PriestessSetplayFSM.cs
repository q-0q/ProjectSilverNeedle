
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
    public unsafe class PriestessSetplayFSM : OwnerActivationSummonFsm
    {
        
        public class PriestessSetplayState : OwnerActivationSummonState
        {
            public static int Tracking;
            public static int Startup;
            public static int Alive;
            public static int Return;
        }
        
        public class PriestessSetplayTrigger : OwnerActivationSummonTrigger
        {
            public static int OwnerStartupComplete;
            public static int OwnerCallUsed;
        }
        
        
        public PriestessSetplayFSM()
        {
            Name = "PriestessSetplay";
            StateType = typeof(PriestessSetplayState);
            KinematicAttachPointOffset = FPVector2.Zero;
            SummonPositionOffset = new FPVector2(FP.FromString("5.75"), FP.FromString("7.75"));

            OwnerActivationTriggers[(PriestessFSM.PriestessState.Summon, 10)] =
                PriestessSetplayTrigger.OwnerStartupComplete;
            
            OwnerActivationTriggers[(PriestessFSM.PriestessState.Summon, 1)] =
                PriestessSetplayTrigger.OwnerCallUsed;
        }
        

        
        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            const int lifeSpan = 40;
            
            StateMapConfig.HitSectionGroup.Dictionary[PriestessSetplayState.Alive] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new (3, null),
                    new(3, new Hit()
                    {
                        // Launches = true,
                        Level = 1,
                        Projectile = true,
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
                                            PosY = 0,
                                            Height = 2,
                                            Width = 2,
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new (1, null),
                }
            };
            
            
            var startupAnimation = new FighterAnimation()
            {
                Path = "Startup",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 4,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(startupAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Startup] = startupAnimation;
            
            
            var trackingAnimation = new FighterAnimation()
            {
                Path = "Tracking",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(trackingAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Tracking] = trackingAnimation;
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Tracking] = 10;
            
            
            var aliveAnimation = new FighterAnimation()
            {
                Path = "Alive",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 6,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(aliveAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Alive] = aliveAnimation;
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Alive] = lifeSpan;
            
            var returnAnimation = new FighterAnimation()
            {
                Path = "Return",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 1,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(returnAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Return] = returnAnimation;
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Return] = 30;

            

        }

        public override void SetupMachine()
        {
            base.SetupMachine();
            
            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, PriestessSetplayState.Startup);

            
            Fsm.Configure(PriestessSetplayState.Startup)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, SummonState.Pooled)
                .Permit(PriestessSetplayTrigger.OwnerStartupComplete, PriestessSetplayState.Tracking);

            Fsm.Configure(PriestessSetplayState.Tracking)
                .OnEntry(SnapToOpponent)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, SummonState.Pooled)
                .Permit(Trigger.Finish, PriestessSetplayState.Alive);
            
            Fsm.Configure(PriestessSetplayState.Alive)
                .SubstateOf(SummonState.Unpooled)
                .Permit(Trigger.Finish, SummonState.Pooled)
                .Permit(PriestessSetplayTrigger.OwnerCallUsed, PriestessSetplayState.Return);

            Fsm.Configure(PriestessSetplayState.Return)
                .SubstateOf(SummonState.Unpooled)
                .Permit(Trigger.Finish, SummonState.Pooled);

        }
        
        //TriggerParams? triggerParams
        private void SnapToOpponent(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam param)
            {
                Debug.LogError("errrrr");
                return;
            };
            var otherPlayerEntity = Util.GetOtherPlayer(param.f, playerOwnerEntity);
            param.f.Unsafe.TryGetPointer<Transform3D>(otherPlayerEntity, out var otherPlayerTransform);
            var pos = new FPVector3(otherPlayerTransform->Position.X, Util.Max(otherPlayerTransform->Position.Y, 3), 0);
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Teleport(param.f, pos);
        }
        
        protected override void SummonMove(Frame f)
        {
            if (Fsm.IsInState(PriestessSetplayState.Startup)) SnapToOwnerPosWithOffset(f);
        }

        
        
    }
}



        
