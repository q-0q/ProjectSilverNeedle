using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.QuantumUser.Simulation.Fighter.Types;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public class StickTwoFSM : PlayerFSM
    {
        public class StickTwoState : PlayerState
        {
            public static int _5L;
            public static int _2L;
            public static int _5M;
            public static int _2M;
            public static int _5H;
            public static int _4H;
            public static int _2H;
            public static int _5S1;
            public static int _5S2;
            public static int _5S3;
            public static int _6S3;
            public static int _2S_ground;
            public static int _2S_air;
            public static int _JL;
            public static int _JM;
            public static int _JH;
            public static int _JS;
            public static int ForwardThrowCutscene;
            public static int BackThrowCutscene;
            public static int Fireball;
        }

        public enum StickTwoAnimationPath
        {
            StandActionable,
            CrouchActionable,
            WalkForward,
            WalkBackward,
            Jump,
            Dash,
            Backdash,
            Airdash,
            AirBackdash,
            Throw,
            ForwardThrowCutscene,
            BackThrowCutscene,

            StandHitHigh,
            StandHitLow,
            CrouchHit,
            AirHit,
            WallBounce,
            GroundBounce,
            StandBlock,
            CrouchBlock,
            AirBlock,
            HardKnockdown,
            SoftKnockdown,
            DeadFromGround,
            DeadFromAir,
            
            Landsquat,
            ThrowTech,
            
            _5P,
            _2P,
            _5M,
            _2M,
            _4H,
            _5H,
            _2H,
            Fireball,

        }
        
        public class StickTwoCutscenes : PlayerFSM.CutsceneIndexes
        {
            // public static int Test;
        }

        public StickTwoFSM()
        {
            Name = "StickTwo";
            StateType = typeof(StickTwoState);
            AnimationPathsEnum = typeof(StickTwoAnimationPath);
            
            JumpCount = 2;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 20;

            KinematicAttachPointOffset = new FPVector2(0, 3);
            
            var jumpHeight = FP.FromString("8");
            var jumpTimeToHeight = 25;
            var jumpForwardSpeed = FP.FromString("10");
            var jumpBackwardSpeed = FP.FromString("-7");

            UpwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = jumpTimeToHeight,
                TrajectoryXVelocity = 0,
                TrajectoryHeight = jumpHeight
            };
            
            ForwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = jumpTimeToHeight,
                TrajectoryXVelocity = jumpForwardSpeed,
                TrajectoryHeight = jumpHeight
            };
            
            BackwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = jumpTimeToHeight,
                TrajectoryXVelocity = jumpBackwardSpeed,
                TrajectoryHeight = jumpHeight
            };

            SummonPools = new List<SummonPool>()
            {
                new()
                {
                    Size = 2,
                    SummonFSMType = typeof(FireballFSM)
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
                Height = 7,
                Width = 3,
            };

            CollisionBox crouchHurtbox = new()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 5,
                Width = 3,
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
            
            var airHitHurtboxCollection = new SectionGroup<CollisionBoxCollection>()
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
                                    PosY = 2,
                                    Height = 4,
                                    Width = 5,
                                }
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
            // StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[PlayerFSM.PlayerState.Air] = airHitHurtboxCollection;
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
                Path = (int)StickTwoAnimationPath.StandActionable,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            var crouchAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.CrouchActionable,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            var walkForwardAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.WalkForward,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            var walkBackwardAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.WalkBackward,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true,
                }
            };
            
            var jumpingAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.Jump,
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };

            var dashAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.Dash,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var backdashAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.Backdash,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var standHitHighAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.StandHitHigh,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var standHitLowAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.StandHitLow,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var crouchHitAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.CrouchHit,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var airHitAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.AirHit,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var wallBounceAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.WallBounce,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var groundBounceAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.GroundBounce,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var standBlockAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.StandBlock,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var crouchBlockAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.CrouchBlock,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var airBlockAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.AirBlock,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var hardKnockdownAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.HardKnockdown,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var softKnockdownAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.SoftKnockdown,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var deadFromGroundAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.DeadFromGround,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var throwAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.Throw,
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
            
            Util.AutoSetupFromAnimationPath(dashAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Dash] = dashAnimation;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.Dash] = dashAnimation.SectionGroup.Duration();
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirDash] = dashAnimation;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.AirDash] = dashAnimation.SectionGroup.Duration();
            
            Util.AutoSetupFromAnimationPath(backdashAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashAnimation;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashAnimation.SectionGroup.Duration();
            
            Util.AutoSetupFromAnimationPath(standHitHighAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandHitHigh] = standHitHighAnimation;
            
            Util.AutoSetupFromAnimationPath(standHitLowAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandHitLow] = standHitLowAnimation;
            
            Util.AutoSetupFromAnimationPath(crouchHitAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchHit] = crouchHitAnimation;
            
            Util.AutoSetupFromAnimationPath(airHitAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHit] = airHitAnimation;
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostGroundBounce] = airHitAnimation;
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CutsceneReactor] = airHitAnimation;
            
            Util.AutoSetupFromAnimationPath(wallBounceAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostWallBounce] = wallBounceAnimation;
            
            Util.AutoSetupFromAnimationPath(groundBounceAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostGroundBounce] = groundBounceAnimation;
            
            Util.AutoSetupFromAnimationPath(standBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandBlock] = standBlockAnimation;

            Util.AutoSetupFromAnimationPath(crouchBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchBlock] = crouchBlockAnimation;
            
            Util.AutoSetupFromAnimationPath(airBlockAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirBlock] = airBlockAnimation;
            
            Util.AutoSetupFromAnimationPath(hardKnockdownAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.HardKnockdown] = hardKnockdownAnimation;
            
            Util.AutoSetupFromAnimationPath(softKnockdownAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.SoftKnockdown] = softKnockdownAnimation;
            
            Util.AutoSetupFromAnimationPath(deadFromGroundAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.DeadFromGround] = deadFromGroundAnimation;
            
            Util.AutoSetupFromAnimationPath(throwAnimation, this);
            StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Throw] = throwAnimation;
            
            // Basic movement
            
            var walkForwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(Util.GetAnimationPathLength(this, walkForwardAnimation.Path), 3))
                }
            };
            
            var walkBackwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(Util.GetAnimationPathLength(this, walkBackwardAnimation.Path), -2))
                }
            };

            var dashMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(3, 0),
                    new(8, 4),
                    new(8, 1),
                    new(12, FP.FromString("0.6")),
                    new (10, 0),
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
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.Dash] = dashMovement;
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.AirDash] = dashMovement;
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashMovement;
            StateMapConfig.InvulnerableBefore.Dictionary[PlayerFSM.PlayerState.Backdash] = 12;
            
            ////////////////////////////////////////////////////////////////////////////////////

            
            var _5MAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath._5M,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var _5MHits = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new Tuple<int, Hit>(10, null),
                    new Tuple<int, Hit>(5, new Hit()
                    {
                        Level = 1,
                        GravityScaling = FP.FromString("1"),
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new Tuple<int, CollisionBoxCollection>(10, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowHeight = false,
                                            GrowWidth = true,
                                            Width = 4,
                                            Height = 2,
                                            PosX = 0,
                                            PosY = 5,
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new Tuple<int, Hit>(10, null)
                }
            };

            var _5MHurtTypes = new SectionGroup<PlayerFSM.HurtType>()
            {
                Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                {
                    new(15, PlayerFSM.HurtType.Counter),
                    new(20, PlayerFSM.HurtType.Punish)
                }
            };

            var _5MHurtboxes = new SectionGroup<CollisionBoxCollection>()
            {
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new (10, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            standHurtbox
                        }
                    }),
                    new (12, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            standHurtbox,
                            new CollisionBox()
                            {
                                GrowHeight = false,
                                GrowWidth = true,
                                Width = FP.FromString("4.5"),
                                Height = 3,
                                PosX = 0,
                                PosY = 5,
                            }
                        }
                    }),
                    new (10, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            standHurtbox
                        }
                    }),
                }
            };


            
            Util.AutoSetupFromAnimationPath(_5MAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[StickTwoState._5M] = _5MAnimation;
            StateMapConfig.Duration.Dictionary[StickTwoState._5M] = _5MAnimation.SectionGroup.Duration();
            StateMapConfig.HitSectionGroup.Dictionary[StickTwoState._5M] = _5MHits;
            StateMapConfig.CancellableAfter.Dictionary[StickTwoState._5M] = 14;
            StateMapConfig.WhiffCancellable.Dictionary[StickTwoState._5M] = false;
            StateMapConfig.HurtTypeSectionGroup.Dictionary[StickTwoState._5M] = _5MHurtTypes;
            StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[StickTwoState._5M] = _5MHurtboxes;
            
            Cutscene forwardThrowCutscene = new Cutscene()
            {
                InitiatorState = StickTwoState.ForwardThrowCutscene,
                ReactorDuration = 33,
                ReactorPositionSectionGroup = new SectionGroup<FPVector2>()
                {
                    Sections = new List<Tuple<int, FPVector2>>()
                    {
                        new (8, new FPVector2(FP.FromString("2.5"), FP.FromString("7.5"))),
                        new (12, new FPVector2(FP.FromString("2.5"), 8)),
                        new (10, new FPVector2(FP.FromString("2.5"), FP.FromString("7"))),
                        new (15, new FPVector2(3, 0)),
                    }
                }
            };

            Cutscenes[PlayerFSM.CutsceneIndexes.ForwardThrow] = forwardThrowCutscene;
            
            Cutscene backThrowCutscene = new Cutscene()
            {
                InitiatorState = StickTwoState.BackThrowCutscene,
                ReactorDuration = 55,
                ReactorPositionSectionGroup = new SectionGroup<FPVector2>()
                {
                    Sections = new List<Tuple<int, FPVector2>>()
                    {
                        new (8, new FPVector2(FP.FromString("2.5"), FP.FromString("7.5"))),
                        new (20, new FPVector2(FP.FromString("2.5"), 8)),
                        new (13, new FPVector2(FP.FromString("0.1"), FP.FromString("7.5"))),
                        new (15, new FPVector2(-4, 0)),
                    }
                }
            };

            Cutscenes[PlayerFSM.CutsceneIndexes.BackwardThrow] = backThrowCutscene;
            
            
            var _2MAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath._2M,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var _2MHits = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new Tuple<int, Hit>(10, null),
                    new Tuple<int, Hit>(5, new Hit()
                    {
                        Level = 2,
                        Type = Hit.HitType.Low,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new Tuple<int, CollisionBoxCollection>(2, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowHeight = false,
                                            GrowWidth = true,
                                            Width = 5,
                                            Height = 3,
                                            PosX = 0,
                                            PosY = 1
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new Tuple<int, Hit>(10, null)
                }
            };
            
            var _2MHurtboxes = new SectionGroup<CollisionBoxCollection>()
            {
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new(10, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            crouchHurtbox
                        }
                    }),
                    new(20, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            crouchHurtbox,
                            new CollisionBox()
                            {
                                GrowHeight = false,
                                GrowWidth = true,
                                Width = 6,
                                Height = 4,
                                PosX = 0,
                                PosY = 1
                            }
                        }
                    })
                }
            };

            var _2MMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new Tuple<int, FP>(2, 1),
                    new Tuple<int, FP>(10, 0),
                    new Tuple<int, FP>(5, 1),
                    new Tuple<int, FP>(10, 0)
                }
            };
            
            var _2MHurtTypes = new SectionGroup<PlayerFSM.HurtType>()
            {
                Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                {
                    new(21, PlayerFSM.HurtType.Counter),
                    new(20, PlayerFSM.HurtType.Punish)
                }
            };
            
            Util.AutoSetupFromAnimationPath(_2MAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[StickTwoState._2M] = _2MAnimation;
            StateMapConfig.Duration.Dictionary[StickTwoState._2M] = _2MAnimation.SectionGroup.Duration();
            StateMapConfig.HitSectionGroup.Dictionary[StickTwoState._2M] = _2MHits;
            StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[StickTwoState._2M] = _2MHurtboxes;
            StateMapConfig.CancellableAfter.Dictionary[StickTwoState._2M] = 16;
            StateMapConfig.WhiffCancellable.Dictionary[StickTwoState._2M] = false;
            StateMapConfig.MovementSectionGroup.Dictionary[StickTwoState._2M] = _2MMovement;
            StateMapConfig.HurtTypeSectionGroup.Dictionary[StickTwoState._2M] = _2MHurtTypes;


            var _2HAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath._2H,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var _2HHits = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new Tuple<int, Hit>(13, null),
                    new Tuple<int, Hit>(4, new Hit()
                    {
                        Level = 3,
                        Type = Hit.HitType.Mid,
                        Launches = true,
                        TrajectoryHeight = 6,
                        TrajectoryXVelocity = 4,
                        GravityScaling = 1,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new Tuple<int, CollisionBoxCollection>(2, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowHeight = false,
                                            GrowWidth = false,
                                            Width = 2,
                                            Height = 5,
                                            PosX = 1,
                                            PosY = 3,
                                        }
                                    }
                                }),
                                new Tuple<int, CollisionBoxCollection>(2, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowHeight = false,
                                            GrowWidth = false,
                                            Width = 2,
                                            Height = 4,
                                            PosX = 0,
                                            PosY = 6
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new Tuple<int, Hit>(10, null)
                }
            };
            
            Util.AutoSetupFromAnimationPath(_2HAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[StickTwoState._2H] = _2HAnimation;
            StateMapConfig.Duration.Dictionary[StickTwoState._2H] = _2HAnimation.SectionGroup.Duration();
            StateMapConfig.HitSectionGroup.Dictionary[StickTwoState._2H] = _2HHits;

            var frontThrowCutsceneAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.ForwardThrowCutscene,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var frontThrowCutsceneHit = new Hit()
            {
                Level = 0,
                Type = Hit.HitType.Mid,
                HardKnockdown = true,
                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new Tuple<int, CollisionBoxCollection>(8, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = false,
                                    Width = 2,
                                    Height = 2,
                                    PosX = 2,
                                    PosY = 6
                                }
                            }
                        })
                    }
                }
            };
            var frontThrowCutsceneHits = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new Tuple<int, Hit>(15, null),
                    new Tuple<int, Hit>(2, frontThrowCutsceneHit),
                    new Tuple<int, Hit>(2, frontThrowCutsceneHit),
                    new Tuple<int, Hit>(2, frontThrowCutsceneHit),
                    new Tuple<int, Hit>(20, null)
                }
            };
            
            Util.AutoSetupFromAnimationPath(frontThrowCutsceneAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[StickTwoState.ForwardThrowCutscene] = frontThrowCutsceneAnimation;
            StateMapConfig.HitSectionGroup.Dictionary[StickTwoState.ForwardThrowCutscene] = frontThrowCutsceneHits;
            StateMapConfig.Duration.Dictionary[StickTwoState.ForwardThrowCutscene] = frontThrowCutsceneAnimation.SectionGroup.Duration();
            
            
            
            var backThrowCutsceneAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.BackThrowCutscene,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var backThrowCutsceneHit = new Hit()
            {
                Level = 0,
                Type = Hit.HitType.Mid,
                HardKnockdown = true,
                HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new Tuple<int, CollisionBoxCollection>(8, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = false,
                                    Width = 2,
                                    Height = 2,
                                    PosX = 2,
                                    PosY = 6
                                }
                            }
                        })
                    }
                }
            };
            var backThrowCutsceneHits = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new Tuple<int, Hit>(15, null),
                    new Tuple<int, Hit>(2, frontThrowCutsceneHit),
                    new Tuple<int, Hit>(2, frontThrowCutsceneHit),
                    new Tuple<int, Hit>(2, frontThrowCutsceneHit),
                    new Tuple<int, Hit>(20, null)
                }
            };
            
            Util.AutoSetupFromAnimationPath(backThrowCutsceneAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[StickTwoState.BackThrowCutscene] = backThrowCutsceneAnimation;
            StateMapConfig.HitSectionGroup.Dictionary[StickTwoState.ForwardThrowCutscene] = backThrowCutsceneHits;
            StateMapConfig.Duration.Dictionary[StickTwoState.BackThrowCutscene] = backThrowCutsceneAnimation.SectionGroup.Duration();



            var fireballAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.Fireball,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            var fireballSummon = new SectionGroup<SummonPool>()
            {
                Sections = new List<Tuple<int, SummonPool>>()
                {
                    new(0, null),
                    new(3, SummonPools[0]), 
                    new(10, null)
                }
            };

            var fireballHurtTypes = new SectionGroup<HurtType>()
            {
                Sections = new List<Tuple<int, HurtType>>()
                {
                    new(24, HurtType.Counter),
                    new(24, HurtType.Punish)
                }
            };

            var fireballHurtboxes = new SectionGroup<CollisionBoxCollection>()
            {
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new(20, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            standHurtbox
                        }
                    }),
                    new(20, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            standHurtbox,
                            new CollisionBox()
                            {
                                GrowWidth = true,
                                GrowHeight = false,
                                Width = 4,
                                Height = 3,
                                PosY = 5,
                            }
                        }
                    }),
                }
            };

            var fireBallHitboxes = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new(19, null),
                    new(2, new Hit()
                    {
                        Level = 1,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new (3, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowHeight = false,
                                            GrowWidth = true,
                                            PosY = 5,
                                            Width = 4,
                                            Height = 2
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new(19, null),
                }
            };
            
            Util.AutoSetupFromAnimationPath(fireballAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[StickTwoState.Fireball] = fireballAnimation;
            StateMapConfig.Duration.Dictionary[StickTwoState.Fireball] = fireballAnimation.SectionGroup.Duration();
            StateMapConfig.UnpoolSummonSectionGroup.Dictionary[StickTwoState.Fireball] = fireballSummon;
            StateMapConfig.HurtTypeSectionGroup.Dictionary[StickTwoState.Fireball] = fireballHurtTypes;
            StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[StickTwoState.Fireball] = fireballHurtboxes;
            StateMapConfig.HitSectionGroup.Dictionary[StickTwoState.Fireball] = fireBallHitboxes;


            {
                int startup = 5;
                int active = 2;
                int hurtboxDuration = 3;
                int path = (int)StickTwoAnimationPath._5P;
                int state = StickTwoState._5L;
                
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
                        new(startup, new CollisionBoxCollection()
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
                                    Width = 5,
                                    GrowWidth = true,
                                    GrowHeight = false,
                                    PosY = 6,
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
                            BlockPushback = 3,
                            HitPushback = 2,
                            GravityScaling = FP.FromString("1.3"),
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
                                                Height = 1,
                                                Width = 4,
                                                GrowWidth = true,
                                                GrowHeight = false,
                                                PosY = 6,
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
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 4;
                
            }
            
            {
                int startup = 5;
                int active = 2;
                int hurtboxDuration = 3;
                int path = (int)StickTwoAnimationPath._2P;
                int state = StickTwoState._2L;
                
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
                        new(startup, new CollisionBoxCollection()
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
                                    Height = FP.FromString("5.25"),
                                    Width = 4,
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
                            BlockPushback = 3,
                            HitPushback = 2,
                            GravityScaling = FP.FromString("1.3"),
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
                                                Height = 5,
                                                Width = 3,
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
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 4;
            }
            
            
            {
                int startup = 12;
                int active = 2;
                int hurtboxDuration = 15;
                int path = (int)StickTwoAnimationPath._5H;
                int state = StickTwoState._5H;
                
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
                        new(6, FP.FromString("0.5")),
                        new(5, 0),
                        new(4, 2),
                        new (6, FP.FromString("0.5")),
                        new (10, 0)
                    }
                };

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup, new CollisionBoxCollection()
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
                                    Width = FP.FromString("5.5"),
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
                            TrajectoryHeight = 1,
                            TrajectoryXVelocity = 30,
                            WallBounce = true,
                            BlockPushback = FP.FromString("3.5"),
                            HitPushback = FP.FromString("3.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.2"),
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
                                                Width = 5,
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
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
            }
            
            {
                int startup = 6;
                int active = 6;
                int hurtboxDuration = 15;
                int path = (int)StickTwoAnimationPath._4H;
                int state = StickTwoState._4H;
                
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
                        new(4, 0),
                        new(2, FP.FromString("-0.75")),
                        new(4, 0)
                    }
                };
                

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup, new CollisionBoxCollection()
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
                                    Height = FP.FromString("8.5"),
                                    Width = 3,
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
                            Level = 3,
                            BonusBlockstun = 6,
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.2"),
                            TrajectoryHeight = 4,
                            // GroundBounce = true,
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
                                                Height = FP.FromString("4.5"),
                                                Width = FP.FromString("2.5"),
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosY = 4,
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
                // StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
            }
            

        }

        public override void SetupMachine()
        {
            base.SetupMachine();
            ActionConfig _5M = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = true,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = true,
                InputWeight = 0,
                RawOk = true,
                State = StickTwoState._5M
            };
            
            ConfigureAction(this, _5M);
            
            ActionConfig _2M = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 2,
                Crouching = true,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 2,
                RawOk = true,
                State = StickTwoState._2M
            };
            
            ConfigureAction(this, _2M);
            
            MakeActionCancellable(this, _5M, _2M);
            
            
            ActionConfig _2H = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 2,
                Crouching = true,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.H,
                JumpCancellable = false,
                InputWeight = 1,
                RawOk = true,
                State = StickTwoState._2H
            };
            
            ConfigureAction(this, _2H);


            ActionConfig frontThrow = new ActionConfig()
            {
                Aerial = false,
                State = StickTwoState.ForwardThrowCutscene,
                IsCutscene = true
            };
            
            ConfigureAction(this, frontThrow);
            
            ActionConfig backThrow = new ActionConfig()
            {
                Aerial = false,
                State = StickTwoState.BackThrowCutscene,
                IsCutscene = true
            };
            
            ConfigureAction(this, backThrow);
            
            
            ActionConfig fireball = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 4,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 1,
                RawOk = true,
                State = StickTwoState.Fireball
            };
            
            ConfigureAction(this, fireball);
            
            MakeActionCancellable(this, _5M, fireball);
            
            
            ActionConfig _5L = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.P,
                JumpCancellable = false,
                InputWeight = 0,
                RawOk = true,
                State = StickTwoState._5L
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
                InputType = InputSystem.InputType.P,
                JumpCancellable = false,
                InputWeight = 1,
                RawOk = true,
                State = StickTwoState._2L
            };
            
            ConfigureAction(this, _2L);
            MakeActionCancellable(this, _5L, _5L);
            MakeActionCancellable(this, _5L, _2L);
            MakeActionCancellable(this, _2L, _5L);
            MakeActionCancellable(this, _2L, _2L);


            
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
                State = StickTwoState._5H
            };
            
            ConfigureAction(this, _5H);
            MakeActionCancellable(this, _5H, fireball);
            
            ActionConfig _4H = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 4,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.H,
                JumpCancellable = false,
                InputWeight = 3,
                RawOk = true,
                State = StickTwoState._4H
            };
            
            ConfigureAction(this, _4H);
            MakeActionCancellable(this, _4H, fireball);
            MakeActionCancellable(this, _4H, _5H);
            MakeActionCancellable(this, _4H, _2H);


            

        }
    }
}



        
