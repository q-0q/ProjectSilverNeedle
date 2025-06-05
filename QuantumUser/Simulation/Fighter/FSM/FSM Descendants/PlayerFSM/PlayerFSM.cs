using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.QuantumUser.Simulation.Fighter.Types;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;
using Random = UnityEngine.Random;


namespace Quantum
{
    public abstract unsafe partial class PlayerFSM : FSM
    {
        public class PlayerState : FSMState
        {
            // Ground
            public static int StandActionable;
            public static int CrouchActionable;
            public static int WalkForward;
            public static int WalkBackward;
            public static int Dash;
            public static int Backdash;
            public static int GroundAction;
            public static int GroundActionable;
            public static int GroundMoveable;
            public static int StandHitHigh;
            public static int StandHitLow;
            public static int CrouchHit;
            public static int StandBlock;
            public static int ProxBlock;
            public static int ProxStandBlock;
            public static int CrouchBlock;
            public static int ProxCrouchBlock;
            public static int GroundBlock;
            public static int HardKnockdown;
            public static int SoftKnockdown;
            public static int Landsquat;
            public static int EmptyLandsquat;
            public static int FullLandsquat;
            public static int BlockLandsquat;
            public static int DeadFromAir;
            public static int DeadFromGround;
            public static int ForwardThrow;
            public static int Backthrow;
            public static int Tech;
            public static int Break;
            public static int RedBreak;
            // public static int Surge;
            

            // Air
            public static int AirDash;
            public static int AirBackdash;
            public static int AirAction;
            public static int AirActionable;
            public static int AirActionableAfterAction;
            public static int AirHit;
            public static int AirHitHigh;
            public static int AirHitLow;
            public static int AirHitLaunch;
            public static int AirBlock;
            public static int AirHitPostGroundBounce;
            public static int AirHitPostWallBounce;
            public static int Jumpsquat;


            // General
            public static int Hit;
            public static int Ground;
            public static int Air;
            public static int Any;
            public static int Action;
            public static int AirSpecialCancellable;
            public static int GroundSpecialCancellable;
            public static int Stand;
            public static int Crouch;
            public static int Block;
            public static int Throw;
            public static int Cutscene;
            public static int CutsceneReactor;
            public static int TechableCutsceneReactor;
            public static int DirectionLocked;
        }

        public class CutsceneIndexes : InheritableEnum.InheritableEnum
        {
            public static int ForwardThrow;
            public static int BackwardThrow;
        }

        public class PlayerTrigger : Trigger
        {
            public static int Land;
            public static int HitWall;
            public static int JumpCancel;
            public static int BecameAerial;
            public static int HitHigh;
            public static int HitLow;
            public static int BlockHigh;
            public static int BlockLow;
            public static int ProxBlockHigh;
            public static int ProxBlockLow;
            public static int EndProxBlock;
            public static int Die;
        }

        public FP DamageTakenModifier = 1;

        public FP FallSpeed;
        public int FallTimeToSpeed;
        public int JumpCount;

        public int MinimumDashDuration = 1000; // set to a huge number to disable plinking by default

        public Trajectory UpwardJumpTrajectory;
        public Trajectory ForwardJumpTrajectory;
        public Trajectory BackwardJumpTrajectory;
        public int JumpsquatDuration;

        public List<ActionConfig> NormalMoveList;
        public List<ActionConfig> AirNormalMoveList;
        public List<ActionConfig> CommandNormalMoveList;
        public List<ActionConfig> SpecialMoveList;
        public List<ActionConfig> SuperMoveList;
        
        public Dictionary<int, int> ActionStartupReduction;

        public static readonly int ThrowStartupDuration = 4;


        public SummonPool JumpGameFXSummonPool;


        public PlayerFSM()
        {
            int currentState = PlayerState.StandActionable;
            Fsm = new Machine<int, int>(currentState);
            NormalMoveList = new List<ActionConfig>();
            AirNormalMoveList = new List<ActionConfig>();
            CommandNormalMoveList = new List<ActionConfig>();
            SpecialMoveList = new List<ActionConfig>();
            SuperMoveList = new List<ActionConfig>();
            ActionStartupReduction = new Dictionary<int, int>();


            JumpGameFXSummonPool = new SummonPool()
            {
                Size = 1,
                SummonFSMType = typeof(JumpGameFXSummonFSM)
            };
        }


        public override void SetupMachine()
        {
            base.SetupMachine();
            
            var machine = Fsm;

            // Ground
            machine.Configure(PlayerState.Ground)
                .OnEntryFrom(PlayerTrigger.Land, OnLand)
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.AirHitLaunch, IsCollisionHitParamLauncher, 1)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.AirHitLow, IsCollisionHitParamLauncher, 1)
                .PermitIf(PlayerTrigger.Die, PlayerState.DeadFromGround, PlayerIsDead, 3)
                .SubstateOf(PlayerState.Any);

