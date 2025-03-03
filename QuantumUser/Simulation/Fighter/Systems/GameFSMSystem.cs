using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Collections;
using Quantum.Types;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Windows;
using Wasp;

namespace Quantum
{
    public unsafe class GameFSMSystem : SystemMainThreadFilter<GameFSMSystem.Filter>
    {
        private static readonly FP RoundStartDistance = 12;
        public static readonly int CountdownDuration = 10;
        public static readonly int RoundEndDuration = 150;
        public static readonly int RoundResetDuration = 60;
        
        public struct Filter
        {
            public EntityRef Entity;
            public GameFSMData* GameFsmData;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            FireStart(f, filter);

            FireFinish(f, filter);

            IncrementClock(f, filter.Entity);
        }

        private static void FireFinish(Frame f, Filter filter)
        {
            GameFSM gameFsm = GameFsmLoader.LoadGameFSM(f);
            FrameParam frameParam = new FrameParam() { f = f, EntityRef = filter.Entity };
            var finish = false;
            if (gameFsm.Fsm.IsInState(GameFSM.State.Countdown))
            {
                finish = gameFsm.FramesInState(f) >= CountdownDuration;
            }
            else if (gameFsm.Fsm.IsInState(GameFSM.State.RoundEnd))
            {
                finish = gameFsm.FramesInState(f) >= RoundEndDuration;
            }
            else if (gameFsm.Fsm.IsInState(GameFSM.State.RoundResetting))
            {
                finish = gameFsm.FramesInState(f) >= RoundResetDuration;
            }
            else if (gameFsm.Fsm.IsInState(GameFSM.State.Loading))
            {
                finish = gameFsm.FramesInState(f) >= 30;
            }

            if (finish)
            {
                gameFsm.Fsm.Fire(GameFSM.Trigger.Finish, frameParam);
            }
        }

        private static void FireStart(Frame f, Filter filter)
        {
            GameFSM gameFsm = GameFsmLoader.LoadGameFSM(f);
            FrameParam frameParam = new FrameParam() { f = f, EntityRef = filter.Entity };
            gameFsm.Fsm.Fire(GameFSM.Trigger.Started, frameParam);
        }


        public static void OnLoading(TriggerParams? triggerParams)
        {

            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;
            
            FsmLoader.InitializeFsms(frame);
            
            foreach (var (entityRef, _) in frame.GetComponentIterator<PlayerLink>())
            {
                InitializePlayerComponents(frame, entityRef);
            }
            
            foreach (var (entityRef, _) in frame.GetComponentIterator<SummonData>())
            {
                InitializeSummonComponents(frame, entityRef);
            }
            
        }
        
