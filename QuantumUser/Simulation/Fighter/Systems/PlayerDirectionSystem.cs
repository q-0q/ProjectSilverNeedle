using System.Diagnostics;
using System.Security.Cryptography;
using Photon.Deterministic;

namespace Quantum
{
    public unsafe class PlayerDirectionSystem : SystemMainThreadFilter<PlayerDirectionSystem.Filter>
    {

        
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public PlayerDirection* PlayerDirection;
        }
        
        public override void Update(Frame f, ref Filter filter)
        {
            // UpdatePlayerDirection(f, filter.Entity);
        }
        
        public static void UpdatePlayerDirection(Frame f, PlayerFSM fsm)
        {
            if (fsm.Fsm.IsInState(PlayerFSM.State.GroundAction)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.AirAction)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.Hit)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.HardKnockdown)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.Dash)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.Backdash)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.AirDash)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.AirBackdash)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.CutsceneReactor)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.DeadFromAir)) return;
            if (fsm.Fsm.IsInState(PlayerFSM.State.DeadFromGround)) return;
            
            ForceUpdatePlayerDirection(f, fsm.EntityRef);
        }

        public static void ForceUpdatePlayerDirection(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerDirection>(entityRef, out var playerDirection);
            playerDirection->FacingRight = IsOnLeft(f, entityRef);
        }

        public static bool IsOnLeft(Frame f, EntityRef entityRef)
        {
            EntityRef opponent = Util.GetOtherPlayer(f, entityRef);
            f.Unsafe.TryGetPointer<Transform3D>(opponent, out var opponentTransform);
            f.Unsafe.TryGetPointer<Transform3D>(entityRef, out var thisTransform);

            return thisTransform->Position.X < opponentTransform->Position.X;
        }

        public static bool IsFacingRight(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerDirection>(entityRef, out var playerDirection);
            return playerDirection->FacingRight;
        }
    }
}