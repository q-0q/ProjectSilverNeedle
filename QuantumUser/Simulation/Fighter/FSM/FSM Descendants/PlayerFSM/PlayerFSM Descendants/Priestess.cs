using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Photon.Deterministic;
using Quantum;
using Quantum.QuantumUser.Simulation.Fighter.Types;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe class PriestessFSM : PlayerFSM
    {
        public class PriestessState : PlayerState
        {
            public static int _5L;
            public static int _2L;
            public static int _5M;
            public static int _2M;
            public static int _5H;
            public static int _4H;
            public static int _2H;
            
            public static int ForwardThrowCutscene;
            public static int BackThrowCutscene;

            public static int JL;
            public static int JM;
            public static int JH;

            public static int SummonHigh;
            public static int SummonLow;
            public static int Return;
            public static int Teleport;
            public static int Fireball;
            
            public static int AirReturn;
            public static int AirTeleport;
            public static int AirFireball;
        }

        public Hit _5HHit;
        public Hit _2HHit;
        
        
        public PriestessFSM()
        {
            Name = "Priestess";
            StateType = typeof(PriestessState);
            JumpCount = 2;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 22;
            DamageDealtModifier = FP.FromString("0.85");
            DamageTakenModifier = FP.FromString("1.35");

            KinematicAttachPointOffset = new FPVector2(0, 3);
            

            MinimumDashDuration = 5;
            // SpriteScale = FP.FromString("1.2");

            UpwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = 18,
                TrajectoryXVelocity = 0,
                TrajectoryHeight = FP.FromString("7")
            };
            
            BackwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = 18,
                TrajectoryXVelocity = FP.FromString("-10"),
                TrajectoryHeight = FP.FromString("7")
            };
            
            ForwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = 18,
                TrajectoryXVelocity = FP.FromString("10"),
                TrajectoryHeight = FP.FromString("7")
            };

            JumpsquatDuration = 6;
            
            SummonPools = new List<SummonPool>()
            {
                new()
                {
                    Size = 1,
                    SummonFSMType = typeof(PriestessSetplayFSM)
                },
                new()
                {
                    Size = 1,
                    SummonFSMType = typeof(PriestessHorizontalFireballFSM)
                }
            };
            
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
            
            CollisionBox standHurtbox = new()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 8,
                Width = FP.FromString("4"),
            };

            CollisionBox crouchHurtbox = new()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 5,
                Width = FP.FromString("3.5"),
            };
            
            
            var standHurtboxCollectionSectionGroup = new SectionGroup<CollisionBoxCollection>()
            {
                Loop = true,
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new(10, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }
                    )
                }
            };
            
            var crouchHurtboxCollection = new SectionGroup<CollisionBoxCollection>()
            {
                Loop = true,
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new (10, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                crouchHurtbox
                            }
                        }
                    )
                }
            };

            var airHurtbox = new CollisionBox()
            {
                GrowHeight = false,
                GrowWidth = false,
                PosX = 0,
                PosY = 2,
                Height = 5,
                Width = 6,
            };
            var airHitHurtboxCollection = new SectionGroup<CollisionBoxCollection>()
            {
                Loop = true,
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new (10, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                airHurtbox
                            }
                        }
                    )
                }
            };
            
            var airHurtboxCollection = new SectionGroup<CollisionBoxCollection>()
            {
                Loop = true,
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new (10, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = false,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 3,
                                    Height = 4,
                                    Width = 7,
                                }
                            }
                        }
                    )
                }
            };
            
            StateMapConfig.HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Stand] = standHurtboxCollectionSectionGroup;
            StateMapConfig.HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Crouch] = crouchHurtboxCollection;
            StateMapConfig.HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Air] = airHitHurtboxCollection;
            StateMapConfig.HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.PlayerState.CutsceneReactor] = airHitHurtboxCollection;

            
            // Pushboxes

            var standPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 6,
                Width = FP.FromString("1.5"),
            };

            var crouchPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 4,
                Width = 2,
            };

            var airPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 1,
                Height = 3,
                Width = FP.FromString("1.5")
            };
            
            StateMapConfig.Pushbox.SuperDictionary[PlayerFSM.PlayerState.Stand] = standPushbox;
            StateMapConfig.Pushbox.SuperDictionary[PlayerFSM.PlayerState.Crouch] = crouchPushbox;
            StateMapConfig.Pushbox.SuperDictionary[PlayerFSM.PlayerState.Air] = airPushbox;
            StateMapConfig.Pushbox.SuperDictionary[PlayerFSM.PlayerState.HardKnockdown] = crouchPushbox;
            StateMapConfig.Pushbox.SuperDictionary[PlayerFSM.PlayerState.SoftKnockdown] = crouchPushbox;
            
            // Basic animations
            
            var standAnimation = new FighterAnimation()
            {
                Path = "Stand",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 1,
                    AutoFromAnimationPath = true
                }
            };
            
            var crouchAnimation = new FighterAnimation()
            {
                Path = "Crouch",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            var walkForwardAnimation = new FighterAnimation()
            {
                Path = "WalkForward",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 1,
                    AutoFromAnimationPath = true
                }
            };
            
            var walkBackwardAnimation = new FighterAnimation()
            {
                Path = "WalkBackward",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 1,
                    AutoFromAnimationPath = true,
                }
            };
            
            var jumpingAnimation = new FighterAnimation()
            {
                Path = "Jump",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var afterAirActionAnimation = new FighterAnimation()
            {
                Path = "JumpAfter",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var dashAnimation = new FighterAnimation()
            {
                Path = "Dash",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    AutoFromAnimationPath = true
                }
            };
            
            var backdashAnimation = new FighterAnimation()
            {
                Path = "Backdash", // temp
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var standHitHighAnimation = new FighterAnimation()
            {
                Path = "StandHitHigh",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var standHitLowAnimation = new FighterAnimation()
            {
                Path = "StandHitLow",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var crouchHitAnimation = new FighterAnimation()
            {
                Path = "CrouchHit",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var airHitHighAnimation = new FighterAnimation()
            {
                Path = "AirHit",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var airHitLowAnimation = new FighterAnimation()
            {
                Path = "AirHitLow",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var airHitLaunchAnimation = new FighterAnimation()
            {
                Path = "AirHitLaunch",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var wallBounceAnimation = new FighterAnimation()
            {
                Path = "WallBounce",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var groundBounceAnimation = new FighterAnimation()
            {
                Path = "GroundBounce",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var standBlockAnimation = new FighterAnimation()
            {
                Path = "StandBlock",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var breakAnimation = new FighterAnimation()
            {
                Path = "Break",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var proxStandBlockAnimation = new FighterAnimation()
            {
                Path = "StandProxBlock",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var crouchBlockAnimation = new FighterAnimation()
            {
                Path = "CrouchBlock",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var proxCrouchBlockAnimation = new FighterAnimation()
            {
                Path = "CrouchProxBlock",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var airBlockAnimation = new FighterAnimation()
            {
                Path = "AirBlock",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var hardKnockdownAnimation = new FighterAnimation()
            {
                Path = "HardKnockdown",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var softKnockdownAnimation = new FighterAnimation()
            {
                Path = "SoftKnockdown",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var deadFromGroundAnimation = new FighterAnimation()
            {
                Path = "DeadFromGround",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var deadFromAirAnimation = new FighterAnimation()
            {
                Path = "DeadFromAir",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var throwAnimation = new FighterAnimation()
            {
                Path = "Throw",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var jumpsquatAnimation = new FighterAnimation()
            {
                Path = "Jumpsquat",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var landsquatAnimation = new FighterAnimation()
            {
                Path = "Landsquat",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            
            Util.AutoSetupFromAnimationPath(standAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandActionable] = standAnimation;
            
            
            Util.AutoSetupFromAnimationPath(crouchAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchActionable] = crouchAnimation;
            
            Util.AutoSetupFromAnimationPath(walkForwardAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.WalkForward] = walkForwardAnimation;

            Util.AutoSetupFromAnimationPath(walkBackwardAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.WalkBackward] = walkBackwardAnimation;
            
            Util.AutoSetupFromAnimationPath(jumpingAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirActionable] = jumpingAnimation;
            
            Util.AutoSetupFromAnimationPath(afterAirActionAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirActionableAfterAction] = afterAirActionAnimation;
            //
            Util.AutoSetupFromAnimationPath(dashAnimation, this);
            StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Dash] = dashAnimation;
            StateMapConfig.Duration.SuperDictionary[PlayerFSM.PlayerState.Dash] = dashAnimation.SectionGroup.Duration();
            //
            //
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirDash] = dashAnimation;
            // StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.AirDash] = dashAnimation.SectionGroup.Duration();
            //
            Util.AutoSetupFromAnimationPath(backdashAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashAnimation;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashAnimation.SectionGroup.Duration();
            //
            Util.AutoSetupFromAnimationPath(standHitHighAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandHitHigh] = standHitHighAnimation;
            
            Util.AutoSetupFromAnimationPath(standHitLowAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandHitLow] = standHitLowAnimation;
            //
            Util.AutoSetupFromAnimationPath(crouchHitAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchHit] = crouchHitAnimation;
            //
            Util.AutoSetupFromAnimationPath(airHitHighAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitHigh] = airHitHighAnimation;
            StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.CutsceneReactor] = airHitHighAnimation;
            
            Util.AutoSetupFromAnimationPath(airHitLowAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitLow] = airHitLowAnimation;
            
            Util.AutoSetupFromAnimationPath(airHitLaunchAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitLaunch] = airHitLaunchAnimation;
            //
            Util.AutoSetupFromAnimationPath(wallBounceAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostWallBounce] = wallBounceAnimation;
            
            Util.AutoSetupFromAnimationPath(groundBounceAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostGroundBounce] = groundBounceAnimation;
            
            Util.AutoSetupFromAnimationPath(standBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandBlock] = standBlockAnimation;
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Tech] = standBlockAnimation;
            
            Util.AutoSetupFromAnimationPath(proxStandBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.ProxStandBlock] = proxStandBlockAnimation;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.ProxStandBlock] = 100;
            
            Util.AutoSetupFromAnimationPath(crouchBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchBlock] = crouchBlockAnimation;
            
            Util.AutoSetupFromAnimationPath(proxCrouchBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.ProxCrouchBlock] = proxCrouchBlockAnimation;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.ProxCrouchBlock] = 100;
            
            Util.AutoSetupFromAnimationPath(airBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirBlock] = airBlockAnimation;
            
            Util.AutoSetupFromAnimationPath(hardKnockdownAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.HardKnockdown] = hardKnockdownAnimation;
            
            Util.AutoSetupFromAnimationPath(softKnockdownAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.SoftKnockdown] = softKnockdownAnimation;
            
            Util.AutoSetupFromAnimationPath(deadFromGroundAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.DeadFromGround] = deadFromGroundAnimation;
            
            Util.AutoSetupFromAnimationPath(deadFromAirAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.DeadFromAir] = deadFromAirAnimation;
            
            Util.AutoSetupFromAnimationPath(throwAnimation, this);
            StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Throw] = throwAnimation;
            //
            Util.AutoSetupFromAnimationPath(jumpsquatAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Jumpsquat] = jumpsquatAnimation;
            //
            Util.AutoSetupFromAnimationPath(landsquatAnimation, this);
            StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Landsquat] = landsquatAnimation;
            //
            Util.AutoSetupFromAnimationPath(breakAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Break] = breakAnimation;
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.RedBreak] = breakAnimation;

            
            // Basic movement
            
            var walkForwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(30, 4))
                }
            };
            
            var walkBackwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(30, -4))
                }
            };

            var dashMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(8, 3),
                }
            };
            
            var backdashMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(3, 0),
                    new(15, -4),
                    new(5, FP.FromString("-0.25")),
                    new (10, 0),
                }
            };
            
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.WalkBackward] = walkBackwardMovement;
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.WalkForward] = walkForwardMovement;
            StateMapConfig.MovementSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Dash] = dashMovement;
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.AirDash] = dashMovement;
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashMovement;
            StateMapConfig.InvulnerableBefore.Dictionary[PlayerFSM.PlayerState.Backdash] = 12;
            
            ////////////////////////////////////////////////////////////////////////////////////

            
            {
                int startup = 9;
                int active = 2;
                int hurtboxDuration = 6;
                string path = "_5M";
                int state = PriestessState._5M;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };

                var move = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(startup - active, 0),
                        new(active, 1),
                        new (10, 0)
                    }
                };

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = FP.FromString("3.5"),
                                    Width = FP.FromString("8.25"),
                                    GrowWidth = true,
                                    GrowHeight = false,
                                    PosY = FP.FromString("4.75"),
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            Level = 3,
                            TrajectoryHeight = FP.FromString("1.5"),
                            TrajectoryXVelocity = 10,
                            BlockPushback = FP.FromString("2.5"),
                            HitPushback = FP.FromString("1.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("2.5"),
                            VisualHitPositionOffset = new FPVector2(10, 5),
                            Damage = 20,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (active, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                Height = 3,
                                                Width = FP.FromString("8.25"),
                                                GrowWidth = true,
                                                GrowHeight = false,
                                                PosY = FP.FromString("4.75"),
                                                PosX = 0
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };

                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(startup, -1),
                        new(6, 0),
                        // new(3, 2),
                        new(10, -1),
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 2;
            }
            
            {
                int startup = 5;
                int active = 2;
                int hurtboxDuration = 7;
                string path = "_5L";
                int state = PriestessState._5L;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = 2,
                                    Width = FP.FromString("5"),
                                    GrowWidth = true,
                                    GrowHeight = false,
                                    PosY = FP.FromString("4.75"),
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            Level = 1,
                            TrajectoryHeight = FP.FromString("2.25"),
                            TrajectoryXVelocity = 8,
                            BlockPushback = FP.FromString("3.5"),
                            HitPushback = FP.FromString("2.5"),
                            GravityScaling = FP.FromString("1.3"),
                            GravityProration = FP.FromString("1.5"),
                            VisualHitPositionOffset = new FPVector2(6, 5),
                            Damage = 20,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (active, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                Height = 2,
                                                Width = FP.FromString("5"),
                                                GrowWidth = true,
                                                GrowHeight = false,
                                                PosY = FP.FromString("4.75"),
                                                PosX = 0
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
            }
            
            {
                int startup = 8;
                int active = 2;
                int hurtboxDuration = 7;
                string path = "_2M";
                int state = PriestessState._2M;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };

                var move = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(startup - active, 0),
                        new(active, 1),
                        new (10, 0)
                    }
                };

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = 2,
                                    Width = FP.FromString("7.5"),
                                    GrowWidth = true,
                                    GrowHeight = true,
                                    PosY = 0,
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            Level = 1,
                            Launches = true,
                            TrajectoryHeight = FP.FromString("3"),
                            TrajectoryXVelocity = 15,
                            BlockPushback = FP.FromString("3.5"),
                            HitPushback = FP.FromString("2.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.3"),
                            VisualHitPositionOffset = new FPVector2(7, 1),
                            VisualAngle = 20,
                            Type = Hit.HitType.Low,
                            Damage = 20,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (active, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                Height = 2,
                                                Width = FP.FromString("7.5"),
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosY = 0,
                                                PosX = 0
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(startup, -1),
                        new(6, 2),
                        // new(3, 5),
                        new(10, -1),
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
            }
            
            {
                int startup = 7;
                int active = 2;
                int hurtboxDuration = 7;
                string path = "_2L";
                int state = PriestessState._2L;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };

                var move = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(startup - active, 0),
                        new(active, 1),
                        new (10, 0)
                    }
                };

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = FP.FromString("1.5"),
                                    Width = FP.FromString("2.5"),
                                    GrowWidth = true,
                                    GrowHeight = false,
                                    PosY = FP.FromString("4.75"),
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            Level = 1,
                            TrajectoryHeight = 2,
                            TrajectoryXVelocity = 8,
                            BlockPushback = FP.FromString("3.5"),
                            HitPushback = FP.FromString("2.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.1"),
                            VisualHitPositionOffset = new FPVector2(4, 1),
                            VisualAngle = 12,
                            Type = Hit.HitType.Low,
                            Damage = 20,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (active, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                Height = 2,
                                                Width = FP.FromString("4"),
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosY = 0,
                                                PosX = 0
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
            }
            
            {
                int startup = 24;
                int active = 2;
                int hurtboxDuration = 7;
                string path = "Summon";
                int state = PriestessState.SummonHigh;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            HitboxCollections = null // ghost hit
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                var summon = new SectionGroup<SummonPool>()
                {
                    Sections = new List<Tuple<int, SummonPool>>()
                    {
                        new (9, null),
                        new (1, SummonPools[0])
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() + 7;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.UnpoolSummonSectionGroup.Dictionary[state] = summon;
            }
            
            
            {
                int startup = 24;
                int active = 2;
                int hurtboxDuration = 7;
                string path = "SummonLow";
                int state = PriestessState.SummonLow;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            HitboxCollections = null // ghost hit
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                var summon = new SectionGroup<SummonPool>()
                {
                    Sections = new List<Tuple<int, SummonPool>>()
                    {
                        new (9, null),
                        new (1, SummonPools[0])
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() + 7;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.UnpoolSummonSectionGroup.Dictionary[state] = summon;
            }
            
            {
                int startup = 16;
                int active = 2;
                int hurtboxDuration = 7;
                string path = "Return";
                int state = PriestessState.Return;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            HitboxCollections = null // ghost hit
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() - 6;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
            }
            
            {
                int startup = 16;
                int active = 2;
                int hurtboxDuration = 7;
                string path = "AirReturn";
                int state = PriestessState.AirReturn;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            HitboxCollections = null // ghost hit
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                var trajectory = new SectionGroup<Trajectory>()
                {
                    Sections = new List<Tuple<int, Trajectory>>()
                    {
                        new(1, new Trajectory()
                        {
                            TimeToTrajectoryHeight = startup + 2,
                            TrajectoryHeight = 1,
                            TrajectoryXVelocity = 0
                        })
                    }
                };
                
                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() - 6;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                // StateMapConfig.TrajectorySectionGroup.Dictionary[state] = trajectory;
            }
            
            {
                int startup = 16;
                string path = "Teleport";
                int state = PriestessState.Teleport;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(1, new Hit()
                        {
                            HitboxCollections = null // ghost hit
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(2, 7),
                        new(2, 8),
                        new(2, 9),
                        new(1, 10),
                        new(1, 11),
                        new(8, 12),
                        new(5, 13),
                        new(5, 14),
                        new(5, -1),
                    }
                };
                
                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() - 1;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
            }
            
            {
                int startup = 16;
                string path = "Teleport";
                int state = PriestessState.AirTeleport;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(1, new Hit()
                        {
                            HitboxCollections = null // ghost hit
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(2, 7),
                        new(2, 8),
                        new(2, 9),
                        new(1, 10),
                        new(1, 11),
                        new(8, 12),
                        new(5, 13),
                        new(5, 14),
                        new(5, -1),
                    }
                };

                var trajectory = new SectionGroup<Trajectory>()
                {
                    Sections = new List<Tuple<int, Trajectory>>()
                    {
                        new(1, new Trajectory()
                        {
                            TimeToTrajectoryHeight = startup + 10,
                            TrajectoryHeight = 1,
                            TrajectoryXVelocity = 0
                        })
                    }
                };
                
                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() - 1;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
                StateMapConfig.TrajectorySectionGroup.Dictionary[state] = trajectory;
            }
            
            {
                int startup = 16;
                string path = "Fireball";
                int state = PriestessState.Fireball;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(1, new Hit()
                        {
                            HitboxCollections = null // ghost hit
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                
                var summon = new SectionGroup<SummonPool>()
                {
                    Sections = new List<Tuple<int, SummonPool>>()
                    {
                        new (1, null),
                        new (1, SummonPools[1])
                    }
                };
                
                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() + 5;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.UnpoolSummonSectionGroup.Dictionary[state] = summon;
            }
            
            
            {
                int startup = 16;
                int active = 2;
                int hurtboxDuration = 6;
                string path = "_5H";
                int state = PriestessState._5H;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };

                var move = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(startup - 2, 0),
                        new(2, 2),
                        new (10, 0)
                    }
                };

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = FP.FromString("7"),
                                    Width = FP.FromString("8"),
                                    GrowWidth = true,
                                    GrowHeight = true,
                                    PosY = FP.FromString("0"),
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };

                _5HHit = new Hit()
                {
                    Level = 3,
                    TrajectoryHeight = FP.FromString("6.8"),
                    TrajectoryXVelocity = 5,
                    BlockPushback = FP.FromString("2.5"),
                    HitPushback = FP.FromString("1.5"),
                    GravityScaling = FP.FromString("1"),
                    GravityProration = FP.FromString("1.4"),
                    GroundBounce = true,
                    VisualHitPositionOffset = new FPVector2(7, 5),
                    VisualAngle = 70,
                    VisualHitLarge = true,
                    Damage = 20,
                    ProxBlockDistance = 9,
                    // Launches = true,
                    HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new (active, new CollisionBoxCollection()
                            {
                                CollisionBoxes = new List<CollisionBox>()
                                {
                                    new CollisionBox()
                                    {
                                        Height = FP.FromString("7"),
                                        Width = FP.FromString("8"),
                                        GrowWidth = true,
                                        GrowHeight = true,
                                        PosY = FP.FromString("0"),
                                        PosX = 0
                                    }
                                }
                            })
                        }
                    }
                };
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, _5HHit),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };

                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(startup, -1),
                        new(8, 6),
                        new(10, -1),
                    }
                };
                


                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration() - 4;
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 6;
            }
            
            
            {
                int startup = 15;
                int active = 2;
                int hurtboxDuration = 6;
                string path = "_2H";
                int state = PriestessState._2H;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };

                var move = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(startup - 2, 0),
                        new(2, FP.FromString("0.5")),
                        new(9, 2),
                        new (10, 0)
                    }
                };

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = FP.FromString("7"),
                                    Width = FP.FromString("3"),
                                    GrowWidth = true,
                                    GrowHeight = true,
                                    PosY = FP.FromString("0"),
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };

                var hit = new Hit()
                {
                    Level = 3,
                    TrajectoryHeight = FP.FromString("7.8"),
                    TrajectoryXVelocity = 4,
                    BlockPushback = FP.FromString("2.5"),
                    HitPushback = FP.FromString("1.5"),
                    GravityScaling = FP.FromString("1"),
                    GravityProration = FP.FromString("3"),
                    VisualHitPositionOffset = new FPVector2(5, 5),
                    VisualAngle = -70,
                    VisualHitLarge = true,
                    Damage = 20,
                    ProxBlockDistance = 9,
                    Launches = true,
                    HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                    {
                        Sections = new List<Tuple<int, CollisionBoxCollection>>()
                        {
                            new (active, new CollisionBoxCollection()
                            {
                                CollisionBoxes = new List<CollisionBox>()
                                {
                                    new CollisionBox()
                                    {
                                        Height = FP.FromString("7"),
                                        Width = FP.FromString("3"),
                                        GrowWidth = true,
                                        GrowHeight = true,
                                        PosY = FP.FromString("0"),
                                        PosX = 0
                                    }
                                }
                            })
                        }
                    }
                };
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, hit),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };

                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(startup, -1),
                        new(4, 15),
                        new(10, -1),
                    }
                };
                
                var summon = new SectionGroup<SummonPool>()
                {
                    Sections = new List<Tuple<int, SummonPool>>()
                    {
                        new (16, null),
                        new (1, SummonPools[1])
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
                // StateMapConfig.UnpoolSummonSectionGroup.Dictionary[state] = summon;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 10;
            }
            
            
            
            {
                int startup = 10;
                int active = 2;
                int hurtboxDuration = 6;
                string path = "JM";
                int state = PriestessState.JM;
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };

                var move = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(startup - active, 0),
                        new(active, -1),
                        new (10, FP.FromString("-1.25"))
                    }
                };

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = FP.FromString("3.5"),
                                    Width = FP.FromString("8.25"),
                                    GrowWidth = true,
                                    GrowHeight = false,
                                    PosY = FP.FromString("4.75"),
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            Level = 3,
                            TrajectoryHeight = 2,
                            TrajectoryXVelocity = 10,
                            BlockPushback = FP.FromString("2.5"),
                            HitPushback = FP.FromString("1.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("2.5"),
                            VisualHitPositionOffset = new FPVector2(10, 5),
                            VisualAngle = 5,
                            Damage = 20,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (active, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            new CollisionBox()
                                            {
                                                Height = 3,
                                                Width = FP.FromString("8.25"),
                                                GrowWidth = true,
                                                GrowHeight = false,
                                                PosY = FP.FromString("4.75"),
                                                PosX = 0
                                            }
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };

                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(startup, -1),
                        new(6, 16),
                        // new(3, 2),
                        new(10, -1),
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 2;
            }
            
            
            {
                int startup = 7;
                int active = 2;
                int hurtboxDuration = 6;
                string path = "JL";
                int state = PriestessState.JL;

                var collisionBox = new CollisionBox()
                {
                    Height = FP.FromString("2.5"),
                    Width = FP.FromString("4.5"),
                    GrowWidth = true,
                    GrowHeight = false,
                    PosY = FP.FromString("0"),
                    PosX = 0
                };
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                collisionBox

                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            Level = 1,
                            TrajectoryHeight = 2,
                            TrajectoryXVelocity = 10,
                            BlockPushback = FP.FromString("2.5"),
                            HitPushback = FP.FromString("1.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("2.5"),
                            VisualHitPositionOffset = new FPVector2(5, 0),
                            VisualAngle = 20,
                            Damage = 20,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (active, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            collisionBox
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 2;
            }
            
            {
                int startup = 13;
                int active = 2;
                int hurtboxDuration = 6;
                string path = "JH";
                int state = PriestessState.JH;

                var collisionBox = new CollisionBox()
                {
                    Height = FP.FromString("6"),
                    Width = FP.FromString("4"),
                    GrowWidth = true,
                    GrowHeight = false,
                    PosY = FP.FromString("0"),
                    PosX = 0
                };
                
                var animation = new FighterAnimation()
                {
                    Path = path,
                    SectionGroup = new SectionGroup<int>()
                    {
                        AutoFromAnimationPath = true
                    }
                };
                
                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + active, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                collisionBox

                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox
                            }
                        }),
                    }
                };
                
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, new Hit()
                        {
                            Level = 3,
                            TrajectoryHeight = FP.FromString("3.75"),
                            TrajectoryXVelocity = 7,
                            BlockPushback = FP.FromString("2.5"),
                            HitPushback = FP.FromString("1.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("2.5"),
                            GroundBounce = true,
                            VisualHitPositionOffset = new FPVector2(5, 0),
                            VisualAngle = 70,
                            VisualHitLarge = true,
                            Damage = 20,
                            HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                            {
                                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                                {
                                    new (active, new CollisionBoxCollection()
                                    {
                                        CollisionBoxes = new List<CollisionBox>()
                                        {
                                            collisionBox
                                        }
                                    })
                                }
                            }
                        }),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(startup + active, HurtType.Counter),
                        new(20, HurtType.Punish)
                    }
                };
                
                var smear = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(startup, -1),
                        new(6, 17),
                        // new(3, 2),
                        new(10, -1),
                    }
                };
                

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.SmearFrame.Dictionary[state] = smear;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 2;
            }
            
            
        }
        
        public override void SetupMachine()
        {
            base.SetupMachine();
            
            Fsm.Configure(PlayerState.Dash)
                .PermitIf(Trigger.NotDash, PlayerState.StandActionable, MinimumDashDurationElapsed)
                .Permit(Trigger.Jump, PlayerState.Jumpsquat)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.ProxBlockHigh, PlayerState.ProxStandBlock, _ => true, -3)
                .PermitIf(PlayerTrigger.ProxBlockLow, PlayerState.ProxCrouchBlock, _ => true, -3)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(PlayerState.Stand)
                .SubstateOf(PlayerState.DirectionLocked)
                .SubstateOf(PlayerState.Ground);
            
            ActionConfig _5L = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.L,
                JumpCancellable = true,
                InputWeight = 0,
                RawOk = true,
                State = PriestessState._5L,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            ConfigureAction(this, _5L);
            
            ActionConfig _2L = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 2,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.L,
                JumpCancellable = false,
                InputWeight = 1,
                RawOk = true,
                State = PriestessState._2L,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, _2L);
            // MakeActionCancellable(this, _5L, _5L);
            MakeActionCancellable(this, _5L, _2L);
            // MakeActionCancellable(this, _2L, _5L);
            // MakeActionCancellable(this, _2L, _2L);
            
            ActionConfig _5M = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.M,
                JumpCancellable = false,
                InputWeight = 0,
                RawOk = true,
                State = PriestessState._5M,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, _5M);
            
            ActionConfig _2M = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 2,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.M,
                JumpCancellable = false,
                InputWeight = 1,
                RawOk = true,
                State = PriestessState._2M,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, _2M);
            MakeActionCancellable(this, _5M, _2M);
            
            ActionConfig _5H = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.H,
                JumpCancellable = false,
                InputWeight = 0,
                RawOk = true,
                IsSpecial = false,
                SpecialCancellable = false,
                State = PriestessState._5H,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, _5H);
            
            
            ActionConfig _2H = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 2,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.H,
                JumpCancellable = false,
                InputWeight = 1,
                RawOk = true,
                IsSpecial = false,
                SpecialCancellable = false,
                State = PriestessState._2H,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, _2H);
            
            
            ActionConfig JM = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = false,
                InputType = InputSystem.InputType.M,
                JumpCancellable = false,
                InputWeight = 0,
                RawOk = true,
                State = PriestessState.JM,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, JM);
            
                        
            ActionConfig JL = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = false,
                InputType = InputSystem.InputType.L,
                JumpCancellable = true,
                InputWeight = 0,
                RawOk = true,
                State = PriestessState.JL,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, JL);
            
            ActionConfig JH = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = false,
                InputType = InputSystem.InputType.H,
                JumpCancellable = true,
                InputWeight = 0,
                RawOk = true,
                State = PriestessState.JH,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, JH);
            
            
            
            ActionConfig Summon = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 2,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 1,
                RawOk = true,
                IsSpecial = true,
                State = PriestessState.SummonHigh,
                
                BonusClause = CanUseSummon,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, Summon);
            
            ActionConfig SummonLow = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 0,
                RawOk = true,
                IsSpecial = true,
                State = PriestessState.SummonLow,
                
                BonusClause = CanUseSummon,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, SummonLow);
            
            ActionConfig Fireball = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 4,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 3,
                RawOk = true,
                IsSpecial = true,
                State = PriestessState.Fireball,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, Fireball);
            
            ActionConfig Return = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 2,
                RawOk = true,
                IsSpecial = true,
                State = PriestessState.Return,
                
                BonusClause = CanActivateReturn,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, Return);
            
            ActionConfig AirReturn = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = false,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 2,
                RawOk = true,
                IsSpecial = true,
                State = PriestessState.AirReturn,
                
                BonusClause = CanActivateReturn,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, AirReturn);
            
            
            ActionConfig Teleport = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 6,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 3,
                RawOk = true,
                IsSpecial = true,
                State = PriestessState.Teleport,
                
                BonusClause = CanActivateTeleport,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, Teleport);
            
            ActionConfig AirTeleport = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 6,
                Crouching = false,
                DashCancellable = false,
                GroundOk = false,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 3,
                RawOk = true,
                IsSpecial = true,
                State = PriestessState.AirTeleport,
                
                BonusClause = CanActivateTeleport,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, AirTeleport);
        }
        
        private bool CanActivateReturn(TriggerParams? triggerParams)
        {
            foreach (var setplayEntity in SummonPools[0].EntityRefs)
            {
                var machine = FsmLoader.FSMs[setplayEntity].Fsm;
                if (machine.IsInState(PriestessSetplayFSM.PriestessSetplayState.Destroy)) return false;
                if (machine.IsInState(PriestessSetplayFSM.PriestessSetplayState.Return)) return false; 
                if (machine.IsInState(SummonFSM.SummonState.Unpooled)) return true;
            }
            return false;
        }
        
        private bool CanActivateTeleport(TriggerParams? triggerParams)
        {
            return CanActivateReturn(triggerParams);
            // foreach (var setplayEntity in SummonPools[0].EntityRefs)
            // {
            //     var machine = FsmLoader.FSMs[setplayEntity].Fsm;
            //     return (machine.IsInState(PriestessSetplayFSM.PriestessSetplayState.Return));
            // }
            // return false;
        }
        
        private bool CanUseSummon(TriggerParams? triggerParams)
        {
            foreach (var setplayEntity in SummonPools[0].EntityRefs)
            {
                var machine = FsmLoader.FSMs[setplayEntity].Fsm;
                if (machine.IsInState(SummonFSM.SummonState.Pooled)) return true;
            }
            return false;
        }
    }
    
}



        