        public static void OnCountdown(TriggerParams? triggerParams)
        {
            if (triggerParams == null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;
            
            // AnimationEntitySystem.Create(frame, AnimationEntities.AnimationEntityEnum.Countdown, FPVector2.Zero, 0, false);
        }
        
        public static void ResetAllFsmData(TriggerParams? triggerParams)
        {
            if (triggerParams == null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;
            
            foreach (var (entityRef, _) in frame.GetComponentIterator<PlayerLink>())
            {
                ResetPlayerFSMData(frame, entityRef);
            }
            
            foreach (var (entityRef, _) in frame.GetComponentIterator<SummonData>())
            {
                ResetSummonFSMData(frame, entityRef);
            }
        }
        
        
        public static void OnRoundEnd(TriggerParams? triggerParams)
        {
            if (triggerParams == null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;
            
            AnimationEntitySystem.Create(frame, AnimationEntities.AnimationEntityEnum.KO, FPVector2.Zero, 0, false);
        }
        
        public static void ResetUnityView(TriggerParams? triggerParams)
        {
            if (triggerParams == null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;

            frame.Events.ResetUnityView();
        }

        private static void InitializePlayerComponents(Frame f, EntityRef entityRef)
        {
            
            f.Add(entityRef, new HealthData());
            f.Add(entityRef, new PlayerDirection());
            f.Add(entityRef, new TrajectoryData());
            f.Add(entityRef, new InputBuffer());
            f.Add(entityRef, new FSMData());
            f.Add(entityRef, new AnimationData()); // This we can get rid of by using state on view to get animation
            f.Add(entityRef, new HitEntitiesTracker()
            {
                HitEntities = new QListPtr<EntityRef>()
            });
            f.Add(entityRef, new StunData());
            f.Add(entityRef, new SlowdownData());
            f.Add(entityRef, new PushbackData());
            f.Add(entityRef, new MomentumData());
            f.Add(entityRef, new ComboData());
            f.Add(entityRef, new WhiffData());
            f.Add(entityRef, new KinematicsData());
            f.Add(entityRef, new FrameMeterData()
            {
                types = new QListPtr<int>(),
                frames = new QListPtr<int>(),
            });
            f.Add(entityRef, new DramaticData());
            f.Add(entityRef, new CutsceneData());
            f.Add(entityRef, new ScoreData() { score = 0 });
            
            ResetPlayerFSMData(f, entityRef);
        }
        
        private static void InitializeSummonComponents(Frame f, EntityRef entityRef)
        {
            f.Add(entityRef, new PlayerDirection());
            f.Add(entityRef, new FSMData());
            f.Add(entityRef, new AnimationData()); // This we can get rid of by using state on view to get animation
            f.Add(entityRef, new HitEntitiesTracker()
            {
                HitEntities = new QListPtr<EntityRef>()
            });
            ResetSummonFSMData(f, entityRef);
        }

        private static void ResetSummonFSMData(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<FSMData>(entityRef, out var fsmData);
            fsmData->currentState = 0;
            fsmData->framesInState = 0;
            fsmData->currentCollisionState = 0;
            fsmData->collisionFramesInState = 0;
            // force call OnPooled()
            (Util.GetFSM(f, entityRef) as SummonFSM)?.OnPooled(new FrameParam() { f = f, EntityRef = entityRef});
            
            f.Unsafe.TryGetPointer<AnimationData>(entityRef, out var animationData);
            animationData->path = 0;

            f.Unsafe.TryGetPointer<HitEntitiesTracker>(entityRef, out var hitEntitiesTracker);
            f.ResolveList(hitEntitiesTracker->HitEntities).Clear();
        }


        private static void ResetPlayerFSMData(Frame f, EntityRef entityRef)
        {
            if (Util.GetFSM(f, entityRef) is not PlayerFSM fsm) return;
            
            f.Unsafe.TryGetPointer<HealthData>(entityRef, out var healthData);
            healthData->health = 500;

            f.Unsafe.TryGetPointer<TrajectoryData>(entityRef, out var trajectoryData);
            trajectoryData->startingTrajectoryHeight = 0;
            trajectoryData->framesInTrajectory = 0;
            trajectoryData->jumpsRemaining = fsm.JumpCount;
            trajectoryData->xVelocity = 0;
            trajectoryData->hardKnockdown = false;
            trajectoryData->groundBounce = false;
            trajectoryData->dashType = TrajectoryDashType.None;

            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            inputBuffer->length = 0;
            inputBuffer->type = 0;

            f.Unsafe.TryGetPointer<FSMData>(entityRef, out var fsmData);
            fsmData->currentState = 0;
            fsmData->framesInState = 0;
            fsmData->currentCollisionState = 0;
            fsmData->collisionFramesInState = 0;

            f.Unsafe.TryGetPointer<AnimationData>(entityRef, out var animationData);
            animationData->path = 0;

            f.Unsafe.TryGetPointer<HitEntitiesTracker>(entityRef, out var hitEntitiesTracker);
            f.ResolveList(hitEntitiesTracker->HitEntities).Clear();

            f.Unsafe.TryGetPointer<StunData>(entityRef, out var stunData);
            stunData->stun = 0;
            
            f.Unsafe.TryGetPointer<SlowdownData>(entityRef, out var slowdownData);
            slowdownData->slowdownRemaining = 0;
            slowdownData->multiplier = 1;
            
            f.Unsafe.TryGetPointer<PushbackData>(entityRef, out var pushbackData);
            pushbackData->framesInPushback = 0;
            pushbackData->pushbackAmount = 0;
            
            f.Unsafe.TryGetPointer<MomentumData>(entityRef, out var momentumData);
            momentumData->framesInMomentum = 0;
            momentumData->momentumAmount = 0;
            
            f.Unsafe.TryGetPointer<ComboData>(entityRef, out var comboData);
            comboData->length = 0;
            comboData->damageScaling = 1;
            comboData->gravityScaling = 1;
            
            f.Unsafe.TryGetPointer<WhiffData>(entityRef, out var whiffData);
            whiffData->whiffed = true;
            
            f.Unsafe.TryGetPointer<KinematicsData>(entityRef, out var kinematicsData);
            kinematicsData->attachPosition = FPVector2.Zero;

            f.Unsafe.TryGetPointer<FrameMeterData>(entityRef, out var frameMeterData);
            f.ResolveList(frameMeterData->types).Clear();
            f.ResolveList(frameMeterData->frames).Clear();
            
            f.Unsafe.TryGetPointer<DramaticData>(entityRef, out var dramaticData);
            dramaticData->remaining = 0;
            
            f.Unsafe.TryGetPointer<CutsceneData>(entityRef, out var cutsceneData);
            cutsceneData->initiator = EntityRef.None;
            cutsceneData->initiatorFacingRight = false;
            cutsceneData->cutsceneIndex = -1;

            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            var player = playerLink->Player;
            
            // Offset the instantiated object in the world, based on its ID.
            if (f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var transform))
            {
                FP xCoord = (int)player == 0
                    ? RoundStartDistance * FP._0_50 * FP.Minus_1
                    : RoundStartDistance * FP._0_50;
                transform->Position.X = xCoord;
                transform->Position.Y = Util.GroundHeight;
            }
            
            PlayerFSM.ForceUpdatePlayerDirection(f, entityRef);
            
        }

        
        private static void IncrementClock(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<GameFSMData>(entityRef, out var gameFsmData);
            gameFsmData->framesInState++;
        }

        // public static GameFSM.State GetGameState(Frame f)
        // {
        //     return LoadGameFsm(f).Fsm.State();
        // }
    }
}