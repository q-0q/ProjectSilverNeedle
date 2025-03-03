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
            
            StateMapConfig.HitSectionGroup.Dictionary[SummonState.Pooled] = new SectionGroup<Hit>()
            {
                Loop = true,
                Sections = new List<Tuple<int, Hit>>()
                {
                    new(6, new Hit()
                    {
                        Launches = true,
                        Level = 2,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new(0, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            Height = 2,
                                            Width = 2
                                        }
                                    }
                                })
                            }
                        }
                    })
                }
            };
            
        }

        public override void SetupMachine()
        {
            base.SetupMachine();
            
            
        }
    }
}



        
