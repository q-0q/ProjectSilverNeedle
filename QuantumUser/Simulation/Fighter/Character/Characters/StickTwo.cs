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
            var JumpBackwardSpeed = FP.FromString("7");
            JumpCount = 2;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 20;

            KinematicAttachPointOffset = new FPVector2(0, 3);

            FP lowDamage = 12;
            FP mediumDamage = 20;
            FP highDamage = 35;
            FP crazyDamage = 45;
            
            FighterAnimation = new StateMap<FighterAnimation>();
            Duration = new StateMap<int>();
            Duration.DefaultValue = 0;
            HurtboxCollectionSectionGroup = new StateMap<SectionGroup<CollisionBoxCollection>>();
            HurtTypeSectionGroup = new StateMap<SectionGroup<PlayerFSM.HurtType>>();
            HitSectionGroup = new StateMap<SectionGroup<Hit>>();
            Pushbox = new StateMap<CollisionBox>();
            MovementSectionGroup = new StateMap<SectionGroup<FP>>();
            AllowCrossupSectionGroup = new StateMap<SectionGroup<bool>>();
            TrajectorySectionGroup = new StateMap<SectionGroup<Trajectory>>();
            InputTypes = new StateMap<InputSystem.InputType>();
            InputTypes.DefaultValue = InputSystem.InputType.P;
            CommandDirection = new StateMap<int>();
            CommandDirection.DefaultValue = 5;
            CancellableAfter = new StateMap<int>();
            CancellableAfter.DefaultValue = 0;
            WhiffCancellable = new StateMap<bool>();
            WhiffCancellable.DefaultValue = false;
            FireReceiverFinishAfter = new StateMap<int>();
            FireReceiverFinishAfter.DefaultValue = 10;
            AttachPositionSectionGroup = new StateMap<SectionGroup<FPVector2>>();
            InvulnerableBefore = new StateMap<int>();
            InvulnerableBefore.DefaultValue = 0;

            var StandHurtboxesCollection = new CollisionBoxCollection()
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
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0),
                        new(1, 1),
                        new(1, 2),
                        new(1, 3),
                        new(1, 4),
                        new(1, 5),
                        new(1, 6),
                        new(1, 7),
                        new(1, 8),
                        new(1, 9),
                        new(1, 10),
                        new(1, 11),
                        new(1, 12),
                        new(1, 13),
                        new(1, 14),
                        new(1, 15),
                        new(1, 16),
                        new(1, 17),
                        new(1, 18),
                        new(1, 19),
                        new(1, 20)
                    }
                }
            };

            FighterAnimation.Dictionary[PlayerFSM.State.StandActionable] = StandAnimation;

            var CrouchAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.CrouchActionable,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0)
                    }
                }
            };
            
            FighterAnimation.Dictionary[PlayerFSM.State.CrouchActionable] = CrouchAnimation;

            var WalkForwardAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.WalkForward,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0),
                        new(1, 1),
                        new(1, 2),
                        new(1, 3),
                        new(1, 4),
                        new(1, 5),
                        new(1, 6),
                        new(1, 7),
                        new(1, 8),
                        new(1, 9),
                        new(1, 10),
                        new(1, 11),
                        new(1, 12),
                        new(1, 13),
                        new(1, 14),
                        new(1, 15),
                        new(1, 16),
                        new(1, 17),
                        new(1, 18),
                        new(1, 19),
                        new(1, 20),
                        new(1, 21),
                        new(1, 22),
                        new(1, 23),
                    }
                }
            };
            
            FighterAnimation.Dictionary[PlayerFSM.State.WalkForward] = WalkForwardAnimation;
            
            

            var WalkBackwardAnimation = new FighterAnimation()
            {
                Path = (int)StickTwoAnimationPath.WalkBackward,
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 2,
                    Sections = new List<Tuple<int, int>>()
                    {
                        new(1, 0),
                        new(1, 1),
                        new(1, 2),
                        new(1, 3),
                        new(1, 4),
                        new(1, 5),
                        new(1, 6),
                        new(1, 7),
                        new(1, 8),
                        new(1, 9),
                        new(1, 10),
                        new(1, 11),
                        new(1, 12),
                        new(1, 13),
                        new(1, 14),
                        new(1, 15),
                        new(1, 16),
                        new(1, 17),
                        new(1, 18),
                        new(1, 19),
                        new(1, 20),
                        new(1, 21),
                        new(1, 22),
                        new(1, 23),
                    }
                }
            };
            
            FighterAnimation.Dictionary[PlayerFSM.State.WalkBackward] = WalkBackwardAnimation;

            var AirActionableRisingAnimation = new RisingTrajectoryAnimation()
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

            var AirActionableFallingAnimation = new FallingTrajectoryAnimation()
            {
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

            var StandHitHighAnimation = new FighterAnimation()
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



        
