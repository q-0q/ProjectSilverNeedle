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
            if (Fsm.IsInState(PlayerState.DirectionLocked)) return;
            
            ForceUpdatePlayerDirection(f, EntityRef);
        }

        public static void ForceUpdatePlayerDirection(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerDirection>(entityRef, out var playerDirection);

            var isOnLeft = IsOnLeft(f, entityRef);
            if (playerDirection->FacingRight != isOnLeft)
            {
                f.Unsafe.TryGetPointer<ProtectionData>(entityRef, out var protectionData);
                protectionData->virtualTimeSinceCrossupProtectionStart = 0;
            }
            playerDirection->FacingRight = isOnLeft;
        }
    }
}