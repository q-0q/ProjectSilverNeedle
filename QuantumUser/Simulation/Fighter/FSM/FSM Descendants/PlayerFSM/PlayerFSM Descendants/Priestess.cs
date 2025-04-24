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
        }
        
        
        public PriestessFSM()
        {
            Name = "Priestess";
            StateType = typeof(PriestessState);
            JumpCount = 3;

            FallSpeed = FP.FromString("50");
            FallTimeToSpeed = 22;
            DamageDealtModifier = FP.FromString("0.85");
            DamageTakenModifier = FP.FromString("1.35");

            KinematicAttachPointOffset = new FPVector2(0, 3);
            
            var jumpHeight = FP.FromString("6");
            var jumpTimeToHeight = 18;
            var jumpForwardSpeed = FP.FromString("15");
            var jumpBackwardSpeed = FP.FromString("-7");

            MinimumDashDuration = 7;

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

            JumpsquatDuration = 6;
            
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
                Width = FP.FromString("3.5"),
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
                    LengthScalar = 4,
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
                Path = "WalkForward",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 1,
                    AutoFromAnimationPath = true,
                }
            };
            //
            // var jumpingAnimation = new FighterAnimation()
            // {
            //     Path = "Jump",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var afterAirActionAnimation = new FighterAnimation()
            // {
            //     Path = "AfterAirAction",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var dashAnimation = new FighterAnimation()
            // {
            //     Path = "Dash",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var backdashAnimation = new FighterAnimation()
            // {
            //     Path = "Backdash",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var standHitHighAnimation = new FighterAnimation()
            // {
            //     Path = "StandHitHigh",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var standHitLowAnimation = new FighterAnimation()
            // {
            //     Path = "StandHitLow",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var crouchHitAnimation = new FighterAnimation()
            // {
            //     Path = "CrouchHit",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var airHitAnimation = new FighterAnimation()
            // {
            //     Path = "AirHit",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var wallBounceAnimation = new FighterAnimation()
            // {
            //     Path = "WallBounce",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var groundBounceAnimation = new FighterAnimation()
            // {
            //     Path = "GroundBounce",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var standBlockAnimation = new FighterAnimation()
            // {
            //     Path = "StandBlock",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var breakAnimation = new FighterAnimation()
            // {
            //     Path = "GuardBreak",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var proxStandBlockAnimation = new FighterAnimation()
            // {
            //     Path = "ProxStandBlock",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var crouchBlockAnimation = new FighterAnimation()
            // {
            //     Path = "CrouchBlock",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var proxCrouchBlockAnimation = new FighterAnimation()
            // {
            //     Path = "ProxCrouchBlock",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var airBlockAnimation = new FighterAnimation()
            // {
            //     Path = "AirBlock",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var hardKnockdownAnimation = new FighterAnimation()
            // {
            //     Path = "HardKnockdown",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var softKnockdownAnimation = new FighterAnimation()
            // {
            //     Path = "SoftKnockdown",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var deadFromGroundAnimation = new FighterAnimation()
            // {
            //     Path = "DeadFromGround",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var deadFromAirAnimation = new FighterAnimation()
            // {
            //     Path = "DeadFromAir",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var throwAnimation = new FighterAnimation()
            // {
            //     Path = "Throw",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var jumpsquatAnimation = new FighterAnimation()
            // {
            //     Path = "Jumpsquat",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            //
            // var landsquatAnimation = new FighterAnimation()
            // {
            //     Path = "Landsquat",
            //     SectionGroup = new SectionGroup<int>()
            //     {
            //         AutoFromAnimationPath = true
            //     }
            // };
            
            
            Util.AutoSetupFromAnimationPath(standAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandActionable] = standAnimation;
            
            
            Util.AutoSetupFromAnimationPath(crouchAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchActionable] = crouchAnimation;
            
            Util.AutoSetupFromAnimationPath(walkForwardAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.WalkForward] = walkForwardAnimation;

            Util.AutoSetupFromAnimationPath(walkBackwardAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.WalkBackward] = walkBackwardAnimation;
            
            // Util.AutoSetupFromAnimationPath(jumpingAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirActionable] = jumpingAnimation;
            //
            // Util.AutoSetupFromAnimationPath(afterAirActionAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirActionableAfterAction] = afterAirActionAnimation;
            //
            // Util.AutoSetupFromAnimationPath(dashAnimation, this);
            // StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Dash] = dashAnimation;
            // StateMapConfig.Duration.SuperDictionary[PlayerFSM.PlayerState.Dash] = dashAnimation.SectionGroup.Duration();
            //
            //
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirDash] = dashAnimation;
            // StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.AirDash] = dashAnimation.SectionGroup.Duration();
            //
            // Util.AutoSetupFromAnimationPath(backdashAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashAnimation;
            // StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashAnimation.SectionGroup.Duration();
            //
            // Util.AutoSetupFromAnimationPath(standHitHighAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandHitHigh] = standHitHighAnimation;
            //
            // Util.AutoSetupFromAnimationPath(standHitLowAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandHitLow] = standHitLowAnimation;
            //
            // Util.AutoSetupFromAnimationPath(crouchHitAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchHit] = crouchHitAnimation;
            //
            // Util.AutoSetupFromAnimationPath(airHitAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHit] = airHitAnimation;
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostGroundBounce] = airHitAnimation;
            // StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.CutsceneReactor] = airHitAnimation;
            //
            // Util.AutoSetupFromAnimationPath(wallBounceAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostWallBounce] = wallBounceAnimation;
            //
            // Util.AutoSetupFromAnimationPath(groundBounceAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirHitPostGroundBounce] = groundBounceAnimation;
            //
            // Util.AutoSetupFromAnimationPath(standBlockAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandBlock] = standBlockAnimation;
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Tech] = standBlockAnimation;
            //
            // Util.AutoSetupFromAnimationPath(proxStandBlockAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.ProxStandBlock] = proxStandBlockAnimation;
            // StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.ProxStandBlock] = 100;
            //
            // Util.AutoSetupFromAnimationPath(crouchBlockAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.CrouchBlock] = crouchBlockAnimation;
            //
            // Util.AutoSetupFromAnimationPath(proxCrouchBlockAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.ProxCrouchBlock] = proxCrouchBlockAnimation;
            // StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.ProxCrouchBlock] = 100;
            //
            // Util.AutoSetupFromAnimationPath(airBlockAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.AirBlock] = airBlockAnimation;
            //
            // Util.AutoSetupFromAnimationPath(hardKnockdownAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.HardKnockdown] = hardKnockdownAnimation;
            //
            // Util.AutoSetupFromAnimationPath(softKnockdownAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.SoftKnockdown] = softKnockdownAnimation;
            //
            // Util.AutoSetupFromAnimationPath(deadFromGroundAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.DeadFromGround] = deadFromGroundAnimation;
            //
            // Util.AutoSetupFromAnimationPath(deadFromAirAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.DeadFromAir] = deadFromAirAnimation;
            //
            // Util.AutoSetupFromAnimationPath(throwAnimation, this);
            // StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Throw] = throwAnimation;
            //
            // Util.AutoSetupFromAnimationPath(jumpsquatAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Jumpsquat] = jumpsquatAnimation;
            //
            // Util.AutoSetupFromAnimationPath(landsquatAnimation, this);
            // StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Landsquat] = landsquatAnimation;
            //
            // Util.AutoSetupFromAnimationPath(breakAnimation, this);
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Break] = breakAnimation;
            // StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.RedBreak] = breakAnimation;

            
            // Basic movement
            
            var walkForwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(Util.GetAnimationPathLength(this, walkForwardAnimation.Path), 10))
                }
            };
            
            var walkBackwardMovement = new SectionGroup<FP>()
            {
                Loop = true,
                Sections = new List<Tuple<int, FP>>()
                {
                    (new(Util.GetAnimationPathLength(this, walkBackwardAnimation.Path), -9))
                }
            };

            var dashMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(3, 0),
                    new(12, 7),
                    new(8, 1),
                    new(12, FP.FromString("0.6")),
                    new (10, 0),
                }
            };
            
            var surgeMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(1, 0),
                    new(2, 0),
                    new(12, 3),
                    new(8, FP.FromString("0.7")),
                    new(12, FP.FromString("0.3")),
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
            StateMapConfig.MovementSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Dash] = dashMovement;
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.AirDash] = dashMovement;
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.Backdash] = backdashMovement;
            StateMapConfig.InvulnerableBefore.Dictionary[PlayerFSM.PlayerState.Backdash] = 12;
            
            ////////////////////////////////////////////////////////////////////////////////////

            

        }
        
        public override void SetupMachine()
        {
            base.SetupMachine();
            
        }
    }
}



        
