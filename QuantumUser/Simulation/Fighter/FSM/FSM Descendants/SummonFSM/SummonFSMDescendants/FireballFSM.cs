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
            
        }

        public enum FireballAnimationPath
        {
            Alive
        }
        
        public FireballFSM()
        {
            Name = "StickTwoFireball";
            StateType = typeof(FireballState);
            AnimationPathsEnum = typeof(FireballAnimationPath);
            KinematicAttachPointOffset = FPVector2.Zero;
            SummonPositionOffset = new FPVector2(5, 5);
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            const int lifeSpan = 50;
            
            StateMapConfig.HitSectionGroup.Dictionary[SummonState.Unpooled] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new(lifeSpan, new Hit()
                    {
                        // Launches = true,
                        Level = 1,
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
            
            StateMapConfig.MovementSectionGroup.Dictionary[SummonState.Unpooled] = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(lifeSpan, 10),
                }
            };

            StateMapConfig.Duration.Dictionary[SummonState.Unpooled] = lifeSpan;

            var aliveAnimation = new FighterAnimation()
            {
                Path = (int)FireballAnimationPath.Alive,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(aliveAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[SummonState.Unpooled] = aliveAnimation;

            

        }

        public override void SetupMachine()
        {
            base.SetupMachine();

            Fsm.Configure(SummonState.Unpooled)
                .Permit(Trigger.Finish, SummonState.Pooled);

        }
    }
}



        
