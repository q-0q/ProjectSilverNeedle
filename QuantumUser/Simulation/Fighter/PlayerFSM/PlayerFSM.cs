using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        
        public class State : InheritableEnum.InheritableEnum
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
            public static int StandHitHigh;
            public static int StandHitLow;
            public static int CrouchHit;
            public static int StandBlock;
            public static int CrouchBlock;
            public static int GroundBlock;
            public static int HardKnockdown;
            public static int SoftKnockdown;
            public static int Landsquat;
            public static int DeadFromAir;
            public static int DeadFromGround;
            public static int ForwardThrow;
            public static int Backthrow;
            
            // Air
            public static int AirDash;
            public static int AirBackdash;
            public static int AirAction;
            public static int AirActionable;
            public static int AirHit;
            public static int AirBlock;
            public static int AirHitPostGroundBounce;
            public static int AirHitPostWallBounce;
            
            
            // General
            public static int Hit;
            public static int Ground;
            public static int Air;
            public static int Any;
            public static int Action;
            public static int Stand;
            public static int Crouch;
            public static int Block;
            public static int Throw;
            public static int CutsceneReactor;
        }
        
        public class CutsceneIndexes : InheritableEnum.InheritableEnum
        {
            public static int ForwardThrow;
            public static int BackwardThrow;
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
            ForwardThrow,
            BackThrow,

            ButtonAndDirection,
            Jump,
            Land,
            HitWall,
            JumpCancel,
            HitHigh,
            HitLow,
            BlockHigh,
            BlockLow,
            Die,
            
        }

        public EntityRef EntityRef;
        public Machine<int, Trigger> Fsm;
        
        
        public PlayerFSM()
        {
            int currentState = State.StandActionable;
            Fsm = new Machine<int, Trigger>(currentState);
            ConfigureBaseFsm(Fsm);
        }

        
        public void ConfigureBaseFsm(Machine<int, Trigger> machine)
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
                .Permit(Trigger.ForwardThrow, State.ForwardThrow)
                .Permit(Trigger.BackThrow, State.Backthrow)
                // .Permit(Trigger.FrontThrow, State.ThrowFrontStartup)
                // .Permit(Trigger.BackThrow, State.ThrowBackStartup)
                .PermitIf(Trigger.BlockHigh, State.StandBlock, _ => true, -2)
                .PermitIf(Trigger.BlockLow, State.CrouchBlock, _ => true, -2)
                // .PermitIf(Trigger.ReceiveKinematics, State.TechableKinematicReceiver, _ => true, 0)
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
                // .Permit(Trigger.Backward, State.WalkBackward)
                .PermitIf(Trigger.BlockHigh, State.StandBlock, _ => true, -2)
                .PermitIf(Trigger.BlockLow, State.CrouchBlock, _ => true, -2)
                .Permit(Trigger.ForwardThrow, State.ForwardThrow)
                .Permit(Trigger.BackThrow, State.Backthrow)
                .OnExitFrom(Trigger.ForwardThrow, StartMomentumCallback)
                .OnExitFrom(Trigger.BackThrow, StartMomentumCallback)
                // .OnExitFrom(Trigger.ThrowTech, StartMomentumCallback)
                .OnExitFrom(Trigger.ButtonAndDirection, StartMomentumCallback)
                .OnExitFrom(Trigger.Jump, StartMomentumCallback)
                .OnExitFrom(Trigger.JumpCancel, StartMomentumCallback)
                .OnExitFrom(Trigger.HitHigh, StartMomentumCallback)
                .OnExitFrom(Trigger.HitLow, StartMomentumCallback)
                .OnExitFrom(Trigger.BlockHigh, StartMomentumCallback)
                .OnExitFrom(Trigger.BlockLow, StartMomentumCallback)
                // .OnExitFrom(Trigger.ThrowConnect, StartMomentumCallback)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(State.Stand)
                .SubstateOf(State.Ground);

            machine.Configure(State.Backdash)
                .Permit(Trigger.Finish, State.StandActionable)
                .OnEntry(OnBackdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(State.Stand)
                .SubstateOf(State.Ground);
            
            machine.Configure(State.GroundAction)
                .Permit(Trigger.Finish, State.StandActionable)
                .Permit(Trigger.JumpCancel, State.AirActionable)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(ResetWhiff)
                .SubstateOf(State.Action)
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
                .SubstateOf(State.Ground);

            machine.Configure(State.SoftKnockdown)
                .OnEntry(EndSlowdown)
                .Permit(Trigger.Finish, State.StandActionable)
                .SubstateOf(State.Ground);

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
            
            machine.Configure(State.Throw)
                .SubstateOf(State.Ground)
                .SubstateOf(State.Stand)
                .Permit(Trigger.Finish, State.StandActionable);

            machine.Configure(State.ForwardThrow)
                .SubstateOf(State.Throw);
            
            machine.Configure(State.Backthrow)
                .SubstateOf(State.Throw);        
            
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
                .OnExitFrom(Trigger.ButtonAndDirection, StartMomentumCallback)
                .OnExitFrom(Trigger.Jump, StartMomentumCallback)
                .OnExitFrom(Trigger.JumpCancel, StartMomentumCallback)
                .OnExitFrom(Trigger.HitHigh, StartMomentumCallback)
                .OnExitFrom(Trigger.HitLow, StartMomentumCallback)
                .OnExitFrom(Trigger.BlockHigh, StartMomentumCallback)
                .OnExitFrom(Trigger.BlockLow, StartMomentumCallback)
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
                .SubstateOf(State.Action)
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
            
            
            
            // General
            machine.Configure(State.Hit);
            machine.Configure(State.Any);

            machine.Configure(State.CutsceneReactor)
                .Permit(Trigger.Finish, State.AirHit);
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
            param.f.Events.EntityVibrate(EntityRef, FP.FromString("1.25"), FP.FromString("0.6"), 40);
        }
        
        private void OnHKD(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            param.f.Events.GameEvent(EntityRef, GameEventType.Knockdown);
        }

        private void ResetStateEnteredFrame(Frame f)
        {
            f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var playerFsmData);
            playerFsmData->framesInState = 0;
            playerFsmData->virtualTimeInState = 0;
        }

        public int FramesInCurrentState(Frame f)
        {
            f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var playerFsmData);
            return Util.FramesFromVirtualTime(playerFsmData->virtualTimeInState);
        }
        
        public static int FramesInCurrentState(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<FSMData>(entityRef, out var playerFsmData);
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