using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMSCollisionBoxSystem : SystemMainThreadFilter<PlayerFSMSCollisionBoxSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public FSMData* PlayerFsmData;

        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            AdvanceFsm(f, filter);
        }

        private static void AdvanceFsm(Frame f, Filter filter)
        {
            var fsm = Util.GetPlayerFSM(f, filter.Entity);
            if (fsm is null) return;
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            // Collision detection
            fsm.Hurtbox(f);
            fsm.Throwbox(f);
            fsm.Hitbox(f);
            PlayerFSM.Pushbox(f, filter.Entity);
            
            Util.WritebackFsm(f, filter.Entity);
        }


        
    }
}