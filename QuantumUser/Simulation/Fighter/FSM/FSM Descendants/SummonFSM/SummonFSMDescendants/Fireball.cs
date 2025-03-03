using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public class Fireball : SummonFSM
    {
        public class FireballState : SummonState
        {
            
        }

        public enum FireballAnimationPath
        {

        }
        
        public Fireball()
        {
            Name = "Fireball";
            StateType = typeof(FireballState);
            AnimationPathsEnum = typeof(FireballAnimationPath);
            KinematicAttachPointOffset = FPVector2.Zero;
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

            const int lifeSpan = 110;
            
            StateMapConfig.HitSectionGroup.Dictionary[SummonState.Pooled] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new(lifeSpan, new Hit()
                    {
                        Launches = true,
                        Level = 2,
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
                                            PosX = -9,
                                            PosY = 2,
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
            
            StateMapConfig.MovementSectionGroup.Dictionary[SummonState.Pooled] = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(80, 0),
                    new(30, 10),
                }
            };
            
        }

        public override void SetupMachine()
        {
            base.SetupMachine();
            
            
        }
    }
}



        
