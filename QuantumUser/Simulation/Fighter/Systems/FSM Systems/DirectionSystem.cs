using System.Diagnostics;
using System.Security.Cryptography;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class DirectionSystem : SystemMainThreadFilter<DirectionSystem.Filter>
    {

        
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public PlayerDirection* PlayerDirection;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            FSM fsm = Util.GetFSM(f, filter.Entity);
            if (fsm is null) return;
            
            if (HitstopSystem.IsHitstopActive(f)) return;
            
            fsm.UpdatePlayerDirection(f);
        }
        

    }
}