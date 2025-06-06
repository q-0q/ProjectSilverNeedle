using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Wasp;
using Random = UnityEngine.Random;

namespace Quantum
{
    public unsafe partial class FSM
    {

        public enum HurtType
        {
            Regular,
            Counter,
            Punish
        }
        
        public class CollisionBoxInternal
        {
            public EntityRef source;
            public CollisionBoxType type;
            public HurtType HurtType;
            public Hit.HitType HitType;
            public FP width;
            public FP height;
            public FPVector2 pos;

            public bool highCrush;
        
            public int level;
            public int bonusHitStun;
            public int bonusBlockStun;
            public int bonusHitStop;
            public FP blockPushback;
            public FP hitPushback;
        
            public FP visualHitAngle;
            public FPVector2 visualHitPos;
            public FP trajectoryHeight;
            public FP trajectoryXVelocity;
            public FP gravityProration;
            public FP gravityScaling;
            public FP damageScaling;
            public FP damage;
            public bool launches;
            public bool hardKnockdown;
            public bool groundBounce;
            public bool wallBounce;
            public bool projectile;

            public int cutsceneIndex;

            public int lookupId = -1;


        }

        public static int ThrowTechWindowSize = 10;
        
        public static int NumNonBlockingJumpFrames = 5;
        
        
        public static List<int> AttackLevelCounterSlowdownDuration = new List<int>() { 35, 35, 35, 35, 35 };
        public static List<int> AttackLevelCounterHitstop = new List<int>() { Hit.AttackLevelHitstop[0], Hit.AttackLevelHitstop[1], Hit.AttackLevelHitstop[2], 21, 31 };
        public static List<int> AttackLeveCounterBonusHitstun = new List<int>() { 0, 6, 6, 13, 18 };
        
        public static int CounterBonusHitstun = 5;
        public static int PunishBonusHitstun = 0;
        public static FP CounterSlowdownMultiplier = FP.FromString("0.5");
        public static FP CounterHitDamageMultiplier = 2;
        public static FP CounterHitDamageScaling = FP.FromString("1.3");
        public static FP CounterHitGravityScalingMultiplier = FP.FromString("0.95");
        public static FP GlobalDamageModifier = FP.FromString("0.09");


        public static int CounterBonusHitstop = 10;
        public static int PunishBonusHitstop = 0;
        
