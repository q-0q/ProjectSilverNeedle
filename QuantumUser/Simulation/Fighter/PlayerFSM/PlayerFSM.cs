using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        
        public enum State
        {
            
            // Ground
            StandActionable,
            CrouchActionable,
            WalkForward,
            WalkBackward,
            Dash,
            Backdash,
            GroundAction,
            GroundActionable,
            StandHitHigh,
            StandHitLow,
            CrouchHit,
            StandBlock,
            CrouchBlock,
            GroundBlock,
            HardKnockdown,
            SoftKnockdown,
            Landsquat,
            ThrowStartup,
            ThrowFrontStartup,
            ThrowBackStartup,
            ThrowConnect,
            FrontThrowConnect,
            BackThrowConnect,
            ThrowWhiff,
            ThrowTech,
            DeadFromAir,
            DeadFromGround,
            
            
            // Air
            AirDash,
            AirBackdash,
            AirAction,
            AirActionable,
            AirHit,
            AirBlock,
            AirHitPostGroundBounce,
            AirHitPostWallBounce,
            
            
            // General
            Hit,
            Ground,
            Air,
            Any,
            Action,
            Stand,
            Crouch,
            Block,
            KinematicSource,
            KinematicReceiver,
            TechableKinematicReceiver,
            
            // Char-specific
            Action1,
            Action2,
            Action3,
            Action4,
            Action5,
            Action6,
            Action7,
            Action8,
            Action9,
            Action10,
            Action11,
            Action12,
            Action13,
            Action14,
            Action15,
            Action16,
            Action17,
            Action18,
            Action19,
            Action20,
        }
        
        // When adding a new trigger
        // 1. Add as an enum here
        // 2. Create a TriggerWithParameter<Frame> field and initialize in ctor
        public enum Trigger
        {
            Finish,
            NeutralInput,
            Down,
            Forward,
            Backward,
            Dash,
            Backdash,
            FrontThrow,
            BackThrow,
            ThrowTech,
            Action,
            Jump,
            Land,
            HitWall,
            JumpCancel,
            HitHigh,
            HitLow,
            BlockHigh,
            BlockLow,
            ThrowConnect,
            ReceiveKinematics,
            Die
        }

        public EntityRef EntityRef;
        public Machine<State, Trigger> Fsm;
        
        
        public PlayerFSM()
        {
            State currentState = State.StandActionable;
            Fsm = new Machine<State, Trigger>(currentState);
            ConfigureBaseFsm(Fsm);
        }

        
        public void ConfigureBaseFsm(Machine<State, Trigger> machine)
        {
            machine.OnTransitionCompleted(OnStateChanged);
            
            // Ground
            machine.Configure(State.Ground)
                .OnEntryFrom(Trigger.Land, OnLand)
                .PermitIf(Trigger.HitHigh, State.AirHit, IsCollisionHitParamLauncher, 1)
                .PermitIf(Trigger.HitLow, State.AirHit, IsCollisionHitParamLauncher, 1)
                .PermitIf(Trigger.Die, State.DeadFromGround, PlayerIsDead, 3)
                .SubstateOf(State.Any);

            machine.Configure(State.Stand)
                .PermitIf(Trigger.HitHigh, State.StandHitHigh, _ => true, -2)
                .PermitIf(Trigger.HitLow, State.StandHitLow, _ => true, -2)
                .SubstateOf(State.Ground);
            
            machine.Configure(State.Crouch)
                .PermitIf(Trigger.HitHigh, State.CrouchHit, _ => true, -2)
                .PermitIf(Trigger.HitLow, State.CrouchHit, _ => true, -2)
                .SubstateOf(State.Ground);
            
            machine.Configure(State.GroundActionable)
                .Permit(Trigger.NeutralInput, State.StandActionable)
                .Permit(Trigger.Down, State.CrouchActionable)
                .Permit(Trigger.Forward, State.WalkForward)
                .Permit(Trigger.Backward, State.WalkBackward)
                .Permit(Trigger.Dash, State.Dash)
                .Permit(Trigger.Backdash, State.Backdash)
                .Permit(Trigger.Jump, State.AirActionable)
                .Permit(Trigger.FrontThrow, State.ThrowFrontStartup)
                .Permit(Trigger.BackThrow, State.ThrowBackStartup)
                .PermitIf(Trigger.BlockHigh, State.StandBlock, _ => true, -2)
                .PermitIf(Trigger.BlockLow, State.CrouchBlock, _ => true, -2)
                .PermitIf(Trigger.ReceiveKinematics, State.TechableKinematicReceiver, _ => true, 0)
                .OnEntry(EndSlowdown)
                .OnEntry(ResetCombo)
                .SubstateOf(State.Ground);

            machine.Configure(State.StandActionable) 
                .SubstateOf(State.GroundActionable)
                .SubstateOf(State.Stand);

            machine.Configure(State.CrouchActionable)
                .SubstateOf(State.GroundActionable)
                .SubstateOf(State.Crouch);

            machine.Configure(State.WalkForward)
                .SubstateOf(State.StandActionable);
            
            machine.Configure(State.WalkBackward)
                .SubstateOf(State.StandActionable);
            
            machine.Configure(State.Dash)
                .Permit(Trigger.Finish, State.StandActionable)
                .Permit(Trigger.Jump, State.AirActionable)
                .Permit(Trigger.Backward, State.WalkBackward)
                .PermitIf(Trigger.BlockHigh, State.StandBlock, _ => true, -2)
                .PermitIf(Trigger.BlockLow, State.CrouchBlock, _ => true, -2)
                .Permit(Trigger.FrontThrow, State.ThrowFrontStartup)
                .Permit(Trigger.BackThrow, State.ThrowBackStartup)
                .OnEntry(StartMomentumCallback)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(State.Stand)
                .SubstateOf(State.Ground);

            machine.Configure(State.Backdash)
                .Permit(Trigger.Finish, State.StandActionable)
                .OnEntry(OnBackdash)
                .SubstateOf(State.Ground);
            
            machine.Configure(State.GroundAction)
                .Permit(Trigger.Finish, State.StandActionable)
                .Permit(Trigger.JumpCancel, State.AirActionable)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(ResetWhiff)
                .SubstateOf(State.Ground);

            machine.Configure(State.ThrowStartup)
                .Permit(Trigger.Finish, State.ThrowWhiff)
                .Permit(Trigger.ReceiveKinematics, State.ThrowTech)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(State.Stand)
                .SubstateOf(State.Ground);

            machine.Configure(State.ThrowFrontStartup)
                .SubstateOf(State.ThrowStartup)
                .Permit(Trigger.ThrowConnect, State.FrontThrowConnect);
            
            machine.Configure(State.ThrowBackStartup)
                .SubstateOf(State.ThrowStartup)
                .Permit(Trigger.ThrowConnect, State.BackThrowConnect);
            
            machine.Configure(State.ThrowWhiff)
                .Permit(Trigger.Finish, State.StandActionable)
                .SubstateOf(State.Stand)
                .SubstateOf(State.Ground);
            
            machine.Configure(State.ThrowConnect)
                .Permit(Trigger.Finish, State.StandActionable)
                .Permit(Trigger.ThrowTech, State.ThrowTech)
                .SubstateOf(State.KinematicSource)
                .SubstateOf(State.Stand)
                .SubstateOf(State.Ground);
            
            machine.Configure(State.FrontThrowConnect)
                .SubstateOf(State.ThrowConnect);
            
            machine.Configure(State.BackThrowConnect)
                .SubstateOf(State.ThrowConnect);
            
            machine.Configure(State.ThrowTech)
                .OnEntry(ResetYPos)
                .OnEntry(OnEnterThrowTech)
                .OnEntry(InputSystem.ClearBufferParams)
                .Permit(Trigger.Finish, State.StandActionable)
                .SubstateOf(State.Stand)
                .SubstateOf(State.Ground);

            machine.Configure(State.StandHitHigh)
                .Permit(Trigger.Finish, State.StandActionable)
                .PermitReentry(Trigger.HitHigh)
                .SubstateOf(State.Hit)
                .SubstateOf(State.Stand);
            
            machine.Configure(State.StandHitLow)
                .Permit(Trigger.Finish, State.StandActionable)
                .PermitReentry(Trigger.HitLow)
                .SubstateOf(State.Hit)
                .SubstateOf(State.Stand);

            machine.Configure(State.CrouchHit)
                .Permit(Trigger.Finish, State.CrouchActionable)
                .PermitReentry(Trigger.HitHigh)
                .PermitReentry(Trigger.HitLow)
                .SubstateOf(State.Hit)
                .SubstateOf(State.Crouch);

            machine.Configure(State.StandBlock)
                .Permit(Trigger.Finish, State.StandActionable)
                .PermitReentry(Trigger.BlockHigh)
                .SubstateOf(State.GroundBlock)
                .SubstateOf(State.Stand);
            
            machine.Configure(State.CrouchBlock)
                .Permit(Trigger.Finish, State.CrouchActionable)
                .PermitReentry(Trigger.BlockLow)
                .SubstateOf(State.GroundBlock)
                .SubstateOf(State.Crouch);

            machine.Configure(State.GroundBlock)
                .PermitIf(Trigger.BlockHigh, State.StandBlock, _ => true, -3)
                .PermitIf(Trigger.BlockLow, State.CrouchBlock, _ => true, -3)
                .SubstateOf(State.Ground)
                .SubstateOf(State.Block);

            machine.Configure(State.HardKnockdown)
                .OnEntry(DoImpactVibrate)
                .OnEntry(OnHKD)
                .OnEntry(EndSlowdown)
                .Permit(Trigger.Finish, State.StandActionable)
                .SubstateOf(State.Ground)
                .SubstateOf(State.Stand);
            
            machine.Configure(State.SoftKnockdown)
                .OnEntry(EndSlowdown)
                .Permit(Trigger.Finish, State.StandActionable)
                .SubstateOf(State.Ground)
                .SubstateOf(State.Stand);

            machine.Configure(State.DeadFromAir)
                .OnEntry(DoImpactVibrate)
                .OnEntry(EndSlowdown)
                .SubstateOf(State.Ground);
            
            machine.Configure(State.DeadFromGround)
                .OnEntry(EndSlowdown)
                .SubstateOf(State.Ground);

            machine.Configure(State.Landsquat)
                .Permit(Trigger.Finish, State.StandActionable)
                .PermitIf(Trigger.BlockHigh, State.StandBlock, _ => true, -2)
                .PermitIf(Trigger.BlockLow, State.CrouchBlock, _ => true, -2)
                .SubstateOf(State.Crouch)
                .SubstateOf(State.Ground);
            
            
            
            // Air
            machine.Configure(State.Air)
                .Permit(Trigger.Land, State.Landsquat)
                .PermitIf(Trigger.HitHigh, State.AirHit, _ => true, -1)
                .PermitIf(Trigger.HitLow, State.AirHit, _ => true, -1)
                .OnEntryFrom(Trigger.Jump, InputSystem.ClearBufferParams)
                .SubstateOf(State.Any);
            
            machine.Configure(State.AirActionable)
                .Permit(Trigger.Dash, State.AirDash)
                .Permit(Trigger.Backdash, State.AirBackdash)
                .OnEntryFrom(Trigger.Jump, StartNewJump)
                .PermitReentry(Trigger.Jump)
                .PermitIf(Trigger.BlockHigh, State.AirBlock, _ => true, -1)
                .PermitIf(Trigger.BlockLow, State.AirBlock, _ => true, -1)
                .OnEntry(ResetCombo)
                .OnEntry(EndSlowdown)
                .SubstateOf(State.Air);
            
            machine.Configure(State.AirDash)
                .Permit(Trigger.Finish, State.AirActionable)
                .OnEntry(StartMomentumCallback)
                .OnEntry(OnAirdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .PermitIf(Trigger.BlockHigh, State.AirBlock, _ => true, -2)
                .PermitIf(Trigger.BlockLow, State.AirBlock, _ => true, -2)
                .SubstateOf(State.Air);
            
            machine.Configure(State.AirBackdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(OnBackdash)
                .OnEntry(OnAirBackdash)
                .Permit(Trigger.Finish, State.AirActionable)
                .SubstateOf(State.Air);

            machine.Configure(State.AirAction)
                .Permit(Trigger.Finish, State.AirActionable)
                .Permit(Trigger.JumpCancel, State.AirActionable)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(ResetWhiff)
                .SubstateOf(State.Air);
            
            machine.Configure(State.AirBlock)
                .PermitIf(Trigger.BlockHigh, State.AirBlock, _ => true, -2)
                .PermitIf(Trigger.BlockLow, State.AirBlock, _ => true, -2)
                .PermitReentry(Trigger.BlockHigh)
                .PermitReentry(Trigger.BlockLow)
                .Permit(Trigger.Finish, State.AirActionable)
                .SubstateOf(State.Air)
                .SubstateOf(State.Block);

            machine.Configure(State.AirHit)
                .SubstateOf(State.Hit)
                .SubstateOf(State.Air)
                .OnEntryFrom(Trigger.HitHigh, StartNewJuggle)
                .OnEntryFrom(Trigger.HitLow, StartNewJuggle)
                .OnEntryFrom(Trigger.Finish, StartNewJuggle)
                .PermitIf(Trigger.HitHigh, State.AirHit, _ => true, -2)
                .PermitIf(Trigger.HitLow, State.AirHit, _ => true, -2)
                .PermitIf(Trigger.Land, State.AirHitPostGroundBounce, IsInGroundBounce, 4)
                .Permit(Trigger.HitWall, State.AirHitPostWallBounce)
                .PermitIf(Trigger.Land, State.DeadFromAir, PlayerIsDead, 3)
                .PermitIf(Trigger.Land, State.HardKnockdown, InHardKnockdown, 2)
                .PermitIf(Trigger.Land, State.SoftKnockdown, _ => true, 1)
                .OnExitFrom(Trigger.Land, OnExitAirHitFromLand)
                .PermitReentry(Trigger.HitHigh)
                .PermitReentry(Trigger.HitLow)
                .PermitReentry(Trigger.Land);

            machine.Configure(State.AirHitPostGroundBounce)
                .OnEntry(OnGroundBounce)
                .SubstateOf(State.AirHit);
            
            machine.Configure(State.AirHitPostWallBounce)
                .OnEntry(OnWallBounce)
                .SubstateOf(State.AirHit);
            
            // Action
            machine.Configure(State.Action1)
                .SubstateOf(State.Action);
            machine.Configure(State.Action2)
                .SubstateOf(State.Action);
            machine.Configure(State.Action3)
                .SubstateOf(State.Action);
            machine.Configure(State.Action4)
                .SubstateOf(State.Action);
            machine.Configure(State.Action5)
                .SubstateOf(State.Action);
            machine.Configure(State.Action6)
                .SubstateOf(State.Action);
            machine.Configure(State.Action7)
                .SubstateOf(State.Action);
            machine.Configure(State.Action8)
                .SubstateOf(State.Action);
            machine.Configure(State.Action9)
                .SubstateOf(State.Action);
            machine.Configure(State.Action10)
                .SubstateOf(State.Action);
            machine.Configure(State.Action11)
                .SubstateOf(State.Action);
            machine.Configure(State.Action12)
                .SubstateOf(State.Action);
            machine.Configure(State.Action13)
                .SubstateOf(State.Action);
            machine.Configure(State.Action14)
                .SubstateOf(State.Action);
            machine.Configure(State.Action15)
                .SubstateOf(State.Action);
            machine.Configure(State.Action16)
                .SubstateOf(State.Action);
            machine.Configure(State.Action17)
                .SubstateOf(State.Action);
            machine.Configure(State.Action18)
                .SubstateOf(State.Action);
            machine.Configure(State.Action19)
                .SubstateOf(State.Action);
            machine.Configure(State.Action20)
                .SubstateOf(State.Action);
            
            // General
            machine.Configure(State.Hit);

            machine.Configure(State.Any)
                .PermitIf(Trigger.ReceiveKinematics, State.KinematicReceiver, _ => true, -1);

            machine.Configure(State.KinematicReceiver)
                .OnEntry(StartDramatics)
                .Permit(Trigger.Finish, State.AirHit);

            machine.Configure(State.TechableKinematicReceiver)
                .PermitIf(Trigger.FrontThrow, State.ThrowTech, CanTechThrow)
                .PermitIf(Trigger.BackThrow, State.ThrowTech, CanTechThrow)
                .SubstateOf(State.KinematicReceiver);


            // return;
            
            // Character-specific

            
        }
        
        
        private void IncrementCombo(Frame f, FP gravityScaling, FP damageScaling)
        {
            f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);
            comboData->length++;
            comboData->gravityScaling *= gravityScaling;
            comboData->damageScaling *= damageScaling;
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
        }

        private void OnStateChanged(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            ResetStateEnteredFrame(param.f);
            PlayerDirectionSystem.ForceUpdatePlayerDirection(param.f, EntityRef);
            
            Util.WritebackFsm(param.f, EntityRef);
        }

        private void DoImpactVibrate(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            HitstopSystem.EnqueueHitstop(param.f, 9);
            param.f.Events.EntityVibrate(EntityRef, FP.FromString("1.25"), FP.FromString("0.4"), 40);
        }
        
        private void OnHKD(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            param.f.Events.GameEvent(EntityRef, GameEventType.Knockdown);
        }

        private void ResetStateEnteredFrame(Frame f)
        {
            f.Unsafe.TryGetPointer<PlayerFSMData>(EntityRef, out var playerFsmData);
            playerFsmData->framesInState = 0;
            playerFsmData->virtualTimeInState = 0;
        }

        public int FramesInCurrentState(Frame f)
        {
            f.Unsafe.TryGetPointer<PlayerFSMData>(EntityRef, out var playerFsmData);
            return Util.FramesFromVirtualTime(playerFsmData->virtualTimeInState);
        }
        
        public static int FramesInCurrentState(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerFSMData>(entityRef, out var playerFsmData);
            return Util.FramesFromVirtualTime(playerFsmData->virtualTimeInState);
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
                transform3D->Position.XY, 0, !PlayerDirectionSystem.IsFacingRight(frameParam.f, EntityRef));
        }
        
        

        
        
        
        
    }
}