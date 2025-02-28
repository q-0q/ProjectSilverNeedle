using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class SummonFSMSystem : SystemMainThreadFilter<SummonFSMSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public FSMData* FsmData;
            
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            
        }
    }
}