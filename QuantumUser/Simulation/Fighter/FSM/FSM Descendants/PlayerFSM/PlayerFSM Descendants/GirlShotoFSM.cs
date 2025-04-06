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
    public unsafe class GirlShotoFSM : PlayerFSM
    {
        public class GirlShotoState : PlayerState
        {
            public static int _5L;
            public static int _2L;
            public static int _5M;
            public static int _2M;
            public static int _5H;
            public static int _4H;
            public static int _2H;
            public static int Rekka1;
            public static int Rekka2A;
            public static int Rekka2B;
            public static int _2S_ground;
            public static int _2S_air;
            public static int ForwardThrowCutscene;
            public static int BackThrowCutscene;
            public static int Fireball;
            public static int DP;

            public static int JL;
            public static int JM;
            public static int JH;
        }
        
        
        public GirlShotoFSM()
        {
            Name = "GirlShoto";
            StateType = typeof(GirlShotoState);
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

            SummonPools = new List<SummonPool>()
            {
                new()
                {
                    Size = 2,
                    SummonFSMType = typeof(FireballFSM)
                },
                // new()
                // {
                //     Size = 1,
                //     SummonFSMType = typeof(VictorOrbFsm)
                // },
            };

        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
            
            Debug.Log("Setting up player state maps");
            
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
                PosY = 0,
                Height = 5,
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
                Path = "StandActionable",
                SectionGroup = new SectionGroup<int>()
                {
                    Loop = true,
                    LengthScalar = 4,
                    AutoFromAnimationPath = true
                }
            };
            
            var crouchAnimation = new FighterAnimation()
            {
                Path = "CrouchActionable",
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
                Path = "AfterAirAction",
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
                    AutoFromAnimationPath = true
                }
            };
            
            var backdashAnimation = new FighterAnimation()
            {
                Path = "Backdash",
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
            
            var airHitAnimation = new FighterAnimation()
            {
                Path = "AirHit",
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
            
            var proxStandBlockAnimation = new FighterAnimation()
            {
                Path = "ProxStandBlock",
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
                Path = "ProxCrouchBlock",
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
            
            // var orbSummon = new SectionGroup<SummonPool>()
            // {
            //     Sections = new List<Tuple<int, SummonPool>>()
            //     {
            //         new (1, SummonPools[1])
            //     }
            // };
            
            
            Util.AutoSetupFromAnimationPath(standAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.StandActionable] = standAnimation;
            // StateMapConfig.UnpoolSummonSectionGroup.Dictionary[PlayerState.StandActionable] = orbSummon;
            
            
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
            StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.CutsceneReactor] = airHitAnimation;
            
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
            
            Util.AutoSetupFromAnimationPath(jumpsquatAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[PlayerFSM.PlayerState.Jumpsquat] = jumpsquatAnimation;
            
            Util.AutoSetupFromAnimationPath(landsquatAnimation, this);
            StateMapConfig.FighterAnimation.SuperDictionary[PlayerFSM.PlayerState.Landsquat] = landsquatAnimation;
            
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
                    new(8, 5),
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


            FP lowDamage = 8;
            FP highDamage = 30;
            
            var _5MAnimation = new FighterAnimation()
            {
                Path = "_5M",
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
                        TrajectoryXVelocity = 6,
                        TrajectoryHeight = FP.FromString("2.25"),
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
            StateMapConfig.FighterAnimation.Dictionary[GirlShotoState._5M] = _5MAnimation;
            StateMapConfig.Duration.Dictionary[GirlShotoState._5M] = _5MAnimation.SectionGroup.Duration();
            StateMapConfig.HitSectionGroup.Dictionary[GirlShotoState._5M] = _5MHits;
            StateMapConfig.CancellableAfter.Dictionary[GirlShotoState._5M] = 14;
            StateMapConfig.WhiffCancellable.Dictionary[GirlShotoState._5M] = false;
            StateMapConfig.HurtTypeSectionGroup.Dictionary[GirlShotoState._5M] = _5MHurtTypes;
            StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[GirlShotoState._5M] = _5MHurtboxes;
            
            Cutscene forwardThrowCutscene = new Cutscene()
            {
                InitiatorState = GirlShotoState.ForwardThrowCutscene,
                ReactorDuration = 33,
                Techable = true,
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
                InitiatorState = GirlShotoState.BackThrowCutscene,
                ReactorDuration = 45,
                Techable = true,
                ReactorPositionSectionGroup = new SectionGroup<FPVector2>()
                {
                    Sections = new List<Tuple<int, FPVector2>>()
                    {
                        new (8, new FPVector2(FP.FromString("2.5"), FP.FromString("7.5"))),
                        new (20, new FPVector2(FP.FromString("2.5"), 8)),
                        new (15, new FPVector2(FP.FromString("0.1"), FP.FromString("7.5"))),
                        new (15, new FPVector2(-4, 0)),
                    }
                }
            };

            Cutscenes[PlayerFSM.CutsceneIndexes.BackwardThrow] = backThrowCutscene;
            
            
            var _2MAnimation = new FighterAnimation()
            {
                Path = "_2M",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var _2MHits = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new Tuple<int, Hit>(9, null),
                    new Tuple<int, Hit>(5, new Hit()
                    {
                        Level = 2,
                        Type = Hit.HitType.Low,
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
                    new(9, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            new()
                            {
                                GrowHeight = true,
                                GrowWidth = false,
                                PosX = 0,
                                PosY = 0,
                                Height = 3,
                                Width = 3,
                            }
                        }
                    }),
                    new(20, new CollisionBoxCollection()
                    {
                        CollisionBoxes = new List<CollisionBox>()
                        {
                            new()
                            {
                                GrowHeight = true,
                                GrowWidth = false,
                                PosX = 0,
                                PosY = 0,
                                Height = 3,
                                Width = 3,
                            },
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
                    new(20, PlayerFSM.HurtType.Counter),
                    new(20, PlayerFSM.HurtType.Punish)
                }
            };
            
            Util.AutoSetupFromAnimationPath(_2MAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[GirlShotoState._2M] = _2MAnimation;
            StateMapConfig.Duration.Dictionary[GirlShotoState._2M] = _2MAnimation.SectionGroup.Duration();
            StateMapConfig.HitSectionGroup.Dictionary[GirlShotoState._2M] = _2MHits;
            StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[GirlShotoState._2M] = _2MHurtboxes;
            StateMapConfig.CancellableAfter.Dictionary[GirlShotoState._2M] = 16;
            StateMapConfig.WhiffCancellable.Dictionary[GirlShotoState._2M] = false;
            // StateMapConfig.MovementSectionGroup.Dictionary[GirlShotoState._2M] = _2MMovement;
            StateMapConfig.HurtTypeSectionGroup.Dictionary[GirlShotoState._2M] = _2MHurtTypes;


            var _2HAnimation = new FighterAnimation()
            {
                Path = "_2H",
                SectionGroup = new SectionGroup<int>()
                {
                    AutoFromAnimationPath = true
                }
            };

            var _2HMovement = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new(1, 1),
                    new(1, 0)
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
                        TrajectoryHeight = 4,
                        TrajectoryXVelocity = 4,
                        GravityScaling = 1,
                        GravityProration = FP.FromString("2"),
                        Damage = highDamage,
                        VisualAngle = -80,
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
            
            var _2HHurtTypes = new SectionGroup<PlayerFSM.HurtType>()
            {
                Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                {
                    new(17, PlayerFSM.HurtType.Counter),
                    new(20, PlayerFSM.HurtType.Punish)
                }
            };
            
            Util.AutoSetupFromAnimationPath(_2HAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[GirlShotoState._2H] = _2HAnimation;
            StateMapConfig.Duration.Dictionary[GirlShotoState._2H] = _2HAnimation.SectionGroup.Duration();
            StateMapConfig.HitSectionGroup.Dictionary[GirlShotoState._2H] = _2HHits;
            StateMapConfig.MovementSectionGroup.Dictionary[GirlShotoState._2H] = _2HMovement;
            StateMapConfig.HurtTypeSectionGroup.Dictionary[GirlShotoState._2H] = _2HHurtTypes;


            var frontThrowCutsceneAnimation = new FighterAnimation()
            {
                Path = "ForwardThrowCutscene",
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
            StateMapConfig.FighterAnimation.Dictionary[GirlShotoState.ForwardThrowCutscene] = frontThrowCutsceneAnimation;
            StateMapConfig.HitSectionGroup.Dictionary[GirlShotoState.ForwardThrowCutscene] = frontThrowCutsceneHits;
            StateMapConfig.Duration.Dictionary[GirlShotoState.ForwardThrowCutscene] = frontThrowCutsceneAnimation.SectionGroup.Duration();
            
            
            
            var backThrowCutsceneAnimation = new FighterAnimation()
            {
                Path = "BackThrowCutscene",
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
                    new Tuple<int, Hit>(2, backThrowCutsceneHit),
                    new Tuple<int, Hit>(2, null),
                    new Tuple<int, Hit>(2, backThrowCutsceneHit),
                    new Tuple<int, Hit>(20, null)
                }
            };
            
            Util.AutoSetupFromAnimationPath(backThrowCutsceneAnimation, this);
            StateMapConfig.FighterAnimation.Dictionary[GirlShotoState.BackThrowCutscene] = backThrowCutsceneAnimation;
            StateMapConfig.HitSectionGroup.Dictionary[GirlShotoState.BackThrowCutscene] = backThrowCutsceneHits;
            StateMapConfig.Duration.Dictionary[GirlShotoState.BackThrowCutscene] = backThrowCutsceneAnimation.SectionGroup.Duration();




            var fireballAnimation = new FighterAnimation()
            {
                Path = "Breath",
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
                        Projectile = true,
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
            StateMapConfig.FighterAnimation.Dictionary[GirlShotoState.Fireball] = fireballAnimation;
            StateMapConfig.Duration.Dictionary[GirlShotoState.Fireball] = fireballAnimation.SectionGroup.Duration();
            StateMapConfig.UnpoolSummonSectionGroup.Dictionary[GirlShotoState.Fireball] = fireballSummon;
            StateMapConfig.HurtTypeSectionGroup.Dictionary[GirlShotoState.Fireball] = fireballHurtTypes;
            StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[GirlShotoState.Fireball] = fireballHurtboxes;
            StateMapConfig.HitSectionGroup.Dictionary[GirlShotoState.Fireball] = fireBallHitboxes;


            {
                int startup = 5;
                int active = 2;
                int hurtboxDuration = 3;
                string path = "_5P";
                int state = GirlShotoState._5L;
                
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
                            Damage = lowDamage,
                            DamageScaling = FP.FromString("0.5"), 
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
                string path = "_2P";
                int state = GirlShotoState._2L;
                
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
                            Damage = lowDamage,
                            DamageScaling = FP.FromString("0.5"), 
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
                string path = "_5H";
                int state = GirlShotoState._5H;
                
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
                            BlockPushback = FP.FromString("4.5"),
                            HitPushback = FP.FromString("3.5"),
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.2"),
                            Damage = highDamage,
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
                string path = "_4H";
                int state = GirlShotoState._4H;
                
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

                var hit = new Hit()
                {
                    Level = 1,
                    BonusBlockstun = 2,
                    GravityScaling = FP.FromString("1"),
                    GravityProration = FP.FromString("1.05"),
                    TrajectoryHeight = FP.FromString("0.5"),
                    TrajectoryXVelocity = 3,
                    // GroundBounce = true,
                    DamageScaling = 1,
                    Damage = highDamage,
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
                };
                
                var hit2 = new Hit()
                {
                    Level = 2,
                    BonusBlockstun = 2,
                    GravityScaling = FP.FromString("1"),
                    GravityProration = FP.FromString("1.05"),
                    TrajectoryHeight = FP.FromString("0.5"),
                    TrajectoryXVelocity = 3,
                    // GroundBounce = true,
                    DamageScaling = 1,
                    Damage = highDamage,
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
                };
                var hitboxes = new SectionGroup<Hit>()
                {
                    Sections = new List<Tuple<int, Hit>>()
                    {
                        new(startup, null),
                        new(active, hit),
                        new(active, hit2),
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

            
            {
                int startup = 19;
                int active = 2;
                int hurtboxDuration = 15;
                string path = "Rekka1";
                int state = GirlShotoState.Rekka1;
                
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
                        new(12, 0),
                        new(4, 3),
                        new(4, 0),
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
                                crouchHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                standHurtbox,
                                new CollisionBox()
                                {
                                    Height = FP.FromString("6"),
                                    Width = FP.FromString("5.5"),
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

                var projectileInvuln = new SectionGroup<bool>()
                {
                    Sections = new List<Tuple<int, bool>>()
                    {
                        new(startup + active, true),
                        new(50, false),
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
                            // BonusBlockstun = 6,
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.6"),
                            TrajectoryHeight = FP.FromString("2.5"),
                            TrajectoryXVelocity = FP.FromString("3"),
                            BlockPushback = 2,
                            HitPushback = 2,
                            // Launches = true,
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
                                                Height = FP.FromString("3.5"),
                                                Width = FP.FromString("5"),
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosY = 2,
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
                StateMapConfig.ProjectileInvulnerable.Dictionary[state] = projectileInvuln;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup - 3;
                StateMapConfig.WhiffCancellable.Dictionary[state] = true;

            }
            
            
            {
                int startup = 19;
                int active = 2;
                int hurtboxDuration = 15;
                string path = "Rekka2A";
                int state = GirlShotoState.Rekka2A;
                
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
                        new(2, FP.FromString("4.5")),
                        new(4, 0),
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

                var projectileInvuln = new SectionGroup<bool>()
                {
                    Sections = new List<Tuple<int, bool>>()
                    {
                        new(startup + active, true),
                        new(50, false),
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
                            // BonusBlockstun = 6,
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.6"),
                            TrajectoryHeight = FP.FromString("4.75"),
                            TrajectoryXVelocity = FP.FromString("3"),
                            Launches = true,
                            GroundBounce = true,
                            Type = Hit.HitType.High,
                            BlockPushback = FP.FromString("0.5"),
                            VisualAngle = 70,
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
                                                Height = FP.FromString("5.5"),
                                                Width = FP.FromString("3"),
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
                int startup = 10;
                int active = 2;
                int hurtboxDuration = 15;
                string path = "Rekka2B";
                int state = GirlShotoState.Rekka2B;
                
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
                        new(startup - 3, 0),
                        new(3, FP.FromString("2")),
                        new(4, 0),
                        new(8, FP.FromString("-1.25")),
                        new(4, 0),
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
                            GravityScaling = FP.FromString("0.925"),
                            GravityProration = FP.FromString("1.6"),
                            TrajectoryHeight = FP.FromString("3.25"),
                            TrajectoryXVelocity = FP.FromString("9"),
                            BlockPushback = FP.FromString("3"),
                            HitPushback = 3,
                            VisualAngle = -30,
                            Launches = true,
                            // HardKnockdown = true,
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
                                                Height = FP.FromString("6.0"),
                                                Width = FP.FromString("5.0"),
                                                GrowWidth = true,
                                                GrowHeight = true,
                                                PosY = FP.FromString("2.5"),
                                                PosX = 0
                                            },
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
                int startup = 5;
                int active = 2;
                int hurtboxDuration = 15;
                string path = "DP";
                int state = GirlShotoState.DP;
                
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
                        new(startup - 3, 0),
                        new(3, FP.FromString("1")),
                        new(50, 4),
                        new(5, 0),
                    }
                };
                

                var hurtboxes = new SectionGroup<CollisionBoxCollection>()
                {
                    Sections = new List<Tuple<int, CollisionBoxCollection>>()
                    {
                        new(startup + 6, null),
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
                    Level = 0,
                    GravityScaling = FP.FromString("1"),
                    GravityProration = FP.FromString("1"),
                    TrajectoryHeight = FP.FromString("2.25"),
                    TrajectoryXVelocity = FP.FromString("12"),
                    BlockPushback = FP.FromString("3"),
                    HitPushback = 3,
                    VisualAngle = -70,
                    Launches = true,
                    HardKnockdown = false,
                    // GroundBounce = true,
                    Damage = 25,
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
                                        Height = FP.FromString("8.0"),
                                        Width = FP.FromString("6.0"),
                                        GrowWidth = false,
                                        GrowHeight = true,
                                        PosY = 0,
                                        PosX = 0
                                    },
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
                        new(active, hit),
                        new(active, hit),
                        // new(active, hit),
                        // new(active, hit),
                        // new(active, hit),
                        new (20, null)
                    }
                };

                var hurtType = new SectionGroup<HurtType>()
                {
                    Sections = new List<Tuple<int, HurtType>>()
                    {
                        new(1000, HurtType.Counter)
                    }
                };

                var trajectory = new SectionGroup<Trajectory>()
                {
                    Sections = new List<Tuple<int, Trajectory>>()
                    {
                        new(startup, new Trajectory()
                        {
                            TimeToTrajectoryHeight = startup,
                            TrajectoryHeight = FP.FromString("0.1"),
                            TrajectoryXVelocity = 0
                        }),
                        new(10, new Trajectory()
                        {
                            TimeToTrajectoryHeight = 30,
                            TrajectoryHeight = 5,
                            TrajectoryXVelocity = 0
                        })
                    } 
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = 1000;
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.MovementSectionGroup.Dictionary[state] = move;
                StateMapConfig.TrajectorySectionGroup.Dictionary[state] = trajectory;
            }
            
            
            {
                int startup = 6;
                int active = 2;
                int hurtboxDuration = 5;
                string path = "JL";
                int state = GirlShotoState.JL;
                
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
                        new(40, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                new CollisionBox()
                                {
                                    GrowHeight = false,
                                    GrowWidth = false,
                                    PosX = -1,
                                    PosY = 2,
                                    Height = 5,
                                    Width = 4,
                                }
                            }
                        })
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
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.2"),
                            Type = Hit.HitType.High,
                            TrajectoryHeight = FP.FromString("0"),
                            TrajectoryXVelocity = 12,
                            Level = 0,
                            VisualAngle = 0,
                            Damage = lowDamage,
                            DamageScaling = FP.FromString("0.5"), 
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
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 4;
                
            }
            
            {
                int startup = 18;
                int active = 2;
                int hurtboxDuration = 5;
                string path = "JH";
                int state = GirlShotoState.JH;
                
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
                                airHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                airHurtbox,
                                new CollisionBox()
                                {
                                    Height = 4,
                                    Width = 3,
                                    GrowWidth = true,
                                    GrowHeight = false,
                                    PosY = -2,
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                airHurtbox
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
                            GravityScaling = FP.FromString("1"),
                            GravityProration = FP.FromString("1.2"),
                            Type = Hit.HitType.High,
                            TrajectoryHeight = FP.FromString("4.25"),
                            TrajectoryXVelocity = 14,
                            Level = 4,
                            VisualAngle = 70,
                            GroundBounce = true,
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
                                                Height = 4,
                                                Width = FP.FromString("2.75"),
                                                GrowWidth = true,
                                                GrowHeight = false,
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

                var trajectory = new SectionGroup<Trajectory>()
                {
                    Sections = new List<Tuple<int, Trajectory>>()
                    {
                        new (5, null),
                        new (10, new Trajectory()
                        {
                            TimeToTrajectoryHeight = 6,
                            TrajectoryHeight = FP.FromString("3.5"),
                            TrajectoryXVelocity = 19,
                        })
                    }
                };

                Util.AutoSetupFromAnimationPath(animation, this);
                StateMapConfig.FighterAnimation.Dictionary[state] = animation;
                StateMapConfig.Duration.Dictionary[state] = animation.SectionGroup.Duration();
                StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[state] = hurtboxes;
                StateMapConfig.HitSectionGroup.Dictionary[state] = hitboxes;
                StateMapConfig.HurtTypeSectionGroup.Dictionary[state] = hurtType;
                StateMapConfig.CancellableAfter.Dictionary[state] = startup + 4;
                StateMapConfig.TrajectorySectionGroup.Dictionary[state] = trajectory;

            }
            
            
            {
                int startup = 11;
                int active = 2;
                int gap = 2;
                int hurtboxDuration = 13;
                string path = "JM";
                int state = GirlShotoState.JM;
                
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
                                airHurtbox
                            }
                        }),
                        new(hurtboxDuration, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                airHurtbox,
                                new CollisionBox()
                                {
                                    Height = 4,
                                    Width = 6,
                                    GrowWidth = true,
                                    GrowHeight = false,
                                    PosY = 2,
                                    PosX = 0
                                }
                            }
                        }),
                        new(20, new CollisionBoxCollection()
                        {
                            CollisionBoxes = new List<CollisionBox>()
                            {
                                airHurtbox
                            }
                        }),
                    }
                };
                
                var trajectoryYMod = new SectionGroup<FP>()
                {
                    Sections = new List<Tuple<int, FP>>()
                    {
                        new(10, FP.FromString("0.8")),
                        new(10, 1),
                    } 
                };

                var hit1 = new Hit()
                {
                    BlockPushback = 3,
                    HitPushback = 2,
                    GravityScaling = FP.FromString("1"),
                    GravityProration = FP.FromString("1.3"),
                    Type = Hit.HitType.High,
                    TrajectoryHeight = 2,
                    Level = 2,
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
                                        Width = FP.FromString("4.5"),
                                        GrowWidth = true,
                                        GrowHeight = false,
                                        PosY = 3,
                                        PosX = 1
                                    }
                                }
                            })
                        }
                    }
                };
                
                var hit2 = new Hit()
                {
                    BlockPushback = 3,
                    HitPushback = 2,
                    GravityScaling = FP.FromString("1"),
                    GravityProration = FP.FromString("1.3"),
                    Type = Hit.HitType.High,
                    TrajectoryHeight = 2,
                    TrajectoryXVelocity = 3,
                    Level = 2,
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
                                        Width = FP.FromString("3"),
                                        GrowWidth = true,
                                        GrowHeight = false,
                                        PosY = 2,
                                        PosX = FP.FromString("2.25")
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
                        new(active, hit1),
                        new(gap, null),
                        new(active, hit2),
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
                // StateMapConfig.TrajectoryYVelocityMod.Dictionary[state] = trajectoryYMod;

            }
            
            
            
            

        }
        
        protected virtual void DashMomentumCallback(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;

            FP amount = IsFacingRight(frameParam.f, EntityRef) ? 4 : -4;
            StartMomentum(frameParam.f, amount);

            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.Dash,
                transform3D->Position.XY, 0, !IsFacingRight(frameParam.f, EntityRef));
        }


        public override void SetupMachine()
        {
            base.SetupMachine();
            
            Fsm.Configure(PlayerState.Dash)
                .Permit(PlayerTrigger.Jump, PlayerState.Jumpsquat)
                // .Permit(Trigger.Backward, State.WalkBackward)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.ProxBlockHigh, PlayerState.ProxStandBlock, _ => true, -3)
                .PermitIf(PlayerTrigger.ProxBlockLow, PlayerState.ProxCrouchBlock, _ => true, -3)
                .Permit(PlayerTrigger.ForwardThrow, PlayerState.ForwardThrow)
                .Permit(PlayerTrigger.BackThrow, PlayerState.Backthrow)
                .OnExitFrom(PlayerTrigger.ForwardThrow, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.BackThrow, DashMomentumCallback)
                // .OnExitFrom(Trigger.ThrowTech, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.ButtonAndDirection, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.Jump, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.JumpCancel, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.HitHigh, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.HitLow, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.BlockHigh, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.BlockLow, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.ProxBlockHigh, DashMomentumCallback)
                .OnExitFrom(PlayerTrigger.ProxBlockLow, DashMomentumCallback)
                // .OnExitFrom(Trigger.ThrowConnect, StartMomentumCallback)
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
                JumpCancellable = false,
                InputWeight = 0,
                RawOk = true,
                State = GirlShotoState._5L,
                
                Name = "Standing light",
                Description = "A fast jab, good at breaking out of pressure."
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
                State = GirlShotoState._2L,
                
                Name = "Crouching light",
                Description = "A fast jab, good at breaking out of pressure."
            };
            
            ConfigureAction(this, _2L);
            MakeActionCancellable(this, _5L, _5L);
            MakeActionCancellable(this, _5L, _2L);
            MakeActionCancellable(this, _2L, _5L);
            MakeActionCancellable(this, _2L, _2L);

            
            ActionConfig _5M = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = true,
                GroundOk = true,
                InputType = InputSystem.InputType.M,
                JumpCancellable = true,
                InputWeight = 0,
                RawOk = true,
                State = GirlShotoState._5M,
                
                Name = "Standing medium",
                AnimationDisplayFrameIndex = 11,
                Description = "A quick mid-ranged poke."
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
                InputType = InputSystem.InputType.M,
                JumpCancellable = false,
                InputWeight = 2,
                RawOk = true,
                State = GirlShotoState._2M,
                
                Name = "Crouching medium",
                Description = "A quick, low, mid-ranged poke.",
                AnimationDisplayFrameIndex = 11
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
                State = GirlShotoState._5H,
                
                Name = "Standing heavy",
                Description = "A powerful swing that carries you forward and sends aerial opponents flying.",
                AnimationDisplayFrameIndex = 13
            };
            
            ConfigureAction(this, _5H);
            
            ActionConfig _4H = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 4,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.H,
                JumpCancellable = true,
                InputWeight = 3,
                RawOk = true,
                State = GirlShotoState._4H,
                
                Name = "Back heavy",
                Description = "A potent close-ranged elbow, great for starting offense and stringing together combos.",
                AnimationDisplayFrameIndex = 7
            };
            ConfigureAction(this, _4H);
            MakeActionCancellable(this, _4H, _5H);

            
            
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
                State = GirlShotoState._2H,
                
                Name = "Crouching heavy",
                Description = "A powerful, upwards kick that launches grounded opponents into the air.",
                AnimationDisplayFrameIndex = 17
            };
            
            ConfigureAction(this, _2H);
            MakeActionCancellable(this, _4H, _2H);
            
            
            ActionConfig JL = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = false,
                InputType = InputSystem.InputType.L,
                JumpCancellable = false,
                InputWeight = 0,
                RawOk = true,
                State = GirlShotoState.JL,
                
                Name = "Jumping light",
                Description = "A quick punch that can be used to stop incoming aerial opponents."
            };
            
            ConfigureAction(this, JL);
            
            
            ActionConfig JM = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 5,
                Crouching = false,
                DashCancellable = false,
                GroundOk = false,
                InputType = InputSystem.InputType.M,
                JumpCancellable = true,
                InputWeight = 0,
                RawOk = true,
                State = GirlShotoState.JM,
                
                Name = "Jumping medium",
                AnimationDisplayFrameIndex = 13,
                Description = "A two-hit bicycle kick that can combo opponents in the air."
            };
            
            ConfigureAction(this, JM);
            
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
                AnimationDisplayFrameIndex = 11,
                State = GirlShotoState.JH,
                
                Name = "Jumping heavy",
                Description = "A downwards kick that can be used to deter approaching opponents."
            };
            
            ConfigureAction(this, JH);
            

            
            ActionConfig frontThrow = new ActionConfig()
            {
                Aerial = false,
                State = GirlShotoState.ForwardThrowCutscene,
                IsCutscene = true,
                
                Name = "Forward throw"
            };
            
            ConfigureAction(this, frontThrow);
            
            ActionConfig backThrow = new ActionConfig()
            {
                Aerial = false,
                State = GirlShotoState.BackThrowCutscene,
                IsCutscene = true,
                
                Name = "Backward throw"
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
                InputWeight = 4,
                RawOk = true,
                State = GirlShotoState.Fireball,
                IsSpecial = true,
                SpecialCancellable = false,
                
                Name = "Breath",
                Description = "A concentrated ball of ki that travels forward, great for controlling space and keeping opponents out.",
                // FlavorText = "Few monks in the Temple's history have attained the ability to manifest their ki into tangible power.",
                AnimationDisplayFrameIndex = 20
            };
            
            ConfigureAction(this, fireball);
            
            // MakeActionCancellable(this, _5M, fireball);
            // MakeActionCancellable(this, _5H, fireball);
            // MakeActionCancellable(this, _4H, fireball);

            
            ActionConfig Rekka1 = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 5,
                Crouching = true,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 3,
                RawOk = true,
                State = GirlShotoState.Rekka1,
                IsSpecial = true,
                SpecialCancellable = false,
                
                Name = "Ki Dipper",
                Description = "A striking, projectile-invulnerable lunge that can be chained into 2 follow-ups. This move can be cancelled early and cancelled on whiff.",
                FlavorText = "\"Flesh and soul are water.\" - Founder Win, 1st Histories of the Dridegion",
                AnimationDisplayFrameIndex = 20
            };
            
            ConfigureAction(this, Rekka1);
            // MakeActionCancellable(this, _4H, Rekka1);
            // MakeActionCancellable(this, _5H, Rekka1);
            // MakeActionCancellable(this, _5M, Rekka1);
            // MakeActionCancellable(this, _2M, Rekka1);
            
            ActionConfig Rekka2A = new ActionConfig()
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
                RawOk = false,
                State = GirlShotoState.Rekka2A,
                IsSpecial = true,
                SpecialCancellable = false,
                
                Name = "Falling Water",
                Description = "A teleporting strike that must be blocked standing. It ground-bounces opponents into the air.",
                // FlavorText = "Snap.",
                AnimationDisplayFrameIndex = 20,
                WhileIn = Rekka1
            };
            
            ConfigureAction(this, Rekka2A);
            MakeActionCancellable(this, Rekka1, Rekka2A);
            
            ActionConfig Rekka2B = new ActionConfig()
            {
                Aerial = false,
                AirOk = false,
                CommandDirection = 4,
                Crouching = false,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 5,
                RawOk = false,
                State = GirlShotoState.Rekka2B,
                IsSpecial = true,
                SpecialCancellable = false,
                
                Name = "Resolution",
                Description = "A fast, long range kick, great for ending pressure sequences safely.",
                AnimationDisplayFrameIndex = 11,
                WhileIn = Rekka1
            };
            
            ConfigureAction(this, Rekka2B);
            MakeActionCancellable(this, Rekka1, Rekka2B);



            ActionConfig dp = new ActionConfig()
            {
                Aerial = true,
                AirOk = true,
                CommandDirection = 2,
                DashCancellable = false,
                GroundOk = true,
                InputType = InputSystem.InputType.S,
                JumpCancellable = false,
                InputWeight = 5,
                RawOk = true,
                State = GirlShotoState.DP,
                IsSpecial = true,
                SpecialCancellable = false,
                
                Description = "A rising multi-hit attack that is invincible on startup, but leaves you highly exposed when blocked. Great as a defensive option.",
                Name = "Fountain",
                AnimationDisplayFrameIndex = 11
            };
            
            ConfigureAction(this, dp);
            // MakeActionCancellable(this, _4H, dp);
            // MakeActionCancellable(this, _5H, dp);
            // MakeActionCancellable(this, _5M, dp);
            // MakeActionCancellable(this, _2M, dp);
            

        }
    }
}



        
