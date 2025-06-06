using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public class FireballFSM : SummonFSM
    {
        public class FireballState : SummonState
        {
            public static int Prime;
            public static int Alive;
            public static int Destroy;
        }
        
        public FireballFSM()
        {
            Name = "StickTwoFireball";
            StateType = typeof(FireballState);
            KinematicAttachPointOffset = FPVector2.Zero;
            SummonPositionOffset = new FPVector2(FP.FromString("3.5"), FP.FromString("4"));
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            const int lifeSpan = 50;
            
            StateMapConfig.HitSectionGroup.Dictionary[FireballState.Alive] = new SectionGroup<Hit>()
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
            
            StateMapConfig.MovementSectionGroup.Dictionary[FireballState.Alive] = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(lifeSpan, 28),
                }
            };

            StateMapConfig.Duration.Dictionary[FireballState.Alive] = lifeSpan;

            var aliveAnimation = new FighterAnimation()
            {
                Path = "Alive",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(aliveAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[FireballState.Alive] = aliveAnimation;
            
            var destroyAnimation = new FighterAnimation()
            {
                Path = "Destroy",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(destroyAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[FireballState.Destroy] = destroyAnimation;
            StateMapConfig.Duration.Dictionary[FireballState.Destroy] = 20;
            
            var primeAnimation = new FighterAnimation()
            {
                Path = "Prime",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(primeAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[FireballState.Prime] = primeAnimation;
            StateMapConfig.Duration.Dictionary[FireballState.Prime] = 18;


        }

        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Pooled)

                .Permit(SummonTrigger.Summoned, FireballState.Prime);
            
            Fsm.Configure(FireballState.Prime)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.OwnerHit, SummonState.Pooled)
                .Permit(Trigger.Finish, FireballState.Alive);
            
            Fsm.Configure(FireballState.Alive)
                .SubstateOf(SummonState.Unpooled)
                .Permit(SummonTrigger.Collided, FireballState.Destroy)
                .Permit(Trigger.Finish, FireballState.Destroy);
            
            Fsm.Configure(FireballState.Destroy)
                .SubstateOf(SummonState.Unpooled)
                .Permit(Trigger.Finish, SummonState.Pooled);

        }
        
        protected override void SummonMove(Frame f)
        {
            if (Fsm.IsInState(FireballState.Prime)) SnapToOwnerPosWithOffset(f);
        }
    }
}



        
