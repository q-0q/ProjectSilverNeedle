using System;
using System.Collections.Generic;
using Photon.Analyzer;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        static FP _crossupThreshhold = FP.FromString("0.01");
        private const bool AllowCrossup = false;
        private static int _pushbackDuration = 23;
        private static int _momentumDuration = 25;
        private static FP _throwTechPushback = FP.FromString("6"); 
        private FP _wallsSkew = FP.FromString("0.99");

        private FP _pushboxResistance = FP.FromString("0.3"); // 0 - 1;

        public readonly static FP WallHalfLength = FP.FromString("46"); // 46

        private static SectionGroup<FP> SoftKnockdownMovementSectionGroup = new SectionGroup<FP>()
        {
            Sections = new List<Tuple<int, FP>>()
            {
                new(8,-1),
                new (10, -4),
                new (20, 0)
            }
        };


        public void Move(Frame f)
        {

            
            FPVector2 movementThisFrame = ComputeMovementThisFrame(f);
            ApplyUnflippedMovement(f, movementThisFrame);
            MomentumMove(f);
            PushbackMove(f);
            PushboxCollide(f);
            KinematicReceiverMove(f);

            ClampPosToWall(f);
            
        }
        
        private FPVector2 ComputeMovementThisFrame(Frame f)
        {
            FP xMoveAmount = 0;
            FP yMoveAmount = 0;
            Character character = Characters.GetPlayerCharacter(f, EntityRef);

            if (Fsm.IsInState(State.WalkForward))
            {
                xMoveAmount = character.WalkForwardSpeed * Util.FrameLengthInSeconds;
            }
            else if (Fsm.IsInState(State.WalkBackward))
            {
                xMoveAmount = character.WalkBackwardSpeed * Util.FrameLengthInSeconds * FP.Minus_1;
            }
            else if (Fsm.IsInState(State.Dash))
            {
                xMoveAmount = GetXMovementFromMovementSectionGroup(f, character.DashMovementSectionGroup);
            }
            else if (Fsm.IsInState(State.Backdash))
            {
                xMoveAmount = GetXMovementFromMovementSectionGroup(f, character.BackdashMovementSectionGroup);
            }
            else if (Fsm.IsInState(State.AirDash))
            {
                xMoveAmount = GetXMovementFromMovementSectionGroup(f, character.DashMovementSectionGroup);
            }
            else if (Fsm.IsInState(State.AirBackdash))
            {
                xMoveAmount = GetXMovementFromMovementSectionGroup(f, character.BackdashMovementSectionGroup);
            }
            else if (Fsm.IsInState(State.SoftKnockdown))
            {
                xMoveAmount = GetXMovementFromMovementSectionGroup(f, SoftKnockdownMovementSectionGroup);
            }
            else if (Fsm.IsInState(State.Action))
            {
                if (character.ActionDict[Fsm.State()].MovementSectionGroup is not null)
                {
                    xMoveAmount = GetXMovementFromMovementSectionGroup(f ,character.ActionDict[Fsm.State()].MovementSectionGroup);
                }
            }
            
            return new FPVector2(xMoveAmount, yMoveAmount);
        }

        private FP GetXMovementFromMovementSectionGroup(Frame f, SectionGroup<FP> sectionGroup)
        {
            FP sectionDistance = sectionGroup.GetCurrentItem(f, this);
            FP sectionDuration = sectionGroup.GetCurrentItemDuration(f, this);
            return sectionDistance / sectionDuration;
        }

        private void ApplyUnflippedMovement(Frame f, FPVector2 v)
        {
            if (!PlayerDirectionSystem.IsFacingRight(f, EntityRef))
            {
                v.X *= FP.Minus_1;
            }
            
            ApplyFlippedMovement(f, v, EntityRef);
        }

        private static void ApplyFlippedMovement(Frame f, FPVector2 v, EntityRef entityRef, bool debug=false)
        {
            
            
            GetPushboxes(f,  entityRef, out var opponentPushboxEntityRef, out var pushboxEntityRef);
            
            if (!AllowCrossup)
            {
                if (AreCollisionBoxesNextToEachOther(f, opponentPushboxEntityRef, pushboxEntityRef, out FP deltaX))
                {
                    // if (!Util.EntityIsCpu(f, entityRef)) Debug.Log(deltaX + " --- " + v.X);
                    v.X = !PlayerDirectionSystem.IsOnLeft(f, entityRef) ? Util.Max(v.X, deltaX + _crossupThreshhold) : Util.Min(v.X, deltaX - _crossupThreshhold);
                }
            }

            
            f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var transform3D);
            FP slowdownMod = Util.GetSlowdownMod(f, entityRef);
            
            
            transform3D->Position += (v.XYO * slowdownMod);
            
            
            Pushbox(f, entityRef, debug);
        }

        private void SetPosition(Frame f, FPVector2 v)
        {
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Position = v.XYO;
        }


        private void PushboxCollide(Frame f)
        {
            if (AllowCrossup) return;
            
            GetPushboxes(f, EntityRef, out var opponentEntityRef, out var entityRef);
            
            if (!CollisionBoxesOverlap(f, opponentEntityRef, entityRef, out var overlapCenter, out var overlapWidth)) return;

            FPVector2 pushboxMovement = new FPVector2(-overlapWidth * _pushboxResistance, 0);
                    
            if (!PlayerDirectionSystem.IsOnLeft(f, EntityRef))
            {
                pushboxMovement.X *= FP.Minus_1;
            }
            
            // if (!Util.EntityIsCpu(f, EntityRef)) Debug.Log(pushboxMovement);
            
            ApplyFlippedMovement(f, pushboxMovement, EntityRef);
        }

        private static void GetPushboxes(Frame f, EntityRef entityRef, out EntityRef opponentPushboxEntityRef, out EntityRef pushboxEntityRef)
        {
            foreach (var (opponentEntity, opponentPushboxData) in f.GetComponentIterator<CollisionBoxData>())
            {
                if (opponentPushboxData.source == entityRef) continue;
                if (opponentPushboxData.type != (int)CollisionBox.CollisionBoxType.Pushbox) continue;

                foreach (var (entity, selfPushboxData) in f.GetComponentIterator<CollisionBoxData>())
                {
                    if (selfPushboxData.source != entityRef) continue;
                    if (selfPushboxData.type != (int)CollisionBox.CollisionBoxType.Pushbox) continue;

                    opponentPushboxEntityRef = opponentEntity;
                    pushboxEntityRef = entity;
                    return;
                }
            }

            opponentPushboxEntityRef = EntityRef.None;
            pushboxEntityRef = EntityRef.None;
        }

        private void PushbackMove(Frame f)
        {
            f.Unsafe.TryGetPointer<PushbackData>(EntityRef, out var pushbackData);
            if (pushbackData->framesInPushback >= _pushbackDuration) return;
            
            FPVector2 v =
                new FPVector2(GetPushbackVelocityThisFrame(pushbackData->framesInPushback,
                    pushbackData->pushbackAmount), 0);

            bool inCorner = Util.IsPlayerInCorner(f, EntityRef);
            EntityRef entityRef = inCorner ? Util.GetOtherPlayer(f, EntityRef) : EntityRef;
            if (inCorner) v.X *= FP.Minus_1;
            
            ApplyFlippedMovement(f, v, entityRef, true);
           
        }

        private void StartPushback(Frame f, FP totalDistance)
        {
            f.Unsafe.TryGetPointer<PushbackData>(EntityRef, out var pushbackData);
            pushbackData->framesInPushback = 0;
            pushbackData->pushbackAmount = totalDistance;
        }

        private FP GetPushbackVelocityThisFrame(int framesInPushback, FP totalDistance)
        {
            FP t = (FP)framesInPushback / (FP)_pushbackDuration;
            return SampleCubicCurve(t) * totalDistance / (FP)_pushbackDuration;
        }
        
        private FP GetMomentumVelocityThisFrame(int framesInMomentum, FP totalDistance)
        {
            FP t = (FP)framesInMomentum / (FP)_momentumDuration;
            return SampleCubicCurve(t) * totalDistance / (FP)_momentumDuration;
        }

        private FP SampleCubicCurve(FP x)
        {
            FP xMinus1 = x - 1;
            FP xCube = xMinus1 * xMinus1;
            return xCube * 3;
        }

        private void KinematicReceiverMove(Frame f)
        {
            if (!Fsm.IsInState(State.KinematicReceiver)) return;

            f.Unsafe.TryGetPointer<KinematicsData>(Util.GetOtherPlayer(f, EntityRef), 
                out var kinematicsData);

            FPVector2 offset = Characters.GetPlayerCharacter(f, EntityRef).KinematicAttachPointOffset;
            SetPosition(f, kinematicsData->attachPosition - offset);
            
        }

        private void ResetYPos(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Position.Y = 0;
        }

        private void OnEnterThrowTech(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;

            var distance = _throwTechPushback;
            if (PlayerDirectionSystem.IsOnLeft(frameParam.f, EntityRef)) distance *= FP.Minus_1;
            StartPushback(frameParam.f, distance);
            Util.StartDramatic(frameParam.f, EntityRef, 35);
            
            // only make the tech animation sprite for player 0
            if (Util.GetPlayerId(frameParam.f, EntityRef) != 0) return;

            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            frameParam.f.Unsafe.TryGetPointer<Transform3D>(Util.GetOtherPlayer(frameParam.f, EntityRef), out var opponentTransform3D);

            FPVector2 attachPos = GetFirstKinematicsAttachPosition(frameParam.f, EntityRef);
            FPVector2 opponentAttachPos = GetFirstKinematicsAttachPosition(frameParam.f, Util.GetOtherPlayer(frameParam.f, EntityRef));

            FPVector2 pos = attachPos + transform3D->Position.XY;
            FPVector2 opponentPos = opponentAttachPos + opponentTransform3D->Position.XY;

            FPVector2 avgPos = (pos + opponentPos) * FP._0_50;
            
            AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.Tech, 
                avgPos, 0, 
                false);
        }

        public void ClampPosToWall(Frame f)
        {
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            
            FP clampX = Fsm.IsInState(State.Air) || Fsm.IsInState(State.KinematicReceiver) ? WallHalfLength + 1 : WallHalfLength;
            if (Util.IsPlayerFacingAwayFromWall(f, EntityRef)) clampX *= _wallsSkew;

            var lerpedX = Util.Lerp(transform3D->Position.X, Util.Clamp(transform3D->Position.X, -clampX, clampX),
                Util.FrameLengthInSeconds * 60);
            transform3D->Position.X = lerpedX;
        }

        private void StartMomentumCallback(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;

            FP amount = PlayerDirectionSystem.IsFacingRight(frameParam.f, EntityRef) ? 3 : -3;
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
        
        private void MomentumMove(Frame f)
        {
            f.Unsafe.TryGetPointer<MomentumData>(EntityRef, out var momentumData);
            if (momentumData->framesInMomentum >= _pushbackDuration) return;
            
            FPVector2 v =
                new FPVector2(GetMomentumVelocityThisFrame(momentumData->framesInMomentum,
                    momentumData->momentumAmount), 0);
            
            ApplyFlippedMovement(f, v, EntityRef);
        }
    }
}