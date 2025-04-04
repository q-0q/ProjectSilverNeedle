using System;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    
    public unsafe class HitstopSystem : SystemMainThreadFilter<HitstopSystem.Filter>
    {
        private static int _baseHitstopAmount = 10;
        
        public struct Filter
        {
            public EntityRef Entity;
            public HitstopData* HitstopData;

        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            filter.HitstopData->hitstopRemaining--;
            TryDequeue(f);
        }

        private static HitstopData* GetHitstopData(Frame f)
        {
            foreach (var (_, hitstopData) in f.GetComponentIterator<HitstopData>())
            {
                return &hitstopData;
            }

            return null;
        }

        public static bool IsHitstopActive(Frame f)
        {
            // return false;
            try
            {
                var hitstopData = GetHitstopData(f);
                return hitstopData->hitstopRemaining > 0 && hitstopData->queued == false;
            }
            catch (Exception e)
            {
                return true;
            }
        }

        public static void TryDequeue(Frame f)
        {
            foreach (var (entity, hitstopData) in f.GetComponentIterator<HitstopData>())
            {
                var component = hitstopData;
                component.queued = false;
                f.Set(entity, component);
            }
        }

        public static void EnqueueHitstop(Frame f, int amount)
        {
            foreach (var (entity, hitstopData) in f.GetComponentIterator<HitstopData>())
            {
                var component = hitstopData;
                component.hitstopRemaining = amount;
                component.queued = true;
                f.Set(entity, component);
            }
        }
        
    }
}