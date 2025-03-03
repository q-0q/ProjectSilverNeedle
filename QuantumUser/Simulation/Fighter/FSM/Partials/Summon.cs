using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe partial class FSM
    {
        public void UnpoolSummon(Frame f)
        {
            var unpoolSummonSectionGroup = StateMapConfig.UnpoolSummonSectionGroup.Get(this);
            if (unpoolSummonSectionGroup is null) return;
            if (!unpoolSummonSectionGroup.IsOnFirstFrameOfSection(f, this)) return;
            var summonPool = unpoolSummonSectionGroup.GetCurrentItem(f, this);
            if (summonPool is null) return;
            Debug.Log("Successfully invoked unpool on " + summonPool.SummonFSMType);
            
            EntityRef summonToUnpool = EntityRef.None;
            foreach (var summonEntity in summonPool.EntityRefs)
            {
                f.Unsafe.TryGetPointer<SummonData>(summonEntity, out var summonData);
                summonData->counter--;
                if (summonData->counter == -1)
                {
                    summonToUnpool = summonEntity;
                    summonData->counter = summonPool.Size - 1;
                }
            }

            if (summonToUnpool == EntityRef.None)
            {
                Debug.LogError("Failed to find a summon entity with counter 0 when trying to unpool");
                return;
            }
            
            if (Util.GetFSM(f, summonToUnpool) is not SummonFSM fsm)
            {
                Debug.LogError("Found a summon entity to unpool, but it is not a SummonFSM");
                return;
            }
            
            fsm.Fsm.Fire(SummonFSM.SummonTrigger.Summoned, new FrameParam() {f = f, EntityRef = summonToUnpool});
        }
        
    }
}