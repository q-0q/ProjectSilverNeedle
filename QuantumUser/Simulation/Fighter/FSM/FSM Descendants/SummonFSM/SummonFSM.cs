using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public abstract unsafe partial class SummonFSM : FSM
    {
        public EntityRef playerOwnerEntity;
        public FPVector2 SummonPositionOffset = FPVector2.Zero;
        public bool DisableUnpoolOwnerSnap = false;

        
        public class SummonState : FSMState
        {
            public static int Pooled;
            public static int Unpooled;

        }
        
        public class SummonTrigger : Trigger
        {
            public static int Summoned;
            public static int Collided;
            public static int OwnerHit;
            public static int OwnerCollided;
            public static int Offscreen;
        }
        
        public SummonFSM()
        {
            int currentState = SummonState.Pooled;
            Fsm = new Machine<int, int>(currentState);
        }
        
        public override void SetupMachine()
        {
            base.SetupMachine();
            
            // Fsm.OnTransitionCompleted(OnStateChanged);

            Fsm.Configure(SummonState.Pooled)
                .OnEntry(OnPooled);

            Fsm.Configure(SummonState.Unpooled)
                // .OnEntry(_ =>
                // {
                //     Debug.LogError("unpooled");
                // })
                .OnEntryFrom(SummonTrigger.Summoned, OnUnpooled);
            
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

        }
        
        public override EntityRef GetPlayer()
        {
            return playerOwnerEntity;
        }
        
        // protected override void OnStateChanged(TriggerParams? triggerParams)
        // {
        //     base.OnStateChanged(triggerParams);
        //     
        //     if (triggerParams == null)
        //     {
        //         return;
        //     }
        //     var param = (FrameParam)triggerParams;
        //     
        //     param.f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var fsmData);
        //     fsmData->currentState = Fsm.State();
        // }

        public void OnPooled(TriggerParams? triggerParams)
        {
            // if (EntityRef == EntityRef.None) return;
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            var f = frameParam.f;
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Teleport(f, new FPVector2(0, -20).XYO);
        }
        
        public void OnUnpooled(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            var f = frameParam.f;
            Debug.Log("Unpooled " + EntityRef + ", frame " + FramesInCurrentState(f, playerOwnerEntity));
            
            // set direction
            f.Unsafe.TryGetPointer<PlayerDirection>(EntityRef, out var playerDirection);
            playerDirection->FacingRight = FSM.IsFacingRight(f, playerOwnerEntity);
            
            // set pos
            if (!DisableUnpoolOwnerSnap) SnapToOwnerPosWithOffset(f);
            
            // force update sprite
            Animation(f);

            // clear hit entities
            f.Unsafe.TryGetPointer<HitEntitiesTracker>(EntityRef, out var hitEntitiesTracker);
            f.ResolveList(hitEntitiesTracker->HitEntities).Clear();
        }

        protected void SnapToOwnerPosWithOffset(Frame f)
        {
            var transform3D = GetSnapPos(f, out var offsetXyo);
            transform3D->Teleport(f, offsetXyo);
        }

        protected Transform3D* GetSnapPos(Frame f, out FPVector3 offsetXyo)
        {
            f.Unsafe.TryGetPointer<PlayerDirection>(EntityRef, out var playerDirection);
            playerDirection->FacingRight = FSM.IsFacingRight(f, playerOwnerEntity);
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            f.Unsafe.TryGetPointer<Transform3D>(playerOwnerEntity, out var playerOwnerTransform3D);
            var flip = playerDirection->FacingRight ? 1 : -1;
            var offset = new FPVector2(SummonPositionOffset.X * flip, SummonPositionOffset.Y);
            offsetXyo = playerOwnerTransform3D->Position.XYO + offset.XYO;
            return transform3D;
        }

        public override void HandleSummonFSMTriggers(Frame f)
        {
            var hitboxInternals = GetCollisionBoxInternalsOfType(f, EntityRef, CollisionBoxType.Hitbox);
            

            var hurtboxSources = Util.GetOpponentFSMEntities(f, playerOwnerEntity);
            
            List<CollisionBoxInternal> hurtboxInternals = new List<CollisionBoxInternal>();
            foreach (var entityRef in hurtboxSources)
            {
                hurtboxInternals.AddRange(GetCollisionBoxInternalsOfType(f, entityRef,
                    CollisionBoxType.Hurtbox));
            }
            
            var triggerParams = new FrameParam() {f = f, EntityRef = EntityRef};
            

            
            foreach (var hitboxInternal in hitboxInternals)
            {
                foreach (var hurtboxInternal in hurtboxInternals)
                {
                    if (!CollisionBoxesOverlap(f, hurtboxInternal, hitboxInternal, out var overlapCenter, out var overlapWidth)) continue;
                    // Debug.Log("owner: " + playerOwnerEntity + ", hurtbox source: " + hurtboxInternal.source + ", hitbox source: " + hitboxInternal.source + ", me: " + EntityRef);

                    Fsm.Fire(SummonTrigger.Collided, new FrameParam() {f = f, EntityRef = EntityRef});
                }
            }

            var ownerMachine = FsmLoader.FSMs[GetPlayer()].Fsm;
            if (ownerMachine.IsInState(PlayerFSM.PlayerState.Hit) || ownerMachine.IsInState(PlayerFSM.PlayerState.CutsceneReactor) || 
                ownerMachine.IsInState(PlayerFSM.PlayerState.DeadFromGround) || ownerMachine.IsInState(PlayerFSM.PlayerState.DeadFromAir))
            {
                Fsm.Fire(SummonTrigger.OwnerHit, triggerParams);
            }
            
            if (GetPlayerFsm() is not PlayerFSM playerFsm) return;
            if (!playerFsm.IsWhiffed(f))
            {
                Fsm.Fire(SummonTrigger.OwnerCollided, triggerParams);
            }
            
            

        }
        

    }

}