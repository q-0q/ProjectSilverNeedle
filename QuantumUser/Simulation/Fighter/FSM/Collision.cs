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
        
            public int level;
            public int bonusHitStun;
            public int bonusBlockStun;
            public FP blockPushback;
            public FP hitPushback;
        
            public FP visualAngle;
            public FP trajectoryHeight;
            public FP trajectoryXVelocity;
            public FP gravityScaling;
            public FP damageScaling;
            public FP damage;
            public bool launches;
            public bool hardKnockdown;
            public bool groundBounce;
            public bool wallBounce;

            public int cutsceneIndex;


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
        public static FP CounterHitComboScaling = FP.FromString("1.3");
        public static FP CounterHitGravityScalingMultiplier = FP.FromString("0.95");
        public static FP GlobalDamageModifier = FP.FromString("0.8");


        public static int CounterBonusHitstop = 10;
        public static int PunishBonusHitstop = 0;
        
        public static List<CollisionBoxInternal> GetCollisionBoxInternalsOfType(Frame f, EntityRef source, CollisionBoxType type)
        {
            f.Unsafe.TryGetPointer<FSMData>(source, out var fsmData);
            int collisionState = fsmData->currentCollisionState;
            int collisionStateFrames = fsmData->collisionFramesInState;
            
            Character character = Characters.GetPlayerCharacter(f, source);
            PlayerFSM playerFsm = Util.GetPlayerFSM(f, source);
            
            
            if (type == CollisionBoxType.Pushbox)
            {
                var pushBox = character.Pushbox.Lookup(collisionState, playerFsm);

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
                if (character.InvulnerableBefore.Lookup(collisionState, playerFsm) > collisionStateFrames)
                    return new List<CollisionBoxInternal>();
                var hurtTypeSectionGroup = character.HurtTypeSectionGroup.Lookup(collisionState, playerFsm);
                var hurtType = HurtType.Regular;
                if (hurtTypeSectionGroup is not null)
                    hurtType = hurtTypeSectionGroup.GetItemFromIndex(collisionStateFrames);
                var hurtBoxCollectionSectionGroup = character.HurtboxCollectionSectionGroup.Lookup(collisionState, playerFsm);
                if (hurtBoxCollectionSectionGroup is null) return new List<CollisionBoxInternal>();
                var hurtboxCollection = hurtBoxCollectionSectionGroup.GetItemFromIndex(collisionStateFrames);
                if (hurtboxCollection is null) return new List<CollisionBoxInternal>();


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
                        pos = GetCollisionBoxWorldPosition(f, source, hurtbox).XY
                    };

                    hurtboxInternals.Add(_internal);
                }

                return hurtboxInternals;

            }

            if (type == CollisionBoxType.Hitbox)
            {
                var hitSectionGroup = character.HitSectionGroup.Lookup(collisionState, playerFsm);
                if (hitSectionGroup is null) return new List<CollisionBoxInternal>();

                var hit = hitSectionGroup.GetItemFromIndex(collisionStateFrames);
                if (hit is null) return new List<CollisionBoxInternal>();

                var firstFrame = hitSectionGroup.GetFirstFrameFromIndex(collisionStateFrames);

                var hitType = hit.Type;
                var hitboxCollection = hit.HitboxCollections.GetItemFromIndex(collisionStateFrames - firstFrame);
                
                
                var hitboxInternals = new List<CollisionBoxInternal>();
                foreach (var hurtbox in hitboxCollection.CollisionBoxes)
                {
                    var _internal = new CollisionBoxInternal()
                    {
                        source = source,
                        type = CollisionBoxType.Hitbox,
                        HitType = hitType,
                        width = hurtbox.Width,
                        height = hurtbox.Height,
                        pos = GetCollisionBoxWorldPosition(f, source, hurtbox).XY,
                        
                        level = hit.Level,
                        bonusHitStun = hit.BonusHitstun,
                        bonusBlockStun = hit.BonusBlockstun,
                        blockPushback = hit.BlockPushback,
                        hitPushback = hit.HitPushback,
                        visualAngle = hit.VisualAngle,
                        trajectoryHeight = hit.TrajectoryHeight,
                        trajectoryXVelocity = hit.TrajectoryXVelocity,
                        gravityScaling = hit.GravityScaling,
                        damageScaling = hit.DamageScaling,
                        damage = hit.Damage,
                        launches = hit.Launches,
                        hardKnockdown = hit.HardKnockdown,
                        groundBounce = hit.GroundBounce,
                        wallBounce = hit.WallBounce,
                        
                        cutsceneIndex = hit.TriggerCutscene
                    };
                    
                    hitboxInternals.Add(_internal);
                }
                
                return hitboxInternals;
            }
            
            return null;
        }

        
        public void HitboxHurtboxCollide(Frame f)
        {

            var hurtboxInternals = GetCollisionBoxInternalsOfType(f, EntityRef, CollisionBoxType.Hurtbox);
            
            // todo:
            // we need a way of comprehensively getting hitboxes from ALL other FSMs, not just the opponent
            // otherwise collision will never happen with Summons
            // Util.GetAllOtherFSMs() -> list of entity refs
            var hitboxInternals = GetCollisionBoxInternalsOfType(f, Util.GetOtherPlayer(f, EntityRef),
                CollisionBoxType.Hitbox);
            
            
            
            foreach (var hitboxInternal in hitboxInternals)
            {
                foreach (var hurtboxInternal in hurtboxInternals)
                {
                    if (!CollisionBoxesOverlap(f, hitboxInternal, hurtboxInternal, out var overlapCenter, out var overlapWidth)) continue;
                    if (!CanBeHitBySource(f, hitboxInternal.source)) continue;
                    
                    AddMeToSourceHitList(f, hitboxInternal.source);
                    InvokeHitboxHurtboxCollision(f, hurtboxInternal, hitboxInternal, overlapCenter);
                    
                }
            }
        }

        protected abstract void InvokeHitboxHurtboxCollision(Frame frame, CollisionBoxInternal hurtboxInternal, CollisionBoxInternal hitboxInternal, FPVector2 overlapCenter);


        private static FPVector3 GetCollisionBoxWorldPosition(Frame f, EntityRef source, CollisionBox collisionBox)
        {
            FP growOffsetX = collisionBox.GrowWidth ? collisionBox.Width * FP._0_50 : 0;
            FP growOffsetY = collisionBox.GrowHeight ? collisionBox.Height * FP._0_50 : 0;
            FP flipXMod = PlayerDirectionSystem.IsFacingRight(f, source) ? FP._1 : FP.Minus_1;
                    
            FPVector3 posOffset = new FPVector3(collisionBox.PosX, collisionBox.PosY, 0);
            FPVector3 growOffset = new FPVector3(growOffsetX, growOffsetY, 0);
            
            FPVector3 offset = (posOffset + growOffset);
            FPVector3 offsetFlipped = new FPVector3(offset.X * flipXMod, offset.Y, 0);
                    
            
            f.Unsafe.TryGetPointer<Transform3D>(source, out var sourceTransform);
            return sourceTransform->Position + offsetFlipped;
        }
        
        
        private bool CanBeHitBySource(Frame f, EntityRef source)
        {
            f.Unsafe.TryGetPointer<HitEntitiesTracker>(source, out var hitEntitiesTracker);
            var hitEntities = f.ResolveList(hitEntitiesTracker->HitEntities);
            foreach (var hitEntity in hitEntities)
            {
                if (hitEntity == EntityRef) return false;
            }
            return true;
        }

        private void AddMeToSourceHitList(Frame f, EntityRef source)
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
            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            SectionGroup<Hit> hitSectionGroup = character.HitSectionGroup.Get(this);
            return hitSectionGroup;
        }

        public void ClearHitEntities(Frame f)
        {
            f.Unsafe.TryGetPointer<HitEntitiesTracker>(EntityRef, out var hitEntitiesTracker);
            var hitEntities = f.ResolveList(hitEntitiesTracker->HitEntities);
            hitEntities.Clear();
        }
        
        static bool CollisionBoxesOverlap(Frame f, CollisionBoxInternal boxAInternal, CollisionBoxInternal boxBInternal, out FPVector2 overlapCenter, out FP overlapWidth)
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
            FP strength = FP.FromString("0.4");
            FP duration = FP.FromString("0.4");
            int vibrato = 45;
            if (playerTrigger == PlayerFSM.PlayerTrigger.BlockHigh || playerTrigger ==  PlayerFSM.PlayerTrigger.BlockLow)
            {
                strength = FP.FromString("0.2"); 
                duration = FP.FromString("0.2");
                vibrato = 35;
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