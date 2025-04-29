using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
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
            return a > b ? b : a;
        }
        public static FP Max(FP a, FP b)
        {
            return a < b ? b : a;
        }

        public static FP Abs(FP fp)
        {
            if (fp > 0) return fp;
            return fp * FP.Minus_1;
        }
        
        public static FP Pow(FP fp, int pow)
        {
            FP o = 1;
            for (int i = 0; i < pow; i++)
            {
                o *= fp;
            }

            return o;
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
        
        public static List<EntityRef> GetOpponentFSMEntities(Frame f, EntityRef player)
        {
            var output = new List<EntityRef>();
            
            foreach (var (e, component) in f.GetComponentIterator<FSMData>()) {
                if (FsmLoader.FSMs[e].GetPlayer() != player)
                {
                    output.Add(e);
                }
            }
            
            return output;
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
            
            f.Unsafe.TryGetPointer<PlayerLink>(FsmLoader.FSMs[entityRef].GetPlayer(), out var playerLink);
            return ((int)playerLink->Player == CPUPlayerId);
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

        
        
        // Todo: refactor
        // public static FSM GetFSM(Frame f, EntityRef entityRef, bool debug=false)
        // {
        //     
        //     var fsm = FsmLoader.GetFsm(entityRef);
        //     if (fsm is null) return null;
        //     
        //     f.Unsafe.TryGetPointer<FSMData>(entityRef, out var playerFsmData);
        //     fsm.EntityRef = entityRef;
        //     
        //     
        //     fsm.Fsm.Assume(playerFsmData->currentState);
        //     
        //     return fsm;
        // }

        public static void StartDramatic(Frame f, EntityRef entityRef, int duration)
        {
            f.Unsafe.TryGetPointer<DramaticData>(entityRef, out var dramaticData);
            dramaticData->remaining = duration;
        }
        
        public static void StartScreenDark(Frame f, EntityRef entityRef, int duration)
        {
            f.Unsafe.TryGetPointer<DramaticData>(entityRef, out var dramaticData);
            dramaticData->darkRemaining = duration;
        }

        public static bool IsPlayerFacingAwayFromWall(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var transform3D);
            bool onLeft = FSM.IsOnLeft(f, entityRef);
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



        public static int FramesFromVirtualTime(FP virtualTime)
        {
            return (virtualTime / FrameLengthInSeconds).AsInt;
        }

        public static void IncrementScore(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<ScoreData>(entityRef, out var scoreData);
            scoreData->score++;
        }


        public static int GetAnimationPathLength(FSM fsm, string path)
        {
            var fsmName = fsm.Name;
    
            // Building the path within the Resources folder
            string fullPath = "Sprites/Characters/" + fsmName + "/FrameGroups/" + path;
    
            // Load all PNG files from the Resources path
            var sprites = Resources.LoadAll<Sprite>(fullPath);  // Assuming you are working with Sprite assets
    
            // Return the count of loaded sprites (this represents the number of PNG files)
            return sprites.Length;
        }
        
        public static void AutoSetupFromAnimationPath(FighterAnimation animation, FSM fsm)
        {
            var sectionGroup = animation.SectionGroup;
            var path = animation.Path;
            if (!sectionGroup.AutoFromAnimationPath) return;
            sectionGroup.Sections = new List<Tuple<int, int>>();
            int length = Util.GetAnimationPathLength(fsm, path);
            sectionGroup.Sections.Capacity = length;
            for (int i = 0; i < length; i++)
            {
                sectionGroup.Sections.Add(new Tuple<int, int>(1, i));
            }
        }


        public static Cutscene GetActiveCutscene(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<CutsceneData>(entityRef, out var cutsceneData);
            var index = cutsceneData->cutsceneIndex;
            var fsm = FsmLoader.FSMs[entityRef];
            
            try
            {
                return fsm.Cutscenes[index];
            }
            catch (Exception e)
            {
                Debug.LogError("CutsceneData index does not map to a cutscene");
                return null;
            }
        }
        
        // Old Action functions that have been migrated

        
        public static bool CanCancelNow(TriggerParams param)
        {
            var frameParam = (FrameParam)param;
            var f = frameParam.f;
            var entityRef = frameParam.EntityRef;

            if (FsmLoader.FSMs[entityRef] is not PlayerFSM fsm)
            {
                Debug.LogError("Something went really wrong, you tried to check CanCancelNow on a non-Player FSM");
                return false;
            }
            
            return (fsm.FramesInCurrentState(f) >= fsm.StateMapConfig.CancellableAfter.Get(fsm)) && (!fsm.IsWhiffed(f) || 
                fsm.StateMapConfig.WhiffCancellable.Get(fsm));
        }
        public static bool DoesInputMatch(PlayerFSM.ActionConfig actionConfig, TriggerParams param)
        {
            if (param is null) return false;
            var buttonAndDirectionParam = (ButtonAndDirectionParam)param; 
             return (actionConfig.InputType == buttonAndDirectionParam.Type &&
                     InputSystem.NumpadMatchesNumpad(buttonAndDirectionParam.CommandDirection, actionConfig.CommandDirection));
         }
        
        
        
        
        public static string PrettyPrint<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict == null)
                return "Dictionary is null";

            var sb = new StringBuilder();
            sb.AppendLine("{");

            foreach (var kvp in dict)
            {
                sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
        
    }
    
    
}