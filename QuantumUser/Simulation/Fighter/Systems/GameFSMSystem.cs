using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Collections;
using Quantum.Types;
using UnityEngine;
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

            GameFSM gameFsm = LoadGameFsm(f);
            IncrementClock(f, filter.Entity);
            WritebackGameFsm(f, gameFsm);
        }

        private static void FireFinish(Frame f, Filter filter)
        {
            GameFSM gameFsm = LoadGameFsm(f);
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

            if (finish)
            {
                gameFsm.Fsm.Fire(GameFSM.Trigger.Finish, frameParam);
            }
            WritebackGameFsm(f, gameFsm);
        }

        private static void FireStart(Frame f, Filter filter)
        {
            GameFSM gameFsm = LoadGameFsm(f);
            FrameParam frameParam = new FrameParam() { f = f, EntityRef = filter.Entity };
            gameFsm.Fsm.Fire(GameFSM.Trigger.Started, frameParam);
            WritebackGameFsm(f, gameFsm);
        }


        public static void OnReady(TriggerParams? triggerParams)
        {
            
            Debug.Log("OnReady");

            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;
            
            Debug.Log("OnReady, after cast");
            
            foreach (var (entityRef, _) in frame.GetComponentIterator<PlayerLink>())
            {
                InitializePlayerComponents(frame, entityRef);
            }
            
            PlayerFsmLoader.InitializePlayerFsms(frame);
        }
        
        public static void OnCountdown(TriggerParams? triggerParams)
        {
            if (triggerParams == null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;
            
            // AnimationEntitySystem.Create(frame, AnimationEntities.AnimationEntityEnum.Countdown, FPVector2.Zero, 0, false);
        }
        
        public static void ResetPlayers(TriggerParams? triggerParams)
        {
            if (triggerParams == null) return;
            var frameParam = (FrameParam)triggerParams;
            var frame = frameParam.f;
            
            foreach (var (entityRef, _) in frame.GetComponentIterator<PlayerLink>())
            {
                ResetPlayer(frame, entityRef);
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
            InheritableEnum.InheritableEnum.Initialize();
            
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
            f.Add(entityRef, new ScoreData() { score = 0 });
            
            ResetPlayer(f, entityRef);
        }


        private static void ResetPlayer(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<HealthData>(entityRef, out var healthData);
            healthData->health = 500;

            f.Unsafe.TryGetPointer<TrajectoryData>(entityRef, out var trajectoryData);
            trajectoryData->startingTrajectoryHeight = 0;
            trajectoryData->framesInTrajectory = 0;
            trajectoryData->jumpsRemaining = Characters.GetPlayerCharacter(f, entityRef).JumpCount;
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
            
            PlayerDirectionSystem.ForceUpdatePlayerDirection(f, entityRef);
            
        }

        public static void FireWriteGameFsm(Frame f, GameFSM.Trigger trigger)
        {
            FrameParam frameParam = new FrameParam() { f = f };
            GameFSM gameFsm = LoadGameFsm(f);
            gameFsm.Fsm.Fire(trigger, frameParam);
            WritebackGameFsm(f, gameFsm);
        }

        public static GameFSM LoadGameFsm(Frame f)
        {
            foreach (var (entityRef, gameFsmData) in f.GetComponentIterator<GameFSMData>())
            {
                return new GameFSM(gameFsmData.currentState, entityRef);
            }

            return null;
        }

        public static void WritebackGameFsm(Frame f, GameFSM gameFsm)
        {
            f.Unsafe.TryGetPointer<GameFSMData>(gameFsm.EntityRef, out var gameFsmData);
            gameFsmData->currentState = (int)gameFsm.Fsm.State();
        }

        private static void IncrementClock(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<GameFSMData>(entityRef, out var gameFsmData);
            gameFsmData->framesInState++;
        }

        public static GameFSM.State GetGameState(Frame f)
        {
            return LoadGameFsm(f).Fsm.State();
        }
    }
}