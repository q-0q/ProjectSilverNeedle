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
            WalkForward,
            WalkBackward,
            CrouchActionable,
            Jump,
            _5M,
            StandHitHigh,
        } 

        public StickTwo()
        {
            Name = "StickTwo";
            StateType = typeof(StickTwoState);
            AnimationPathsEnum = typeof(StickTwoAnimationPath);

            var WalkForwardSpeed = FP.FromString("12");
            var WalkBackwardSpeed = FP.FromString("9");

            var JumpHeight = FP.FromString("5.5");
            var JumpTimeToHeight = 25;
            var JumpForwardSpeed = FP.FromString("10");
            var JumpBackwardSpeed = FP.FromString("-7");
            JumpCount = 2;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 20;

            KinematicAttachPointOffset = new FPVector2(0, 3);

            FP lowDamage = 12;
            FP mediumDamage = 20;
            FP highDamage = 35;
            FP crazyDamage = 45;
            
            SetupStateMaps();

            UpwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = JumpTimeToHeight,
                TrajectoryXVelocity = 0,
                TrajectoryHeight = JumpHeight
            };
            
            ForwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = JumpTimeToHeight,
                TrajectoryXVelocity = JumpForwardSpeed,
                TrajectoryHeight = JumpHeight
            };
            
            BackwardJumpTrajectory = new Trajectory()
            {
                TimeToTrajectoryHeight = JumpTimeToHeight,
                TrajectoryXVelocity = JumpBackwardSpeed,
                TrajectoryHeight = JumpHeight
            };


            var StandHurtbox = new SectionGroup<CollisionBoxCollection>()
            {
                Loop = true,
                Sections = new List<Tuple<int, CollisionBoxCollection>>()
                {
                    new Tuple<int, CollisionBoxCollection>(10, new CollisionBoxCollection()
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
                        }
                    )
                }
            };

            HurtboxCollectionSectionGroup.SuperDictionary[PlayerFSM.State.Stand] = StandHurtbox;

            var CrouchHurtboxCollection = new CollisionBoxCollection()
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

            var AirHitHurtboxCollection = new CollisionBoxCollection()
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

            var StandPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 6,
                Width = 1,
            };

            var CrouchPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 0,
                Height = 4,
                Width = 1,
            };

            var AirPushbox = new CollisionBox()
            {
                GrowHeight = true,
                GrowWidth = false,
                PosX = 0,
                PosY = 1,
                Height = 3,
                Width = 1,
            };

            var TallPushbox = new CollisionBox()
            {
                GrowHeight = false,
                GrowWidth = false,
                PosX = 0,
                PosY = 3,
                Height = 16,
                Width = 1,
            };

            var StandAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.StandActionable,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(StandAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.StandActionable] = StandAnimation;

            var CrouchAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.CrouchActionable,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(CrouchAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.CrouchActionable] = CrouchAnimation;

            var WalkForwardAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.WalkForward,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(WalkForwardAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.WalkForward] = WalkForwardAnimation;

            var WalkForwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(Util.GetAnimationPathLength(this, WalkForwardAnimation.Path), 3))
                }
            };

            MovementSectionGroup.Dictionary[PlayerFSM.State.WalkForward] = WalkForwardMovement;

            var StandHitHighAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.StandHitHigh,
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(StandHitHighAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.StandHitHigh] = StandHitHighAnimation;

            var WalkBackwardAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.WalkBackward,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    AutoFromAnimationPath = true,
                }
            };
            
            Util.AutoSetupFromAnimationPath(WalkBackwardAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.WalkBackward] = WalkBackwardAnimation;
            
            var WalkBackwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(Util.GetAnimationPathLength(this, WalkBackwardAnimation.Path), -2))
                }
            };
            MovementSectionGroup.Dictionary[PlayerFSM.State.WalkBackward] = WalkBackwardMovement;

            MovementSectionGroup.Dictionary[PlayerFSM.State.WalkForward] = WalkForwardMovement;

            var JumpingAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.Jump,
                SectionGroup = new SectionGroup<int>()
                {
                    LengthScalar = 2,
                    AutoFromAnimationPath = true
                }
            };
            
            Util.AutoSetupFromAnimationPath(JumpingAnimation, this);
            FighterAnimation.Dictionary[PlayerFSM.State.AirActionable] = JumpingAnimation;


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
                                            Width = 5,
                                            Height = 2,
                                            PosX = 0,
                                            PosY = 4
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new Tuple<int, Hit>(10, null)
                }
            };
            
            Util.AutoSetupFromAnimationPath(_5MAnimation, this);
            FighterAnimation.Dictionary[StickTwoState._5M] = _5MAnimation;
            Duration.Dictionary[StickTwoState._5M] = _5MAnimation.SectionGroup.Duration();
            InputTypes.Dictionary[StickTwoState._5M] = InputSystem.InputType.S;
            HitSectionGroup.Dictionary[StickTwoState._5M] = _5MHits;
            
            
            

            var AirHitPostGroundBounceRisingAnimation = new RisingTrajectoryAnimation()
            {
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

            var AirPostHitGroundBounceFallingAnimation = new FallingTrajectoryAnimation()
            {
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



            var StandHitLowAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };

            var CrouchHitAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };

            var AirHitRisingAnimation = new RisingTrajectoryAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0),
                        new(8, 1),
                    }
                }
            };

            var AirHitFallingAnimation = new FallingTrajectoryAnimation()
            {
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

            var KinematicReceiverAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(18, 0),
                    }
                }
            };

            var StandBlockAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };
            var ThrowTechAnimation = StandBlockAnimation;

            var CrouchBlockAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };

            var AirBlockAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(7, 0),
                        new(7, 1),
                    }
                }
            };

            var HardKnockdownAnimation = new FighterAnimation()
            {
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

            var DeadFromAirAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(8, 0),
                        new(20, 1),
                    }
                }
            };

            var DeadFromGroundAnimation = new FighterAnimation()
            {
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

            var SoftKnockdownAnimation = new FighterAnimation()
            {
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

            var LandsquatAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0)
                    }
                }
            };

            var DashAnimation = new FighterAnimation()
            {
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

            var DashMovementSectionGroup = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(8, 0),
                    new(7, 3),
                    new(3, 0),
                }
            };

            var BackdashAnimation = new FighterAnimation()
            {
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
            var AirdashAnimation = DashAnimation;

            var BackdashMovementSectionGroup = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(6, 0),
                    new(7, -4),
                    new(9, 0),
                }
            };
            var AirBackdashAnimation = BackdashAnimation;

            var ThrowStartupAnimation = new FighterAnimation()
            {
                SectionGroup = new SectionGroup<int>()
                {
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(10, 0)
                    }
                }
            };

            var FrontThrowKinematics = new Kinematics()
            {
                FireReceiverFinishAfter = 40,
                Animation = new FighterAnimation()
                {
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
                        new(16, null),
                        new(4, new Hit()
                        {
                            Damage = 50
                        }),
                        new(20, null)
                    }
                }

            };

            var BackThrowKinematics = new Kinematics()
            {
                FireReceiverFinishAfter = 37,
                Animation = new FighterAnimation()
                {
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
                        new(16, null),
                        new(4, new Hit()
                        {
                            Damage = 50
                        }),
                        new(20, null)
                    }
                }

            };

            var ThrowWhiffAnimation = new FighterAnimation()
            {
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


        }
        
        public override void ConfigureCharacterFsm(PlayerFSM playerFsm)
        {
            
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._5M);
            
            return;
            
            // 5L
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._5L);
            
            // 2L
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._2L, false, true, 1);
            
            // 5M
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._5M);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5M, StickTwo.StickTwoState._2M);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5M, StickTwo.StickTwoState._2H);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5M, StickTwo.StickTwoState._5S1);
            
            // 2M
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._2M, false, true, 1);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._2M, StickTwo.StickTwoState._2H);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._2M, StickTwo.StickTwoState._5S1);
            
            // 2H
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._2H, false, true, 1);
            
            // 5H
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._5H, true);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5H, StickTwo.StickTwoState._2H);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5H, StickTwo.StickTwoState._5S1);
            
            // 5S1
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._5S1);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5S1, StickTwo.StickTwoState._5S2);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5S1, StickTwo.StickTwoState._5S3, 1);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._5S1, StickTwo.StickTwoState._6S3, 1);
            
            // 5S2
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._5S2, false, true, 0, false);
            
            // 4S3
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._5S3, false, false, 0, false);
            
            // 6S3
            ConfigureGroundAction(playerFsm, StickTwo.StickTwoState._6S3, false, true, 0, false);
            
            // 2S - grounded
            ConfigureGroundToAirAction(playerFsm, StickTwo.StickTwoState._2S_ground, false, 1, true);
            
            // 2S - air
            ConfigureAirAction(playerFsm, StickTwo.StickTwoState._2S_air, false, 1);
            
            // JL
            ConfigureAirAction(playerFsm, StickTwo.StickTwoState._JL);
            
            // JM
            ConfigureAirAction(playerFsm, StickTwo.StickTwoState._JM);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._JM, StickTwo.StickTwoState._JH, 1);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._JM, StickTwo.StickTwoState._JS, 1);
            
            // JH
            ConfigureAirAction(playerFsm, StickTwo.StickTwoState._JH);
            MakeActionCancellable(playerFsm, StickTwo.StickTwoState._JH, StickTwo.StickTwoState._JS, 1);
            
            // JS
            ConfigureAirAction(playerFsm, StickTwo.StickTwoState._JS);
        }
    }
}



        