            machine.Configure(PlayerState.Stand)
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.StandHitHigh, _ => true, -2)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.StandHitLow, _ => true, -2)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.Crouch)
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.CrouchHit, _ => true, -2)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.CrouchHit, _ => true, -2)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.GroundActionable)
                .Permit(Trigger.ForwardThrow, PlayerState.ForwardThrow)
                .Permit(Trigger.BackThrow, PlayerState.Backthrow)
                .Permit(PlayerTrigger.BecameAerial, PlayerState.AirActionable)
                // .Permit(Trigger.FrontThrow, State.ThrowFrontStartup)
                // .Permit(Trigger.BackThrow, State.ThrowBackStartup)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.ProxBlockHigh, PlayerState.ProxStandBlock, _ => true, -3)
                .PermitIf(PlayerTrigger.ProxBlockLow, PlayerState.ProxCrouchBlock, _ => true, -3)
                // .PermitIf(Trigger.ReceiveKinematics, State.TechableKinematicReceiver, _ => true, 0)
                .OnEntry(EndSlowdown)
                .OnEntry(ResetCombo)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.GroundMoveable)
                .Permit(Trigger.NeutralInput, PlayerState.StandActionable)
                .Permit(Trigger.Down, PlayerState.CrouchActionable)
                .Permit(Trigger.Forward, PlayerState.WalkForward)
                .Permit(Trigger.Backward, PlayerState.WalkBackward)
                .Permit(Trigger.Dash, PlayerState.Dash)
                .Permit(Trigger.Backdash, PlayerState.Backdash)
                .Permit(Trigger.Jump, PlayerState.Jumpsquat)
                .SubstateOf(PlayerState.GroundActionable);

            machine.Configure(PlayerState.StandActionable)
                .SubstateOf(PlayerState.GroundMoveable)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.CrouchActionable)
                .SubstateOf(PlayerState.GroundMoveable)
                .SubstateOf(PlayerState.Crouch);

            machine.Configure(PlayerState.WalkForward)
                .SubstateOf(PlayerState.StandActionable);

            machine.Configure(PlayerState.WalkBackward)
                .SubstateOf(PlayerState.StandActionable);
            
            // machine.Configure(PlayerState.Surge)
            //     .OnEntry(OnRedBreak)
            //     .SubstateOf(PlayerState.Dash);

            machine.Configure(PlayerState.Backdash)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .OnEntry(OnBackdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(PlayerState.Stand)
                .SubstateOf(PlayerState.DirectionLocked)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.GroundAction)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .Permit(PlayerTrigger.JumpCancel, PlayerState.Jumpsquat)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(ResetWhiff)
                .OnEntry(HandleEmpoweredStartup)
                .OnExit(ResetEmpoweredHit)
                .SubstateOf(PlayerState.Action)
                .SubstateOf(PlayerState.DirectionLocked)
                .PermitIf(PlayerTrigger.ButtonAndDirection, PlayerState.RedBreak, IsBreakUnwhiffedInput)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.StandHitHigh)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .AllowReentry(PlayerTrigger.HitHigh)
                .Permit(PlayerTrigger.HitHigh, PlayerState.StandHitHigh)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.StandHitLow)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .AllowReentry(PlayerTrigger.HitLow)
                .Permit(PlayerTrigger.HitLow, PlayerState.StandHitLow)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.CrouchHit)
                .Permit(PlayerTrigger.Finish, PlayerState.CrouchActionable)
                .AllowReentry(PlayerTrigger.HitHigh)
                .AllowReentry(PlayerTrigger.HitLow)
                .Permit(PlayerTrigger.HitHigh, PlayerState.CrouchHit)
                .Permit(PlayerTrigger.HitLow, PlayerState.CrouchHit)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Crouch);

            machine.Configure(PlayerState.StandBlock)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .AllowReentry(PlayerTrigger.BlockHigh)
                .Permit(PlayerTrigger.BlockHigh, PlayerState.StandBlock)
                .SubstateOf(PlayerState.GroundBlock)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.CrouchBlock)
                .Permit(PlayerTrigger.Finish, PlayerState.CrouchActionable)
                .AllowReentry(PlayerTrigger.BlockLow)
                .Permit(PlayerTrigger.BlockLow, PlayerState.CrouchBlock)
                .SubstateOf(PlayerState.GroundBlock)
                .SubstateOf(PlayerState.Crouch);

            machine.Configure(PlayerState.GroundBlock)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -3)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -3)
                .PermitIf(Trigger.ButtonAndDirection, PlayerState.Break, IsMeterInput)
                .SubstateOf(PlayerState.Ground)
                .SubstateOf(PlayerState.Block);


            machine.Configure(PlayerState.ProxBlock)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -3)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -3)
                .SubstateOf(PlayerState.GroundActionable)
                .Permit(Trigger.NeutralInput, PlayerState.StandActionable)
                // .Permit(Trigger.Down, PlayerState.CrouchActionable)
                .Permit(Trigger.Forward, PlayerState.WalkForward)
                .Permit(Trigger.Dash, PlayerState.Dash)
                .Permit(Trigger.Backdash, PlayerState.Backdash)
                .Permit(Trigger.Jump, PlayerState.Jumpsquat);


            machine.Configure(PlayerState.ProxStandBlock)
                .SubstateOf(PlayerState.ProxBlock)
                .SubstateOf(PlayerState.Stand)
                .Permit(PlayerTrigger.EndProxBlock, PlayerState.StandActionable);

            machine.Configure(PlayerState.ProxCrouchBlock)
                .SubstateOf(PlayerState.ProxBlock)
                .SubstateOf(PlayerState.Crouch)
                .Permit(PlayerTrigger.EndProxBlock, PlayerState.CrouchActionable);
                
            
            machine.Configure(PlayerState.HardKnockdown)
                .OnEntry(DoImpactVibrate)
                .OnEntry(OnHKD)
                .OnEntry(EndSlowdown)
                .OnExitFrom(Trigger.Finish, StartThrowProtection)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .SubstateOf(PlayerState.DirectionLocked)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.SoftKnockdown)
                .OnEntry(EndSlowdown)
                .OnExitFrom(Trigger.Finish, StartThrowProtection)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.DeadFromAir)
                .OnEntry(DoImpactVibrate)
                .OnEntry(EndSlowdown)
                .SubstateOf(PlayerState.DirectionLocked)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.DeadFromGround)
                .OnEntry(EndSlowdown)
                .SubstateOf(PlayerState.DirectionLocked)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.Landsquat)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .SubstateOf(PlayerState.Crouch)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.EmptyLandsquat)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -3)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -3)
                .SubstateOf(PlayerState.GroundActionable)
                .Permit(Trigger.Backdash, PlayerState.Backdash)
                .SubstateOf(PlayerState.Landsquat);
            
            machine.Configure(PlayerState.FullLandsquat)
                .SubstateOf(PlayerState.Landsquat);
            
            machine.Configure(PlayerState.BlockLandsquat)
                .SubstateOf(PlayerState.GroundBlock)
                .OnEntry(InvokeLandingRecoveryBlockstun)
                .SubstateOf(PlayerState.Landsquat);

            machine.Configure(PlayerState.Throw)
                .SubstateOf(PlayerState.Ground)
                .SubstateOf(PlayerState.Stand)
                .OnEntry(InputSystem.ClearBufferParams)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable);

            machine.Configure(PlayerState.ForwardThrow)
                .SubstateOf(PlayerState.Throw);

            machine.Configure(PlayerState.Backthrow)
                .SubstateOf(PlayerState.Throw);

            machine.Configure(PlayerState.Tech)
                .OnEntry(OnEnterTech)
                .SubstateOf(PlayerState.GroundAction);
            
            machine.Configure(PlayerState.Break)
                .SubstateOf(PlayerState.Ground)
                .SubstateOf(PlayerState.Stand)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(OnBreak)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable);
            
            machine.Configure(PlayerState.RedBreak)
                .SubstateOf(PlayerState.Ground)
                .SubstateOf(PlayerState.Stand)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(OnRedBreak)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable);

            // Air
            machine.Configure(PlayerState.Air)
                .PermitIf(PlayerTrigger.Land, PlayerState.EmptyLandsquat, IsTrajectoryEmpty)
                .PermitIf(PlayerTrigger.Land, PlayerState.FullLandsquat, x => !IsTrajectoryEmpty(x))
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.AirHitLaunch, IsCollisionHitParamLauncher, 1)
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.AirHitHigh, _ => true, -1)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.AirHitLow, _ => true, -1)
                .SubstateOf(PlayerState.Any);

            machine.Configure(PlayerState.AirActionable)
                // .Permit(PlayerTrigger.Dash, PlayerState.AirDash)
                // .Permit(PlayerTrigger.Backdash, PlayerState.AirBackdash)
                .AllowReentry(PlayerTrigger.Jump)
                .OnEntryFrom(PlayerTrigger.Jump, StartNewJump)
                .OnEntryFrom(PlayerTrigger.Jump, InputSystem.ClearBufferParams)
                .OnEntryFrom(PlayerTrigger.JumpCancel, StartNewJump)
                .OnEntryFrom(PlayerTrigger.BecameAerial, StartNewFallFromApex)
                .Permit(Trigger.Jump, PlayerState.AirActionable)
                .Permit(PlayerTrigger.JumpCancel, PlayerState.AirActionable)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.AirBlock, _ => true, -1)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.AirBlock, _ => true, -1)
                .OnEntry(ResetCombo)
                .OnEntry(EndSlowdown)
                .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirActionableAfterAction)
                .SubstateOf(PlayerState.AirActionable);

            // machine.Configure(PlayerState.AirDash)
            //     .Permit(PlayerTrigger.Finish, PlayerState.AirActionable)
            //     .OnExitFrom(PlayerTrigger.ButtonAndDirection, DashMomentumCallback)
            //     .OnExitFrom(PlayerTrigger.Jump, DashMomentumCallback)
            //     .OnExitFrom(PlayerTrigger.JumpCancel, DashMomentumCallback)
            //     .OnExitFrom(PlayerTrigger.HitHigh, DashMomentumCallback)
            //     .OnExitFrom(PlayerTrigger.HitLow, DashMomentumCallback)
            //     .OnExitFrom(PlayerTrigger.BlockHigh, DashMomentumCallback)
            //     .OnExitFrom(PlayerTrigger.BlockLow, DashMomentumCallback)
            //     .OnEntry(OnAirdash)
            //     .OnEntry(MakeTrajectoryFull)
            //     .OnEntry(InputSystem.ClearBufferParams)
            //     .PermitIf(PlayerTrigger.BlockHigh, PlayerState.AirBlock, _ => true, -2)
            //     .PermitIf(PlayerTrigger.BlockLow, PlayerState.AirBlock, _ => true, -2)
            //     .SubstateOf(PlayerState.DirectionLocked)
            //     .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirBackdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(OnBackdash)
                .OnEntry(OnAirBackdash)
                .OnEntry(MakeTrajectoryFull)
                .Permit(PlayerTrigger.Finish, PlayerState.AirActionable)
                .SubstateOf(PlayerState.DirectionLocked)
                .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirAction)
                .Permit(PlayerTrigger.Finish, PlayerState.AirActionableAfterAction)
                .Permit(PlayerTrigger.JumpCancel, PlayerState.AirActionable)
                .SubstateOf(PlayerState.DirectionLocked)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(ResetWhiff)
                .OnEntry(HandleEmpoweredStartup)
                .OnExit(ResetEmpoweredHit)
                .OnEntry(MakeTrajectoryFull)
                .SubstateOf(PlayerState.Action)
                .SubstateOf(PlayerState.DirectionLocked)
                .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirBlock)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.AirBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.AirBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.Land, PlayerState.BlockLandsquat, _ => true, 3)
                .AllowReentry(PlayerTrigger.BlockHigh)
                .AllowReentry(PlayerTrigger.BlockLow)
                .Permit(PlayerTrigger.BlockHigh, PlayerState.AirBlock)
                .Permit(PlayerTrigger.BlockLow, PlayerState.AirBlock)
                .Permit(PlayerTrigger.Finish, PlayerState.AirActionable)
                .SubstateOf(PlayerState.Air)
                .SubstateOf(PlayerState.Block);

            machine.Configure(PlayerState.AirHit)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Air)
                .OnEntryFrom(PlayerTrigger.HitHigh, StartNewJuggle)
                .OnEntryFrom(PlayerTrigger.HitLow, StartNewJuggle)
                // .OnEntryFrom(PlayerTrigger.Finish, StartNewJuggle) ????
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.AirHitHigh, _ => true, -2)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.AirHitLow, _ => true, -2)
                .PermitIf(PlayerTrigger.Land, PlayerState.AirHitPostGroundBounce, IsInGroundBounce, 4)
                .Permit(PlayerTrigger.HitWall, PlayerState.AirHitPostWallBounce)
                .PermitIf(PlayerTrigger.Land, PlayerState.DeadFromAir, PlayerIsDead, 3)
                .PermitIf(PlayerTrigger.Land, PlayerState.HardKnockdown, InHardKnockdown, 2)
                .PermitIf(PlayerTrigger.Land, PlayerState.SoftKnockdown, _ => true, 1)
                .OnExitFrom(PlayerTrigger.Land, OnExitAirHitFromLand)
                .AllowReentry(PlayerTrigger.HitHigh)
                .AllowReentry(PlayerTrigger.HitLow)
                .AllowReentry(PlayerTrigger.Land)
                .Permit(PlayerTrigger.HitHigh, PlayerState.AirHitHigh)
                .Permit(PlayerTrigger.HitLow, PlayerState.AirHitLow)
                .Permit(PlayerTrigger.Land, PlayerState.AirHit);

            machine.Configure(PlayerState.AirHitHigh)
                .SubstateOf(PlayerState.AirHit);
            
            machine.Configure(PlayerState.AirHitLow)
                .SubstateOf(PlayerState.AirHit);
            
            machine.Configure(PlayerState.AirHitLaunch)
                .Permit(Trigger.Finish, PlayerState.AirHitHigh)
                .SubstateOf(PlayerState.AirHit);

            machine.Configure(PlayerState.AirHitPostGroundBounce)
                .OnEntry(OnGroundBounce)
                .SubstateOf(PlayerState.AirHit);

            machine.Configure(PlayerState.AirHitPostWallBounce)
                .OnEntry(OnWallBounce)
                .SubstateOf(PlayerState.AirHit);

            machine.Configure(PlayerState.Jumpsquat)
                .SubstateOf(PlayerState.Crouch)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntryFrom(PlayerTrigger.Finish, StartNewJump)
                .OnEntryFrom(PlayerTrigger.Jump, StartNewJump)
                .OnEntryFrom(PlayerTrigger.JumpCancel, StartNewJump)
                .OnEntry(MakeTrajectoryEmpty)
                .Permit(Trigger.Finish, PlayerState.AirActionable);

            // General
            machine.Configure(PlayerState.Hit)
                .SubstateOf(PlayerState.DirectionLocked)
                .OnExitFrom(Trigger.Finish, StartThrowProtection);
            machine.Configure(PlayerState.Block)
                .OnExitFrom(Trigger.Finish, StartThrowProtection);
            
            machine.Configure(PlayerState.Any);

            machine.Configure(PlayerState.Cutscene)
                .Permit(Trigger.Tech, PlayerState.Tech);
            
            machine.Configure(PlayerState.CutsceneReactor)
                .SubstateOf(PlayerState.DirectionLocked)
                .Permit(PlayerTrigger.Finish, PlayerState.AirHitHigh);
                
            machine.Configure(PlayerState.TechableCutsceneReactor)
                .SubstateOf(PlayerState.CutsceneReactor)
                .Permit(Trigger.Tech, PlayerState.Tech);

            machine.Configure(PlayerState.AirSpecialCancellable)
                .SubstateOf(PlayerState.Action);
            
            machine.Configure(PlayerState.GroundSpecialCancellable)
                .SubstateOf(PlayerState.Action);
            
            machine.Assume(PlayerState.StandActionable);
        }



        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
            
            
            
            StateMapConfig.Duration.DefaultValue = 0;
            StateMapConfig.Duration.SuperFuncDictionary[PlayerFSM.PlayerState.Hit] = GetStun;
            StateMapConfig.Duration.SuperFuncDictionary[PlayerFSM.PlayerState.Block] = GetStun;
            StateMapConfig.Duration.SuperFuncDictionary[PlayerFSM.PlayerState.CutsceneReactor] = GetCutsceneReactorDuration;
            
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.HardKnockdown] = 50;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.SoftKnockdown] = 20;
            StateMapConfig.Duration.SuperDictionary[PlayerFSM.PlayerState.Throw] = 40;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.Tech] = 23;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.Jumpsquat] = JumpsquatDuration;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.EmptyLandsquat] = 8;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.FullLandsquat] = 7;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.AirHitLaunch] = 7;
            
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.Break] = 20;
            StateMapConfig.Duration.Dictionary[PlayerFSM.PlayerState.RedBreak] = 15;

            // StateMapConfig.HurtboxCollectionSectionGroup.Dictionary[PlayerState.Cutscene] = null;

            var oneSectionGroup = new SectionGroup<FP>()
                { Sections = new List<Tuple<int, FP>>() { new(1, 1) } };
            
            StateMapConfig.TrajectoryYVelocityMod.DefaultValue = oneSectionGroup;
            StateMapConfig.TrajectoryXVelocityMod.DefaultValue = oneSectionGroup;
            
            StateMapConfig.HurtTypeSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Break] = new SectionGroup<PlayerFSM.HurtType>()
            {
                Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                {
                    new(100, PlayerFSM.HurtType.Punish)
                }
            };
            
            StateMapConfig.HurtTypeSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Throw] = new SectionGroup<PlayerFSM.HurtType>()
            {
                Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                {
                    new(100, PlayerFSM.HurtType.Counter)
                }
            };


            var throwActiveFrames = 1;
            StateMapConfig.HitSectionGroup.SuperDictionary[PlayerFSM.PlayerState.ForwardThrow] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new(ThrowStartupDuration, null),
                    new(throwActiveFrames, new Hit()
                    {
                        Type = Hit.HitType.Throw,
                        TriggerCutscene = PlayerFSM.CutsceneIndexes.ForwardThrow,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new(throwActiveFrames, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowWidth = false,
                                            GrowHeight = false,
                                            PosY = 4,
                                            Width = FP.FromString("4.75"),
                                            Height = 1,
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new(10, null)
                }
            };

            StateMapConfig.HitSectionGroup.SuperDictionary[PlayerFSM.PlayerState.Backthrow] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new(ThrowStartupDuration, null),
                    new(throwActiveFrames, new Hit()
                    {
                        Type = Hit.HitType.Throw,
                        TriggerCutscene = PlayerFSM.CutsceneIndexes.BackwardThrow,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new(throwActiveFrames, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowWidth = false,
                                            GrowHeight = false,
                                            PosY = 4,
                                            Width = FP.FromString("4.75"),
                                            Height = 1,
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new(10, null)
                }
            };

            
            
            StateMapConfig.MovementSectionGroup.Dictionary[PlayerFSM.PlayerState.SoftKnockdown] = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new Tuple<int, FP>(20, -6)
                }
            };

            
            
            StateMapConfig.CancellableAfter.DefaultValue = 0;
            
            StateMapConfig.WhiffCancellable.DefaultValue = false;
            
            StateMapConfig.FireReceiverFinishAfter.DefaultValue = 10;
            
            StateMapConfig.InvulnerableBefore.DefaultValue = 0;

            StateMapConfig.UnpoolSummonSectionGroup.DefaultValue = null;

            StateMapConfig.SmearFrame.DefaultValue = null;


            StateMapConfig.UnpoolSummonSectionGroup.Dictionary[PlayerState.AirActionable] =
                new SectionGroup<SummonPool>()
                {
                    Sections = new List<Tuple<int, SummonPool>>()
                    {
                        new(1, JumpGameFXSummonPool)
                    }
                };
            
            
            return;


            int GetStun(FrameParam frameParam)
            {
                if (frameParam is null) return 0;

                frameParam.f.Unsafe.TryGetPointer<StunData>(frameParam.EntityRef, out var stunData);
                var stun = stunData->stun;
                return stun;
            }

            int GetCutsceneReactorDuration(FrameParam frameParam)
            {
                if (frameParam is null) return 0;
                var cutscene = Util.GetActiveCutscene(frameParam.f, frameParam.EntityRef);
                return cutscene?.ReactorDuration ?? 0;
            }
        }


        private void IncrementCombo(Frame f, FP gravityScaling, FP damageScaling, int hitTableId)
        {

        }

        private bool IsCollisionHitParamLauncher(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            var param = (CollisionHitParams)triggerParams;
            return param.Launches || Fsm.IsInState(PlayerState.Backdash) || Fsm.IsInState(PlayerState.Jumpsquat);
        }



        protected void StartMomentum(Frame f, FP totalDistance)
        {
            f.Unsafe.TryGetPointer<MomentumData>(EntityRef, out var pushbackData);
            pushbackData->framesInMomentum = 0;
            pushbackData->virtualTimeInMomentum = 0;
            pushbackData->momentumAmount = totalDistance;
        }
        
        private void OnEnterTech(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            
            Debug.Log("OnEnterTech");
            
            frameParam.f.Unsafe.TryGetPointer<CutsceneData>(EntityRef, out var cutsceneData);
            FsmLoader.FSMs[Util.GetOtherPlayer(frameParam.f, EntityRef)].Fsm.Fire(FSM.Trigger.Tech, frameParam);

            var isFacingRight = IsFacingRight(frameParam.f, EntityRef);
            
            FP pushback = isFacingRight ? -4 : 4;
            FP momentum = isFacingRight ? -2 : 2;
            StartPushback(frameParam.f, pushback);
            StartMomentum(frameParam.f, momentum);

            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            if (Util.GetPlayerId(frameParam.f, EntityRef) == 0)
            {
                AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.Tech,
                    new FPVector2(transform3D->Position.X, 5), 0, true);
            }
            
            // AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.Backdash,
            //     new FPVector2(transform3D->Position.X, 0), 0, isFacingRight);
        }
        
        private void InvokeLandingRecoveryBlockstun(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            Frame f = param.f;
            
            InvokeStun(f, Hit.LandingRecoveryBlockstun);
        }
        
        

        public void ResetCombo(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            Frame f = param.f;

            f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);
            comboData->length = 0;
            comboData->gravityScaling = 1;
            comboData->damageScaling = 1;
            f.ResolveDictionary(comboData->hitCounts).Clear();
        }

        protected override void OnStateChanged(TriggerParams? triggerParams)
        {
            base.OnStateChanged(triggerParams);
            
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            ForceUpdatePlayerDirection(param.f, EntityRef);
        }

        protected void StartThrowProtection(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            
            param.f.Unsafe.TryGetPointer<ProtectionData>(param.EntityRef, out var protectionData);
            protectionData->virtualTimeSinceThrowProtectionStart = 0;
        }

        private void DoImpactVibrate(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            HitstopSystem.EnqueueHitstop(param.f, 9);
            param.f.Events.EntityVibrate(EntityRef, FP.FromString("1.25"), FP.FromString("0.6"), 40);
        }

        private void OnHKD(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            param.f.Events.GameEvent(EntityRef, GameEventType.Knockdown);
        }


        private void StartDramatics(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            Util.StartDramatic(frameParam.f, EntityRef, 55);
        }

        private bool InHardKnockdown(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            var frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            return trajectoryData->hardKnockdown;
        }

        private bool PlayerIsDead(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            var frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            return healthData->health <= 0;
        }

        private void OnExitAirHitFromLand(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->groundBounce = false;
            trajectoryData->wallBounce = false;
        }

        private void OnBackdash(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;

            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.Backdash,
                transform3D->Position.XY, 0, !IsFacingRight(frameParam.f, EntityRef));
        }
        

        public override void IncrementClockByAmount(Frame f, EntityRef entityRef, FP virtualTimeIncrement)
        {
            base.IncrementClockByAmount(f, entityRef, virtualTimeIncrement);
            f.Unsafe.TryGetPointer<DramaticData>(entityRef, out var dramaticData);
            dramaticData->remaining = Math.Max(dramaticData->remaining - 1, 0);
            dramaticData->darkRemaining = Math.Max(dramaticData->darkRemaining - 1, 0);

            f.Unsafe.TryGetPointer<SlowdownData>(entityRef, out var slowdownData);
            slowdownData->slowdownRemaining--;
            
            f.Unsafe.TryGetPointer<PushbackData>(entityRef, out var pushbackData);
            pushbackData->framesInPushback++;
            pushbackData->virtualTimeInPushback += virtualTimeIncrement;
            
            f.Unsafe.TryGetPointer<MomentumData>(entityRef, out var momentumData);
            momentumData->framesInMomentum++;
            momentumData->virtualTimeInMomentum += virtualTimeIncrement;
            
            f.Unsafe.TryGetPointer<ProtectionData>(entityRef, out var protectionData);
            protectionData->virtualTimeSinceCrossupProtectionStart += virtualTimeIncrement;
            protectionData->virtualTimeSinceThrowProtectionStart += virtualTimeIncrement;
            
            f.Unsafe.TryGetPointer<HealthData>(entityRef, out var healthData);
            healthData->virtualTimeSinceEmpowered += virtualTimeIncrement;
            
            if (Fsm.IsInState(PlayerState.Jumpsquat)) return;
            f.Unsafe.TryGetPointer<TrajectoryData>(entityRef, out var trajectoryData);
            
            trajectoryData->virtualTimeInTrajectory += (virtualTimeIncrement);
        }

        public override EntityRef GetPlayer()
        {
            return EntityRef;
        }

        // FSM helper functions


        public class ActionConfig
        {
            public int State = -1;
            public InputSystem.InputType InputType = InputSystem.InputType.L;
            public int CommandDirection = 5;
            public bool JumpCancellable = false;
            public bool DashCancellable = false;
            public bool SpecialCancellable = true;
            public bool IsSpecial = false;
            public bool GroundOk = true;
            public bool AirOk = false;
            public bool RawOk = true;
            public bool Crouching = false;
            public bool Aerial = false;
            public int InputWeight = 0;
            public bool IsCutscene = false;

            // an additional optional clause that must be true to enter this action
            public Func<TriggerParams?, bool> BonusClause = _ => true;

            // non-authoritative fields for MoveList rendering
            public string Name = "No name provided";
            public string FlavorText = "";
            public string Description = "";
            public int AnimationDisplayFrameIndex = 5;
            public ActionConfig WhileIn = null;
            
            public int MinStandHitFrameAdvantage;
            public int MaxStandHitFrameAdvantage;
            
            public int MinCrouchHitFrameAdvantage;
            public int MaxCrouchHitFrameAdvantage;
            
            public int MinGroundBlockFrameAdvantage;
            public int MaxGroundBlockFrameAdvantage;
            
            public int MinAirBlockFrameAdvantage;
            public int MaxAirBlockFrameAdvantage;
            
        }

        protected void ConfigureAction(PlayerFSM fsm, ActionConfig actionConfig)
        {
            fsm.Fsm.Configure(actionConfig.State)
                .SubstateOf(actionConfig.Aerial ? PlayerFSM.PlayerState.AirAction : PlayerFSM.PlayerState.GroundAction);

            if (!actionConfig.Aerial)
            {
                fsm.Fsm.Configure(actionConfig.State)
                    .SubstateOf(actionConfig.Crouching ? PlayerFSM.PlayerState.Crouch : PlayerFSM.PlayerState.Stand);
            }
            
            if (actionConfig.IsCutscene)
            {
                fsm.Fsm.Configure(actionConfig.State)
                    .SubstateOf(PlayerState.Cutscene);
                return;
            }

            if (actionConfig.RawOk)
            {
                if (actionConfig.GroundOk)
                {
                    AllowRawFromState(fsm, actionConfig, PlayerFSM.PlayerState.GroundActionable);
                    AllowRawFromDash(fsm, actionConfig);
                }

                if (actionConfig.AirOk)
                {
                    AllowRawFromState(fsm, actionConfig, PlayerFSM.PlayerState.AirActionable);
                    AllowRawFromState(fsm, actionConfig, PlayerFSM.PlayerState.AirDash);
                }
            }
            

            Func<TriggerParams?, bool>? cancelClause = param =>
            {
                var bonus = actionConfig.BonusClause?.Invoke(param) ?? true;
                return Util.CanCancelNow(param) && bonus;
            };

            if (actionConfig.JumpCancellable)
            {
                fsm.Fsm.Configure(actionConfig.State)
                    .PermitIf(PlayerFSM.Trigger.Jump, actionConfig.Aerial? PlayerFSM.PlayerState.AirActionable : PlayerFSM.PlayerState.Jumpsquat, 
                        param => Util.CanCancelNow(param) && actionConfig.BonusClause(param));
            }

            if (actionConfig.DashCancellable)
            {
                fsm.Fsm.Configure(actionConfig.State)
                    .PermitIf(PlayerFSM.Trigger.Dash, PlayerFSM.PlayerState.Dash, 
                        param => Util.CanCancelNow(param) && actionConfig.BonusClause(param));
            }
            
            if (actionConfig.SpecialCancellable)
            {            
                fsm.Fsm.Configure(actionConfig.State)
                    .SubstateOf(actionConfig.Aerial ? PlayerFSM.PlayerState.AirSpecialCancellable : PlayerFSM.PlayerState.GroundSpecialCancellable);
            }
            
            // Startup reduction
            ActionStartupReduction[actionConfig.State] = ComputeStartupReduction(actionConfig, fsm);
            Debug.Log(Name + ": " + InheritableEnum.InheritableEnum.GetFieldNameByValue(actionConfig.State, fsm.StateType) + "(" + actionConfig.State + ") reduction: " + ActionStartupReduction[actionConfig.State]);
            

            
            // Movelist stuff
            if (actionConfig.IsSpecial)
            {
                MakeSpecialAction(fsm, actionConfig);
                SpecialMoveList.Add(actionConfig);
            }
            else
            {
                if (actionConfig.Aerial)
                    AirNormalMoveList.Add(actionConfig);
                else if (actionConfig.CommandDirection is 5 or 2)
                    NormalMoveList.Add(actionConfig);
                else
                    CommandNormalMoveList.Add(actionConfig);
            }
            
            
            // Compute frame advantage
            var sectionGroup = fsm.StateMapConfig.HitSectionGroup;
            if (sectionGroup is null) return;
            var hitSectionGroup = sectionGroup.Lookup(actionConfig.State, fsm);
            if (hitSectionGroup is null) return;
            Hit prevHit = null;
            int d = fsm.StateMapConfig.Duration.Lookup(actionConfig.State, fsm);

            for (int i = 0; i < hitSectionGroup.Duration(); i++)
            {
                Hit currentHit = hitSectionGroup.GetItemFromIndex(i); ;
                if (prevHit is null && currentHit is not null)
                {
                    actionConfig.MinStandHitFrameAdvantage =
                        (Hit.AttackLevelStandHitstun[currentHit.Level] + currentHit.BonusHitstun) - (d - i);
                    actionConfig.MinCrouchHitFrameAdvantage =
                        (Hit.AttackLevelCrouchHitstun[currentHit.Level] + currentHit.BonusHitstun) - (d - i);
                    actionConfig.MinGroundBlockFrameAdvantage =
                        (Hit.AttackLevelGroundBlockstun[currentHit.Level] + currentHit.BonusBlockstun) - (d - i);
                    actionConfig.MinAirBlockFrameAdvantage =
                        (Hit.AttackLevelAirBlockstun[currentHit.Level] + currentHit.BonusBlockstun) - (d - i);
                }
                if (currentHit is null && prevHit is not null)
                {
                    actionConfig.MaxStandHitFrameAdvantage =
                        (Hit.AttackLevelStandHitstun[prevHit.Level] + prevHit.BonusHitstun) - (d - i);
                    actionConfig.MaxCrouchHitFrameAdvantage =
                        (Hit.AttackLevelCrouchHitstun[prevHit.Level] + prevHit.BonusHitstun) - (d - i);
                    actionConfig.MaxGroundBlockFrameAdvantage =
                        (Hit.AttackLevelGroundBlockstun[prevHit.Level] + prevHit.BonusBlockstun) - (d - i);
                    actionConfig.MaxAirBlockFrameAdvantage =
                        (Hit.AttackLevelAirBlockstun[prevHit.Level] + prevHit.BonusBlockstun) - (d - i);
                }

                prevHit = currentHit;
            }

        }
        
        // private static int ComputeAdvantageFromFrameIndex()

        private int ComputeStartupReduction(ActionConfig actionConfig, PlayerFSM fsm)
        {
            var hitStateMap = fsm.StateMapConfig.HitSectionGroup;
            var hitSectionGroup = hitStateMap?.Lookup(actionConfig.State, fsm);
            
            var trajectoryStateMap = fsm.StateMapConfig.TrajectorySectionGroup;
            var trajectorySectionGroup = trajectoryStateMap?.Lookup(actionConfig.State, fsm);
            

            int actualStartup = -1;
            
            // Find the frame index of the first hitbox
            for (int i = 0; i < StateMapConfig.Duration.Lookup(actionConfig.State, fsm); i++)
            {
                if (hitSectionGroup?.GetItemFromIndex(i) != null)
                {
                    actualStartup = i;
                    break;
                }
                
                if (trajectorySectionGroup?.GetItemFromIndex(i) != null)
                {
                    actualStartup = i;
                    break;
                }

            }

            // If no hitbox found, no reduction is safe
            if (actualStartup == -1) return 0;

            // Calculate the maximum allowed reduction
            int maxAllowedReduction = actualStartup - SurgeMinimumStartup;

            if (maxAllowedReduction <= 0) return 0;

            return Math.Min(SurgeMaxStartupReduction, maxAllowedReduction);
        }

        private static void AllowRawFromState(PlayerFSM fsm, ActionConfig actionConfig, int state)
        {
            fsm.Fsm.Configure(state)
                .PermitIf(PlayerFSM.PlayerTrigger.ButtonAndDirection,
                    actionConfig.State,
                    param =>
                        Util.DoesInputMatch(actionConfig, param) && actionConfig.BonusClause(param),
                    actionConfig.InputWeight);
        }
        
        private static void AllowRawFromDash(PlayerFSM fsm, ActionConfig actionConfig)
        {
            fsm.Fsm.Configure(PlayerFSM.PlayerState.Dash)
                .PermitIf(PlayerFSM.PlayerTrigger.ButtonAndDirection,
                    actionConfig.State,
                    param =>
                    {
                        if (param is not FrameParam frameParam) return false;
                        return Util.DoesInputMatch(actionConfig, param) && 
                               fsm.FramesInCurrentState(frameParam.f) > fsm.MinimumDashDuration &&
                               actionConfig.BonusClause(param);
                    },
                    actionConfig.InputWeight);
        }

        protected void MakeActionCancellable(PlayerFSM fsm, ActionConfig source,
            ActionConfig destination)
        {
            if (source == destination)
            {
                fsm.Fsm.Configure(source.State)
                    .AllowReentry(PlayerFSM.Trigger.ButtonAndDirection);
            }
            
            fsm.Fsm.Configure(source.State)
                .PermitIf(PlayerFSM.Trigger.ButtonAndDirection,
                    destination.State,
                    param =>
                        (Util.CanCancelNow(param) && 
                         Util.DoesInputMatch(destination, param) &&
                         destination.BonusClause(param)),
                    destination.InputWeight);
        }

        protected void MakeSpecialAction(PlayerFSM fsm, ActionConfig actionConfig)
        {
            if (!actionConfig.RawOk) return;

            if (actionConfig.GroundOk)
            {
                fsm.Fsm.Configure(PlayerState.GroundSpecialCancellable)
                    .PermitIf(PlayerFSM.Trigger.ButtonAndDirection,
                        actionConfig.State,
                        param =>
                            (Util.CanCancelNow(param) && 
                             Util.DoesInputMatch(actionConfig, param) &&
                             actionConfig.BonusClause(param)),
                        actionConfig.InputWeight);
            }
            
            if (actionConfig.AirOk)
            {
                fsm.Fsm.Configure(PlayerState.AirSpecialCancellable)
                    .PermitIf(PlayerFSM.Trigger.ButtonAndDirection,
                        actionConfig.State,
                        param =>
                            (Util.CanCancelNow(param) && 
                             Util.DoesInputMatch(actionConfig, param) &&
                             actionConfig.BonusClause(param)),
                        actionConfig.InputWeight);
            }
        }

        public void AddMeter(Frame f, FP amount)
        {
            f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            healthData->meter += amount;
            // healthData->meter = Util.Clamp(healthData->meter, 0, 100);
            healthData->meter = Util.Clamp(healthData->meter, 0, 100);
        }

        public bool IsMeterInput(TriggerParams? triggerParams)
        {
            if (triggerParams is not ButtonAndDirectionParam param) return false;
            return param.Type == InputSystem.InputType.X;

            // param.f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            // return healthData->meter >= FP.FromString("33.33");
        }
        
        public bool IsBreakUnwhiffedInput(TriggerParams? triggerParams)
        {
            if (triggerParams is not ButtonAndDirectionParam param) return false;
            if (param.Type != InputSystem.InputType.X) return false;
            // if (!IsMeterInput(triggerParams)) return false;

            return !IsWhiffed(param.f);
        }
        
        public bool IsSurgeInput(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam param) return false;
            param.f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            return healthData->meter >= FP.FromString("11.11") && FramesInCurrentState(param.f) >= 5;
        }
         
        public void OnBreak(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam param) return;
            HitstopSystem.EnqueueHitstop(param.f, 8);

            var otherFsm = FsmLoader.FSMs[Util.GetOtherPlayer(param.f, EntityRef)];
            if (otherFsm is not PlayerFSM otherPlayerFsm) return;
            
            StartPushback(param.f, 0);
            var pushbackDistance = FP.FromString("3");
            if (IsFacingRight(param.f, EntityRef)) pushbackDistance *= FP.Minus_1;
            otherPlayerFsm.StartMomentum(param.f, pushbackDistance * FP.Minus_1);
            param.f.Events.EntityVibrate(EntityRef, FP.FromString("0.5"), FP.FromString("0.7"), 20);
            param.f.Events.EntityVibrate(Util.GetOtherPlayer(param.f, EntityRef), FP.FromString("0.5"), FP.FromString("0.7"), 20);
            
            Util.StartScreenDark(param.f, EntityRef, 8);
            Util.StartDramatic(param.f, EntityRef, 12);
            otherPlayerFsm.StartSlowdown(param.f, 25, FP.FromString("0.4"));

            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            FPVector2 pos = new FPVector2(transform3D->Position.X, 4);
            
            AnimationEntitySystem.Create(param.f, AnimationEntities.AnimationEntityEnum.Break, pos, 0, 
                IsFacingRight(param.f, EntityRef));
        }
        
        public void OnRedBreak(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam param) return;
            param.f.Unsafe.TryGetPointer<SlowdownData>(EntityRef, out var slowdownData);
            slowdownData->slowdownRemaining = 0;
            param.f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            healthData->virtualTimeSinceEmpowered = 0;
            
            AddMeter(param.f, FP.FromString("-11.11"));

            Util.StartScreenDark(param.f, EntityRef, 8);
            Util.StartDramatic(param.f, EntityRef, 2);
            // StartMomentum(param.f, 0);
            
            var otherFsm = FsmLoader.FSMs[Util.GetOtherPlayer(param.f, EntityRef)];
            if (otherFsm is not PlayerFSM otherPlayerFsm) return;
            otherPlayerFsm.StartSlowdown(param.f, 25, FP.FromString("0.25"));

            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            FPVector2 pos = new FPVector2(transform3D->Position.X, 3);
            
            param.f.Events.EntityVibrate(EntityRef, FP.FromString("0.5"), FP.FromString("0.7"), 20);
            
            AnimationEntitySystem.Create(param.f, AnimationEntities.AnimationEntityEnum.BreakRed, pos, 0, 
                IsFacingRight(param.f, EntityRef));
        }

        public override bool IsTimeStopped(Frame f)
        {
            if (FsmLoader.FSMs[Util.GetOtherPlayer(f, EntityRef)] is PlayerFSM opponentPlayerFsm)
            {
                if (opponentPlayerFsm.Fsm.IsInState(PlayerState.RedBreak) && Fsm.IsInState(PlayerState.Break)) return true;
                // if (opponentPlayerFsm.Fsm.IsInState(PlayerState.Break) && !Fsm.IsInState(PlayerState.RedBreak)) return true;
            }

            return base.IsTimeStopped(f);
        }

        protected bool MinimumDashDurationElapsed(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam frameParam) return false;
            return FramesInCurrentState(frameParam.f) > MinimumDashDuration;
        }
        
    }
}