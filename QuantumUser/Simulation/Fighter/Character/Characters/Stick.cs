using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;

namespace Quantum
{
    public class Stick : Character
    {
        
        public Stick()
        {
            Name = "Stick";
            
            WalkForwardSpeed = FP.FromString("18");
            WalkBackwardSpeed = FP.FromString("6");

            JumpHeight = FP.FromString("7");
            JumpTimeToHeight = 25;
            JumpForwardSpeed = FP.FromString("10");
            JumpBackwardSpeed = FP.FromString("7");
            JumpCount = 2;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 20;

            KinematicAttachPointOffset = new FPVector2(0, 3);
            
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
                        Height = 8,
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
                    LengthScalar = 6,
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
                    LengthScalar = 10,
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
                SpriteSheetOffset = 48,
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
                SpriteSheetOffset = 49,
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
                SpriteSheetOffset = 54,
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
                SpriteSheetOffset = 54,
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
                SpriteSheetOffset = 54,
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
                SpriteSheetOffset = 52,
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
                SpriteSheetOffset = 58,
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
                SpriteSheetOffset = 58,
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
                SpriteSheetOffset = 97,
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
                SpriteSheetOffset = 92,
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
                SpriteSheetOffset = 75,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(5, 0),
                        new(13, 1),
                    }
                }
            };

            DashMovementSectionGroup = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(8, 0),
                    new(7, 7), //7
                    new(3, 0),
                }
            };
            
            BackdashAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 88,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(9, 0),
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
                    new(9, 0),
                    new(7, -4),
                    new(9, 0),
                }
            };
            AirBackdashAnimation = BackdashAnimation;

            ThrowStartupAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 77,
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
                    SpriteSheetOffset = 78,
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
                    SpriteSheetOffset = 83,
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
                            
                        }),
                        new (20, null)
                    }
                }
                
            };
            
            ThrowWhiffAnimation = new FighterAnimation()
            {
                SpriteSheetOffset = 82,
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0),
                    }
                }
            };
            
            
            FighterAction needle = new FighterAction()
            {
                InputType = InputSystem.InputType.P,
                CancellableAfter = 15,
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 39,
                    SectionGroup = new SectionGroup<int>()
                    {
                        LengthScalar = 2,
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(2, 0),
                            new(1, 1),
                            new(4, 2),
                            new(4, 3),
                            new(3, 4),
                            new(2, 5),
                            new(2, 6),
                        }
                    }
                },
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (4, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 5,
                                    Width = 8,
                                }
                            }
                        }),
                        new (18, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = -3,
                                    PosY = 0,
                                    Height = 4,
                                    Width = 7,
                                }
                            }
                        }),
                        new (8, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 1,
                                    PosY = 0,
                                    Height = 4,
                                    Width = 8,
                                },
                                new()
                                {
                                    GrowWidth = true,
                                    GrowHeight = true,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = FP.FromString("2.5"),
                                    Width = 9,
                                }
                            }
                        })
                    }
                },
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(22, null),
                        new(5, new Hit()
                        {
                            BlockPushback = 0,
                            Type = Hit.HitType.Low,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(3, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new()
                                            {
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosX = 0,
                                                PosY = 0,
                                                Height = 2,
                                                Width = 10,
                                            }
                                        }
                                    }),
                                    new(2, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new()
                                            {
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosX = 0,
                                                PosY = 0,
                                                Height = 2,
                                                Width = FP.FromString("10.5"),
                                            }
                                        }
                                    }),
                                    new(1, null)
                                }
                            }
                        }),
                        new(5, null)
                        
                    }
                }
                ,HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(27, PlayerFSM.HurtType.Counter),
                        new(50, PlayerFSM.HurtType.Punish)
                    }
                }
                
            };
            
            FighterAction flip = new FighterAction()
            {
                InputType = InputSystem.InputType.P,
                CommandDirection = 6,
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 24,
                    SectionGroup = new SectionGroup<int>()
                    {
                        LengthScalar = 2,
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(2, 0),
                            new(2, 1),
                            new(6, 2),
                            new(4, 3),
                            new(6, 4),
                        }
                    }
                },
                
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new (50, new CollisionBoxCollection()
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
                                    Width = 8,
                                }
                            }
                        })
                    }
                },
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(20, null),
                        new(8, new Hit()
                        {
                            Type = Hit.HitType.High,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(6, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new()
                                            {
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosX = 0,
                                                PosY = 0,
                                                Height = 6,
                                                Width = 4,
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (50, null)
                    }
                }
                
            };

            FighterAction poke = new FighterAction()
            {
                CancellableAfter = 26,
                InputType = InputSystem.InputType.K,
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 18,
                    SectionGroup = new SectionGroup<int>()
                    {
                        LengthScalar = 4,
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(1,0),
                            new(4,1),
                            new(2,2),
                            new(4,3),
                            new(1,4),
                            new(1,5),
                        }
                    }
                },
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    LengthScalar = 4,
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(5, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = true,
                                    Height = 6,
                                    Width = 5,
                                    PosX = -3
                                }
                            }
                        }),
                        new(2, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = true,
                                    Height = 6,
                                    Width = 6,
                                    PosX = -2
                                },
                                new CollisionBox()
                                {
                                    GrowWidth = true,
                                    Height = FP.FromString("2.5"),
                                    Width = FP.FromString("8.5"),
                                    PosY = FP.FromString("2.5")
                                },
                                new CollisionBox()
                                {
                                    GrowWidth = true,
                                    Height = FP.FromString("2.5"),
                                    Width = FP.FromString("5.5"),
                                    PosY = FP.FromString("3.5"),
                                    PosX = 7
                                },
                            }
                        }),
                        new(50, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = true,
                                    Height = 6,
                                    Width = 6,
                                    PosX = -2
                                }
                            }
                        })
                    }
                },
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(20, null),
                        new(8, new Hit()
                        {
                            // HardKnockdown = true,
                            TrajectoryHeight = 2,
                            TrajectoryXVelocity = 28,
                            VisualAngle = FP.FromString("-12"),
                            Launches = true,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(8, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 8,
                                                PosY = FP.FromString("2.5")
                                            },
                                            new CollisionBox()
                                            {
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 5,
                                                PosY = FP.FromString("3.5"),
                                                PosX = 7
                                            },
                                        }
                                    })
                                }
                            }
                        }),
                        new(50, null)
                    }
                },
                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(28, PlayerFSM.HurtType.Counter),
                        new(50, PlayerFSM.HurtType.Punish)
                    }
                },
                MovementSectionGroup = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(5, -1),
                        new(13, 0),
                        new(5, 2),
                        new(20, 0)
                    }
                }
            };


            FighterAction knee = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 64,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(7, 0),
                            new(9, 1),
                            new(9, 2),
                            new(9, 3),
                        }
                    }
                },
                CancellableAfter = 12,
                InputType = InputSystem.InputType.H,
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(7, null),
                        new(5, new Hit()
                        {
                            VisualAngle = -55,
                            Type = Hit.HitType.Mid,
                            TrajectoryHeight = 4,
                            TrajectoryXVelocity = 2,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(5, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = true,
                                                GrowWidth = true,
                                                Height = 5,
                                                Width = 2,
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new(50, null)
                    }
                },
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(50, StandHurtboxesCollection)
                    }
                },
                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(12, PlayerFSM.HurtType.Counter),
                        new(50, PlayerFSM.HurtType.Punish)
                    }
                }
            };
            
            
            FighterAction launcher = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 68,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(5, 0),
                            new(6, 1),
                            new(10, 2),
                            new(8, 3),
                            new(8, 1),
                        }
                    }
                },
                CancellableAfter = 26,
                InputType = InputSystem.InputType.H,
                CommandDirection = 2,
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(11, null),
                        new(5, new Hit()
                        {
                            VisualAngle = -70,
                            Type = Hit.HitType.Mid,
                            TrajectoryHeight = 8,
                            TrajectoryXVelocity = 5,
                            Launches = true,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(5, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = true,
                                                GrowWidth = true,
                                                Height = FP.FromString("6.5"),
                                                Width = 4,
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new(50, null)
                    }
                },
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(50, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = true,
                                    Height = 6,
                                    Width = 6,
                                    PosX = -2
                                }
                            }
                        })
                    }
                },
                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(16, PlayerFSM.HurtType.Counter),
                        new(50, PlayerFSM.HurtType.Punish)
                    }
                }
            };
            
            FighterAction airJab = new FighterAction()
            {
                Animation = new FighterAnimation()
                {
                    SpriteSheetOffset = 72,
                    SectionGroup = new SectionGroup<int>()
                    {
                        Sections = new List<Tuple<int, int>>()
                        {
                            new(5, 0),
                            new(4, 1),
                            new(4, 2),
                        }
                    }
                },
                InputType = InputSystem.InputType.H,
                HitSectionGroup = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(5, null),
                        new(5, new Hit()
                        {
                            VisualAngle = 10,
                            Type = Hit.HitType.High,
                            TrajectoryHeight = 2,
                            TrajectoryXVelocity = 8,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new(5, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                GrowHeight = false,
                                                GrowWidth = true,
                                                Height = 2,
                                                Width = 4,
                                                PosY = 3
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new(50, null)
                    }
                },
                HurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(5, new CollisionBoxCollection()
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
                        }),
                        new (50, new CollisionBoxCollection() {
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
                                new()
                                {
                                    GrowHeight = false,
                                    GrowWidth = true,
                                    Height = 3,
                                    Width = 5,
                                    PosY = 3,
                                },
                            }
                        })
                    }
                },
                HurtTypeSectionGroup = new SectionGroup<PlayerFSM.HurtType>()
                {
                    Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                    {
                        new(5, PlayerFSM.HurtType.Counter),
                        new(50, PlayerFSM.HurtType.Punish)
                    }
                }
            };
                
            ActionDict = new Dictionary<PlayerFSM.State, FighterAction>
            {
                [PlayerFSM.State.Action1] = needle,
                [PlayerFSM.State.Action2] = needle,
                [PlayerFSM.State.Action3] = flip,
                [PlayerFSM.State.Action4] = poke,
                [PlayerFSM.State.Action5] = knee,
                [PlayerFSM.State.Action6] = launcher,
                [PlayerFSM.State.Action7] = airJab
            };

            InvulnerableBefore = new Dictionary<PlayerFSM.State, int>()
            {
                [PlayerFSM.State.Backdash] = 10
            };
        }

        public override void ConfigureCharacterFsm(PlayerFSM playerFsm)
        {
            // var fsm = playerFsm.Fsm;
            //
            // // Punch - ground
            // ConfigureGroundAction(PlayerFSM.State.Action1, true);
            // MakeActionCancellable(PlayerFSM.State.Action1, PlayerFSM.State.Action3, TODO);
            //
            // // Punch - air
            // ConfigureAirAction(PlayerFSM.State.Action2, TODO);
            // //MakeJumpDashCancellable(PlayerFSM.State.Action2);
            //
            // // Flip
            // ConfigureGroundAction(PlayerFSM.State.Action3, false, false, 1);
            //
            // // Poke
            // ConfigureGroundAction(PlayerFSM.State.Action4, true);
            // MakeActionCancellable(PlayerFSM.State.Action4, PlayerFSM.State.Action1, TODO);
            // MakeActionCancellable(PlayerFSM.State.Action4, PlayerFSM.State.Action5, TODO);
            //
            // // Knee
            // ConfigureGroundAction(PlayerFSM.State.Action5);
            // MakeActionCancellable(PlayerFSM.State.Action5, PlayerFSM.State.Action6, TODO);
            // MakeActionCancellable(PlayerFSM.State.Action5, PlayerFSM.State.Action4, TODO);
            //
            // // Launcher
            // ConfigureGroundAction(PlayerFSM.State.Action6, true, false, 1);
            //
            // // Airjab
            // ConfigureAirAction(PlayerFSM.State.Action7, TODO, true);

        }
        
    }
}