        public static List<CollisionBoxInternal> GetCollisionBoxInternalsOfType(Frame f, EntityRef source, CollisionBoxType type)
        {
            f.Unsafe.TryGetPointer<FSMData>(source, out var fsmData);
            int collisionState = fsmData->currentCollisionState;
            int collisionStateFrames = fsmData->collisionFramesInState;
            
            var fsm = FsmLoader.FSMs[source];
            
            
            if (type == CollisionBoxType.Pushbox)
            {
                var pushBox = fsm.StateMapConfig.Pushbox.Lookup(collisionState, fsm);

                if (pushBox is null) return null;
                
                var pushboxInternal = new CollisionBoxInternal()
                {
                    source = source,
                    type = CollisionBoxType.Pushbox,
                    width = pushBox.Width,
                    height = pushBox.Height,
                    pos = GetCollisionBoxWorldPosition(f, source, pushBox).XY
                };
                
                return new List<CollisionBoxInternal>() { pushboxInternal };
            }

            if (type == CollisionBoxType.Hurtbox)
            {
                if (fsm.StateMapConfig.InvulnerableBefore.Lookup(collisionState, fsm) > collisionStateFrames)
                    return new List<CollisionBoxInternal>();
                var hurtTypeSectionGroup = fsm.StateMapConfig.HurtTypeSectionGroup.Lookup(collisionState, fsm);
                var hurtType = HurtType.Regular;
                if (hurtTypeSectionGroup is not null)
                    hurtType = hurtTypeSectionGroup.GetItemFromIndex(collisionStateFrames);
                var hurtBoxCollectionSectionGroup = fsm.StateMapConfig.HurtboxCollectionSectionGroup.Lookup(collisionState, fsm);
                if (hurtBoxCollectionSectionGroup is null) return new List<CollisionBoxInternal>();
                var hurtboxCollection = hurtBoxCollectionSectionGroup.GetItemFromIndex(collisionStateFrames);
                if (hurtboxCollection is null) return new List<CollisionBoxInternal>();
                
                
                // TODO: once we have priority implemented for StateMap Get, we can remove this check
                if (fsm is PlayerFSM && fsm.Fsm.IsInState(PlayerFSM.PlayerState.Cutscene))
                    return new List<CollisionBoxInternal>();

                var highCrush = (fsm is PlayerFSM && fsm.Fsm.IsInState(PlayerFSM.PlayerState.Dash));
                
                var hurtboxInternals = new List<CollisionBoxInternal>();
                foreach (var hurtbox in hurtboxCollection.CollisionBoxes)
                {
                    var _internal = new CollisionBoxInternal()
                    {
                        source = source,
                        type = CollisionBoxType.Hurtbox,
                        HurtType = hurtType,
                        width = hurtbox.Width,
                        height = hurtbox.Height,
                        pos = GetCollisionBoxWorldPosition(f, source, hurtbox).XY,
                        highCrush = false
                    };

                    hurtboxInternals.Add(_internal);
                }

                return hurtboxInternals;

            }

            if (type == CollisionBoxType.Hitbox)
            {
                var hitSectionGroup = fsm.StateMapConfig.HitSectionGroup.Lookup(collisionState, fsm);
                if (hitSectionGroup is null) return new List<CollisionBoxInternal>();

                var hit = hitSectionGroup.GetItemFromIndex(collisionStateFrames);
                if (hit is null) return new List<CollisionBoxInternal>();
                if (hit.HitboxCollections is null) return new List<CollisionBoxInternal>();

                var firstFrame = hitSectionGroup.GetFirstFrameFromIndex(collisionStateFrames);

                var hitType = hit.Type;
                var hitboxCollection = hit.HitboxCollections.GetItemFromIndex(collisionStateFrames - firstFrame);
                
                
                var hitboxInternals = new List<CollisionBoxInternal>();
                foreach (var hurtbox in hitboxCollection.CollisionBoxes)
                {
                    var pos = GetCollisionBoxWorldPosition(f, source, hurtbox).XY;
                    f.Unsafe.TryGetPointer<Transform3D>(source, out var sourceTransform);
                    var offset = new FPVector2(hit.VisualHitPositionOffset.X * (IsFacingRight(f, source) ? 1 : -1), hit.VisualHitPositionOffset.Y);
                    var _internal = new CollisionBoxInternal()
                    {
                        source = source,
                        type = CollisionBoxType.Hitbox,
                        HitType = hitType,
                        width = hurtbox.Width,
                        height = hurtbox.Height,
                        pos = pos,
                        
                        level = hit.Level,
                        bonusHitStun = hit.BonusHitstun,
                        bonusBlockStun = hit.BonusBlockstun,
                        bonusHitStop = hit.BonusHitstop,
                        blockPushback = hit.BlockPushback,
                        hitPushback = hit.HitPushback,
                        visualHitAngle = hit.VisualAngle,
                        visualHitPos = offset + sourceTransform->Position.XY,
                        trajectoryHeight = hit.TrajectoryHeight,
                        trajectoryXVelocity = hit.TrajectoryXVelocity,
                        gravityProration = hit.GravityProration,
                        gravityScaling = hit.GravityScaling,
                        damageScaling = hit.DamageScaling,
                        damage = hit.Damage,
                        launches = hit.Launches,
                        hardKnockdown = hit.HardKnockdown,
                        groundBounce = hit.GroundBounce,
                        wallBounce = hit.WallBounce,
                        projectile = hit.Projectile,
                        
                        
                        cutsceneIndex = hit.TriggerCutscene,
                        
                        lookupId = hit.LookupId
                    };
                    
                    hitboxInternals.Add(_internal);
                }
                
                return hitboxInternals;
            }
            
            return null;
        }

        
        public void HitboxHurtboxCollide(Frame f)
        {

            HandleProxBlock(f);
            
            var hurtboxInternals = GetCollisionBoxInternalsOfType(f, EntityRef, CollisionBoxType.Hurtbox);
            var myHitboxInternals = GetCollisionBoxInternalsOfType(f, EntityRef, CollisionBoxType.Hitbox);

            var hitboxSources = Util.GetOpponentFSMEntities(f, EntityRef);
            List<CollisionBoxInternal> opponentHitboxInternals = new List<CollisionBoxInternal>();
            foreach (var entityRef in hitboxSources)
            {
                opponentHitboxInternals.AddRange(GetCollisionBoxInternalsOfType(f, entityRef,
                    CollisionBoxType.Hitbox));
            }
            
            foreach (var hitboxInternal in opponentHitboxInternals)
            {
                
                // clashes
                foreach (var myHitboxInternal in myHitboxInternals)
                {
                    if (myHitboxInternal.HitType == Hit.HitType.Throw) continue;
                    if (hitboxInternal.HitType == Hit.HitType.Throw) continue;
                    if (!CollisionBoxesOverlap(f, hitboxInternal, myHitboxInternal, out var overlapCenter, out var overlapWidth)) continue;
                    if (!CanBeHitBySource(f, hitboxInternal.source)) continue;
                    
                    AddMeToSourceHitList(f, hitboxInternal.source);
                    InvokeClash(f, this, hitboxInternal, myHitboxInternal, hitboxInternal.pos);
                    
                    if (FsmLoader.FSMs[hitboxInternal.source] is not PlayerFSM opponentPlayerFsm) continue;
                    if (!opponentPlayerFsm.CanBeHitBySource(f, myHitboxInternal.source)) continue;
                    opponentPlayerFsm.AddMeToSourceHitList(f, myHitboxInternal.source);
                    InvokeClash(f, opponentPlayerFsm, myHitboxInternal, hitboxInternal, overlapCenter);
                }
                
                
                foreach (var hurtboxInternal in hurtboxInternals)
                {
                    if (!CollisionBoxesOverlap(f, hitboxInternal, hurtboxInternal, out var overlapCenter, out var overlapWidth)) continue;
                    if (!CanBeHitBySource(f, hitboxInternal.source)) continue;
                    
                    AddMeToSourceHitList(f, hitboxInternal.source);
                    InvokeHitboxHurtboxCollision(f, hurtboxInternal, hitboxInternal, overlapCenter);
                    
                }
            }
        }

