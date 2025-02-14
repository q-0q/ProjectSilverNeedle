using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public class StickTwo : Character
    {
        public class StickTwoState : PlayerFSM.State
        {
            public static int _5L;
            public static int _2L;
            public static int _5M;
            public static int _2M;
            public static int _5H;
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
            ThrowStartup,
            FrontThrow,
            BackThrow,
            ThrowWhiff,

            StandHitHigh,
            StandHitLow,
            CrouchHit,
            AirHit,
            StandBlock,
            CrouchBlock,
            AirBlock,
            HardKnockdown,
            SoftKnockdown,
            DeadFromGround,
            DeadFromAir,
            
            Landsquat,
            ThrowTech,
            
            _5M,
            _2M,
        } 

        public StickTwo()
        {
            Name = "StickTwo";
            StateType = typeof(StickTwoState);
            AnimationPathsEnum = typeof(StickTwoAnimationPath);
            
            JumpCount = 2;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 20;

            KinematicAttachPointOffset = new FPVector2(0, 3);
            
            var jumpHeight = FP.FromString("5.5");
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
            
            SetupStateMaps();
            
            // Hurtboxes
            
            var standHurtbox = new SectionGroup<CollisionBoxCollection>()
            {
                Loop = true,
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new(10, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 7,
                                    Width = 3,
                                },
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
                                new()
                                {
                                    GrowHeight = true,
                                    GrowWidth = false,
                                    PosX = 0,
                                    PosY = 0,
                                    Height = 5,
                                    Width = 3,
                                }
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
                                    Height = 6,
                                    Width = 7,
                                }
                            }
                        }
                    )
                }
            };
            
            HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.State.Stand] = standHurtbox;
            HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.State.Crouch] = crouchHurtboxCollection;
            HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.State.Air] = airHitHurtboxCollection;

            
            // Pushboxes

            var standPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 6,
                Width = 1,
            };

            var crouchPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 4,
                Width = 1,
            };

            var airPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 1,
                Height = 3,
                Width = 1,
            };
            
            Pushbox.SuperDictionary[PlayerFSM.State.Stand] = standPushbox;
            Pushbox.SuperDictionary[PlayerFSM.State.Crouch] = crouchPushbox;
            Pushbox.SuperDictionary[PlayerFSM.State.Air] = airPushbox;
            Pushbox.SuperDictionary[PlayerFSM.State.HardKnockdown] = crouchPushbox;
            Pushbox.SuperDictionary[PlayerFSM.State.SoftKnockdown] = crouchPushbox;
            
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
            
            Util.AutoSetupFromAnimationPath(standAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.StandActionable] = standAnimation;

            Util.AutoSetupFromAnimationPath(crouchAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.CrouchActionable] = crouchAnimation;
            
            Util.AutoSetupFromAnimationPath(walkForwardAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.WalkForward] = walkForwardAnimation;

            Util.AutoSetupFromAnimationPath(walkBackwardAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.WalkBackward] = walkBackwardAnimation;
            
            Util.AutoSetupFromAnimationPath(jumpingAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.AirActionable] = jumpingAnimation;
            
            Util.AutoSetupFromAnimationPath(dashAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.Dash] = dashAnimation;
            Duration.Dictionary[PlayerFSM.State.Dash] = dashAnimation.SectionGroup.Duration();
            
            Util.AutoSetupFromAnimationPath(standHitHighAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.StandHitHigh] = standHitHighAnimation;
            
            Util.AutoSetupFromAnimationPath(standHitLowAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.StandHitLow] = standHitLowAnimation;
            
            Util.AutoSetupFromAnimationPath(crouchHitAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.CrouchHit] = crouchHitAnimation;
            
            Util.AutoSetupFromAnimationPath(airHitAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.AirHit] = airHitAnimation;
            FighterAnimation.Dictionary[PlayerFSM.State.AirHitPostGroundBounce] = airHitAnimation;
            FighterAnimation.Dictionary[PlayerFSM.State.KinematicReceiver] = airHitAnimation;
            
            Util.AutoSetupFromAnimationPath(standBlockAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.StandBlock] = standBlockAnimation;

            Util.AutoSetupFromAnimationPath(crouchBlockAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.CrouchBlock] = crouchBlockAnimation;
            
            Util.AutoSetupFromAnimationPath(airBlockAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.AirBlock] = airBlockAnimation;
            
            Util.AutoSetupFromAnimationPath(hardKnockdownAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.HardKnockdown] = hardKnockdownAnimation;
            
            Util.AutoSetupFromAnimationPath(softKnockdownAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.SoftKnockdown] = softKnockdownAnimation;
            
            Util.AutoSetupFromAnimationPath(deadFromGroundAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.DeadFromGround] = deadFromGroundAnimation;
            
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
                    new(11, 4),
                    new(10, FP.FromString("0.25")),
                    new (10, 0),
                }
            };
            
            MovementSectionGroup.Dictionary[PlayerFSM.State.WalkBackward] = walkBackwardMovement;
            MovementSectionGroup.Dictionary[PlayerFSM.State.WalkForward] = walkForwardMovement;
            MovementSectionGroup.Dictionary[PlayerFSM.State.Dash] = dashMovement;
            

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
                        Level = 0,
                        // HardKnockdown = true,
                        // Damage = 100,
                        // Launches = true,
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
            
            Util.AutoSetupFromAnimationPath(_5MAnimation, this);
            FighterAnimation.Dictionary[StickTwoState._5M] = _5MAnimation;
            Duration.Dictionary[StickTwoState._5M] = _5MAnimation.SectionGroup.Duration();
            HitSectionGroup.Dictionary[StickTwoState._5M] = _5MHits;
            CancellableAfter.Dictionary[StickTwoState._5M] = 14;
            WhiffCancellable.Dictionary[StickTwoState._5M] = false;
            HurtTypeSectionGroup.Dictionary[StickTwoState._5M] = _5MHurtTypes;
            
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
                    new Tuple<int, Hit>(16, null),
                    new Tuple<int, Hit>(5, new Hit()
                    {
                        Level = 2,
                        Type = Hit.HitType.Low,
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
                                            PosY = 0
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new Tuple<int, Hit>(10, null)
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
            FighterAnimation.Dictionary[StickTwoState._2M] = _2MAnimation;
            Duration.Dictionary[StickTwoState._2M] = _2MAnimation.SectionGroup.Duration();
            HitSectionGroup.Dictionary[StickTwoState._2M] = _2MHits;
            CancellableAfter.Dictionary[StickTwoState._2M] = 16;
            WhiffCancellable.Dictionary[StickTwoState._2M] = false;
            MovementSectionGroup.Dictionary[StickTwoState._2M] = _2MMovement;
            HurtTypeSectionGroup.Dictionary[StickTwoState._2M] = _2MHurtTypes;


        }
        
        public override void ConfigureCharacterFsm(PlayerFSM playerFsm)
        {

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
            
            ConfigureAction(playerFsm, _5M);
            
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
                InputWeight = 1,
                RawOk = true,
                State = StickTwoState._2M
            };
            
            ConfigureAction(playerFsm, _2M);
            
            MakeActionCancellable(playerFsm, _5M, _2M);
        }
    }
}



        
