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
    public unsafe partial class PlayerFSM
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
        

        public static FP ThrowDistance = FP.FromString("2");
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
        
        
        // Todo:
        // remove this function. instead, add a hit to the throw
        // startup state in PlayerFSM.ConfigureBaseFsm().
        
        // further, we will want to generalize CollisionBoxType.Throwbox into
        // CollisionBoxType.Kinematic. The goal is to create a hitbox type that can
        // also be used by character-specific hits to trigger command grabs, supers,
        // parries, or any other "micro cutscene" style interactions... :)
        
        // Maybe Kinematic should be renamed to Cutscene?
        // Break "KinematicReciever" state into multiple states that can be used
        // to create a movie with code.... lol wtf
        // public void Throwbox(Frame f)
        // {
        //     ClearCollisionBoxesOfType(f, CollisionBox.CollisionBoxType.Throwbox, EntityRef);
        //     
        //     if (!Fsm.IsInState(State.ThrowStartup)) return;
        //     if (FramesInCurrentState(f) != ThrowStartupFrames) return;
        //     
        //     var throwbox = new CollisionBox()
        //     {
        //         GrowHeight = true,
        //         GrowWidth = false,
        //         Width = 2 * ThrowDistance,
        //         Height = 5,
        //     };
        //
        //     CollisionBoxCollection collisionBoxCollection = new CollisionBoxCollection()
        //     {
        //         CollisionBoxes = new List<CollisionBox>() { throwbox }
        //     };
        //     
        //     RenderCollisionBoxCollection(f, collisionBoxCollection, CollisionBox.CollisionBoxType.Throwbox, EntityRef,
        //         0, 0, 0, 0,0, 0,1,1,0);
        // }

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

        // public void ThrowboxCollide(Frame f)
        // {
        //     FrameParam frameParam = new FrameParam() { f = f, EntityRef = EntityRef };
        //     if (EntityIsHitByThrowbox(f, EntityRef, Util.GetOtherPlayer(f, EntityRef)))
        //     {
        //         var hurtType = GetHurtType(f);
        //         if ((hurtType is HurtType.Counter or HurtType.Punish) && 
        //             ! (Fsm.IsInState(State.ThrowConnect) || Fsm.IsInState(State.ThrowStartup)))
        //         {
        //             f.Events.GameEvent(EntityRef, GameEventType.Punish);
        //         }
        //
        //         f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
        //         trajectoryData->hardKnockdown = true;
        //         
        //         Fsm.Fire(Trigger.ReceiveKinematics, frameParam);
        //     }
        //     if (EntityIsHitByThrowbox(f, Util.GetOtherPlayer(f, EntityRef), EntityRef))
        //     {
        //         Fsm.Fire(Trigger.ThrowConnect, frameParam);
        //     }
        // }

        // private bool EntityIsHitByThrowbox(Frame f, EntityRef targetEntityRef, EntityRef sourceEntityRef)
        // {
        //     foreach (var (throwboxEntityRef, throwboxData) in f.GetComponentIterator<CollisionBoxInternal>())
        //     {
        //         if (throwboxData.type != (int) CollisionBox.CollisionBoxType.Throwbox) continue;
        //         if (throwboxData.source != sourceEntityRef) continue;
        //         
        //
        //         foreach (var (hurtboxEntityRef, hurtboxData) in f.GetComponentIterator<CollisionBoxInternal>())
        //         {
        //             if (hurtboxData.type != (int)CollisionBox.CollisionBoxType.Hurtbox) continue;
        //             if (hurtboxData.source != targetEntityRef) continue;
        //             if (!CollisionBoxesOverlap(f, throwboxEntityRef, 
        //                     hurtboxEntityRef, out var overlapCenter, out var overlapWidth)) continue;
        //             
        //             var targetFsm = Util.GetPlayerFSM(f, targetEntityRef);
        //             if (targetFsm is null) return false;
        //             
        //             if (targetFsm.Fsm.IsInState(State.Air)) return false;
        //             if (targetFsm.Fsm.IsInState(State.Hit)) return false;
        //             if (targetFsm.Fsm.IsInState(State.Block)) return false;
        //             if (targetFsm.Fsm.IsInState(State.Backdash)) return false;
        //             
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }
        //
        

        


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
        
        
        
        private HurtType GetHurtType(Frame f)
        {
            if (Util.EntityIsCpu(f, EntityRef))
            {
                if (Util.GetCpuControllerData(f)->forceCounter &&
                    (Fsm.IsInState(State.GroundActionable) || Fsm.IsInState(State.AirActionable))) 
                    return HurtType.Counter;
            }
            
            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            var hurtTypeSectionGroup = character.HurtTypeSectionGroup.Get(this);
            return hurtTypeSectionGroup?.GetCurrentItem(f, this) ?? HurtType.Regular;
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


        private void InvokeHitboxHurtboxCollision(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData, FPVector2 location)
        {
            if (hitboxData.HitType != Hit.HitType.Throw)
            {
                InvokeNonThrowCollision(f, hurtboxData, hitboxData, location);
            }
            else
            {
                if (Fsm.IsInState(State.Air) || Fsm.IsInState(State.Backdash) ||
                    Fsm.IsInState(State.Hit) || Fsm.IsInState(State.Block)) return;
            }
            
            HandleCutsceneTrigger(f, hurtboxData, hitboxData);
        }

        private void InvokeNonThrowCollision(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData,
            FPVector2 location)
        {
            Hit.HitType hitType = hitboxData.HitType;
            HurtType hurtType = hurtboxData.HurtType;
            var isBlocking = IsBlockingHitType(f, hitType);
            var trigger = GetCollisionTrigger(f, hitType, isBlocking);
            
            InvokeCollisionVibrate(f, trigger);

            var xVelocity = hitboxData.trajectoryXVelocity;
            if (!PlayerDirectionSystem.IsFacingRight(f, hitboxData.source))
            {
                xVelocity *= FP.Minus_1;
            }
            
            FrameParam juggleParam = new CollisionHitParams(){f = f, EntityRef = EntityRef, 
                XVelocity = Util.FrameLengthInSeconds * xVelocity, 
                TrajectoryHeight = hitboxData.trajectoryHeight,
                Launches = hitboxData.launches,
                GroundBounces = hitboxData.groundBounce,
                WallBounces = hitboxData.wallBounce
            };
            
            MakeNotWhiffed(f, hitboxData.source);
            Fsm.Fire(trigger, juggleParam);

            //Fsm.IsInState(State.Ground) || Util.IsPlayerInCorner(f, EntityRef)
            if (true) {
                FP pushback = isBlocking ? hitboxData.blockPushback : hitboxData.hitPushback;
                FP pushbackDistance = pushback;
                if (PlayerDirectionSystem.IsFacingRight(f, EntityRef)) pushbackDistance *= FP.Minus_1;
                StartPushback(f, pushbackDistance);
            }
            
            if (isBlocking)
            {
                var stun = Fsm.IsInState(State.Air)
                    ? Hit.AttackLevelAirBlockstun[hitboxData.level]
                    : Hit.AttackLevelGroundBlockstun[hitboxData.level];
                var stop = Hit.AttackLevelHitstop[hitboxData.level];
                InvokeStun(f, stun);
                HitstopSystem.EnqueueHitstop(f, stop);
                
                AnimationEntitySystem.Create(f, AnimationEntities.AnimationEntityEnum.Block, location, hitboxData.visualAngle, 
                    !PlayerDirectionSystem.IsFacingRight(f, hitboxData.source));
            }
            else
            {
                InvokeDamagingCollisionCore(f, hurtboxData, hitboxData, hurtType, location);
            }
        }

        private void InvokeDamagingCollisionCore(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData,
            HurtType hurtType, FPVector2 location)
        {
            
            EndSlowdown(new FrameParam() { f = f, EntityRef = EntityRef});
            var animationEntityEnum = hurtType is HurtType.Counter
                ? AnimationEntities.AnimationEntityEnum.Counter
                : AnimationEntities.AnimationEntityEnum.Hit;
            AnimationEntitySystem.Create(f, animationEntityEnum, location, hitboxData.visualAngle, 
                !PlayerDirectionSystem.IsFacingRight(f, hitboxData.source));

            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->hardKnockdown = hitboxData.hardKnockdown;

            f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);

            var damage = hurtType is HurtType.Counter ? hitboxData.damage * CounterHitDamageMultiplier : hitboxData.damage;
            var damageScaling = hurtType is HurtType.Counter ? CounterHitComboScaling : hitboxData.damageScaling;
            var gravityScaling =  hurtType is HurtType.Counter ? hitboxData.gravityScaling * CounterHitGravityScalingMultiplier : hitboxData.gravityScaling;
            healthData->health -= (damage * comboData->damageScaling * GlobalDamageModifier);
            
            IncrementCombo(f, gravityScaling, damageScaling);
            
            
            var stun = Fsm.IsInState(State.Crouch)
                ? Hit.AttackLevelCrouchHitstun[hitboxData.level]
                : Hit.AttackLevelStandHitstun[hitboxData.level];
            
            var stop = Hit.AttackLevelHitstop[hitboxData.level];
            
            
            if (hurtType == HurtType.Counter)
            {
                f.Events.GameEvent(EntityRef, GameEventType.Counter);
                Util.StartDramatic(f, EntityRef, 13);
                InputSystem.ClearBuffer(f, Util.GetOtherPlayer(f, EntityRef));
                stop = AttackLevelCounterHitstop[hitboxData.level];
                
                if (Fsm.IsInState(State.Air))
                {
                    StartSlowdown(f, AttackLevelCounterSlowdownDuration[hitboxData.level], CounterSlowdownMultiplier);
                }
                else
                {
                    stun += AttackLeveCounterBonusHitstun[hitboxData.level];
                }
                
            }
            else if (hurtType == HurtType.Punish)
            {
                f.Events.GameEvent(EntityRef, GameEventType.Punish);
            }
            
            InvokeStun(f, stun);
            HitstopSystem.EnqueueHitstop(f, stop);
            
            if (healthData->health <= 0) InvokePlayerDeath(f);
        }

        private void HandleCutsceneTrigger(Frame f, CollisionBoxInternal hurtboxInternal, CollisionBoxInternal hitboxInternal)
        {
            var cutsceneIndex = hitboxInternal.cutsceneIndex;
            if (cutsceneIndex == -1) return;

            var border = "=====================================================================================";
            Debug.Log(border);
            Debug.Log(border);
            Debug.Log("Cutscene " + cutsceneIndex + " triggered frame " + f.Number);
            
            var hurtboxPlayerFsm = Util.GetPlayerFSM(f, hurtboxInternal.source);
            var hitboxPlayerFsm = Util.GetPlayerFSM(f, hitboxInternal.source);

            Cutscene cutscene;
            
            try
            {
                cutscene = Characters.GetPlayerCharacter(f, hitboxInternal.source).Cutscenes[cutsceneIndex];
            }
            catch (Exception)
            {
                Debug.LogError("You tried to trigger a cutscene index that has no cutscene mapped");
                throw;
            }

            // Util.StartDramatic(f, EntityRef, 30);
            hurtboxPlayerFsm.Fsm.Jump(State.CutsceneReactor, new FrameParam() { f = f, EntityRef = hurtboxInternal.source } );
            hitboxPlayerFsm.Fsm.Jump(cutscene.InitiatorState, new FrameParam() { f = f, EntityRef = hitboxInternal.source } );

            
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->hardKnockdown = cutscene.HardKnockdown;
            
            f.Unsafe.TryGetPointer<CutsceneData>(hurtboxInternal.source, out var cutsceneData);
            cutsceneData->initiator = hitboxInternal.source;
            cutsceneData->cutsceneIndex = cutsceneIndex;
            cutsceneData->initiatorFacingRight = PlayerDirectionSystem.IsFacingRight(f, hitboxInternal.source);
            
            // Let's remember this, hopefully it wont give any weird issues.
            Util.WritebackFsm(f, hitboxInternal.source);
        }


        private void InvokePlayerDeath(Frame f)
        {
            GameFSMSystem.FireWriteGameFsm(f, GameFSM.Trigger.PlayerDeath);
            Util.StartDramatic(f, EntityRef, 120);
            int slowdownDuration = 80;
            FP slowdownAmount = FP.FromString("0.5");
            StartSlowdown(f, slowdownDuration, slowdownAmount);
            Fsm.Fire(Trigger.Die, new FrameParam(){ f= f, EntityRef = EntityRef});
            
            Util.IncrementScore(f, Util.GetOtherPlayer(f, EntityRef));
        }
        

        private void InvokeStun(Frame f, int amount)
        {
            f.Unsafe.TryGetPointer<StunData>(EntityRef, out var stunData);
            stunData->stun = amount;
        }

        private void InvokeCollisionVibrate(Frame f, Trigger trigger)
        {
            FP strength = FP.FromString("0.4");
            FP duration = FP.FromString("0.4");
            int vibrato = 45;
            if (trigger is Trigger.BlockHigh or Trigger.BlockLow)
            {
                strength = FP.FromString("0.2"); 
                duration = FP.FromString("0.2");
                vibrato = 35;
            }
            
            f.Events.EntityVibrate(EntityRef, strength, duration, vibrato);
        }

        private Trigger GetCollisionTrigger(Frame f, Hit.HitType hitType, bool isBlocking)
        {
            Trigger trigger = Trigger.NeutralInput;

            if (hitType is Hit.HitType.Throw) return trigger;
            
            // Block
            if (isBlocking && hitType is Hit.HitType.High)
            {
                trigger = Trigger.BlockHigh;
            }
            else if (isBlocking && hitType is Hit.HitType.Mid)
            {
                var numpad = InputSystem.Numpad(f, EntityRef);
                trigger = numpad == 1 ? Trigger.BlockLow : Trigger.BlockHigh;
            }
            else if (isBlocking && hitType is Hit.HitType.Low)
            {
                trigger = Trigger.BlockLow;
            }
            
            // Hit
            else if (hitType == Hit.HitType.High)
            {
                trigger = Trigger.HitHigh;
            }
            else if (hitType == Hit.HitType.Mid)
            {
                trigger = Trigger.HitHigh;
            }
            else if (hitType == Hit.HitType.Low)
            {
                trigger = Trigger.HitLow;
            }

            return trigger;
        }

        private bool IsBlockingHitType(Frame f, Hit.HitType type)
        {
            if (type is Hit.HitType.Throw) return false;
            
            if (!Fsm.IsInState(State.AirActionable) && !Fsm.IsInState(State.GroundActionable) && 
                !Fsm.IsInState(State.Block) && !Fsm.IsInState(State.Landsquat) && 
                !Fsm.IsInState(State.Dash) && !Fsm.IsInState(State.AirDash)) return false;
            
            if (Util.EntityIsCpu(f, EntityRef))
            {
                return Util.GetCpuControllerData(f)->block;
            }
            
            var numpad = InputSystem.Numpad(f, EntityRef);

            if (Fsm.IsInState(State.Air)) return (numpad is 1 or 4 or 7) && (FramesInCurrentState(f) > NumNonBlockingJumpFrames);
            if (type == Hit.HitType.High) return numpad is 4 or 7;
            if (type == Hit.HitType.Mid) return numpad is 1 or 4 or 7;
            if (type == Hit.HitType.Low) return numpad is 1;
            
            return false; 
        }

        private bool IsCollisionHitParamLauncher(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return false;
            var param = (CollisionHitParams)triggerParams;
            return param.Launches || Fsm.IsInState(State.Backdash);
        }

        private bool CanTechThrow(TriggerParams? triggerParams)
        {
            // TODO: need some kind of check to determine whether the kinematicreceiver state
            // came from a throw, otherwise you can tech out of any kinematicreceiver situation
            
            if (triggerParams is null) return false;
            var frameParam = (FrameParam)triggerParams;
            return FramesInCurrentState(frameParam.f) < ThrowTechWindowSize;
        }
        
        public void StartSlowdown(Frame f, int duration, FP multiplier)
        {
            f.Unsafe.TryGetPointer<SlowdownData>(EntityRef, out var slowdownData);
            slowdownData->slowdownRemaining = duration;
            slowdownData->multiplier = multiplier;
        }
        
        private void EndSlowdown(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            frameParam.f.Unsafe.TryGetPointer<SlowdownData>(EntityRef, out var slowdownData);
            slowdownData->slowdownRemaining = 0;
        }
    }
}