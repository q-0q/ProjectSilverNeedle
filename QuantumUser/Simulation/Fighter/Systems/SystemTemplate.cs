using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class SystemTemplate : SystemMainThreadFilter<SystemTemplate.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            
        }
    }
}