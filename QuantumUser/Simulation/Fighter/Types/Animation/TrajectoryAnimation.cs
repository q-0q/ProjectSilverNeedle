using Photon.Deterministic;
using UnityEngine;

namespace Quantum.Types
{
    public unsafe class TrajectoryAnimation : FighterAnimation
    {
        protected enum TrajectoryType
        {
            Rise,
            Fall
        }
        
        protected FP GetPercentage(Frame f, PlayerFSM fsm, TrajectoryType type)
        {
            f.Unsafe.TryGetPointer<TrajectoryData>(fsm.EntityRef, out var trajectoryData);
            FP output = 0;
            int frames = Util.FramesFromVirtualTime(trajectoryData->virtualTimeInTrajectory);
            switch (type)
            {
                case TrajectoryType.Rise:
                {
                    output = (FP)frames / (FP)trajectoryData->timeToTrajectoryHeight;
                    break;
                }
                case TrajectoryType.Fall:
                {
                    if (trajectoryData->groundBounce)
                    {
                        output = 1;
                        break;
                    }
                    output = (FP)(frames - trajectoryData->timeToTrajectoryHeight) / (FP)trajectoryData->timeToTrajectoryHeight;
                    break;
                }
                default:
                {
                    output = 0;
                    break;
                }
            }
            
            return Util.Clamp(output, 0, FP._1);
        }
    }
}