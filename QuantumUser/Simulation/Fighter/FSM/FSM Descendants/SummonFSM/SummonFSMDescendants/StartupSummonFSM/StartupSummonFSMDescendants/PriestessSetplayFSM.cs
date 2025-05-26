
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
            // public static int Startup;
            public static int Alive;
            public static int Return;
            public static int Teleport;
            public static int Active;
            public static int Destroy;
        }
        
        public class PriestessSetplayTrigger : OwnerActivationSummonTrigger
        {
            public static int OwnerStartupComplete;
            public static int OwnerCallUsed;
            public static int Teleport;
            public static int ReturnComplete;
        }

        private int ReturnStartup = 16;
        
        public PriestessSetplayFSM()
        {
            Name = "PriestessSetplay";
            StateType = typeof(PriestessSetplayState);
            KinematicAttachPointOffset = FPVector2.Zero;
            DisableUnpoolOwnerSnap = true;
            SummonPositionOffset = new FPVector2(FP.FromString("5.75"), FP.FromString("-1"));

            OwnerActivationFrameTriggers[(PriestessFSM.PriestessState.SummonHigh, 10)] =
                PriestessSetplayTrigger.OwnerStartupComplete;
            
            OwnerActivationFrameTriggers[(PriestessFSM.PriestessState.SummonLow, 10)] =
                PriestessSetplayTrigger.OwnerStartupComplete;
            
            OwnerActivationMaxFrameTriggers[PriestessFSM.PriestessState.Return] =
                (12, PriestessSetplayTrigger.OwnerCallUsed);
            
            OwnerActivationMaxFrameTriggers[PriestessFSM.PriestessState.AirReturn] =
                (12, PriestessSetplayTrigger.OwnerCallUsed);
            
            OwnerActivationMaxFrameTriggers[PriestessFSM.PriestessState.Teleport] =
                (12, PriestessSetplayTrigger.Teleport);
            
            OwnerActivationMaxFrameTriggers[PriestessFSM.PriestessState.AirTeleport] =
                (12, PriestessSetplayTrigger.Teleport);
        }
        

        
        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            const int lifeSpan = 165;
            
            StateMapConfig.HitSectionGroup.Dictionary[PriestessSetplayState.Active] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new (3, null),
                    new(3, new Hit()
                    {
                        // Launches = true,
                        Level = 1,
                        Projectile = true,
                        HitPushback = 0,
                        BlockPushback = 0,
                        // GroundBounce = true,
                        TrajectoryHeight = FP.FromString("0.5"),
                        TrajectoryXVelocity = 13,
                        GravityProration = FP.FromString("1.7"),
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
                    new (1, null),
                }
            };
            
            StateMapConfig.HitSectionGroup.Dictionary[PriestessSetplayState.Return] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new (ReturnStartup, null),
                    new(30, new Hit()
                    {
                        Level = 1,
                        Projectile = true,
                        TrajectoryHeight = FP.FromString("2"),
                        TrajectoryXVelocity = -8,
                        HitPushback = -2,
                        BlockPushback = -4,
                        GravityProration = FP.FromString("1.7"),
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new(30, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowWidth = false,
                                            GrowHeight = false,
                                            PosX = 0,
                                            PosY = 1,
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

            var aliveMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(15, FP.FromString("6")),
                    new(15, FP.FromString("3")),
                    new(15, FP.FromString("1.5")),
                    new(10, FP.FromString("0"))
                }
            };
            StateMapConfig.MovementSectionGroup.Dictionary[PriestessSetplayState.Alive] = aliveMovement;
            
            
            // var startupAnimation = new FighterAnimation()
            // {
            //     Path = "Startup",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         Loop = true,
            //         LengthScalar = 4,
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // Util.AutoSetupFromAnimationPath(startupAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Startup] = startupAnimation;
            
            
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
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Tracking] = 13;
            
            var activeAnimation = new FighterAnimation()
            {
                Path = "Active",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 4,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(activeAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Active] = activeAnimation;
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Active] = 6;
            
            var aliveAnimation = new FighterAnimation()
            {
                Path = "Alive",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 7,
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
                    LengthScalar = 3,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(returnAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Return] = returnAnimation;
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Return] = 40;
            
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
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Destroy] = destroyAnimation;
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Destroy] = 12;
            
            
            var teleportAnimation = new FighterAnimation()
            {
                Path = "Teleport",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 4,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(teleportAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessSetplayState.Teleport] = teleportAnimation;
            StateMapConfig.Duration.Dictionary[PriestessSetplayState.Teleport] = 28;

            

        }

        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, PriestessSetplayState.Tracking);
                
            // Fsm.Configure(PriestessSetplayState.Startup)
            //     .SubstateOf(SummonState.Unpooled)
            //     .Permit(SummonTrigger.OwnerHit, PriestessSetplayState.Destroy)
            //     .Permit(PriestessSetplayTrigger.OwnerStartupComplete, PriestessSetplayState.Tracking);

            Fsm.Configure(PriestessSetplayState.Tracking)
                .OnEntryFrom(SummonTrigger.Summoned, SnapToPos)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, PriestessSetplayState.Destroy)
                .Permit(Trigger.Finish, PriestessSetplayState.Active);

            Fsm.Configure(PriestessSetplayState.Active)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, PriestessSetplayState.Destroy)
                .Permit(Trigger.Finish, PriestessSetplayState.Alive);

            Fsm.Configure(PriestessSetplayState.Alive)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, PriestessSetplayState.Destroy)
                .Permit(Trigger.Finish, PriestessSetplayState.Destroy)
                .Permit(PriestessSetplayTrigger.OwnerCallUsed, PriestessSetplayState.Return)
                .Permit(PriestessSetplayTrigger.Teleport, PriestessSetplayState.Teleport);

            Fsm.Configure(PriestessSetplayState.Return)
                .SubstateOf(SummonState.Unpooled)
                .Permit(Trigger.Finish, PriestessSetplayState.Destroy)
                .Permit(SummonTrigger.OwnerHit, PriestessSetplayState.Destroy)
                .Permit(PriestessSetplayTrigger.ReturnComplete, PriestessSetplayState.Destroy);

            Fsm.Configure(PriestessSetplayState.Teleport)
                .SubstateOf(SummonState.Unpooled)
                .Permit(Trigger.Finish, SummonState.Pooled);

            Fsm.Configure(PriestessSetplayState.Destroy)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.Summoned, PriestessSetplayState.Tracking)
                .Permit(Trigger.Finish, SummonState.Pooled);

        }
        
        //TriggerParams? triggerParams
        private void SnapToPos(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam param) return;
            var otherPlayerEntity = Util.GetOtherPlayer(param.f, playerOwnerEntity);
            param.f.Unsafe.TryGetPointer<Transform3D>(otherPlayerEntity, out var otherPlayerTransform);
            param.f.Unsafe.TryGetPointer<Transform3D>(playerOwnerEntity, out var ownerTransform);

            bool low = GetPlayerFsm().Fsm.IsInState(PriestessFSM.PriestessState.SummonLow);
            var isFacingRight = IsFacingRight(param.f, playerOwnerEntity);
            var maxDistance = 11;
            var x = isFacingRight
                ? Util.Min(otherPlayerTransform->Position.X, ownerTransform->Position.X + maxDistance)
                : Util.Max(otherPlayerTransform->Position.X, ownerTransform->Position.X - maxDistance);
            
            
            var pos = new FPVector3(x, low ? 2 : 9, 0);
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Teleport(param.f, pos);
        }
        
        protected override void SummonMove(Frame f)
        {
            // if (Fsm.IsInState(PriestessSetplayState.Startup)) SnapToOwnerPosWithOffset(f);
            if (Fsm.IsInState(PriestessSetplayState.Return))
            {
                f.Unsafe.TryGetPointer<Transform3D>(playerOwnerEntity, out var ownerTransform3d);
                f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
                
                var dirMod = IsFacingRight(f, EntityRef) ? 1 : -1;
                var destinationOffset = new FPVector2(FP.FromString("1.5") * dirMod, 3);
                var destination = ownerTransform3d->Position + destinationOffset.XYO;
                var direction = destination - transform3D->Position;
                if (direction.Magnitude < FP.FromString("0.65"))
                {
                    Fsm.Fire(PriestessSetplayTrigger.ReturnComplete, new FrameParam() { f = f, EntityRef = EntityRef});
                    return;
                }

                FP speed;
                int frames = FramesInCurrentState(f);
                if (frames <= ReturnStartup) speed = FP.FromString("0.01");
                else if (frames <= ReturnStartup + 5) speed = FP.FromString("0.15");
                else speed = FP.FromString("0.85");
                
                direction = direction.Normalized * speed;
                
                ApplyFlippedMovement(f, direction.XY, EntityRef);
            }
            
            if (Fsm.IsInState(PriestessSetplayState.Teleport))
            {
                f.Unsafe.TryGetPointer<Transform3D>(playerOwnerEntity, out var ownerTransform3d);
                f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
                
                
                int frames = GetPlayerFsm().FramesInCurrentState(f);
                if (frames != 16) return;

                var p = transform3D->Position;
                p.Y = (p.Y < 5 ? 0 : p.Y - 2);
                ownerTransform3d->Teleport(f, p);
                
            }
        }

        
        
    }
}



        
