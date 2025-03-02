using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class FSM
    {
        
        public virtual void UpdatePlayerDirection(Frame f)
        {

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