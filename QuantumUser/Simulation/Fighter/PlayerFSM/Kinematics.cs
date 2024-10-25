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

            FPVector2 attachPosOffset = new FPVector2(1,0);
            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            
            if (Fsm.State() == State.FrontThrowConnect)
            {
                attachPosOffset = character.FrontThrowKinematics.GrabPositionSectionGroup
                    .GetCurrentItem(f, this);
            }
            else if (Fsm.State() == State.BackThrowConnect)
            {
                attachPosOffset = character.BackThrowKinematics.GrabPositionSectionGroup
                    .GetCurrentItem(f, this);
            }

            if (!PlayerDirectionSystem.IsFacingRight(f, EntityRef))
            {
                attachPosOffset.X *= FP.Minus_1;
            }

            kinematicsData->attachPosition = transform3D->Position.XY + attachPosOffset;
        }

        public static FPVector2 GetFirstKinematicsAttachPosition(Frame f, EntityRef entityRef)
        {
            var attachPosOffset = Characters.GetPlayerCharacter(f, entityRef).FrontThrowKinematics.GrabPositionSectionGroup
                .GetItemFromIndex(0);
            
            if (!PlayerDirectionSystem.IsFacingRight(f, entityRef))
            {
                attachPosOffset.X *= FP.Minus_1;
            }

            return attachPosOffset;
        }
    }
}