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
        private static readonly FP UniversalJuggleFallSpeed = 65;
        private static readonly int UniversalJuggleTimeToFallSpeed = 25;
        private static readonly FP UniversalJuggleLaunchSpeed = 8;
        private static readonly FP UniversalGroundBounceSpikeVelocity = -40; // this should be negative lol
        private static readonly FP GroundBounceGravityScaling = FP.FromString("1.05");

        private static readonly FP GlobalGravityScalingMod = FP.FromString("0.9");

        private static readonly int ExtraFramesUntilMaxFallSpeedAfterAirdash = 20;
        
        public enum JumpType
        {
            Up,
            Forward,
            Backward
        }
        
        public void TrajectoryArc(Frame f)
        {
            
            if (!Fsm.IsInState(State.Air)) return;
            
            CheckForActionTrajectory(f);
            CheckForHitWall(f);
            
            CheckToEndDashTrajectory(f);
            if (IsInDashTrajectory(f)) return;
            
            FPVector2 movementThisFrame = ComputeTrajectoryMovementThisFrame(f);
            ApplyFlippedMovement(f, movementThisFrame, EntityRef);
        }
        
        public FPVector2 ComputeTrajectoryMovementThisFrame(Frame f)
        {
            FP xMoveAmount = 0;
            FP yMoveAmount = 0;
            
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            
            int framesInTrajectory = GetFramesInTrajectory(f);
            xMoveAmount = trajectoryData->xVelocity;
            FP trajectoryHeight = trajectoryData->trajectoryHeight;
            int timeToTrajectoryHeight = trajectoryData->timeToTrajectoryHeight;
            FP fallSpeed = trajectoryData->fallSpeed;
            int timeToFallSpeed = trajectoryData->timeToFallSpeed;
            

            if (trajectoryData->groundBounce)
            {
                yMoveAmount = UniversalGroundBounceSpikeVelocity * Util.FrameLengthInSeconds;
            }
            else if (IsRising(f))
            {
                FP t = (FP)framesInTrajectory / (FP)timeToTrajectoryHeight;
                yMoveAmount = (SampleCubicCurve(t) * trajectoryHeight) / (FP)timeToTrajectoryHeight;
            }
            else
            {
                if (timeToFallSpeed > 0)
                {
                    FP t = (FP)(framesInTrajectory - timeToTrajectoryHeight) / (FP)timeToFallSpeed;
                    yMoveAmount = Util.Lerp(0, fallSpeed * FP.Minus_1 * Util.FrameLengthInSeconds, t);
                }
                else
                {
                    yMoveAmount = -10 * Util.FrameLengthInSeconds;
                }
            }


            
            return new FPVector2(xMoveAmount, yMoveAmount);
        }


        private bool IsInGroundBounce(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            FrameParam frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            
            return trajectoryData->groundBounce;
        }
        
        private void OnGroundBounce(TriggerParams? triggerParams)
        {
            
            if (triggerParams is null) return;
            FrameParam frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            frameParam.f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);
            comboData->gravityScaling *= GroundBounceGravityScaling;
            
            frameParam.f.Events.EntityVibrate(EntityRef, FP.FromString("0.4"), FP.FromString("0.4"), 40);
            HitstopSystem.EnqueueHitstop(frameParam.f, 10);
            
            // AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.GroundBounce, transform3D->Position.XY, 0, false);
            
            StartNewTrajectory(frameParam.f, trajectoryData->trajectoryHeight, 
                trajectoryData->timeToTrajectoryHeight, trajectoryData->xVelocity, 
                trajectoryData->fallSpeed, trajectoryData->timeToFallSpeed, false);
        }

        private void OnWallBounce(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            FrameParam frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            frameParam.f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);
            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            trajectoryData->wallBounce = false;
            
            frameParam.f.Events.EntityVibrate(EntityRef, FP.FromString("0.6"), FP.FromString("0.5"), 40);

            HitstopSystem.EnqueueHitstop(frameParam.f, 15);
            
            
            // var angle = transform3D->Position.X < 0 ? 90 : -90;
            // AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.GroundBounce, transform3D->Position.XY, angle, false);
            
            StartTrajectoryWithGravityScaling(frameParam.f, trajectoryData->xVelocity * -1 * FP.FromString("0.2"),
                5, false, false);
            
        }

        public void TryToFireJump(Frame f, JumpType type)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            if (trajectoryData->jumpsRemaining <= 0) return;
            if (GetFramesInTrajectory(f) < 20 && Fsm.IsInState(State.Air)) return;
            
            var param = new JumpParam() { f = f, Type = type, EntityRef = EntityRef};
            
            Fsm.Fire(Trigger.Jump, param);
        }

        private void StartNewJump(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (JumpParam)triggerParams;
            var xVelocity = GetFlippedXVelocityFromJumpType(param.f, param.Type);
            var character = Characters.GetPlayerCharacter(param.f, EntityRef);
            
            StartNewTrajectory(param.f, character.JumpHeight, character.JumpTimeToHeight, 
                xVelocity, character.FallSpeed, character.FallTimeToSpeed, false);

            param.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->jumpsRemaining--;
        }
        
        

        private void StartNewJuggle(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (CollisionHitParams)triggerParams;

            var xVelocity = param.XVelocity;
            
            StartTrajectoryWithGravityScaling(param.f, xVelocity,  param.TrajectoryHeight, param.GroundBounces, param.WallBounces);
        }

        private void StartTrajectoryWithGravityScaling(Frame f, FP xVelocity, FP trajectoryHeight, bool groundBounces, bool wallBounces)
        {
            f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);
            var gravityScaling = comboData->gravityScaling * comboData->gravityScaling * GlobalGravityScalingMod;
            var height = trajectoryHeight / gravityScaling;
            var fallSpeed = UniversalJuggleFallSpeed * gravityScaling;
            var timeToTrajectoryHeight = ((FP)GetJuggleTimeToHeight(trajectoryHeight) / gravityScaling).AsInt;
            
            StartNewTrajectory(f, height, timeToTrajectoryHeight, 
                xVelocity, fallSpeed, UniversalJuggleTimeToFallSpeed, groundBounces, wallBounces);
        }

        private int GetJuggleTimeToHeight(FP trajectoryHeight)
        {
            FP roughLog = trajectoryHeight;

            if (trajectoryHeight < 3) roughLog = trajectoryHeight * FP.FromString("1.3");
            else if (trajectoryHeight < 6) roughLog = trajectoryHeight * FP.FromString("0.95");
            else if (trajectoryHeight < 10) roughLog = trajectoryHeight * FP.FromString("0.6");
            else roughLog = trajectoryHeight * FP.FromString("0.4");

            return (roughLog / (UniversalJuggleLaunchSpeed * Util.FrameLengthInSeconds)).AsInt;
        }

        private void StartNewTrajectory(Frame f, FP trajectoryHeight, int timeToTrajectoryHeight, FP xVelocity, 
            FP fallSpeed, int timeToFallSpeed, bool groundBounce, bool wallBounce = false, TrajectoryDashType trajectoryDashType = TrajectoryDashType.None)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            trajectoryData->framesInTrajectory = 0;
            trajectoryData->virtualTimeInTrajectory = 0;
            trajectoryData->xVelocity = xVelocity;
            trajectoryData->startingTrajectoryHeight = transform3D->Position.Y;
            trajectoryData->trajectoryHeight = trajectoryHeight;
            trajectoryData->timeToTrajectoryHeight = timeToTrajectoryHeight;
            trajectoryData->fallSpeed = fallSpeed;
            trajectoryData->timeToFallSpeed = timeToFallSpeed;
            trajectoryData->groundBounce = groundBounce;
            trajectoryData->wallBounce = wallBounce;
            trajectoryData->dashType = trajectoryDashType;
        }

        public void CheckForLand(Frame f)
        {
            
            if (!Fsm.IsInState(State.Air)) return;
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            FP posY = transform3D->Position.Y;
            
            if (posY > Util.GroundHeight) return;
            if (ComputeTrajectoryMovementThisFrame(f).Y > 0) return;
            
            var param = new FrameParam() { f = f, EntityRef = EntityRef};
            
            Fsm.Fire(Trigger.Land, param);
            
        }

        private void OnLand(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            
            var param = (FrameParam)triggerParams;
            
            param.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            
            trajectoryData->jumpsRemaining = Characters.GetPlayerCharacter(param.f, EntityRef).JumpCount;
            transform3D->Position.Y = Util.GroundHeight;
        }

        private FP GetFlippedXVelocityFromJumpType(Frame f, JumpType type)
        {
            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            
            FP xMoveAmount = 0;
            
            if (type == JumpType.Forward)
            {
                xMoveAmount = character.JumpForwardSpeed * Util.FrameLengthInSeconds;
            }
            else if (type == JumpType.Backward)
            {
                xMoveAmount = character.JumpBackwardSpeed * Util.FrameLengthInSeconds * FP.Minus_1;
            }
            
            if (!PlayerDirectionSystem.IsFacingRight(f, EntityRef))
            {
                xMoveAmount *= FP.Minus_1;
            }

            return xMoveAmount;
        }

        public int GetFramesInTrajectory(Frame f)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var jumpData);
            // int framesInJump = jumpData->framesInTrajectory + 1;
            int framesInJump = Util.FramesFromVirtualTime(jumpData->virtualTimeInTrajectory) + 1;
            return framesInJump;
        }

        public bool IsRising(Frame f)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            int framesInJump = GetFramesInTrajectory(f);
            if (trajectoryData->groundBounce) return false;
            return framesInJump < trajectoryData->timeToTrajectoryHeight;
        }


        private void CheckForActionTrajectory(Frame f)
        {
            if (!Fsm.IsInState(State.Action)) return;

            var character = Characters.GetPlayerCharacter(f, EntityRef);
            var trajectorySectionGroup = character.ActionDict?[Fsm.State()].TrajectorySectionGroup;
            if (trajectorySectionGroup is null) return;
            if (!trajectorySectionGroup.IsOnFirstFrameOfSection(f, this)) return;
            var trajectory = trajectorySectionGroup.GetCurrentItem(f, this);
            if (trajectory is null) return;

            FP flipMod = PlayerDirectionSystem.IsFacingRight(f, EntityRef) ? 1 : -1;
            
            StartNewTrajectory(f, trajectory.TrajectoryHeight, trajectory.TimeToTrajectoryHeight, 
                trajectory.TrajectoryXVelocity * Util.FrameLengthInSeconds * flipMod, character.FallSpeed, character.FallTimeToSpeed, false);
        }

        private void CheckForHitWall(Frame f)
        {
            if (!Fsm.IsInState(State.AirHit)) return;
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            
            if (!trajectoryData->wallBounce) return;
            
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            if (Util.Abs(transform3D->Position.X) < WallHalfLength) return;
            
            Fsm.Fire(Trigger.HitWall, new FrameParam(){ f= f, EntityRef =  EntityRef});
        }
        
        
        private void OnAirdash(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            var character = Characters.GetPlayerCharacter(param.f, EntityRef);
            
            StartNewTrajectory(param.f, 0, 1, 
                0, character.FallSpeed, character.FallTimeToSpeed + ExtraFramesUntilMaxFallSpeedAfterAirdash,
                false, false, TrajectoryDashType.Forward);
        }
        
        
        private void OnAirBackdash(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            var character = Characters.GetPlayerCharacter(param.f, EntityRef);
            
            StartNewTrajectory(param.f, 0, 1, 
                0, character.FallSpeed, character.FallTimeToSpeed + ExtraFramesUntilMaxFallSpeedAfterAirdash,
                false, false, TrajectoryDashType.Backward);
        }

        private bool IsInDashTrajectory(Frame f)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            return trajectoryData->dashType != TrajectoryDashType.None;
        }

        private void CheckToEndDashTrajectory(Frame f)
        {
            var length = 0;
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            var character = Characters.GetPlayerCharacter(f, EntityRef);
            if (trajectoryData->dashType == TrajectoryDashType.Forward)
            {
                length = character.DashMovementSectionGroup.Duration();
            }
            else if (trajectoryData->dashType == TrajectoryDashType.Backward)
            {
                length = character.BackdashMovementSectionGroup.Duration();
            }

            if (FramesInCurrentState(f) >= length) trajectoryData->dashType = TrajectoryDashType.None;

        }
    }
}