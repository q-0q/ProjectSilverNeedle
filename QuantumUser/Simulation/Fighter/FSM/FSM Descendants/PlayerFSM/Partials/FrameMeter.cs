using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEditor.Experimental;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {

        private static int FrameMeterSize = 30;
        
        public enum FrameMeterType
        {
            None,
            Startup,
            Active,
            Recovery,
            HitStun,
            BlockStun,
            Oki,
        }
        
        public override void ReportFrameMeterType(Frame f)
        {
            FrameMeterType type = GetFrameMeterType(f);
            
            f.Unsafe.TryGetPointer<FrameMeterData>(EntityRef, out var frameMeterData);
            var types = f.ResolveList(frameMeterData->types);
            var frames = f.ResolveList(frameMeterData->frames);
            
            if (types.Count >= FrameMeterSize)
                types.Clear();
            
            if (frames.Count >= FrameMeterSize)
                frames.Clear();
            
            types.Add((int)type);
            frames.Add(f.Number);
            
            if (Util.EntityIsCpu(f, EntityRef)) return;
        }

        private FrameMeterType GetFrameMeterType(Frame f)
        {
            if (HasHitActive(f)) return FrameMeterType.Active;
            // if (GetHurtType(f) == HurtType.Counter) return FrameMeterType.Startup;
            // if (GetHurtType(f) == HurtType.Punish) return FrameMeterType.Recovery;
            if (Fsm.IsInState(PlayerState.Hit)) return FrameMeterType.HitStun;
            if (Fsm.IsInState(PlayerState.Block)) return FrameMeterType.BlockStun;
            if (Fsm.IsInState(PlayerState.HardKnockdown)) return FrameMeterType.Oki;
            return FrameMeterType.None;
        }
    }
}