        protected virtual void InvokeHitboxHurtboxCollision(Frame frame, CollisionBoxInternal hurtboxInternal, CollisionBoxInternal hitboxInternal, FPVector2 overlapCenter) {}
        
        protected virtual void HandleProxBlock(Frame frame) {}
        
        protected virtual void InvokeClash(Frame frame, FSM fsm, CollisionBoxInternal myHitboxInternal, CollisionBoxInternal hitboxInternal, FPVector2 overlapCenter) {}

        
        
        public virtual void HandleSummonFSMTriggers(Frame f) { }

        private static FPVector3 GetCollisionBoxWorldPosition(Frame f, EntityRef source, CollisionBox collisionBox)
        {
            FP growOffsetX = collisionBox.GrowWidth ? collisionBox.Width * FP._0_50 : 0;
            FP growOffsetY = collisionBox.GrowHeight ? collisionBox.Height * FP._0_50 : 0;
            FP flipXMod = IsFacingRight(f, source) ? FP._1 : FP.Minus_1;
                    
            FPVector3 posOffset = new FPVector3(collisionBox.PosX, collisionBox.PosY, 0);
            FPVector3 growOffset = new FPVector3(growOffsetX, growOffsetY, 0);
            
            FPVector3 offset = (posOffset + growOffset);
            FPVector3 offsetFlipped = new FPVector3(offset.X * flipXMod, offset.Y, 0);
                    
            
            f.Unsafe.TryGetPointer<Transform3D>(source, out var sourceTransform);
            return sourceTransform->Position + offsetFlipped;
        }
        
        
        public bool CanBeHitBySource(Frame f, EntityRef source)
        {
            f.Unsafe.TryGetPointer<HitEntitiesTracker>(source, out var hitEntitiesTracker);
            var hitEntities = f.ResolveList(hitEntitiesTracker->HitEntities);
            foreach (var hitEntity in hitEntities)
            {
                if (hitEntity == EntityRef) return false;
            }
            return true;
        }

        public void AddMeToSourceHitList(Frame f, EntityRef source)
        {
            f.Unsafe.TryGetPointer<HitEntitiesTracker>(source, out var hitEntitiesTracker);
            var hitEntities = f.ResolveList(hitEntitiesTracker->HitEntities);
            hitEntities.Add(EntityRef);
        }
        
        public bool IsOnFirstFrameOfHit(Frame f)
        {
            if (!HasHitActive(f)) return false;
            var hitSectionGroup = GetCurrentHitSectionGroup(f);
            if (hitSectionGroup == null) return false;
            return (hitSectionGroup.IsOnFirstFrameOfSection(f, this));
        }

        private SectionGroup<Hit> GetCurrentHitSectionGroup(Frame f)
        {
            SectionGroup<Hit> hitSectionGroup = StateMapConfig.HitSectionGroup.Get(this);
            return hitSectionGroup;
        }

