using Photon.Deterministic;

namespace Quantum
{
    public class FrameParam : Wasp.TriggerParams
    {
        public EntityRef EntityRef;
        public Frame f;
    }
    
    public class JumpParam : FrameParam
    {
        public PlayerFSM.JumpType Type;
    }
    
    public class CollisionHitParams : FrameParam
    {
        public FP XVelocity;
        public FP TrajectoryHeight;
        public bool Launches;
        public bool GroundBounces;
        public bool WallBounces;
    }
    
    public class ActionParam : FrameParam
    {
        public InputSystem.InputType Type;
        public int CommandDirection;
    }
    
    
    
    
    
    // public class FrameParam : Wasp.TriggerParams
    // {
    //     public Frame f;
    // }
}