using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;

namespace Quantum
{
    public class StickTwo : Character
    {
        public StickTwo()
        {
            Name = "StickTwo";
            
            WalkForwardSpeed = FP.FromString("12");
            WalkBackwardSpeed = FP.FromString("9");

            JumpHeight = FP.FromString("5.5");
            JumpTimeToHeight = 25;
            JumpForwardSpeed = FP.FromString("10");
            JumpBackwardSpeed = FP.FromString("7");
            JumpCount = 2;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 20;

            KinematicAttachPointOffset = new FPVector2(0, 3);
            
            FP lowDamage = 12;
            FP mediumDamage = 20;
            FP highDamage = 35;
            FP crazyDamage = 45;
            
            StandHurtboxesCollection = new CollisionBoxCollection()
            {
                CollisionBoxes = new List<CollisionBox>()
                {
                    new()
                    {
                        GrowHeight = true,
                        GrowWidth = false,
                        PosX = 0,
                        PosY = 0,
                        Height = 6,
                        Width = 3,
                    },
                    
                }
            };
            
            CrouchHurtboxCollection = new CollisionBoxCollection()
            {
                CollisionBoxes = new List<CollisionBox>()
                {
                    new()
                    {
                        GrowHeight = true,
                        GrowWidth = false,
                        PosX = 0,
                        PosY = 0,
                        Height = 4,
                        Width = 3,
                    }
                }
            };

            AirHitHurtboxCollection = new CollisionBoxCollection()
            {
                CollisionBoxes = new List<CollisionBox>()
                {
                    new()
                    {
                        GrowHeight = false,
                        GrowWidth = false,
                        PosX = 0,
                        PosY = 2,
                        Height = 6,
                        Width = 7,
                    }
                }
            };

            StandPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 6,
                Width = 1,
            };
            
            CrouchPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 4,
                Width = 1,
            };
            
            AirPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 1,
                Height = 3,
                Width = 1,
            };
            
            TallPushbox = new CollisionBox()
            {
                GrowHeight = false,
                GrowWidth = false,
                PosX = 0,
                PosY = 3,
                Height = 16,
                Width = 1,
            };

            StandAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 0,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0)
                    }
                }
            };
            
            CrouchAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 1,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0)
                    }
                }
            };
            
            CrouchAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 1,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0)
                    }
                }
            };
            
            WalkForwardAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 12,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 9,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0),
                        new(1, 1),
                        new(1, 2),
                        new(1, 3),
                        new(1, 4),
                        new(1, 5),
                    }
                }
            };
            
            WalkBackwardAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 12,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 14,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 5),
                        new(1, 4),
                        new(1, 3),
                        new(1, 2),
                        new(1, 1),
                        new(1, 0),
                    }
                }
            };

            AirActionableRisingAnimation = new RisingTrajectoryAnimation()
            {
                SpriteSheetOffset = 18,
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 5,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0),
                        new(1, 1),
                    }
                }
            };
            
            AirActionableFallingAnimation = new FallingTrajectoryAnimation()
            {
                SpriteSheetOffset = 19,
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 5,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 1),
                        new(1, 2),
                    }
                }
            };

            AirHitPostGroundBounceRisingAnimation = new RisingTrajectoryAnimation()
            {
                SpriteSheetOffset = 28,
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 5,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0),
                        new(1, 1),
                    }
                }
            };
            
            AirPostHitGroundBounceFallingAnimation = new FallingTrajectoryAnimation()
            {
                SpriteSheetOffset = 28,
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 5,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 1),
                        new(1, 0),
                    }
                }
            };
            
            StandHitHighAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 6,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };
            
            StandHitLowAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 8,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };
            
            CrouchHitAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 10,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };

            AirHitRisingAnimation = new RisingTrajectoryAnimation()
            {
                SpriteSheetOffset = 24,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0),
                        new(8, 1),
                    }
                }
            };
            
            AirHitFallingAnimation = new FallingTrajectoryAnimation()
            {
                SpriteSheetOffset = 24,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(8, 1),
                        new(8, 2),
                        new(8, 3),
                    }
                }
            };

            KinematicReceiverAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 24,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(18, 0),
                    }
                }
            };

            StandBlockAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 2,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };
            ThrowTechAnimation = StandBlockAnimation;
            
            CrouchBlockAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 4,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };

            AirBlockAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 22,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };

            HardKnockdownAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 30,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(8, 0),
                        new(20, 1),
                        new(10, 2),
                        new(10, 3),
                        new(7, 4),
                        new(7, 5),
                    }
                }
            };

            DeadFromAirAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 30,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(8, 0),
                        new(20, 1),
                    }
                }
            };
            
            DeadFromGroundAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 59,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(15, 0),
                        new(5, 1),
                        new(25, 2),
                        new(5, 3),
                        new(5, 4),
                        new(5, 5),
                    }
                }
            };
            
            SoftKnockdownAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 54,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(8, 1),
                        new(5, 2),
                        new(5, 3),
                        new(5, 4),
                    }
                }
            };

            LandsquatAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 1,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0)
                    }
                }
            };

            DashAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 36,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(5, 0),
                        new(10, 1),
                        new(6, 17),
                    }
                }
            };

            DashMovementSectionGroup = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(8, 0),
                    new(7, 3),
                    new(3, 0),
                }
            };
            
            BackdashAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 50,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(6, 0),
                        new(4, 1),
                        new(6, 2),
                        new(6, 3),
                    }
                }
            };
            AirdashAnimation = DashAnimation;
            
            BackdashMovementSectionGroup = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(6, 0),
                    new(7, -4),
                    new(9, 0),
                }
            };
            AirBackdashAnimation = BackdashAnimation;

            ThrowStartupAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 38,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0)
                    }
                }
            };
            
            FrontThrowKinematics = new Kinematics()
            {
                FireReceiverFinishAfter = 40,
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 39,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(8, 0),
                            new(18, 1),
                            new(10, 2),
                            new(25, 3)
                        }
                    }
                },
                
                GrabPositionSectionGroup = new SectionGroup<FPVector2>()
                {
                    Sections = new List<Tuple<int, FPVector2>>()
                    {
                        new(8, new FPVector2(2, 3)),
                        new(23, new FPVector2(2, 4)),
                        new(5, new FPVector2(2, 3)),
                        new(25, new FPVector2(2, 1))
                    }
                },
                
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new (16, null),
                        new (4, new Hit()
                        {
                            Damage = 50
                        }),
                        new (20, null)
                    }
                }
                
            };
            
            BackThrowKinematics = new Kinematics()
            {
                FireReceiverFinishAfter = 37,
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 45,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(8, 0),
                            new(18, 1),
                            new(10, 2),
                            new(12, 3),
                            new(18, 4)
                        }
                    }
                },
                
                GrabPositionSectionGroup = new SectionGroup<FPVector2>()
                {
                    Sections = new List<Tuple<int, FPVector2>>()
                    {
                        new(8, new FPVector2(2, 3)),
                        new(18, new FPVector2(2, 4)),
                        new(10, new FPVector2(0, 5)),
                        new(25, new FPVector2(-6, 1))
                    }
                },
                
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new (16, null),
                        new (4, new Hit()
                        {
                            Damage = 50
                        }),
                        new (20, null)
                    }
                }
                
            };
            
            ThrowWhiffAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 43,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0),
                        new(10, 1),
                    }
                }
            };
            
            
            
            ////////////////////////////////////////////////////////////////////////////////////




            FP crazyScaling = FP.FromString("0.6");
            FP highScaling = FP.FromString("0.75");
            FP mediumScaling = FP.FromString("0.85");
            FP lowScaling = FP.FromString("0.95");


            
            FighterAction _2L = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 68,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(5, 0),
                            new(2, 1),
                            new(4, 2)
                        }
                    }
                },

                CancellableAfter = 6,
                CommandDirection = 2,
                InputType = InputSystem.InputType.P,
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(5, null),
                        new(2, new Hit()
                        {
                            Type = Hit.HitType.Low,
                            HitPushback = 2,
                            BlockPushback = 2,
                            DamageScaling = crazyScaling,
                            Damage = lowDamage,
                            VisualAngle = 5,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(20, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = true,
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 4,
                                                PosX = 0,
                                                PosY = 0
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (5, StandHurtboxesCollection),
                        new (2, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 6,
                                    Width = 3,
                                },
                                new CollisionBox()
                                {
                                    GrowHeight = true,
                                    GrowWidth = true,
                                    Height = 3,
                                    Width = 5,
                                    PosX = 0,
                                    PosY = 0
                                }
                            }
                        }),
                        new (5, StandHurtboxesCollection)
                    }
                },

                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(7, PlayerFSM.HurtType.Counter),
                        new(5, PlayerFSM.HurtType.Punish)
                    }
                },
            };
            
            
            
            
            
            
            
            FighterAction _5L = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 65,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(5, 0),
                            new(2, 1),
                            new(8, 2)
                        }
                    }
                },

                CancellableAfter = 6,
                CommandDirection = 5,
                InputType = InputSystem.InputType.P,
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(5, null),
                        new(2, new Hit()
                        {
                            HitPushback = 2,
                            BlockPushback = 2,
                            Damage = lowDamage,
                            VisualAngle = -5,
                            DamageScaling = crazyScaling,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(20, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = false,
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 3,
                                                PosX = 0,
                                                PosY = 4
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (5, StandHurtboxesCollection),
                        new (2, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 6,
                                    Width = 3,
                                },
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = true,
                                    Height = 3,
                                    Width = 4,
                                    PosX = 0,
                                    PosY = 4
                                }
                            }
                        }),
                        new (5, StandHurtboxesCollection)
                    }
                },

                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(7, PlayerFSM.HurtType.Counter),
                        new(5, PlayerFSM.HurtType.Punish)
                    }
                },
            };




            FighterAction _5M = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 71,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(2, 0),
                            new(6, 1),
                            new(7, 3),
                            new(13, 5),
                        }
                    }
                },

                CancellableAfter = 10,
                CommandDirection = 5,
                InputType = InputSystem.InputType.K,
                MovementSectionGroup = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(6, 0),
                        new(2, 1),
                        new(20, 0),
                    }
                },
                
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(8, null),
                        new(3, new Hit()
                        {
                            BlockPushback = 2,
                            GravityScaling = 1,
                            TrajectoryXVelocity = 20,
                            TrajectoryHeight = 1,
                            Level = 2,
                            Damage = mediumDamage,
                            VisualAngle = 6,
                            DamageScaling = mediumScaling,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(20, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = false,
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 4,
                                                PosX = 0,
                                                PosY = 4
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (8, StandHurtboxesCollection),
                        new (3, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 6,
                                    Width = 3,
                                },
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = true,
                                    Height = 3,
                                    Width = FP.FromString("4.5"),
                                    PosX = 0,
                                    PosY = 4
                                }
                            }
                        }),
                        new (10, StandHurtboxesCollection)
                    }
                },

                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(11, PlayerFSM.HurtType.Counter),
                        new(10, PlayerFSM.HurtType.Punish)
                    }
                },
            };
            
            
            FighterAction _2M = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 77,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(2, 0),
                            new(8, 1),
                            new(2, 2),
                            new(13, 3),
                            new(12, 4),
                        }
                    }
                },

                CancellableAfter = 10,
                CommandDirection = 2,
                InputType = InputSystem.InputType.K,
                MovementSectionGroup = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(8, 0),
                        new(2, 1),
                        new(20, 0),
                    }
                },
                
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(10, null),
                        new(3, new Hit()
                        {
                            BlockPushback = 2,
                            DamageScaling = mediumScaling,
                            Type = Hit.HitType.Low,
                            Level = 2,
                            Damage = mediumDamage,
                            VisualAngle = 10,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(20, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = true,
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 4,
                                                PosX = 0,
                                                PosY = 0,
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (10, CrouchHurtboxCollection),
                        new (20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = -2,
                                    PosY = 0,
                                    Height = 4,
                                    Width = 4,
                                },
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = true,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 2,
                                    Width = 5,
                                },
                            }
                        })
                    }
                },

                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(13, PlayerFSM.HurtType.Counter),
                        new(10, PlayerFSM.HurtType.Punish)
                    }
                },
            };
            
            
            
            FighterAction _2H = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 82,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(7, 0),
                            new(8, 1),
                            new(10, 3),
                            new(10, 4),
                            new(9, 5),
                        }
                    }
                },
                
                CommandDirection = 2,
                InputType = InputSystem.InputType.H,
                
                
                MovementSectionGroup = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(15, 0),
                        new(2, 1),
                        new(20, 0),
                    }
                },
                
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(15, null),
                        new(3, new Hit()
                        {
                            BlockPushback = 2,
                            DamageScaling = lowScaling,
                            // GravityScaling = FP.FromString("1.1"),
                            Launches = true,
                            TrajectoryHeight = FP.FromString("6.5"),
                            TrajectoryXVelocity = 3,
                            VisualAngle = -90,
                            Level = 4,
                            Damage = highDamage,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(20, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = true,
                                                GrowWidth = false,
                                                Height = 3,
                                                Width = 7,
                                                PosX = 0,
                                                PosY = 0,
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (25, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    Height = 7,
                                    Width = 7,
                                    PosX = 0,
                                    PosY = 0,
                                }
                            }
                        })
                    }
                },

                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(18, PlayerFSM.HurtType.Counter),
                        new(10, PlayerFSM.HurtType.Punish)
                    }
                },
            };


            FighterAction _5H = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 88,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new (8, 0),
                            new (3, 1),
                            new (2, 2),
                            new (11, 4),
                            new (7, 5),
                            new (7, 6),
                            new (4, 7),
                        }
                    }
                },
                
                CancellableAfter = 16,
                CommandDirection = 5,
                InputType = InputSystem.InputType.H,
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new (15, null),
                        new (5, new Hit()
                        {
                            VisualAngle = 30,
                            GroundBounce = true,
                            DamageScaling = lowScaling,
                            TrajectoryHeight = 8,
                            Level = 3,
                            Type = Hit.HitType.High,
                            TrajectoryXVelocity = 7,
                            Damage = highDamage,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (20, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = false,
                                                GrowWidth = true,
                                                Height = FP.FromString("4"),
                                                Width = FP.FromString("3.5"),
                                                PosX = 0,
                                                PosY = FP.FromString("4"),
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null),
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (50, StandHurtboxesCollection),
                        new (30, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                StandHurtboxesCollection.CollisionBoxes[0],
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = true,
                                    Height = 3,
                                    Width = FP.FromString("2.5"),
                                    PosX = 0,
                                    PosY = 3,
                                }
                            }
                        })
                    }
                },
                
                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new (20, PlayerFSM.HurtType.Counter),
                        new (30, PlayerFSM.HurtType.Punish)
                    }
                },
                
                MovementSectionGroup = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(11, 0),
                        new(2, FP.FromString("1.5")),
                        new (20, 0),
                    }
                }
            };



            FighterAction _5S1;
            {
                int startup = 11;
                int active = 3;
                CollisionBox mainHurtbox = new CollisionBox()
                {
                    GrowHeight = true,
                    GrowWidth = true,
                    Width = 3,
                    Height = 6,
                    PosX = -1,
                    PosY = 0,
                };
                
                _5S1 = new FighterAction()
                {
                    InputType = InputSystem.InputType.S,
                    CommandDirection = 5,
                    CancellableAfter = 5,
                    WhiffCancellable = true,
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 101,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(3, 0),
                                new(2, 1),
                                new(2, 2),
                                new(startup - 7, 3),
                                new(active + 2, 4),
                                new(9, 5),
                                new(6, 6),
                            }
                        }
                    },

                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(startup, null),
                            new(active, new Hit()
                            {
                                Level = 3,
                                TrajectoryHeight = 2,
                                TrajectoryXVelocity = 11,
                                HitPushback = 3,
                                BlockPushback = 4,
                                Damage = mediumDamage,
                                VisualAngle = -10,
                                DamageScaling = highScaling,
                                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                                {
                                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                    {
                                        new(20, new CollisionBoxCollection()
                                        {
                                            CollisionBoxes = new List<CollisionBox>()
                                            {
                                                new CollisionBox()
                                                {
                                                    GrowHeight = false,
                                                    GrowWidth = true,
                                                    Height = 2,
                                                    Width = 5,
                                                    PosX = 0,
                                                    PosY = 4
                                                }
                                            }
                                        })
                                    }
                                }
                            }),
                            new (20, null)
                        }
                    },
                    
                    HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                    {
                        Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                        {
                            new(startup + active, PlayerFSM.HurtType.Counter),
                            new(20, PlayerFSM.HurtType.Punish)
                        }
                    },
                    
                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(startup, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}}),
                            new(active + 3, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox, new CollisionBox()
                            {
                                GrowHeight = false,
                                GrowWidth = true,
                                Height = 3,
                                Width = FP.FromString("5.5"),
                                PosX = 0,
                                PosY = 4
                            } }}),
                            new(startup, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}})
                        }
                    }
                };
            }
            
            
            
            
            FighterAction _5S2;
            {
                int startup = 27;
                int active = 3;
                CollisionBox mainHurtbox = new CollisionBox()
                {
                    GrowHeight = true,
                    GrowWidth = false,
                    Width = 4,
                    Height = 3,
                    PosX = 0,
                    PosY = 0,
                };
                
                _5S2 = new FighterAction()
                {
                    InputType = InputSystem.InputType.S,
                    CommandDirection = 5,
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 108,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(3, 0),
                                new(2, 1),
                                new(2, 2),
                                new(6, 3),
                                new(startup - 13, 4),
                                new(active + 2, 5),
                                new(6, 6),
                            }
                        }
                    },

                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(startup, null),
                            new(active, new Hit()
                            {
                                Level = 3,
                                TrajectoryHeight = 2,
                                TrajectoryXVelocity = 10,
                                HitPushback = 3,
                                Damage = lowDamage,
                                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                                {
                                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                    {
                                        new(20, new CollisionBoxCollection()
                                        {
                                            CollisionBoxes = new List<CollisionBox>()
                                            {
                                                new CollisionBox()
                                                {
                                                    GrowHeight = true,
                                                    GrowWidth = true,
                                                    Height = 2,
                                                    Width = FP.FromString("5.5"),
                                                    PosX = 0,
                                                    PosY = 0
                                                }
                                            }
                                        })
                                    }
                                }
                            }),
                            new (20, null)
                        }
                    },
                    
                    HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                    {
                        Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                        {
                            new(startup + active, PlayerFSM.HurtType.Counter),
                            new(20, PlayerFSM.HurtType.Punish)
                        }
                    },
                    
                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(startup, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}}),
                            new(active + 3, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox, new CollisionBox()
                            {
                                GrowHeight = true,
                                GrowWidth = true,
                                Height = 3,
                                Width = FP.FromString("6"),
                                PosX = 0,
                                PosY = 0
                            } }}),
                            new(startup, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}})
                        }
                    },
                    
                    MovementSectionGroup = new SectionGroup<FP>()
                    {
                        Sections = new List<Tuple<int, FP>>()
                        {
                            new(startup - 2, 0),
                            new (2, 4),
                            new (30, 0)
                        }
                    }
                };
            }
            
            
            
            FighterAction _5S3;
            {
                int startup = 21;
                int active = 3;
                CollisionBox mainHurtbox = new CollisionBox()
                {
                    GrowHeight = true,
                    GrowWidth = true,
                    Width = 3,
                    Height = 6,
                    PosX = -1,
                    PosY = 0,
                };
                
                _5S3 = new FighterAction()
                {
                    InputType = InputSystem.InputType.S,
                    CommandDirection = 4,
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 115,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(4, 0),
                                new(4, 1),
                                new(4, 2),
                                new(5, 3), // hit
                                new(4, 4),
                                new(5, 5), // hit
                                new(4, 6),
                                new(4, 7),
                                new(4, 9),
                            }
                        }
                    },


                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(12, null),
                            new(3, new Hit()
                            {
                                Level = 2,
                                TrajectoryHeight = 3,
                                TrajectoryXVelocity = 11,
                                HitPushback = 1,
                                BlockPushback = 1,
                                GravityScaling = FP.FromString("1.05"),
                                Launches = true,
                                Type = Hit.HitType.Low,
                                Damage = mediumDamage,
                                VisualAngle = 18,
                                DamageScaling = lowScaling,
                                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                                {
                                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                    {
                                        new(20, new CollisionBoxCollection()
                                        {
                                            CollisionBoxes = new List<CollisionBox>()
                                            {
                                                new CollisionBox()
                                                {
                                                    GrowHeight = true,
                                                    GrowWidth = true,
                                                    Height = 3,
                                                    Width = 5,
                                                    PosX = 0,
                                                    PosY = 0
                                                }
                                            }
                                        })
                                    }
                                }
                            }),
                            new(6, null),
                            new(3, new Hit()
                            {
                                Level = 3,
                                TrajectoryHeight = 4,
                                TrajectoryXVelocity = 12,
                                HitPushback = 4,
                                BlockPushback = 4,
                                GroundBounce = true,
                                Damage = mediumDamage,
                                VisualAngle = -18,
                                GravityScaling = FP.FromString("1.04"),
                                DamageScaling = mediumScaling,
                                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                                {
                                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                    {
                                        new(20, new CollisionBoxCollection()
                                        {
                                            CollisionBoxes = new List<CollisionBox>()
                                            {
                                                new CollisionBox()
                                                {
                                                    GrowHeight = false,
                                                    GrowWidth = true,
                                                    Height = 2,
                                                    Width = 5,
                                                    PosX = 0,
                                                    PosY = 4
                                                }
                                            }
                                        })
                                    }
                                }
                            }),
                            new (20, null)
                        }
                    },
                    
                    HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                    {
                        Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                        {
                            new(26, PlayerFSM.HurtType.Counter),
                            new(20, PlayerFSM.HurtType.Punish)
                        }
                    },
                    
                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(12, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}}),
                            new(14, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox, new CollisionBox()
                            {
                                GrowHeight = false,
                                GrowWidth = true,
                                Height = 3,
                                Width = FP.FromString("5.5"),
                                PosX = 0,
                                PosY = 4
                            } }}),
                            new(20, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}})
                        }
                    },
                    
                    MovementSectionGroup = new SectionGroup<FP>()
                    {
                        Sections = new List<Tuple<int, FP>>()
                        {
                            new(10, 2),
                            new (2, 1),
                            new (3, 0),
                            new (2, 1),
                            new (30, 4)
                        }
                    }
                };
            }
            
            
            
            
            
            FighterAction _6S3;
            {
                int startup = 16;
                int active = 2;
                CollisionBox mainHurtbox = new CollisionBox()
                {
                    GrowHeight = true,
                    GrowWidth = false,
                    Width = 6,
                    Height = 4,
                    PosX = 0,
                    PosY = 0,
                };

                Hit hit = new Hit()
                {
                    Level = 4,
                    TrajectoryHeight = 5,
                    TrajectoryXVelocity = 18,
                    HitPushback = 3,
                    GroundBounce = true,
                    HardKnockdown = true,
                    Damage = crazyDamage,
                    VisualAngle = 80,
                    DamageScaling = lowScaling,
                    HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(20, new CollisionBoxCollection()
                            {
                                CollisionBoxes = new List<CollisionBox>()
                                {
                                    new CollisionBox()
                                    {
                                        GrowHeight = true,
                                        GrowWidth = true,
                                        Height = 5,
                                        Width = FP.FromString("5.5"),
                                        PosX = 0,
                                        PosY = 0
                                    }
                                }
                            })
                        }
                    }
                };
                
                _6S3 = new FighterAction()
                {
                    InputType = InputSystem.InputType.S,
                    CommandDirection = 6,
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 125,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(3, 0),
                                // new(3, 1),
                                // new(3, 2),
                                new(3, 3),
                                new(startup - 6, 4),
                                new(active, 5),
                                new(active, 6),
                                new(10, 7),
                                new(8, 8),
                                new(8, 9),
                            }
                        }
                    },

                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(startup, null),
                            new(active, hit),
                            new(active, hit),
                            new (20, null)
                        }
                    },
                    
                    HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                    {
                        Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                        {
                            new(startup + active + active, PlayerFSM.HurtType.Counter),
                            new(20, PlayerFSM.HurtType.Punish)
                        }
                    },
                    
                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(startup, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}}),
                            new(active + active + 3, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox, new CollisionBox()
                            {
                                GrowHeight = true,
                                GrowWidth = true,
                                Height = 3,
                                Width = FP.FromString("6"),
                                PosX = 0,
                                PosY = 0
                            } }}),
                            new(startup, new CollisionBoxCollection() { CollisionBoxes = new List<CollisionBox>() { mainHurtbox}})
                        }
                    },
                    
                    MovementSectionGroup = new SectionGroup<FP>()
                    {
                        Sections = new List<Tuple<int, FP>>()
                        {
                            new(startup - 2, 0),
                            new (2, 4),
                            new (30, 3)
                        }
                    }
                };
            }

            FighterAction _2S;
            {
                _2S = new FighterAction()
                {
                    InputType = InputSystem.InputType.S,
                    CommandDirection = 2,
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 135,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(5, 0),
                                new(3, 1),
                                new(3, 2),
                                new(15, 3),
                                new(10, 4),
                                new(200, 5),
                            }
                        }
                    },
                    TrajectorySectionGroup = new SectionGroup<ActionTrajectory>()
                    {
                        Sections = new List<Tuple<int, ActionTrajectory>>()
                        {
                            new(5, new ActionTrajectory()
                            {
                                TrajectoryHeight = FP.FromString("0.5"),
                                TrajectoryXVelocity = 1,
                                TimeToTrajectoryHeight = 5,
                            }),
                            new(10, new ActionTrajectory()
                            {
                                TrajectoryHeight = 4,
                                TrajectoryXVelocity = 5,
                                TimeToTrajectoryHeight = 35,
                            }),
                            new (10, null)
                        }
                    },
                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(5, null),
                            new(15, new Hit()
                            {
                                Launches = true,
                                TrajectoryXVelocity = 35,
                                TrajectoryHeight = FP.FromString("1.5"),
                                Level = 0,
                                Damage = mediumDamage,
                                VisualAngle = -20,
                                DamageScaling = lowScaling,
                                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                                {
                                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                    {
                                        new Tuple<int, CollisionBoxCollection>(100, new CollisionBoxCollection()
                                        {
                                            CollisionBoxes = new List<CollisionBox>()
                                            {
                                                new CollisionBox()
                                                {
                                                    GrowHeight = true,
                                                    GrowWidth = false,
                                                    Height = 7,
                                                    Width = 7,
                                                    PosX = 0,
                                                    PosY = 0,
                                                }
                                            }
                                        })
                                    }
                                }
                            }),
                            new(200, null)
                        }
                    },
                    
                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new (12, null),
                            new (200, new CollisionBoxCollection()
                            {
                                CollisionBoxes = new List<CollisionBox>()
                                {
                                    new CollisionBox()
                                    {
                                        GrowHeight = true,
                                        GrowWidth = false,
                                        Height = 7,
                                        Width = 7,
                                        PosX = 0,
                                        PosY = 0,
                                    }
                                }
                            })
                        }
                    },
                    
                    HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                    {
                        Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                        {
                            new(300, PlayerFSM.HurtType.Counter)
                        }
                    }
                };
            }
            
            // Air actions ------------------------------------------------------------------------
            
            FighterAction _JL = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 141,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(5, 0),
                            new(2, 1),
                            new(8, 2)
                        }
                    }
                },

                CancellableAfter = 6,
                CommandDirection = 5,
                InputType = InputSystem.InputType.P,
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(5, null),
                        new(2, new Hit()
                        {
                            HitPushback = 2,
                            BlockPushback = 2,
                            Type = Hit.HitType.High,
                            Damage = lowDamage,
                            VisualAngle = 8,
                            DamageScaling = crazyScaling,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(20, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = false,
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 3,
                                                PosX = 0,
                                                PosY = 4
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (5, StandHurtboxesCollection),
                        new (2, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 6,
                                    Width = 3,
                                },
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = true,
                                    Height = 3,
                                    Width = 4,
                                    PosX = 0,
                                    PosY = 4
                                }
                            }
                        }),
                        new (5, StandHurtboxesCollection)
                    }
                },

                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(7, PlayerFSM.HurtType.Counter),
                        new(5, PlayerFSM.HurtType.Punish)
                    }
                },
            };


            FighterAction _JM;
            {
                CollisionBox mainHurtbox = new CollisionBox()
                {
                    Width = 4,
                    Height = 4,
                    PosY = 3,
                    GrowHeight = false,
                    GrowWidth = false
                };
                    
                _JM = new FighterAction()
                {
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 144,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(2, 0),
                                new(8, 1),
                                new(2, 2),
                                new(13, 3),
                                new(12, 4),
                            }
                        }
                    },

                    CancellableAfter = 11,
                    CommandDirection = 5,
                    InputType = InputSystem.InputType.K,
                    MovementSectionGroup = new SectionGroup<FP>()
                    {
                        Sections = new List<Tuple<int, FP>>()
                        {
                            new(8, 0),
                            new(2, -1),
                            new(20, 0),
                        }
                    },

                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(10, null),
                            new(3, new Hit()
                            {
                                Type = Hit.HitType.High,
                                Level = 3,
                                TrajectoryXVelocity = 9,
                                GravityScaling = 1,
                                HitPushback = 3,
                                Damage = mediumDamage,
                                VisualAngle = -8,
                                DamageScaling = mediumScaling,
                                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                                {
                                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                    {
                                        new(20, new CollisionBoxCollection()
                                        {
                                            CollisionBoxes = new List<CollisionBox>()
                                            {
                                                new CollisionBox()
                                                {
                                                    GrowHeight = false,
                                                    GrowWidth = true,
                                                    Height = 2,
                                                    Width = 4,
                                                    PosX = 0,
                                                    PosY = 2,
                                                }
                                            }
                                        })
                                    }
                                }
                            }),
                            new(20, null)
                        }
                    },

                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(10, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>() {
                                        mainHurtbox
                                    }
                                }
                            ),
                            new(20, new CollisionBoxCollection()
                            {
                                CollisionBoxes = new List<CollisionBox>()
                                {
                                    mainHurtbox,
                                    new()
                                    {
                                        GrowHeight = false,
                                        GrowWidth = true,
                                        PosX = 0,
                                        PosY = 2,
                                        Height = 3,
                                        Width = 5,
                                    },
                                }
                            })
                        }
                    },

                    HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                    {
                        Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                        {
                            new(13, PlayerFSM.HurtType.Counter),
                            new(10, PlayerFSM.HurtType.Punish)
                        }
                    },
                };
            }


            FighterAction _JH;
            {
                int startup = 15;
                CollisionBox mainHurtbox = new CollisionBox()
                {
                    Width = 4,
                    Height = 4,
                    PosY = 3,
                    GrowHeight = false,
                    GrowWidth = false
                };
                    
                _JH = new FighterAction()
                {
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 149,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(4, 0),
                                new(4, 1),
                                new(startup - 8, 2),
                                // new(5, 3),
                                new(3, 4),
                                new(7, 5),
                                new(13, 6),
                            }
                        }
                    },

                    CancellableAfter = startup + 1,
                    CommandDirection = 5,
                    InputType = InputSystem.InputType.H,

                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(startup, null),
                            new(3, new Hit()
                            {
                                Type = Hit.HitType.High,
                                Level = 4,
                                TrajectoryXVelocity = 9,
                                TrajectoryHeight = 3,
                                GravityScaling = 1,
                                HitPushback = 3,
                                Damage = highDamage,
                                VisualAngle = -40,
                                DamageScaling = mediumScaling,
                                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                                {
                                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                    {
                                        new(20, new CollisionBoxCollection()
                                        {
                                            CollisionBoxes = new List<CollisionBox>()
                                            {
                                                new CollisionBox()
                                                {
                                                    GrowHeight = false,
                                                    GrowWidth = true,
                                                    Height = 4,
                                                    Width = 4,
                                                    PosX = 0,
                                                    PosY = FP.FromString("3.5"),
                                                }
                                            }
                                        })
                                    }
                                }
                            }),
                            new(20, null)
                        }
                    },

                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(startup, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>() {
                                        mainHurtbox
                                    }
                                }
                            ),
                            new(3, new CollisionBoxCollection()
                            {
                                CollisionBoxes = new List<CollisionBox>()
                                {
                                    mainHurtbox,
                                    new()
                                    {
                                        GrowHeight = false,
                                        GrowWidth = true,
                                        Height = 5,
                                        Width = 5,
                                        PosX = 0,
                                        PosY = FP.FromString("3.5"),
                                    },
                                }
                            }),
                            new(20, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>() {
                                        mainHurtbox
                                    }
                                }
                            ),
                        }
                    },

                    HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                    {
                        Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                        {
                            new(startup + 3, PlayerFSM.HurtType.Counter),
                            new(10, PlayerFSM.HurtType.Punish)
                        }
                    },
                };
            }


            FighterAction _JS;
            {
                CollisionBox mainHurtBox = new CollisionBox()
                {
                    Width = 5,
                    Height = 5,
                    GrowWidth = false,
                    GrowHeight = false,
                    PosX = 0,
                    PosY = FP.FromString("2.5"),
                };
                
                int startup = 18;
                int active = 4;
                Hit hit = new Hit()
                {
                    GravityScaling = FP.FromString("1.02"),
                    Level = 2,
                    TrajectoryHeight = 4,
                    TrajectoryXVelocity = 35,
                    HitPushback = 3,
                    WallBounce = true,
                    Damage = mediumDamage,
                    DamageScaling = mediumScaling,
                    Type = Hit.HitType.High,
                    HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new(100, new CollisionBoxCollection()
                            {
                                CollisionBoxes = new List<CollisionBox>()
                                {
                                    new CollisionBox()
                                    {
                                        GrowWidth = true,
                                        GrowHeight = false,
                                        Height = 7,
                                        Width = 5,
                                        PosX = 0,
                                        PosY = 2
                                    }
                                }
                            })
                        }
                    }
                };
                
                _JS = new FighterAction()
                {
                    InputType = InputSystem.InputType.S,
                    Animation = new FighterAnimation()
                    {
                        SpriteSheetOffset = 156,
                        SectionGroup = new SectionGroup<int>()
                        {
                            Sections = new List<Tuple<int, int>>()
                            {
                                new(4, 0),
                                new(4, 1),
                                new(4, 2),
                                new(startup - 12, 3),
                                new(active + 4, 4),
                                new(18, 6)
                            }
                        }
                    },
                    
                    
                    TrajectorySectionGroup = new SectionGroup<ActionTrajectory>()
                    {
                        Sections = new List<Tuple<int, ActionTrajectory>>()
                        {
                            new(startup, new ActionTrajectory()
                            {
                                TrajectoryHeight = FP.FromString("2"),
                                TimeToTrajectoryHeight = 9
                            })
                        }
                    },
                    
                    HitSectionGroup = new SectionGroup<Hit>()
                    {
                        Sections = new List<Tuple<int, Hit>>()
                        {
                            new(startup, null),
                            new (1, hit),
                            // new (active - 1, hit),
                            new(100, null)
                        }
                    },
                    
                    HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new (startup, new CollisionBoxCollection(){ CollisionBoxes = new List<CollisionBox>() {mainHurtBox}}),
                            new (active + 4, new CollisionBoxCollection(){ CollisionBoxes = new List<CollisionBox>()
                            {
                                mainHurtBox,
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = true,
                                    Width = FP.FromString("5.5"),
                                    Height = 8,
                                    PosX = 0,
                                    PosY = 2
                                }
                            }}),
                            new (100, new CollisionBoxCollection(){ CollisionBoxes = new List<CollisionBox>() {mainHurtBox}}),
                        }
                    }
                };
            }
            
            
            
            ActionDict = new Dictionary<PlayerFSM.State, FighterAction>
            {
                { PlayerFSM.State.Action1 , _5L},
                { PlayerFSM.State.Action2 , _2L},
                { PlayerFSM.State.Action3 , _5M},
                { PlayerFSM.State.Action4 , _2M},
                { PlayerFSM.State.Action5 , _2H},
                { PlayerFSM.State.Action6 , _5H},
                { PlayerFSM.State.Action7 , _5S1},
                { PlayerFSM.State.Action8 , _5S2},
                { PlayerFSM.State.Action9 , _5S3},
                { PlayerFSM.State.Action10 , _6S3},
                { PlayerFSM.State.Action11 , _2S},
                { PlayerFSM.State.Action12 , _2S},
                { PlayerFSM.State.Action13 , _JL},
                { PlayerFSM.State.Action14 , _JM},
                { PlayerFSM.State.Action15 , _JH},
                { PlayerFSM.State.Action16 , _JS},
            };

            InvulnerableBefore = new Dictionary<PlayerFSM.State, int>()
            {
                [PlayerFSM.State.Backdash] = 10
            };
            
        }

        public override void ConfigureCharacterFsm(PlayerFSM playerFsm)
        {
            var fsm = playerFsm.Fsm;
            
            // 5L
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action1);
            
            // 2L
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action2, false, true, 1);
            
            // 5M
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action3);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action3, PlayerFSM.State.Action4);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action3, PlayerFSM.State.Action5);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action3, PlayerFSM.State.Action7);
            
            // 2M
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action4, false, true, 1);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action4, PlayerFSM.State.Action5);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action4, PlayerFSM.State.Action7);
            
            // 2H
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action5, false, true, 1);
            
            // 5H
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action6, true);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action6, PlayerFSM.State.Action5);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action6, PlayerFSM.State.Action7);
            
            // 5S1
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action7);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action7, PlayerFSM.State.Action8);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action7, PlayerFSM.State.Action9, 1);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action7, PlayerFSM.State.Action10, 1);
            
            // 5S2
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action8, false, true, 0, false);
            
            // 4S3
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action9, false, false, 0, false);
            
            // 6S3
            ConfigureGroundAction(playerFsm, PlayerFSM.State.Action10, false, true, 0, false);
            
            // 2S - grounded
            ConfigureGroundToAirAction(playerFsm, PlayerFSM.State.Action11, false, 1, true);
            
            // 2S - air
            ConfigureAirAction(playerFsm, PlayerFSM.State.Action12, false, 1);
            
            // JL
            ConfigureAirAction(playerFsm, PlayerFSM.State.Action13);
            
            // JM
            ConfigureAirAction(playerFsm, PlayerFSM.State.Action14);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action14, PlayerFSM.State.Action15, 1);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action14, PlayerFSM.State.Action16, 1);
            
            // JH
            ConfigureAirAction(playerFsm, PlayerFSM.State.Action15);
            MakeActionCancellable(playerFsm, PlayerFSM.State.Action15, PlayerFSM.State.Action16, 1);
            
            // JS
            ConfigureAirAction(playerFsm, PlayerFSM.State.Action16);
        }
        
    }
}