using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using Wasp;


namespace Quantum
{
    public abstract class Character
    {
        // Metadata
        public string Name;
        public Type StateType;
        public FPVector2 KinematicAttachPointOffset;

        public FP FallSpeed;
        public int FallTimeToSpeed;
        public int JumpCount;
        
        public Trajectory UpwardJumpTrajectory;
        public Trajectory ForwardJumpTrajectory;
        public Trajectory BackwardJumpTrajectory;
        
        
        // State maps
        public StateMap<FighterAnimation> FighterAnimation;
        public StateMap<int> Duration;
        public StateMap<SectionGroup<CollisionBoxCollection>> HurtboxCollectionSectionGroup;
        public StateMap<SectionGroup<PlayerFSM.HurtType>> HurtTypeSectionGroup;
        public StateMap<SectionGroup<Hit>> HitSectionGroup;
        public StateMap<CollisionBox> Pushbox;
        public StateMap<SectionGroup<FP>> MovementSectionGroup;
        public StateMap<SectionGroup<bool>> AllowCrossupSectionGroup;
        public StateMap<SectionGroup<Trajectory>> TrajectorySectionGroup;
        public StateMap<InputSystem.InputType> InputTypes;
        public StateMap<int> CommandDirection;
        public StateMap<int> CancellableAfter;
        public StateMap<bool> WhiffCancellable;
        public StateMap<int> FireReceiverFinishAfter;
        public StateMap<SectionGroup<FPVector2>> AttachPositionSectionGroup;
        public StateMap<int> InvulnerableBefore;
        
        public abstract void ConfigureCharacterFsm(PlayerFSM playerFsm);
        
        
        // FSM helper functions
        protected void MakeActionCancellable(PlayerFSM fsm, int source,
            int destination,int weight = 0)
        {
            fsm.Fsm.Configure(source)
                .PermitIf(PlayerFSM.Trigger.Action, destination, param =>
                {
                    if (param is null) return false;
                    var actionParam = (ActionParam)param;
                    return (Util.CanCancelNow(actionParam.f, actionParam.EntityRef) &&
                            Util.DoesInputMatch(actionParam.f, actionParam.EntityRef, actionParam.CommandDirection,
                                actionParam.Type));
                }, weight);
        }
        
        protected void ConfigureGroundAction(PlayerFSM fsm, int actionState, 
            bool jumpCancellable = false, bool crouch = false, int weight = 0, bool usableRaw = true)
        {
            if (usableRaw) {
                fsm.Fsm.Configure(PlayerFSM.State.GroundActionable)
                    .PermitIf(PlayerFSM.Trigger.Action, actionState, param =>
                        {
                            if (param is null) return false;
                            var actionParam = (ActionParam)param;
                            return Util.DoesInputMatch(actionParam.f, actionParam.EntityRef,
                                actionParam.CommandDirection,
                                actionParam.Type);
                        }, weight);

                fsm.Fsm.Configure(PlayerFSM.State.Dash)
                    .PermitIf(PlayerFSM.Trigger.Action, actionState,
                        param =>
                        {
                            if (param is null) return false;
                            var actionParam = (ActionParam)param;
                            return Util.DoesInputMatch(actionParam.f, actionParam.EntityRef,
                                actionParam.CommandDirection,
                                actionParam.Type);
                        }, weight);
            }

            fsm.Fsm.Configure(actionState)
                .SubstateOf(crouch ? PlayerFSM.State.Crouch : PlayerFSM.State.Stand)
                .SubstateOf(PlayerFSM.State.GroundAction);
            
            if (!jumpCancellable) return;
            
            fsm.Fsm.Configure(actionState)
                .PermitIf(PlayerFSM.Trigger.Jump, PlayerFSM.State.AirActionable, param =>
                {
                    if (param is null) return false;
                    var jumpParam = (JumpParam)param;
                    return Util.CanCancelNow(jumpParam.f, jumpParam.EntityRef);
                });
            
            // fsm.Fsm.Configure(actionState)
            //     .PermitIf(PlayerFSM.Trigger.Dash, PlayerFSM.State.Dash, param =>
            //     {
            //         if (param is null) return false;
            //         var frameParam = (FrameParam)param;
            //         return ActionDict[actionState].CanCancelNow(frameParam.f, fsm);
            //     });
        }

