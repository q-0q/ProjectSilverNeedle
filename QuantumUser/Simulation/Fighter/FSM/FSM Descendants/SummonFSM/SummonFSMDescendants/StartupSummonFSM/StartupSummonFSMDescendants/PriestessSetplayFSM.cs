
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
    public unsafe class PriestessSetplayFSM : StartupSummonFSM
    {
        public class PriestessSetplayState : StartupSummonState
        {
            public static int Tracking;
        }
        
        public PriestessSetplayFSM()
        {
            Name = "PriestessSetplay";
            StateType = typeof(PriestessSetplayState);
            KinematicAttachPointOffset = FPVector2.Zero;
            SummonPositionOffset = new FPVector2(FP.FromString("5.75"), FP.FromString("7.75"));
            OwnerActivationFrame = 14;
            SpriteScale = FP.FromString("0.6");
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            const int lifeSpan = 40;
            
            StateMapConfig.HitSectionGroup.Dictionary[StartupSummonState.Alive] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new(lifeSpan, new Hit()
                    {
                        // Launches = true,
                        Level = 1,
                        Projectile = true,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new(lifeSpan, new CollisionBoxCollection()
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
                    })
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
            StateMapConfig.FighterAnimation.Dictionary[StartupSummonState.Startup] = startupAnimation;
            
            
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
            StateMapConfig.FighterAnimation.Dictionary[StartupSummonState.Alive] = aliveAnimation;
            StateMapConfig.Duration.Dictionary[StartupSummonState.Alive] = lifeSpan;

            

        }

        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(StartupSummonState.Startup)
                .PermitIf(StartupSummonTrigger.OwnerActivated, PriestessSetplayState.Tracking, _ => true, 1);
            
            Fsm.Configure(PriestessSetplayState.Tracking)
                .OnEntry(SnapToOpponent)
                .PermitIf(Trigger.Finish, StartupSummonState.Alive, _ => true, 1)
                .Permit(SummonTrigger.OwnerHit, SummonState.Pooled)
                .SubstateOf(StartupSummonState.Alive);
            
            Fsm.Configure(StartupSummonState.Alive)
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
            var pos = new FPVector2(otherPlayerTransform->Position.X, Util.Max(otherPlayerTransform->Position.Y, 3));
            SetPosition(param.f, pos);
        }
        
        
    }
}



        
