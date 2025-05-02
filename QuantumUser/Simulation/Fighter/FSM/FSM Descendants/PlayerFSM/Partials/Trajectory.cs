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
        private static readonly FP UniversalGroundBounceSpikeVelocity = -60; // this should be negative lol
        private static readonly FP GroundBounceGravityScaling = FP.FromString("1.05");

        private static readonly FP GlobalGravityScalingMod = FP.FromString("1.05");

        private static readonly int ExtraFramesUntilMaxFallSpeedAfterAirdash = 20;
        

        
        public override void TrajectoryArc(Frame f)
        {
            
            if (!Fsm.IsInState(PlayerState.Air)) return;
            
            CheckForActionTrajectory(f);
            CheckForHitWall(f);
            
            // CheckToEndDashTrajectory(f);
            // if (IsInDashTrajectory(f)) return;
            
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

            // New multiplier retrieval (you can change the logic or source of this value)
            var yMod = StateMapConfig.TrajectoryYVelocityMod
                .Get(this, new FrameParam() { f = f, EntityRef = EntityRef }).GetCurrentItem(f, this);

            // Calculate a factor or multiplier to apply to the y velocity
            FP yVelocityMultiplier = 1; // Default is no change

            // For example, apply the multiplier if the virtual time in trajectory is within a specific range
            // if (trajectoryData->virtualTimeInTrajectory > certainThresholdStart && 
            //     trajectoryData->virtualTimeInTrajectory < certainThresholdEnd)
            // {
            //     
            // }
            
            yVelocityMultiplier = yMod; // Use the multiplier during this time

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

            // Apply the multiplier to the vertical movement (yMoveAmount)
            yMoveAmount *= yVelocityMultiplier;
            
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
            HitstopSystem.EnqueueHitstop(frameParam.f, 9);
            
            // AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.GroundBounce, transform3D->Position.XY, 0, false);
            
            
            // multiply height by (4/5) == 0.8 to make the popup time off a ground bounce slightly faster than a normal launch would
            StartNewTrajectory(frameParam.f, trajectoryData->trajectoryHeight, 
                trajectoryData->timeToTrajectoryHeight * 4 / 5, trajectoryData->xVelocity, 
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

            HitstopSystem.EnqueueHitstop(frameParam.f, 12);
            
            
            // var angle = transform3D->Position.X < 0 ? 90 : -90;
            // AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.GroundBounce, transform3D->Position.XY, angle, false);
            
            StartTrajectoryWithGravityScaling(frameParam.f, trajectoryData->xVelocity * -1 * FP.FromString("0.5"),
                1, false, false);
            
        }

        public override void TryToFireJump(Frame f, JumpType type)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            if (trajectoryData->jumpsRemaining <= 0)
            {
                // Debug.Log("No jumps remaining.");
                return;
            }
            // if (GetFramesInTrajectory(f) < 5 && Fsm.IsInState(PlayerState.Air)) return;
            
            var param = new JumpParam() { f = f, Type = type, EntityRef = EntityRef};
            
            Fsm.Fire(PlayerTrigger.Jump, param);
        }

        private void StartNewJump(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (JumpParam)triggerParams;
            var trajectory = GetFlippedTrajectoryFromJumpType(param.f, param.Type);
            if (trajectory is null) return;
            
            
            StartNewTrajectory(param.f, trajectory.TrajectoryHeight, trajectory.TimeToTrajectoryHeight, 
                trajectory.TrajectoryXVelocity * Util.FrameLengthInSeconds, FallSpeed, FallTimeToSpeed, false);

            param.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->jumpsRemaining--;
        }

        private bool IsTrajectoryEmpty(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            var param = (FrameParam)triggerParams;
            param.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            return trajectoryData->empty;
        }

        private void MakeTrajectoryEmpty(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            param.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->empty = true;
        }
        
        private void MakeTrajectoryFull(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            param.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->empty = false;
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
            var gravityScaling = comboData->gravityScaling;
            var height = trajectoryHeight / gravityScaling;
            var fallSpeed = UniversalJuggleFallSpeed * gravityScaling;
            var timeToTrajectoryHeight = ((FP)GetJuggleTimeToHeight(trajectoryHeight) / gravityScaling).AsInt;
            
            StartNewTrajectory(f, height, timeToTrajectoryHeight + 6, 
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

        public override void CheckForLand(Frame f)
        {
            
            if (!Fsm.IsInState(PlayerState.Air)) return;
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            FP posY = transform3D->Position.Y;
            
            if (posY > Util.GroundHeight) return;
            if (ComputeTrajectoryMovementThisFrame(f).Y > 0) return;
            
            var param = new FrameParam() { f = f, EntityRef = EntityRef};
            
            Fsm.Fire(PlayerTrigger.Land, param);
            
        }

        private void OnLand(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            
            var param = (FrameParam)triggerParams;
            
            param.f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            trajectoryData->jumpsRemaining = JumpCount;
            transform3D->Position.Y = Util.GroundHeight;
        }

        private Trajectory GetFlippedTrajectoryFromJumpType(Frame f, JumpType type)
        {

            Trajectory trajectory = new Trajectory();
            Trajectory template = type switch
            {
                JumpType.Forward => ForwardJumpTrajectory,
                JumpType.Backward => BackwardJumpTrajectory,
                _ => UpwardJumpTrajectory
            };

            if (template is null) return null;
            
            trajectory.TrajectoryXVelocity = template.TrajectoryXVelocity;
            trajectory.TrajectoryHeight = template.TrajectoryHeight;
            trajectory.TimeToTrajectoryHeight = template.TimeToTrajectoryHeight;

            if (!IsFacingRight(f, EntityRef))
            {
                trajectory.TrajectoryXVelocity *= FP.Minus_1;
            }

            return trajectory;
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
            if (!Fsm.IsInState(PlayerState.Action)) return;

            var trajectorySectionGroup = StateMapConfig.TrajectorySectionGroup.Get(this);
            if (trajectorySectionGroup is null) return;
            if (!trajectorySectionGroup.IsOnFirstFrameOfSection(f, this)) return;
            var trajectory = trajectorySectionGroup.GetCurrentItem(f, this);
            if (trajectory is null) return;

            FP flipMod = IsFacingRight(f, EntityRef) ? 1 : -1;
            
            StartNewTrajectory(f, trajectory.TrajectoryHeight, trajectory.TimeToTrajectoryHeight, 
                trajectory.TrajectoryXVelocity * Util.FrameLengthInSeconds * flipMod, FallSpeed, FallTimeToSpeed, false);
        }

        private void CheckForHitWall(Frame f)
        {
            if (!Fsm.IsInState(PlayerState.AirHit)) return;
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            
            if (!trajectoryData->wallBounce) return;
            
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            if (Util.Abs(transform3D->Position.X) < WallHalfLength) return;
            
            Fsm.Fire(PlayerTrigger.HitWall, new FrameParam(){ f= f, EntityRef =  EntityRef});
        }
        
        
        private void OnAirdash(TriggerParams? triggerParams)
        {
            Debug.Log("AIRDASH");
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            
            StartNewTrajectory(param.f, 0, 1, 
                0, FallSpeed, FallTimeToSpeed + ExtraFramesUntilMaxFallSpeedAfterAirdash,
                false, false, TrajectoryDashType.Forward);
        }
        
        
        private void OnAirBackdash(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            
            param.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            
            StartNewTrajectory(param.f, 0, 1, 
                0, FallSpeed, FallTimeToSpeed + ExtraFramesUntilMaxFallSpeedAfterAirdash,
                false, false, TrajectoryDashType.Backward);
        }

        private bool IsInDashTrajectory(Frame f)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            return trajectoryData->dashType != TrajectoryDashType.None;
        }

        private void CheckToEndDashTrajectory(Frame f)
        {
            // var length = 0;
            // f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            // var character = Characters.GetPlayerCharacter(f, EntityRef);
            // if (trajectoryData->dashType == TrajectoryDashType.Forward)
            // {
            //     length = character.DashMovementSectionGroup.Duration();
            // }
            // else if (trajectoryData->dashType == TrajectoryDashType.Backward)
            // {
            //     length = character.BackdashMovementSectionGroup.Duration();
            // }
            //
            // if (FramesInCurrentState(f) >= length) trajectoryData->dashType = TrajectoryDashType.None;

        }
    }
}