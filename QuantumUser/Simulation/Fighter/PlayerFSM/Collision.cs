using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Wasp;

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
        
        protected class CollisionBoxInternal
        {
            public EntityRef source;
            public CollisionBox.CollisionBoxType type;
            public HurtType HurtType;
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
        

        
        
        public void Hitbox(Frame f)
        {
            if (IsOnFirstFrameOfHit(f))
            {
                ClearHitEntities(f);
            }
        }

        
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

        private static List<CollisionBoxInternal> GetCollisionBoxInternalsOfType(Frame f, EntityRef source, CollisionBox.CollisionBoxType type)
        {
            f.Unsafe.TryGetPointer<FSMData>(source, out var fsmData);
            int collisionState = fsmData->currentCollisionState;
            
            Character character = Characters.GetPlayerCharacter(f, source);
            PlayerFSM playerFsm = Util.GetPlayerFSM(f, source);
            
            
            int frames = playerFsm.FramesInCurrentState(f);
            int collisionStateFrames = frames - 1; // This is  stupid
            
            if (type == CollisionBox.CollisionBoxType.Pushbox)
            {
                var pushBox = character.Pushbox.Lookup(collisionStateFrames, playerFsm);
                var pushboxInternal = new CollisionBoxInternal()
                {
                    source = source,
                    type = CollisionBox.CollisionBoxType.Pushbox,
                    width = pushBox.Width,
                    height = pushBox.Height,
                    pos = GetCollisionBoxWorldPosition(f, source, pushBox).XY
                };

                return new List<CollisionBoxInternal>() { pushboxInternal };
            }
            else if (type == CollisionBox.CollisionBoxType.Hurtbox)
            {
                var hurtBoxCollectionSectionGroup = character.HurtboxCollectionSectionGroup.Lookup(collisionStateFrames, playerFsm);
            }
            else if (type == CollisionBox.CollisionBoxType.Hitbox)
            {
                var hitSectionGroup = character.HitSectionGroup.Lookup(collisionStateFrames, playerFsm);
            }
            else if (type == CollisionBox.CollisionBoxType.Throwbox)
            {
                
            }

            return null;



            //var hurtboxCollection = hurtboxCollectionSectionGroup.GetItemFromIndex(collisionStateFrames);
        }

        
        // public void HitboxHurtboxCollide(Frame f)
        // {
        //     
        //     
        //     
        //     foreach (var (hitboxEntityRef, hitboxData) in f.GetComponentIterator<CollisionBoxInternal>())
        //     {
        //         if (hitboxData.source == EntityRef) continue;
        //         if (hitboxData.type != (int)CollisionBox.CollisionBoxType.Hitbox) continue;
        //
        //         foreach (var (hurtboxEntityRef, hurtboxData) in f.GetComponentIterator<CollisionBoxInternal>())
        //         {
        //             if (hurtboxData.source != EntityRef) continue;
        //             if (hurtboxData.type != (int)CollisionBox.CollisionBoxType.Hurtbox) continue;
        //             
        //             
        //             if (!CollisionBoxesOverlap(f, hitboxEntityRef, hurtboxEntityRef, out var overlapCenter, out var overlapWidth)) continue;
        //             if (!CanBeHitBySource(f, hitboxData.source)) continue;
        //             
        //             AddMeToSourceHitList(f, hitboxData.source);
        //             InvokeHitboxHurtboxCollision(f, hurtboxData, hitboxData, overlapCenter);
        //             
        //         }
        //     }
        // }

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
        
        private bool IsOnFirstFrameOfHit(Frame f)
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

        private void ClearHitEntities(Frame f)
        {
            f.Unsafe.TryGetPointer<HitEntitiesTracker>(EntityRef, out var hitEntitiesTracker);
            var hitEntities = f.ResolveList(hitEntitiesTracker->HitEntities);
            hitEntities.Clear();
        }
        
        static bool CollisionBoxesOverlap(Frame f, CollisionBoxInternal boxAInternal, CollisionBoxInternal boxBInternal, out FPVector2 overlapCenter, out FP overlapWidth)
        {
            overlapCenter = FPVector2.Zero; // Or any default value indicating no overlap
            overlapWidth = 0;
            
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

            FPVector2 posA = boxAInternal.pos.XY;
            FPVector2 posB = boxBInternal.pos.XY;
            
            FP aHalfHeight = (boxAInternal.height * FP._0_50);
            FP bHalfHeight = (boxBInternal.height * FP._0_50);

            FP deltaY = Util.Abs(posA.Y - posB.Y);
            

            // Calculate deltaX based on edges
            deltaX = posA.X - posB.X;

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
            Hit.HitType hitType = (Hit.HitType)hitboxData.HurtType;
            HurtType hurtType = GetHurtType(f);
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