        public void ClearHitEntities(Frame f)
        {
            f.Unsafe.TryGetPointer<HitEntitiesTracker>(EntityRef, out var hitEntitiesTracker);
            var hitEntities = f.ResolveList(hitEntitiesTracker->HitEntities);
            hitEntities.Clear();
        }
        
        public static bool CollisionBoxesOverlap(Frame f, CollisionBoxInternal boxAInternal, CollisionBoxInternal boxBInternal, out FPVector2 overlapCenter, out FP overlapWidth)
        {
            overlapCenter = FPVector2.Zero; // Or any default value indicating no overlap
            overlapWidth = 0;
            
            if (boxAInternal is null || boxBInternal is null) return false;
            
            FPVector2 extentsA = new FPVector2(boxAInternal.width * FP._0_50, boxAInternal.height * FP._0_50);
            FPVector2 extentsB = new FPVector2(boxBInternal.width * FP._0_50, boxBInternal.height * FP._0_50);
            FPVector2 posA = boxAInternal.pos.XY;
            FPVector2 posB = boxBInternal.pos.XY;
            
            FP aHalfWidth = extentsA.X;
            FP aHalfHeight = extentsA.Y;
            FP bHalfWidth = extentsB.X;
            FP bHalfHeight = extentsB.Y;

            FP aCenterX = posA.X;
            FP aCenterY = posA.Y;
            FP bCenterX = posB.X;
            FP bCenterY = posB.Y;

            FP deltaX = Util.Abs(aCenterX - bCenterX);
            FP deltaY = Util.Abs(aCenterY - bCenterY);

            FP minDistanceX = aHalfWidth + bHalfWidth;
            FP minDistanceY = aHalfHeight + bHalfHeight;
            
            bool overlapOnX = deltaX < minDistanceX;
            bool overlapOnY = deltaY < minDistanceY;
            
            if (overlapOnX && overlapOnY)
            {
                FP overlapCenterX = (aCenterX + bCenterX) * FP._0_50;
                FP overlapCenterY = (aCenterY + bCenterY) * FP._0_50;
                
                FP overlapWidthX = minDistanceX - deltaX;
                FP overlapWidthY = minDistanceY - deltaY;
                overlapWidth = Util.Min(overlapWidthX, overlapWidthY);

                overlapCenter = new FPVector2(overlapCenterX, overlapCenterY);
                return true;
            }
            
            return false;
        }
        
        static bool AreCollisionBoxesNextToEachOther(Frame f, CollisionBoxInternal boxAInternal, CollisionBoxInternal boxBInternal, out FP deltaX)
        {
            deltaX = 0;

            if (boxAInternal is null || boxBInternal is null) return false;

            FPVector2 posA = boxAInternal.pos.XY;
            FPVector2 posB = boxBInternal.pos.XY;
            
            FP aHalfHeight = (boxAInternal.height * FP._0_50);
            FP bHalfHeight = (boxBInternal.height * FP._0_50);

            FP deltaY = Util.Abs(posA.Y - posB.Y);
            

            // Calculate deltaX based on edges
            deltaX = posA.X - posB.X;
            
            // Debug.Log("posA: " + posA + "posB: " + posB);

            // Calculate the distance needed to be "next to each other"
            FP minDistanceY = aHalfHeight + bHalfHeight;

            // Check if they are next to each other
            return deltaY < minDistanceY;
        }
        
        public bool HasHitActive(Frame f)
        {
            var hit = GetCurrentHitSectionGroup(f)?.GetCurrentItem(f, this);
            return hit != null;
        }



        protected void InvokeCollisionVibrate(Frame f, int playerTrigger)
        {
            FP strength = FP.FromString("0.5");
            FP duration = FP.FromString("0.4");
            int vibrato = 45;
            if (playerTrigger == PlayerFSM.PlayerTrigger.BlockHigh || playerTrigger ==  PlayerFSM.PlayerTrigger.BlockLow)
            {
                strength = FP.FromString("0.35"); 
                duration = FP.FromString("0.2");
                vibrato = 40;
            }
            
            f.Events.EntityVibrate(EntityRef, strength, duration, vibrato);
        }

        public void StartSlowdown(Frame f, int duration, FP multiplier)
        {
            f.Unsafe.TryGetPointer<SlowdownData>(EntityRef, out var slowdownData);
            slowdownData->slowdownRemaining = duration;
            slowdownData->multiplier = multiplier;
        }
        
        protected void EndSlowdown(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<SlowdownData>(EntityRef, out var slowdownData);
            slowdownData->slowdownRemaining = 0;
        }
        
    }
}