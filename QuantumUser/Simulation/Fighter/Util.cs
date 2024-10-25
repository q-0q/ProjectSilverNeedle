using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Photon.Deterministic;
using UnityEngine.SocialPlatforms.Impl;
using Wasp;
using Debug = UnityEngine.Debug;

namespace Quantum
{
    public static unsafe class Util
    {
        public static FP GroundHeight = FP.FromString("0");
        public static FP FrameLengthInSeconds = FP.FromString("0.016666");
        public static int CPUPlayerId = 1;
        
        public static FP Min(FP a, FP b)
        {
            if (a > b) return b;
            return a;
        }
        public static FP Max(FP a, FP b)
        {
            if (a < b) return b;
            return a;
        }

        public static FP Abs(FP fp)
        {
            if (fp > 0) return fp;
            return fp * FP.Minus_1;
        }
        
        public static EntityRef GetOtherPlayer(Frame f, EntityRef player)
        {
            EntityRef opponent = player;
            foreach (var (e, component) in f.GetComponentIterator<PlayerLink>()) {
                if (e != player)
                {
                    opponent = e;
                }
            }
            
            // If this check fails then we failed to find the opponent's entityref
            Assert.Check(opponent != player);
            
            return opponent;
        }

        public static EntityRef GetPlayer(Frame f, int player)
        {
            foreach (var (e, component) in f.GetComponentIterator<PlayerLink>())
            {
                if (component.Player == player) return e;
            }

            return EntityRef.None;
        }
        
        public static void Log(Frame f, EntityRef entity, string msg)
        {
            if (GetPlayerId(f, entity) == CPUPlayerId) return;
            string prefix = GetPlayerId(f, entity) == CPUPlayerId ? "[CPU]    " : "[Player] ";
            Debug.Log(prefix + msg);
        }
        
        public static int GetPlayerId(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            return playerLink->Player;
        }

        public static FP Clamp(FP x, FP min, FP max)
        {
            return Max(min, Min(max, x));
        }

        public static FP Lerp(FP a, FP b, FP t)
        {
            return a + (b - a) * Clamp(t, 0, FP._1);
        }

        public static bool EntityIsCpu(Frame f, EntityRef entityRef)
        {
            if (!GetCpuControllerData(f)->cpuEnabled) return false;
            
            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            return ((int)playerLink->Player == CPUPlayerId);
        }
        
        public static void WritebackFsm(Frame f, EntityRef entityRef)
        {
            var fsm = PlayerFsmLoader.GetPlayerFsm(f, entityRef);
            
            f.Unsafe.TryGetPointer<PlayerFSMData>(entityRef, out var playerFsmData);
            playerFsmData->currentState = (int)fsm.Fsm.State();
           
        }

        public static CpuControllerData* GetCpuControllerData(Frame f)
        {
            foreach (var (_, cpuControllerData) in f.GetComponentIterator<CpuControllerData>())
            {
                return &cpuControllerData;
            }

            return null;
        }

        public static InteractionControllerData GetInteractionControllerData(Frame f)
        {
            foreach (var (_, interactionControllerData) in f.GetComponentIterator<InteractionControllerData>())
            {
                return interactionControllerData;
            }

            return new InteractionControllerData();
        }

        public static PlayerFSM GetPlayerFSM(Frame f, EntityRef entityRef, bool debug=false)
        {

            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            var player = (int)playerLink->Player;
            
            
            // var fsm = Characters.GetPlayerCharacter(f, entityRef).PlayerFsms[player];

            var fsm = PlayerFsmLoader.GetPlayerFsm(f, entityRef);

            
            f.Unsafe.TryGetPointer<PlayerFSMData>(entityRef, out var playerFsmData);

            
            fsm.Fsm.Assume((PlayerFSM.State)playerFsmData->currentState);
            fsm.EntityRef = entityRef;
            
           
            return fsm;
        }

        public static void StartDramatic(Frame f, EntityRef entityRef, int duration)
        {
            f.Unsafe.TryGetPointer<DramaticData>(entityRef, out var dramaticData);
            dramaticData->remaining = duration;
        }

        public static bool IsPlayerFacingAwayFromWall(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var transform3D);
            bool onLeft = PlayerDirectionSystem.IsOnLeft(f, entityRef);
            if (onLeft && transform3D->Position.X < 0) return true;
            return !onLeft && transform3D->Position.X > 0;
        }

        public static bool IsPlayerInCorner(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var transform3D);
            
            // if (GetPlayerId(f, entityRef) == 1) Debug.Log(transform3D->Position.X);
            return IsPlayerFacingAwayFromWall(f, entityRef) && Abs(transform3D->Position.X) 
                > (PlayerFSM.WallHalfLength - 1);
        }

        public static FP GetSlowdownMod(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<SlowdownData>(entityRef, out var slowdownData);
            return slowdownData->slowdownRemaining <= 0 ? 1 : slowdownData->multiplier;
        }

        public static int FramesFromVirtualTime(FP virtualTime)
        {
            return (virtualTime / FrameLengthInSeconds).AsInt;
        }

        public static void IncrementScore(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<ScoreData>(entityRef, out var scoreData);
            scoreData->score++;
        }
        
    }
    
    
}