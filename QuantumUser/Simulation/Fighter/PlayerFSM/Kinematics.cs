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
        public void UpdateKinematicsAttachPosition(Frame f)
        {
            f.Unsafe.TryGetPointer<KinematicsData>(EntityRef, out var kinematicsData);
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);

            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            FPVector2 attachPosOffset = character.AttachPositionSectionGroup.Get(this).GetCurrentItem(f, this);
            
            if (!PlayerDirectionSystem.IsFacingRight(f, EntityRef))
            {
                attachPosOffset.X *= FP.Minus_1;
            }

            kinematicsData->attachPosition = transform3D->Position.XY + attachPosOffset;
        }

        public static FPVector2 GetFirstKinematicsAttachPosition(Frame f, EntityRef entityRef)
        {
            
            // TODO: maybe have a character value for this
            
            return FPVector2.Zero;
        }
    }
}