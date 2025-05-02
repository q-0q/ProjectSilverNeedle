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
        public static FP MaxThrowDistance = FP.FromString("3.5");
        public static FP OffenseMeterMultiplier = FP.FromString("0.2");
        public static FP DefenseMeterMultiplier = FP.FromString("0.125");
        public static int ClashHitstopBonus = 8;
        
        
        public static FP SurgeHitGravityScalingMod = FP.FromString("1.1");
        public static int SurgeEmpoweredBuffDuration = 35;
        public static int SurgeHitstopBonus = 4;
        public static int SurgeBlockHitstopBonus = 10;
        public static int SurgeMaxStartupReduction = 5;
        public static int SurgeMinimumStartup = 3;
        
        public static int SurgeBlockStun = 3;
        public static int SurgeBlockStunReduction = 3;
        public static int MinimumBlockStunWithSurgeReduction = 2;
        
        protected override void InvokeHitboxHurtboxCollision(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData, FPVector2 location)
        {
            if (hitboxData.HitType != Hit.HitType.Throw)
            {
                InvokeNonThrowCollision(f, hurtboxData, hitboxData, location);
            }
            else
            {
                if (GetXDistance(f,hitboxData.source, hurtboxData.source) > MaxThrowDistance) return;
                if (GetFramesSinceThrowProtectionStart(f) < ThrowProtectionDuration) return;
                if (Fsm.IsInState(PlayerState.Air) || Fsm.IsInState(PlayerState.Backdash) ||
                    Fsm.IsInState(PlayerState.Hit) || Fsm.IsInState(PlayerState.Block)) return;
            }
            
            HandleCutsceneTrigger(f, hurtboxData, hitboxData);
        }
        
        protected override void InvokeClash(Frame f, FSM fsm, CollisionBoxInternal myHitboxInternal, CollisionBoxInternal hitboxData, FPVector2 location)
        {
            if (fsm is not PlayerFSM playerFsm) return;
            
            f.Events.EntityVibrate(fsm.EntityRef, FP.FromString("0.40"), FP.FromString("0.8"), 20);
            playerFsm.MakeNotWhiffed(f, hitboxData.source);
            var stop = Hit.AttackLevelHitstop[hitboxData.level] + ClashHitstopBonus;
            HitstopSystem.EnqueueHitstop(f, stop);
            
            playerFsm.AddMeter(f, myHitboxInternal.damage * OffenseMeterMultiplier);
            Util.StartDramatic(f, EntityRef, 7);
            Util.StartScreenDark(f, EntityRef, 2);
            playerFsm.HandlePushback(f, hitboxData, true, FP.FromString("0.65"));
            
            
            if (Util.GetPlayerId(f, myHitboxInternal.source) != 0) return;
            AnimationEntitySystem.Create(f, AnimationEntities.AnimationEntityEnum.Clash, hitboxData.visualHitPos, hitboxData.visualHitAngle, 
                !IsFacingRight(f, hitboxData.source));
            
        }

        private FPVector2 GetVisualCollisionPosition(Frame f, FPVector2 visualHitPos, EntityRef playerA, EntityRef playerB)
        {
            f.Unsafe.TryGetPointer<Transform3D>(playerA, out var aTransform3D);
            f.Unsafe.TryGetPointer<Transform3D>(playerB, out var bTransform3D);

            var minX = Util.Min(aTransform3D->Position.X, bTransform3D->Position.X);
            var maxX = Util.Max(aTransform3D->Position.X, bTransform3D->Position.X);

            // Clamp to the range first
            var clampedX = Util.Clamp(visualHitPos.X, minX, maxX);

            // Determine the direction toward the center
            var centerX = (minX + maxX) * FP.FromString("0.5");
            var direction = centerX < clampedX ? -1 : 1;

            var padAmount = FP.FromString("0.5"); // Can be parameterized if needed
            var paddedX = clampedX + direction * padAmount;

            // Ensure it still stays within bounds after padding
            paddedX = Util.Clamp(paddedX, minX, maxX);

            return new FPVector2(paddedX, visualHitPos.Y);
        }

        private bool IsProjectileInvulnerable(Frame f)
        {
            var sectionGroup = StateMapConfig.ProjectileInvulnerable?.Get(this, new FrameParam() { f = f, EntityRef = EntityRef });
            if (sectionGroup is null) return false;
            return sectionGroup.GetCurrentItem(f, this);
            
        }

        private FP GetXDistance(Frame f, EntityRef a, EntityRef b)
        {
            f.Unsafe.TryGetPointer<Transform3D>(a, out var transformA);
            f.Unsafe.TryGetPointer<Transform3D>(b, out var transformB);

            var xDistance = Util.Abs(transformA->Position.X - transformB->Position.X);
            return xDistance;
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
            
            if (hurtboxData.highCrush && hitboxData.HitType != Hit.HitType.Low)
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

            
            HandlePushback(f, hitboxData, isBlocking, 1);

            if (isBlocking)
            {
                var stun = Fsm.IsInState(PlayerState.Air)
                    ? Hit.AttackLevelAirBlockstun[hitboxData.level] + hitboxData.bonusBlockStun
                    : Hit.AttackLevelGroundBlockstun[hitboxData.level] + hitboxData.bonusBlockStun;
                var stop = Hit.AttackLevelHitstop[hitboxData.level];
                var empowered = IsEmpowered(f, hitboxData.source);

                if (empowered)
                {
                    // stun = Math.Max(stun - SurgeBlockStunReduction, MinimumBlockStunWithSurgeReduction);
                    stun = SurgeBlockStun;
                    stop += SurgeBlockHitstopBonus;
                    f.Events.EntityVibrate(FsmLoader.FSMs[hitboxData.source].GetPlayer(), FP.FromString("0.5"), FP.FromString("0.5"), 40);
                }
                
                
                InvokeStun(f, stun);
                HitstopSystem.EnqueueHitstop(f, stop);

                var animationEntityEnum = empowered ? AnimationEntities.AnimationEntityEnum.SurgeBlock : AnimationEntities.AnimationEntityEnum.Block;
                AnimationEntitySystem.Create(f, animationEntityEnum, GetVisualCollisionPosition(f, hitboxData.visualHitPos, EntityRef, hitboxData.source), hitboxData.visualHitAngle, 
                    !IsFacingRight(f, hitboxData.source));
                
                AddMeter(f, hitboxData.damage * DefenseMeterMultiplier);
                f.Events.PlayerBlocked(location, hitboxData.visualHitAngle);
                
                EndSlowdown(new FrameParam() { f = f, EntityRef = EntityRef});
            }
            else
            {
                InvokeDamagingCollisionCore(f, hurtboxData, hitboxData, hurtType, location);
            }
            
            Fsm.Fire(trigger, juggleParam);
        }

        private void HandlePushback(Frame f, CollisionBoxInternal hitboxData, bool isBlocking, FP mod)
        {
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
        }

        public void StartPushback(Frame f, FP totalDistance)
        {
            f.Unsafe.TryGetPointer<PushbackData>(EntityRef, out var pushbackData);
            pushbackData->framesInPushback = 0;
            pushbackData->virtualTimeInPushback = 0;
            pushbackData->pushbackAmount = totalDistance;
        }

        public static bool IsEmpowered(Frame f, EntityRef source)
        {
            var entity = FsmLoader.FSMs[source].GetPlayer();
            if (FsmLoader.FSMs[entity] is not PlayerFSM) return false;
            f.Unsafe.TryGetPointer<HealthData>(entity, out var healthData);
            return healthData->nextHitEmpowered;
        }

        private void InvokeDamagingCollisionCore(Frame f, CollisionBoxInternal hurtboxData, CollisionBoxInternal hitboxData,
            HurtType hurtType, FPVector2 location)
        {
            
            EndSlowdown(new FrameParam() { f = f, EntityRef = EntityRef});

            var empowered = IsEmpowered(f, hitboxData.source);
            if (empowered)
            {
                var entityRef = FsmLoader.FSMs[hitboxData.source].GetPlayer();
                f.Unsafe.TryGetPointer<HealthData>(entityRef, out var opponentHealthData);
                Util.StartDramatic(f, EntityRef, 6);
                Util.StartScreenDark(f, EntityRef, 3);
                AnimationEntitySystem.Create(f, AnimationEntities.AnimationEntityEnum.SurgeHit, GetVisualCollisionPosition(f, hitboxData.visualHitPos, EntityRef, hitboxData.source), hitboxData.visualHitAngle, 
                    !IsFacingRight(f, hitboxData.source));
                opponentHealthData->virtualTimeSinceEmpowered = 10;
                opponentHealthData->nextHitEmpowered = false;
            }

            if (!empowered)
            {
                var animationEntityEnum = hurtType is HurtType.Counter
                    ? AnimationEntities.AnimationEntityEnum.Counter
                    : AnimationEntities.AnimationEntityEnum.Hit;
                AnimationEntitySystem.Create(f, animationEntityEnum, GetVisualCollisionPosition(f, hitboxData.visualHitPos, EntityRef, hitboxData.source), hitboxData.visualHitAngle,
                    !IsFacingRight(f, hitboxData.source));
            }
            
            f.Events.PlayerHit(location, hitboxData.visualHitAngle);


            f.Unsafe.TryGetPointer<TrajectoryData>(EntityRef, out var trajectoryData);
            trajectoryData->hardKnockdown = hitboxData.hardKnockdown;

            f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            f.Unsafe.TryGetPointer<ComboData>(EntityRef, out var comboData);
            
            FP damageMod = FsmLoader.FSMs[hitboxData.source].DamageDealtModifier;
            var rawDamage = hurtType is HurtType.Counter ? hitboxData.damage * CounterHitDamageMultiplier : hitboxData.damage;
            var rawDamageScaling = hurtType is HurtType.Counter ? CounterHitDamageScaling : hitboxData.damageScaling;
            healthData->health -= (rawDamage * comboData->damageScaling * GlobalDamageModifier * DamageTakenModifier * damageMod);
            comboData->damageScaling *= rawDamageScaling;
            
            
            var d = f.ResolveDictionary(comboData->hitCounts);
            if (empowered)
            {
                d.Clear();
                comboData->gravityScaling = Util.Min(comboData->gravityScaling, SurgeHitGravityScalingMod);
                comboData->damageScaling = 1;
            }
            
            int hitTableId = hitboxData.lookupId;
            d.TryAdd(hitTableId, 0);
            var hitGravityScaling = d[hitTableId] == 0 ? hitboxData.gravityScaling : Util.Pow(hitboxData.gravityProration, d[hitTableId]);
            var rawGravityScaling =  hurtType is HurtType.Counter ? hitGravityScaling * CounterHitGravityScalingMultiplier : hitGravityScaling;
            // if (empowered) rawGravityScaling *= SurgeHitGravityScalingMod;
            comboData->gravityScaling *= rawGravityScaling;
            comboData->length++;
            d[hitTableId] += 1;
            
            
            var stun = Fsm.IsInState(PlayerState.Crouch)
                ? Hit.AttackLevelCrouchHitstun[hitboxData.level] + hitboxData.bonusHitStun
                : Hit.AttackLevelStandHitstun[hitboxData.level] + hitboxData.bonusHitStun;
            
            var stop = Hit.AttackLevelHitstop[hitboxData.level] + hitboxData.bonusHitStop;
            if (empowered) stop += SurgeHitstopBonus;
            
            
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

            if (FsmLoader.FSMs[hitboxData.source] is PlayerFSM playerFsm)
            {
                playerFsm.AddMeter(f, hitboxData.damage * OffenseMeterMultiplier);
            }


            
            InvokeStun(f, stun);
            HitstopSystem.EnqueueHitstop(f, stop);
            
            if (healthData->health <= 0) InvokePlayerDeath(f);
        }


        private void HandleEmpoweredStartup(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam param) return;
            var f = param.f;
            if (!IsBuffActive(f)) return;

            f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            healthData->nextHitEmpowered = true;
            
            FP virtualTimeIncrement = Util.FrameLengthInSeconds * ActionStartupReduction[Fsm.State()];

            
            base.IncrementClockByAmount(f, EntityRef, virtualTimeIncrement);

        }

        public bool IsBuffActive(Frame f)
        {
            if (Fsm.IsInState(PlayerState.Throw)) return false;
            if (Fsm.IsInState(PlayerState.Cutscene)) return false;
            f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            var framesFromVirtualTime = Util.FramesFromVirtualTime(healthData->virtualTimeSinceEmpowered);
            if (framesFromVirtualTime > SurgeEmpoweredBuffDuration) return false;
            return true;
        }

        private void ResetEmpoweredHit(TriggerParams? triggerParams)
        {
            if (triggerParams is not FrameParam param) return;
            var f = param.f;
            f.Unsafe.TryGetPointer<HealthData>(EntityRef, out var healthData);
            healthData->nextHitEmpowered = false;
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
            if ((hurtboxPlayerFsm.Fsm.IsInState(PlayerState.Throw)) &&
                (hurtboxPlayerFsm.FramesInCurrentState(f) <= ThrowStartupDuration))
            {
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
                if (Fsm.IsInState(PlayerState.Air)) return (numpad is not (2 or 5 or 8));
                if (type == Hit.HitType.High) return numpad is 4 or 7 or 6 or 9;
                if (type == Hit.HitType.Mid) return numpad is not (2 or 5 or 8);
                if (type == Hit.HitType.Low) return numpad is 1 or 3;

                return false;
            }

            if (Fsm.IsInState(PlayerState.Air)) return (numpad is 1 or 4 or 7);
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

        protected override void HandleProxBlock(Frame frame)
        {
            var numpad = InputSystem.Numpad(frame, EntityRef);
            
            var proxBlockIsActive = InputSystem.NumpadMatchesNumpad(numpad, 4) &&
                                     OpponentThreateningProxBlock(frame);
            Fsm.Fire(proxBlockIsActive ? (InputSystem.NumpadMatchesNumpad(numpad, 2)
                ? PlayerTrigger.ProxBlockLow : PlayerTrigger.ProxBlockHigh)
                : PlayerTrigger.EndProxBlock,
                new FrameParam() {f = frame, EntityRef = EntityRef});
        }

        private bool OpponentThreateningProxBlock(Frame f)
        {
            int Lookahead = 13;
            var opponentEntity = Util.GetOtherPlayer(f, EntityRef);
            var opponentFsm = FsmLoader.GetFsm(opponentEntity);
            var hitSectionGroup = opponentFsm.StateMapConfig.HitSectionGroup.
                Get(opponentFsm, new FrameParam() {f = f, EntityRef = opponentFsm.EntityRef});
            if (hitSectionGroup is null) return false;
            
            // walk forward Lookahead # of frames to find the next hit
            var framesInCurrentState = opponentFsm.FramesInCurrentState(f);
            for (int i = framesInCurrentState; i < framesInCurrentState + Lookahead; i++)
            {
                var hit = hitSectionGroup.GetItemFromIndex(i);
                if (hit is null) continue;
                if (hit.Type is Hit.HitType.Throw) continue;
                var distance = GetXDistance(f, EntityRef, opponentEntity);
                if (distance >= hit.ProxBlockDistance) continue;
                return true;
            }

            return false;
        }
    }
}