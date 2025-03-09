using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class FSMSystemA : SystemMainThreadFilter<FSMSystemA.Filter>, 
        ISignalOnComponentAdded<HitEntitiesTracker>, ISignalOnComponentRemoved<HitEntitiesTracker>, 
        ISignalOnComponentAdded<FrameMeterData>, ISignalOnComponentRemoved<FrameMeterData>,
        ISignalOnComponentAdded<ComboData>, ISignalOnComponentRemoved<ComboData>
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


            
            // fire transitional triggers
            fsm.DoFinish(f);
            fsm.CheckForLand(f);
            InputSystem.FireFsmFromInput(f, fsm);
            
            // clear hit entities
            if (fsm.IsOnFirstFrameOfHit(f))
            {
                fsm.ClearHitEntities(f);
            }
            
            // Capture collision snapshot
            filter.FsmData->currentCollisionState = fsm.Fsm.State();
            filter.FsmData->collisionFramesInState = fsm.FramesInCurrentState(f);

            // Generic move
            fsm.Move(f);
            
            // Trajectory move
            fsm.TrajectoryArc(f);
            
            // Update direction
            fsm.UpdateDirection(f);
            
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
        
        public void OnAdded(Frame f, EntityRef entity, ComboData* component)
        {
            component->hitCounts = f.AllocateDictionary<int, int>();
        }
        
        public void OnRemoved(Frame f, EntityRef entity, ComboData* component)
        {
            f.FreeDictionary(component->hitCounts);
            component->hitCounts = default;
        }
    }
}