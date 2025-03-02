using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMTransitionSystem : SystemMainThreadFilter<PlayerFSMTransitionSystem.Filter>, ISignalOnComponentAdded<HitEntitiesTracker>, ISignalOnComponentRemoved<HitEntitiesTracker>, ISignalOnComponentAdded<FrameMeterData>, ISignalOnComponentRemoved<FrameMeterData>
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
            FSM fsm = Util.GetFSM(f, filter.Entity);
            if (fsm is null) return;
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            // Handle necessary state transitions
            fsm.DoFinish(f);
            fsm.CheckForLand(f);
            // fsm.CheckForOpponentThrowTech(f);
            InputSystem.FireFsmFromInput(f, fsm);
            
            Util.WritebackFsm(f, filter.Entity);
        }
        
        
        public void OnAdded(Frame f, EntityRef entity, HitEntitiesTracker* component)
        {
            component->HitEntities = f.AllocateList<EntityRef>();
        }

        public void OnRemoved(Frame f, EntityRef entity, HitEntitiesTracker* component)
        {
            f.FreeList(component->HitEntities);
            component->HitEntities = default;
        }
        
        public void OnAdded(Frame f, EntityRef entity, FrameMeterData* component)
        {
            component->types = f.AllocateList<int>();
            component->frames = f.AllocateList<int>();
        }
        
        public void OnRemoved(Frame f, EntityRef entity, FrameMeterData* component)
        {
            f.FreeList(component->types);
            f.FreeList(component->frames);
            component->types = default;
            component->frames = default;
        }


    }
}