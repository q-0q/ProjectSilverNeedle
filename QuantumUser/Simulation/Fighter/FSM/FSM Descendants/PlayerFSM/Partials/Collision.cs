using System;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe partial class PlayerFSM

    {

        public const int CrossupProtectionDuration = 4;
        public const int ThrowProtectionDuration = 6;
        
        protected override void InvokeHitboxHurtboxCollision(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData, FPVector2 location)
        {
            if (hitboxData.HitType != Hit.HitType.Throw)
            {
                InvokeNonThrowCollision(f, hurtboxData, hitboxData, location);
            }
            else
            {
                if (GetFramesSinceThrowProtectionStart(f) < ThrowProtectionDuration) return;
                if (Fsm.IsInState(PlayerState.Air) || Fsm.IsInState(PlayerState.Backdash) ||
                    Fsm.IsInState(PlayerState.Hit) || Fsm.IsInState(PlayerState.Block)) return;
            }
            
            HandleCutsceneTrigger(f, hurtboxData, hitboxData);
        }

        private bool IsProjectileInvulnerable(Frame f)
        {
            var sectionGroup = StateMapConfig.ProjectileInvulnerable?.Get(this, new FrameParam() { f = f, EntityRef = EntityRef });
            if (sectionGroup is null) return false;
            return sectionGroup.GetCurrentItem(f, this);
        }

        private void InvokeNonThrowCollision(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData,
            FPVector2 location)
        {
            Hit.HitType hitType = hitboxData.HitType;
            HurtType hurtType = hurtboxData.HurtType;
            var isBlocking = IsBlockingHitType(f, hitType);
            var trigger = GetCollisionTrigger(f, hitType, isBlocking);
            
            InvokeCollisionVibrate(f, trigger);

            if (IsProjectileInvulnerable(f) && hitboxData.projectile)
            {
                HitstopSystem.EnqueueHitstop(f, 12);
                return;
            }

            var xVelocity = hitboxData.trajectoryXVelocity;
            if (!IsFacingRight(f, hitboxData.source))
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

            
            FP pushbackDistance;
            if (Fsm.IsInState(PlayerState.Ground) || Util.IsPlayerInCorner(f, EntityRef)) {
                pushbackDistance = isBlocking ? hitboxData.blockPushback : hitboxData.hitPushback;
            }
            else
            {
                // universal air pushback
                pushbackDistance = isBlocking ? 2 : 1;
            }
            if (IsFacingRight(f, EntityRef)) pushbackDistance *= FP.Minus_1;
            StartPushback(f, pushbackDistance);
            
            if (isBlocking)
            {
                var stun = Fsm.IsInState(PlayerState.Air)
                    ? Hit.AttackLevelAirBlockstun[hitboxData.level] + hitboxData.bonusBlockStun
                    : Hit.AttackLevelGroundBlockstun[hitboxData.level] + hitboxData.bonusBlockStun;
                var stop = Hit.AttackLevelHitstop[hitboxData.level];
                InvokeStun(f, stun);
                HitstopSystem.EnqueueHitstop(f, stop);
                
                AnimationEntitySystem.Create(f, AnimationEntities.AnimationEntityEnum.Block, location, hitboxData.visualAngle, 
                    !IsFacingRight(f, hitboxData.source));
            }
            else
            {
                InvokeDamagingCollisionCore(f, hurtboxData, hitboxData, hurtType, location);
            }
            
            Fsm.Fire(trigger, juggleParam);
        }
        
        private void StartPushback(Frame f, FP totalDistance)
        {
            f.Unsafe.TryGetPointer<PushbackData>(EntityRef, out var pushbackData);
            pushbackData->framesInPushback = 0;
            pushbackData->pushbackAmount = totalDistance;
        }

        private void InvokeDamagingCollisionCore(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData,
            HurtType hurtType, FPVector2 location)
        {
            
            EndSlowdown(new FrameParam() { f = f, EntityRef = EntityRef});
            var animationEntityEnum = hurtType is HurtType.Counter
                ? AnimationEntities.AnimationEntityEnum.Counter
                : AnimationEntities.AnimationEntityEnum.Hit;
            AnimationEntitySystem.Create(f, animationEntityEnum, location, hitboxData.visualAngle, 
                !IsFacingRight(f, hitboxData.source));

            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->hardKnockdown = hitboxData.hardKnockdown;

            f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);

            
            
            
            var rawDamage = hurtType is HurtType.Counter ? hitboxData.damage * CounterHitDamageMultiplier : hitboxData.damage;
            var rawDamageScaling = hurtType is HurtType.Counter ? CounterHitDamageScaling : hitboxData.damageScaling;
            healthData->health -= (rawDamage * comboData->damageScaling * GlobalDamageModifier);
            comboData->damageScaling *= rawDamageScaling;
            
            
            var d = f.ResolveDictionary(comboData->hitCounts);
            int hitTableId = hitboxData.lookupId;
            Debug.Log(hitTableId);
            d.TryAdd(hitTableId, 0);
            var hitGravityScaling = d[hitTableId] == 0 ? hitboxData.gravityScaling : Util.Pow(hitboxData.gravityProration, d[hitTableId]);
            var rawGravityScaling =  hurtType is HurtType.Counter ? hitGravityScaling * CounterHitGravityScalingMultiplier : hitGravityScaling;
            comboData->gravityScaling *= rawGravityScaling;
            Debug.Log("hit scaling: " + hitGravityScaling);
            comboData->length++;
            d[hitTableId] += 1;
            
            
            var stun = Fsm.IsInState(PlayerState.Crouch)
                ? Hit.AttackLevelCrouchHitstun[hitboxData.level] + hitboxData.bonusHitStun
                : Hit.AttackLevelStandHitstun[hitboxData.level] + hitboxData.bonusHitStun;
            
            var stop = Hit.AttackLevelHitstop[hitboxData.level];
            
            
            if (hurtType == HurtType.Counter)
            {
                f.Events.GameEvent(EntityRef, GameEventType.Counter);
                Util.StartDramatic(f, EntityRef, 13);
                InputSystem.ClearBuffer(f, Util.GetOtherPlayer(f, EntityRef));
                stop = AttackLevelCounterHitstop[hitboxData.level];
                
                if (Fsm.IsInState(PlayerFSM.PlayerState.Air) || hitboxData.launches)
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
            
            var hurtboxPlayerFsm = FsmLoader.FSMs[hurtboxInternal.source];
            var hitboxPlayerFsm = FsmLoader.FSMs[hitboxInternal.source];

            Cutscene cutscene;
            
            try
            {
                cutscene = Cutscenes[cutsceneIndex];
            }
            catch (Exception)
            {
                Debug.LogError("You tried to trigger a cutscene index that has no cutscene mapped");
                throw;
            }

            // Util.StartDramatic(f, EntityRef, 30);

            bool actionable = hurtboxPlayerFsm.Fsm.IsInState(PlayerState.GroundActionable) ||
                            hurtboxPlayerFsm.Fsm.IsInState(PlayerState.AirActionable);
            
            
            var hurtboxParam = new FrameParam() { f = f, EntityRef = hurtboxInternal.source };
            var hitboxParam = new FrameParam() { f = f, EntityRef = hitboxInternal.source };
            
            // handle throw vs throw startup collision auto-tech (ie, early tech)
            if ((actionable || hurtboxPlayerFsm.Fsm.IsInState(PlayerState.Throw)) &&
                (hurtboxPlayerFsm.FramesInCurrentState(f) <= ThrowStartupDuration))
            {
                Debug.Log(hurtboxPlayerFsm.FramesInCurrentState(f));
                hurtboxPlayerFsm.Fsm.Jump(PlayerState.Tech, hurtboxParam);
                hitboxPlayerFsm.Fsm.Jump(PlayerState.Tech, hitboxParam);
                return;
            }

            hurtboxPlayerFsm.Fsm.Jump(actionable? PlayerState.TechableCutsceneReactor : PlayerState.CutsceneReactor, hurtboxParam );
            hitboxPlayerFsm.Fsm.Jump(cutscene.InitiatorState, hitboxParam );

            
            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->hardKnockdown = cutscene.HardKnockdown;
            
            f.Unsafe.TryGetPointer<CutsceneData>(hurtboxInternal.source, out var cutsceneData);
            cutsceneData->initiator = hitboxInternal.source;
            cutsceneData->cutsceneIndex = cutsceneIndex;
            cutsceneData->initiatorFacingRight = IsFacingRight(f, hitboxInternal.source);
            
            // Let's remember this, hopefully it wont give any weird issues.
            // Util.WritebackFsm(f, hitboxInternal.source);
        }


        private void InvokePlayerDeath(Frame f)
        {
            GameFsmLoader.LoadGameFSM(f).Fsm.Fire(GameFSM.Trigger.PlayerDeath, new FrameParam() { f = f, EntityRef = EntityRef});
            Util.StartDramatic(f, EntityRef, 120);
            int slowdownDuration = 80;
            FP slowdownAmount = FP.FromString("0.5");
            StartSlowdown(f, slowdownDuration, slowdownAmount);
            Fsm.Fire(PlayerFSM.PlayerTrigger.Die, new FrameParam(){ f= f, EntityRef = EntityRef});
            
            Util.IncrementScore(f, Util.GetOtherPlayer(f, EntityRef));
        }
        

        private void InvokeStun(Frame f, int amount)
        {
            f.Unsafe.TryGetPointer<StunData>(EntityRef, out var stunData);
            stunData->stun = amount;
        }
        
        
        private int GetCollisionTrigger(Frame f, Hit.HitType hitType, bool isBlocking)
        {
            int playerTrigger = PlayerFSM.PlayerTrigger.NeutralInput;

            if (hitType is Hit.HitType.Throw) return playerTrigger;
            
            // Block
            if (isBlocking && hitType is Hit.HitType.High)
            {
                playerTrigger = PlayerFSM.PlayerTrigger.BlockHigh;
            }
            else if (isBlocking && hitType is Hit.HitType.Mid)
            {
                var numpad = InputSystem.Numpad(f, EntityRef);
                playerTrigger = numpad == 1 ? PlayerFSM.PlayerTrigger.BlockLow : PlayerFSM.PlayerTrigger.BlockHigh;
            }
            else if (isBlocking && hitType is Hit.HitType.Low)
            {
                playerTrigger = PlayerFSM.PlayerTrigger.BlockLow;
            }
            
            // Hit
            else if (hitType == Hit.HitType.High)
            {
                playerTrigger = PlayerFSM.PlayerTrigger.HitHigh;
            }
            else if (hitType == Hit.HitType.Mid)
            {
                playerTrigger = PlayerFSM.PlayerTrigger.HitHigh;
            }
            else if (hitType == Hit.HitType.Low)
            {
                playerTrigger = PlayerFSM.PlayerTrigger.HitLow;
            }

            return playerTrigger;
        }

        private bool IsBlockingHitType(Frame f, Hit.HitType type)
        {
            if (type is Hit.HitType.Throw) return false;
            
            if (!Fsm.IsInState(PlayerState.AirActionable) && !Fsm.IsInState(PlayerState.GroundActionable) && 
                !Fsm.IsInState(PlayerState.Block) && !Fsm.IsInState(PlayerState.Landsquat) && 
                !Fsm.IsInState(PlayerState.Dash) && !Fsm.IsInState(PlayerState.AirDash)) return false;
            
            if (Util.EntityIsCpu(f, EntityRef))
            {
                return Util.GetCpuControllerData(f)->block;
            }
            
            var numpad = InputSystem.Numpad(f, EntityRef);

            if (GetFramesSinceCrossupProtectionStart(f) < CrossupProtectionDuration)
            {
                if (Fsm.IsInState(PlayerState.Air)) return (numpad is not (2 or 5 or 8)) && (GetFramesInTrajectory(f) > NumNonBlockingJumpFrames);
                if (type == Hit.HitType.High) return numpad is 4 or 7 or 6 or 9;
                if (type == Hit.HitType.Mid) return numpad is not (2 or 5 or 8);
                if (type == Hit.HitType.Low) return numpad is 1 or 3;

                return false;
            }

            // todo: jumpsquat
            if (Fsm.IsInState(PlayerState.Air)) return (numpad is 1 or 4 or 7) && (GetFramesInTrajectory(f) > NumNonBlockingJumpFrames);
            if (type == Hit.HitType.High) return numpad is 4 or 7;
            if (type == Hit.HitType.Mid) return numpad is 1 or 4 or 7;
            if (type == Hit.HitType.Low) return numpad is 1;
            
            return false; 
        }


        public int GetFramesSinceThrowProtectionStart(Frame f)
        {
            f.Unsafe.TryGetPointer<ProtectionData>(EntityRef, out var protectionData);
            return Util.FramesFromVirtualTime(protectionData->virtualTimeSinceThrowProtectionStart);
        }
        
        public int GetFramesSinceCrossupProtectionStart(Frame f)
        {
            f.Unsafe.TryGetPointer<ProtectionData>(EntityRef, out var protectionData);
            return Util.FramesFromVirtualTime(protectionData->virtualTimeSinceCrossupProtectionStart);
        }

    }
}