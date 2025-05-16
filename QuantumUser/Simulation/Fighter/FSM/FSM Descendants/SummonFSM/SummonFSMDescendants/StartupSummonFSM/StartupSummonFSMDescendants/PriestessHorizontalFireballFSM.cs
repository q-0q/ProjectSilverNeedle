
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
        
        public PriestessHorizontalFireballFSM()
        {
            Name = "PriestessHorizontalFireball";
            StateType = typeof(PriestessHorizontalFireballState);
            KinematicAttachPointOffset = FPVector2.Zero;
            SummonPositionOffset = new FPVector2(FP.FromString("-4"), FP.FromString("11"));

            OwnerActivationFrameTriggers[(PriestessFSM.PriestessState.Fireball, 18)] =
                PriestessHorizontalFireballTrigger.OwnerStartupComplete;
        }
        

        
        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
            
            
            StateMapConfig.HitSectionGroup.Dictionary[PriestessHorizontalFireballState.Active] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new (1, null),
                    new(3, new Hit()
                    {
                        // Launches = true,
                        Level = 1,
                        Projectile = true,
                        // GroundBounce = true,
                        TrajectoryHeight = 2,
                        TrajectoryXVelocity = 8,
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
                }
            };
            


            
            
            var startupAnimation = new FighterAnimation()
            {
                Path = "Startup",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 7,
                    AutoFromAnimationPath = true
                }
            };
            var startupMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(10, FP.FromString("1")),
                }
            };
            
            Util.AutoSetupFromAnimationPath(startupAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessHorizontalFireballState.Startup] = startupAnimation;
            StateMapConfig.MovementSectionGroup.Dictionary[PriestessHorizontalFireballState.Startup] = startupMovement;
            
            
            
            var activeAnimation = new FighterAnimation()
            {
                Path = "Active",
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 3,
                    Loop = true,
                    AutoFromAnimationPath = true
                }
            };
            

            

            
            Util.AutoSetupFromAnimationPath(activeAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PriestessHorizontalFireballState.Active] = activeAnimation;
            StateMapConfig.MovementSectionGroup.FuncDictionary[PriestessHorizontalFireballState.Active] = GetActiveMovement;
            StateMapConfig.Duration.Dictionary[PriestessHorizontalFireballState.Active] = 100;
            
            
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
            StateMapConfig.MovementSectionGroup.FuncDictionary[PriestessHorizontalFireballState.Destroy] = GetActiveMovement;
            StateMapConfig.Duration.Dictionary[PriestessHorizontalFireballState.Destroy] = 12;

            

        }

        public override void SetupMachine()
        {
            base.SetupMachine();
            
            

            Fsm.Configure(SummonState.Pooled)
                .Permit(SummonTrigger.Summoned, PriestessHorizontalFireballState.Startup)
                .OnExitFrom(PriestessHorizontalFireballTrigger.OwnerStartupComplete, OnUnpooled)
                .Permit(PriestessHorizontalFireballTrigger.OwnerStartupComplete, PriestessHorizontalFireballState.Active);
            
            
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
        
        protected override void SummonMove(Frame f)
        {
            if (Fsm.IsInState(SummonState.Pooled)) return;
            base.SummonMove(f);
            
            if (!SetplayActive(f)) return;

            f.Unsafe.TryGetPointer<Transform3D>(GetPlayerFsm().SummonPools[0].EntityRefs[0], out var setplayTransform);
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform);

            var dY = (setplayTransform->Position.Y - transform->Position.Y);

            transform->Position.Y += dY * FP.FromString("0.05");
            
        }
        
        SectionGroup<FP> GetActiveMovement(FrameParam frameParam)
        {
            if (frameParam is null) return null;
            var activeMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(10, FP.FromString("2.25"))
                }
            };

            return activeMovement;
        }

        bool SetplayActive(Frame f)
        {
            var setplayFsm = FsmLoader.FSMs[GetPlayerFsm().SummonPools[0].EntityRefs[0]];
            return setplayFsm.Fsm.IsInState(SummonState.Unpooled);
        }

        
        
    }
}



        
