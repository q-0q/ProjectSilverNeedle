using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        private void ResetWhiff(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;

            var param = (FrameParam)triggerParams;
            param.f.Unsafe.TryGetPointer<WhiffData>(EntityRef, out var whiffData);
            whiffData->whiffed = true;
        }

        private void MakeNotWhiffed(Frame f, CollisionBoxInternal hitboxInternal)
        {
            var entityRef = FsmLoader.FSMs[hitboxInternal.source].GetPlayer();
            if (f.Unsafe.TryGetPointer<WhiffData>(entityRef, out var whiffData))
            {
                whiffData->whiffed = false;
                whiffData->notWhiffedHitId = hitboxInternal.lookupId;
            };
        }

        public bool IsWhiffed(Frame f)
        {
            f.Unsafe.TryGetPointer<WhiffData>(EntityRef, out var whiffData);
            return whiffData->whiffed;
        }
        
        public bool IsNotWhiffedFromHit(Frame f, int hitId)
        {
            f.Unsafe.TryGetPointer<WhiffData>(EntityRef, out var whiffData);
            return !whiffData->whiffed && whiffData->notWhiffedHitId == hitId;
        }
    }
}