        protected void ConfigureAirAction(PlayerFSM fsm, int actionState,
            bool jumpCancellable = false,
            int weight = 0)
        {
            fsm.Fsm.Configure(PlayerFSM.State.AirActionable)
                .PermitIf(PlayerFSM.Trigger.Action, actionState,
                    param =>
                    {
                        if (param is null) return false;
                        var actionParam = (ActionParam)param;
                        return Util.DoesInputMatch(actionParam.f, actionParam.EntityRef,
                            actionParam.CommandDirection,
                            actionParam.Type);
                    }, weight);

            fsm.Fsm.Configure(actionState)
                .SubstateOf(PlayerFSM.State.AirAction);
            
            fsm.Fsm.Configure(PlayerFSM.State.AirDash)
                .PermitIf(PlayerFSM.Trigger.Action, actionState,
                    param =>
                    {
                        if (param is null) return false;
                        var actionParam = (ActionParam)param;
                        return Util.DoesInputMatch(actionParam.f, actionParam.EntityRef,
                            actionParam.CommandDirection,
                            actionParam.Type);
                    }, weight);
            
            if (!jumpCancellable) return;
            
            fsm.Fsm.Configure(actionState)
                .PermitIf(PlayerFSM.Trigger.Jump, PlayerFSM.State.AirActionable, param =>
                {
                    if (param is null) return false;
                    var jumpParam = (JumpParam)param;
                    return Util.CanCancelNow(jumpParam.f, jumpParam.EntityRef);
                });
            
            // fsm.Fsm.Configure(actionState)
            //     .PermitIf(PlayerFSM.Trigger.Dash, PlayerFSM.State.AirDash, param =>
            //     {
            //         if (param is null) return false;
            //         var frameParam = (FrameParam)param;
            //         return ActionDict[actionState].CanCancelNow(frameParam.f, fsm);
            //     });
        }
        
        protected void ConfigureGroundToAirAction(PlayerFSM fsm,
            int actionState,
            bool jumpCancellable = false, int weight = 0, bool usableRaw = true)
        {
            if (usableRaw) {
                fsm.Fsm.Configure(PlayerFSM.State.GroundActionable)
                    .PermitIf(PlayerFSM.Trigger.Action, actionState,
                        param =>
                        {
                            if (param is null) return false;
                            var actionParam = (ActionParam)param;
                            return (Util.CanCancelNow(actionParam.f, actionParam.EntityRef) &&
                                    Util.DoesInputMatch(actionParam.f, actionParam.EntityRef, actionParam.CommandDirection,
                                        actionParam.Type));
                        }, weight);

                fsm.Fsm.Configure(PlayerFSM.State.Dash)
                    .PermitIf(PlayerFSM.Trigger.Action, actionState,
                        param =>
                        {
                            if (param is null) return false;
                            var actionParam = (ActionParam)param;
                            return (Util.CanCancelNow(actionParam.f, actionParam.EntityRef) &&
                                    Util.DoesInputMatch(actionParam.f, actionParam.EntityRef, actionParam.CommandDirection,
                                        actionParam.Type));
                        }, weight);
            }

            fsm.Fsm.Configure(actionState)
                .SubstateOf(PlayerFSM.State.AirAction);
            
            if (!jumpCancellable) return;
            
            fsm.Fsm.Configure(actionState)
                .PermitIf(PlayerFSM.Trigger.Jump, PlayerFSM.State.AirActionable, param =>
                {
                    if (param is null) return false;
                    var jumpParam = (JumpParam)param;
                    return Util.CanCancelNow(jumpParam.f, jumpParam.EntityRef);
                });
            
            // fsm.Fsm.Configure(actionState)
            //     .PermitIf(PlayerFSM.Trigger.Dash, PlayerFSM.State.Dash, param =>
            //     {
            //         if (param is null) return false;
            //         var frameParam = (FrameParam)param;
            //         return ActionDict[actionState].CanCancelNow(frameParam.f, fsm);
            //     });
        }
        
        
        
        

    }
}