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
            var unpoolSummon = unpoolSummonSectionGroup.GetCurrentItem(f, this);
            if (unpoolSummon is null) return;
            Debug.Log("Successfully invoked unpool on " + unpoolSummon.SummonFSMType);
        }
        
    }
}