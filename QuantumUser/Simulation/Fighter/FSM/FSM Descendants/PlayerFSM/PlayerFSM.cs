using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public unsafe partial class PlayerFSM : FSM
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

        public class PlayerTrigger : Trigger
        {
            public static int Finish;
            public static int NeutralInput;
            public static int Down;
            public static int Forward;
            public static int Backward;
            public static int Dash;
            public static int Backdash;
            public static int ForwardThrow;
            public static int BackThrow;
            public static int ButtonAndDirection;
            public static int Jump;
            public static int Land;
            public static int HitWall;
            public static int JumpCancel;
            public static int HitHigh;
            public static int HitLow;
            public static int BlockHigh;
            public static int BlockLow;
            public static int Die;
        }


        public PlayerFSM()
        {
            int currentState = PlayerState.StandActionable;
            Fsm = new Machine<int, int>(currentState);
            ConfigureBaseFsm(Fsm);
        }


        public void ConfigureBaseFsm(Machine<int, int> machine)
        {
            machine.OnTransitionCompleted(OnStateChanged);

            // Ground
            machine.Configure(PlayerState.Ground)
                .OnEntryFrom(PlayerTrigger.Land, OnLand)
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.AirHit, IsCollisionHitParamLauncher, 1)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.AirHit, IsCollisionHitParamLauncher, 1)
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
                .Permit(PlayerTrigger.NeutralInput, PlayerState.StandActionable)
                .Permit(PlayerTrigger.Down, PlayerState.CrouchActionable)
                .Permit(PlayerTrigger.Forward, PlayerState.WalkForward)
                .Permit(PlayerTrigger.Backward, PlayerState.WalkBackward)
                .Permit(PlayerTrigger.Dash, PlayerState.Dash)
                .Permit(PlayerTrigger.Backdash, PlayerState.Backdash)
                .Permit(PlayerTrigger.Jump, PlayerState.AirActionable)
                .Permit(PlayerTrigger.ForwardThrow, PlayerState.ForwardThrow)
                .Permit(PlayerTrigger.BackThrow, PlayerState.Backthrow)
                // .Permit(Trigger.FrontThrow, State.ThrowFrontStartup)
                // .Permit(Trigger.BackThrow, State.ThrowBackStartup)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -2)
                // .PermitIf(Trigger.ReceiveKinematics, State.TechableKinematicReceiver, _ => true, 0)
                .OnEntry(EndSlowdown)
                .OnEntry(ResetCombo)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.StandActionable)
                .SubstateOf(PlayerState.GroundActionable)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.CrouchActionable)
                .SubstateOf(PlayerState.GroundActionable)
                .SubstateOf(PlayerState.Crouch);

            machine.Configure(PlayerState.WalkForward)
                .SubstateOf(PlayerState.StandActionable);

            machine.Configure(PlayerState.WalkBackward)
                .SubstateOf(PlayerState.StandActionable);

            machine.Configure(PlayerState.Dash)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .Permit(PlayerTrigger.Jump, PlayerState.AirActionable)
                // .Permit(Trigger.Backward, State.WalkBackward)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -2)
                .Permit(PlayerTrigger.ForwardThrow, PlayerState.ForwardThrow)
                .Permit(PlayerTrigger.BackThrow, PlayerState.Backthrow)
                .OnExitFrom(PlayerTrigger.ForwardThrow, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.BackThrow, StartMomentumCallback)
                // .OnExitFrom(Trigger.ThrowTech, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.ButtonAndDirection, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.Jump, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.JumpCancel, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.HitHigh, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.HitLow, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.BlockHigh, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.BlockLow, StartMomentumCallback)
                // .OnExitFrom(Trigger.ThrowConnect, StartMomentumCallback)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(PlayerState.Stand)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.Backdash)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .OnEntry(OnBackdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .SubstateOf(PlayerState.Stand)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.GroundAction)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .Permit(PlayerTrigger.JumpCancel, PlayerState.AirActionable)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(ResetWhiff)
                .SubstateOf(PlayerState.Action)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.StandHitHigh)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .PermitReentry(PlayerTrigger.HitHigh)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.StandHitLow)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .PermitReentry(PlayerTrigger.HitLow)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.CrouchHit)
                .Permit(PlayerTrigger.Finish, PlayerState.CrouchActionable)
                .PermitReentry(PlayerTrigger.HitHigh)
                .PermitReentry(PlayerTrigger.HitLow)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Crouch);

            machine.Configure(PlayerState.StandBlock)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .PermitReentry(PlayerTrigger.BlockHigh)
                .SubstateOf(PlayerState.GroundBlock)
                .SubstateOf(PlayerState.Stand);

            machine.Configure(PlayerState.CrouchBlock)
                .Permit(PlayerTrigger.Finish, PlayerState.CrouchActionable)
                .PermitReentry(PlayerTrigger.BlockLow)
                .SubstateOf(PlayerState.GroundBlock)
                .SubstateOf(PlayerState.Crouch);

            machine.Configure(PlayerState.GroundBlock)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -3)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -3)
                .SubstateOf(PlayerState.Ground)
                .SubstateOf(PlayerState.Block);

            machine.Configure(PlayerState.HardKnockdown)
                .OnEntry(DoImpactVibrate)
                .OnEntry(OnHKD)
                .OnEntry(EndSlowdown)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.SoftKnockdown)
                .OnEntry(EndSlowdown)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.DeadFromAir)
                .OnEntry(DoImpactVibrate)
                .OnEntry(EndSlowdown)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.DeadFromGround)
                .OnEntry(EndSlowdown)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.Landsquat)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.StandBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.CrouchBlock, _ => true, -2)
                .SubstateOf(PlayerState.Crouch)
                .SubstateOf(PlayerState.Ground);

            machine.Configure(PlayerState.Throw)
                .SubstateOf(PlayerState.Ground)
                .SubstateOf(PlayerState.Stand)
                .Permit(PlayerTrigger.Finish, PlayerState.StandActionable);

            machine.Configure(PlayerState.ForwardThrow)
                .SubstateOf(PlayerState.Throw);

            machine.Configure(PlayerState.Backthrow)
                .SubstateOf(PlayerState.Throw);

            // Air
            machine.Configure(PlayerState.Air)
                .Permit(PlayerTrigger.Land, PlayerState.Landsquat)
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.AirHit, _ => true, -1)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.AirHit, _ => true, -1)
                .OnEntryFrom(PlayerTrigger.Jump, InputSystem.ClearBufferParams)
                .SubstateOf(PlayerState.Any);

            machine.Configure(PlayerState.AirActionable)
                .Permit(PlayerTrigger.Dash, PlayerState.AirDash)
                .Permit(PlayerTrigger.Backdash, PlayerState.AirBackdash)
                .OnEntryFrom(PlayerTrigger.Jump, StartNewJump)
                .PermitReentry(PlayerTrigger.Jump)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.AirBlock, _ => true, -1)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.AirBlock, _ => true, -1)
                .OnEntry(ResetCombo)
                .OnEntry(EndSlowdown)
                .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirDash)
                .Permit(PlayerTrigger.Finish, PlayerState.AirActionable)
                .OnExitFrom(PlayerTrigger.ButtonAndDirection, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.Jump, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.JumpCancel, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.HitHigh, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.HitLow, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.BlockHigh, StartMomentumCallback)
                .OnExitFrom(PlayerTrigger.BlockLow, StartMomentumCallback)
                .OnEntry(OnAirdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.AirBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.AirBlock, _ => true, -2)
                .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirBackdash)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(OnBackdash)
                .OnEntry(OnAirBackdash)
                .Permit(PlayerTrigger.Finish, PlayerState.AirActionable)
                .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirAction)
                .Permit(PlayerTrigger.Finish, PlayerState.AirActionable)
                .Permit(PlayerTrigger.JumpCancel, PlayerState.AirActionable)
                .OnEntry(InputSystem.ClearBufferParams)
                .OnEntry(ResetWhiff)
                .SubstateOf(PlayerState.Action)
                .SubstateOf(PlayerState.Air);

            machine.Configure(PlayerState.AirBlock)
                .PermitIf(PlayerTrigger.BlockHigh, PlayerState.AirBlock, _ => true, -2)
                .PermitIf(PlayerTrigger.BlockLow, PlayerState.AirBlock, _ => true, -2)
                .PermitReentry(PlayerTrigger.BlockHigh)
                .PermitReentry(PlayerTrigger.BlockLow)
                .Permit(PlayerTrigger.Finish, PlayerState.AirActionable)
                .SubstateOf(PlayerState.Air)
                .SubstateOf(PlayerState.Block);

            machine.Configure(PlayerState.AirHit)
                .SubstateOf(PlayerState.Hit)
                .SubstateOf(PlayerState.Air)
                .OnEntryFrom(PlayerTrigger.HitHigh, StartNewJuggle)
                .OnEntryFrom(PlayerTrigger.HitLow, StartNewJuggle)
                .OnEntryFrom(PlayerTrigger.Finish, StartNewJuggle)
                .PermitIf(PlayerTrigger.HitHigh, PlayerState.AirHit, _ => true, -2)
                .PermitIf(PlayerTrigger.HitLow, PlayerState.AirHit, _ => true, -2)
                .PermitIf(PlayerTrigger.Land, PlayerState.AirHitPostGroundBounce, IsInGroundBounce, 4)
                .Permit(PlayerTrigger.HitWall, PlayerState.AirHitPostWallBounce)
                .PermitIf(PlayerTrigger.Land, PlayerState.DeadFromAir, PlayerIsDead, 3)
                .PermitIf(PlayerTrigger.Land, PlayerState.HardKnockdown, InHardKnockdown, 2)
                .PermitIf(PlayerTrigger.Land, PlayerState.SoftKnockdown, _ => true, 1)
                .OnExitFrom(PlayerTrigger.Land, OnExitAirHitFromLand)
                .PermitReentry(PlayerTrigger.HitHigh)
                .PermitReentry(PlayerTrigger.HitLow)
                .PermitReentry(PlayerTrigger.Land);

            machine.Configure(PlayerState.AirHitPostGroundBounce)
                .OnEntry(OnGroundBounce)
                .SubstateOf(PlayerState.AirHit);

            machine.Configure(PlayerState.AirHitPostWallBounce)
                .OnEntry(OnWallBounce)
                .SubstateOf(PlayerState.AirHit);


            // General
            machine.Configure(PlayerState.Hit);
            machine.Configure(PlayerState.Any);

            machine.Configure(PlayerState.CutsceneReactor)
                .Permit(PlayerTrigger.Finish, PlayerState.AirHit);
        }


        private void IncrementCombo(Frame f, FP gravityScaling, FP damageScaling)
        {
            f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);
            comboData->length++;
            comboData->gravityScaling *= gravityScaling;
            comboData->damageScaling *= damageScaling;
        }

        private bool IsCollisionHitParamLauncher(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            var param = (CollisionHitParams)triggerParams;
            return param.Launches || Fsm.IsInState(PlayerState.Backdash);
        }

        private void StartMomentumCallback(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;

            FP amount = PlayerDirectionSystem.IsFacingRight(frameParam.f, EntityRef) ? 4 : -4;
            StartMomentum(frameParam.f, amount);

            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.Dash,
                transform3D->Position.XY, 0, !PlayerDirectionSystem.IsFacingRight(frameParam.f, EntityRef));
        }

        private void StartMomentum(Frame f, FP totalDistance)
        {
            f.Unsafe.TryGetPointer<MomentumData>(EntityRef, out var pushbackData);
            pushbackData->framesInMomentum = 0;
            pushbackData->momentumAmount = totalDistance;
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