using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class FSMSystemB : SystemMainThreadFilter<FSMSystemB.Filter>, ISignalOnComponentAdded<HitEntitiesTracker>, ISignalOnComponentRemoved<HitEntitiesTracker>, ISignalOnComponentAdded<FrameMeterData>, ISignalOnComponentRemoved<FrameMeterData>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public FSMData* FsmData;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            FsmLoader.ReadAllFSMsFromNetwork(f);
            FSM fsm = FsmLoader.FSMs[filter.Entity];
            if (fsm is null) return;
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            
            // Receive collisions
            fsm.HitboxHurtboxCollide(f);

            // Fire summon triggers
            fsm.HandleSummonFSMTriggers(f);
            
            // Animation
            fsm.Animation(f);
            
            // Report frame meter
            fsm.ReportFrameMeterType(f);
            
            // Clock
            fsm.IncrementClock(f, filter.Entity);
            
            // Done!
            FsmLoader.WriteAllFSMsToNetwork(f);
            
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