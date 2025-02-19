using System;
using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class PlayerFSMSReportSystem : SystemMainThreadFilter<PlayerFSMSReportSystem.Filter>
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
            PlayerFSM fsm = Util.GetPlayerFSM(f, filter.Entity);
            if (fsm is null) return;
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            // fsm.Move(f);
            PlayerDirectionSystem.UpdatePlayerDirection(f, fsm);
            fsm.TrajectoryArc(f);
            fsm.Animation(f);
            fsm.UpdateKinematicsAttachPosition(f);
            fsm.ReportFrameMeterType(f);
            
            IncrementClock(f, filter.Entity);
            Util.WritebackFsm(f, filter.Entity);
        }
        
        private static void IncrementClock(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<DramaticData>(entityRef, out var dramaticData);
            dramaticData->remaining = Math.Max(dramaticData->remaining - 1, 0);

            f.Unsafe.TryGetPointer<SlowdownData>(entityRef, out var slowdownData);
            slowdownData->slowdownRemaining--;

            // if (Util.EntityIsCpu(f, entityRef))
            // {
            //     Debug.Log(slowdownData->slowdownRemaining);
            // }

            FP virtualTimeIncrement = Util.FrameLengthInSeconds * Util.GetSlowdownMod(f, entityRef);
            
            
            f.Unsafe.TryGetPointer<FSMData>(entityRef, out var playerFsmData);
            playerFsmData->framesInState++;
            playerFsmData->virtualTimeInState += virtualTimeIncrement;
            
            f.Unsafe.TryGetPointer<PushbackData>(entityRef, out var pushbackData);
            pushbackData->framesInPushback++;
            pushbackData->virtualTimeInPushback += virtualTimeIncrement;
            
            f.Unsafe.TryGetPointer<MomentumData>(entityRef, out var momentumData);
            momentumData->framesInMomentum++;
            momentumData->virtualTimeInMomentum += virtualTimeIncrement;
            
            
            f.Unsafe.TryGetPointer<TrajectoryData>(entityRef, out var trajectoryData);
            trajectoryData->virtualTimeInTrajectory += (virtualTimeIncrement);
        }




        
    }
}