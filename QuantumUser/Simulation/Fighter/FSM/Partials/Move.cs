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
    public unsafe partial class FSM
    {
        public 
        static FP _crossupThreshhold = FP.FromString("0.01");
        // private const bool AllowCrossup = false;
        protected static int _pushbackDuration = 23;
        protected static int _momentumDuration = 35;
        

        private FP _pushboxResistance = FP.FromString("0.3"); // 0 - 1;

        public readonly static FP WallHalfLength = FP.FromString("46"); // 46

        
        public virtual void Move(Frame f)
        {
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            FPVector2 movementThisFrame = ComputeMovementThisFrame(f);
            ApplyUnflippedMovement(f, movementThisFrame);
            MomentumMove(f);
            PushbackMove(f);
            PushboxCollide(f);
            SummonMove(f);
        }

        protected virtual void SummonMove(Frame f) { }


        private FPVector2 ComputeMovementThisFrame(Frame f)
        {
            FP xMoveAmount = GetXMovementFromMovementSectionGroup(f, StateMapConfig.MovementSectionGroup.Get(this, new FrameParam() { f = f, EntityRef = EntityRef}));

            return new FPVector2(xMoveAmount, 0);
        }

        private FP GetXMovementFromMovementSectionGroup(Frame f, SectionGroup<FP> sectionGroup)
        {
            if (sectionGroup is null) return 0;
            FP sectionDistance = sectionGroup.GetCurrentItem(f, this);
            FP sectionDuration = sectionGroup.GetCurrentItemDuration(f, this);
            return sectionDistance / sectionDuration;
        }

        private void ApplyUnflippedMovement(Frame f, FPVector2 v)
        {
            if (!IsFacingRight(f, EntityRef))
            {
                v.X *= FP.Minus_1;
            }

            ApplyFlippedMovement(f, v, EntityRef);
        }

        protected static void ApplyFlippedMovement(Frame f, FPVector2 v, EntityRef entityRef, bool debug = false)
        {
            GetPushboxes(f,  entityRef, out var opponentPushboxInternal, out var pushboxInternal);
            
            if (!CanCrossup(f, pushboxInternal, opponentPushboxInternal))
            {
                if (AreCollisionBoxesNextToEachOther(f, opponentPushboxInternal, pushboxInternal, out FP deltaX))
                {
                    v.X = !IsOnLeft(f, entityRef)
                        ? Util.Max(v.X, deltaX + _crossupThreshhold)
                        : Util.Min(v.X, deltaX - _crossupThreshhold);
                    
                }
            }
            
            f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var transform3D);
            FP slowdownMod = FsmLoader.FSMs[entityRef].GetSlowdownMod(f, entityRef);
            var v3 = (v.XYO * slowdownMod);
            
            if (FsmLoader.FSMs[entityRef] is PlayerFSM){
                f.Unsafe.TryGetPointer<Transform3D>(Util.GetOtherPlayer(f, entityRef), out var opponentTransform3d);
                var dX = Util.Abs((transform3D->Position + v3).X - opponentTransform3d->Position.X);
                var maxPlayerDistance = 26;
                if (dX > maxPlayerDistance)
                {
                    v3.X = 0;
                }
            }
            
            transform3D->Position += v3;
            
        }

        private static bool CanCrossup(Frame f, CollisionBoxInternal a, CollisionBoxInternal b)
        {
            if (a is null || b is null)
            {
                return true;
            }
            
            var fsmA = FsmLoader.FSMs[a.source];
            var fsmB = FsmLoader.FSMs[b.source];

            var aCrossup = fsmA?.StateMapConfig?.AllowCrossupSectionGroup?.Get(fsmA,
                    new FrameParam() { f = f, EntityRef = a.source })?.GetCurrentItem(f, fsmA);
            var bCrossup = fsmB?.StateMapConfig?.AllowCrossupSectionGroup?.Get(fsmB,
                new FrameParam() { f = f, EntityRef = b.source })?.GetCurrentItem(f, fsmB);

            var aBool = aCrossup ?? false;
            var bBool = bCrossup ?? false;
            return (aBool || bBool);
        }

        public virtual FP GetSlowdownMod(Frame f, EntityRef entityRef)
        {
            return 1;
        }

        protected void SetPosition(Frame f, FPVector2 v)
        {
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Position = v.XYO;
        }


        private void PushboxCollide(Frame f)
        {

            GetPushboxes(f, EntityRef, out var opponentPushboxInternal, out var collisionBoxInternal);
            
            if (CanCrossup(f, collisionBoxInternal, opponentPushboxInternal)) return;
            
            if (!CollisionBoxesOverlap(f, opponentPushboxInternal, collisionBoxInternal, out var overlapCenter,
                    out var overlapWidth)) return;

            FPVector2 pushboxMovement = new FPVector2(-overlapWidth * _pushboxResistance, 0);

            if (!IsOnLeft(f, EntityRef))
            {
                pushboxMovement.X *= FP.Minus_1;
            }

            // if (!Util.EntityIsCpu(f, EntityRef)) Debug.Log(pushboxMovement);

            ApplyFlippedMovement(f, pushboxMovement, EntityRef);
        }

        private static void GetPushboxes(Frame f, EntityRef entityRef, out CollisionBoxInternal opponentPushboxInternal,
            out CollisionBoxInternal pushboxInternal)
        {
            pushboxInternal= GetCollisionBoxInternalsOfType(f, entityRef, CollisionBoxType.Pushbox)?[0];
            opponentPushboxInternal = GetCollisionBoxInternalsOfType(f, Util.GetOtherPlayer(f, entityRef), CollisionBoxType.Pushbox)?[0];
        }

        protected virtual void PushbackMove(Frame f) { }
        
        protected FP GetPushbackVelocityThisFrame(int framesInPushback, FP totalDistance)
        {
            FP t = (FP)framesInPushback / (FP)_pushbackDuration;
            return SampleCubicCurve(t) * totalDistance / (FP)_pushbackDuration;
        }

        protected FP GetMomentumVelocityThisFrame(int framesInMomentum, FP totalDistance)
        {
            FP t = (FP)framesInMomentum / (FP)_momentumDuration;
            return SampleCubicCurve(t) * totalDistance / (FP)_momentumDuration;
        }

        protected FP SampleCubicCurve(FP x)
        {
            FP xMinus1 = x - 1;
            FP xCube = xMinus1 * xMinus1;
            return xCube * 3;
        }



        private void ResetYPos(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Position.Y = 0;
        }




        
        protected virtual void MomentumMove(Frame f) { }
    }
}