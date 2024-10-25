using System;
using Photon.Deterministic;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;

namespace Quantum.Types
{
    public class FighterAction
    {
        public InputSystem.InputType InputType = InputSystem.InputType.P;
        public int CommandDirection = 5;
        public int CancellableAfter = int.MaxValue;
        public bool WhiffCancellable = false;
        
        public FighterAnimation Animation;
        public SectionGroup<CollisionBoxCollection> HurtboxCollectionSectionGroup;
        public SectionGroup<Hit> HitSectionGroup;
        public SectionGroup<PlayerFSM.HurtType> HurtTypeSectionGroup;
        public SectionGroup<FP> MovementSectionGroup;
        public SectionGroup<bool> AllowCrossupSectionGroup;
        public SectionGroup<ActionTrajectory> TrajectorySectionGroup; 

        public bool DoesInputMatch(TriggerParams? param)
        {
            if (param is null) return false;

            var actionParam = (ActionParam)param;
            
            return (InputType == actionParam.Type &&
                    InputSystem.NumpadMatchesNumpad(actionParam.CommandDirection, CommandDirection));
            
        }

        public bool CanCancelNow(Frame f, PlayerFSM fsm)
        {
            return (fsm.FramesInCurrentState(f) >= CancellableAfter) && (!fsm.IsWhiffed(f) || WhiffCancellable);
        }
    }
}