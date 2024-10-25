using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMThrowCollisionSystem : SystemMainThreadFilter<PlayerFSMThrowCollisionSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerFSMData* PlayerFsmData;

        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            AdvanceFsm(f, filter);
        }

        private static void AdvanceFsm(Frame f, Filter filter)
        {
            PlayerFSM fsm = Util.GetPlayerFSM(f, filter.Entity);
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            fsm.ThrowboxCollide(f);
            
            Util.WritebackFsm(f, filter.Entity);
        }


        
    }
}