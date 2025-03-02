using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        public override void UpdateDirection(Frame f)
        {
            if (Fsm.IsInState(PlayerFSM.PlayerState.GroundAction)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.AirAction)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.Hit)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.HardKnockdown)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.Dash)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.Backdash)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.AirDash)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.AirBackdash)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.CutsceneReactor)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.DeadFromAir)) return;
            if (Fsm.IsInState(PlayerFSM.PlayerState.DeadFromGround)) return;
            
            ForceUpdatePlayerDirection(f, EntityRef);
        }

        public static void ForceUpdatePlayerDirection(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerDirection>(entityRef, out var playerDirection);
            playerDirection->FacingRight = IsOnLeft(f, entityRef);
        }
    }
}