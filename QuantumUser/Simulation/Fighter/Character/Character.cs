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
        
        // Animations
        public FighterAnimation StandAnimation;
        public FighterAnimation CrouchAnimation;
        public FighterAnimation WalkForwardAnimation;
        public FighterAnimation WalkBackwardAnimation;
        public RisingTrajectoryAnimation AirActionableRisingAnimation;
        public FallingTrajectoryAnimation AirActionableFallingAnimation;
        public RisingTrajectoryAnimation AirHitRisingAnimation;
        public FallingTrajectoryAnimation AirHitFallingAnimation;
        public RisingTrajectoryAnimation AirHitPostGroundBounceRisingAnimation;
        public FallingTrajectoryAnimation AirPostHitGroundBounceFallingAnimation;
        public FighterAnimation DashAnimation;
        public FighterAnimation BackdashAnimation;
        public FighterAnimation AirdashAnimation;
        public FighterAnimation AirBackdashAnimation;
        public FighterAnimation StandHitHighAnimation;
        public FighterAnimation StandHitLowAnimation;
        public FighterAnimation CrouchHitAnimation;
        public FighterAnimation StandBlockAnimation;
        public FighterAnimation CrouchBlockAnimation;
        public FighterAnimation AirBlockAnimation;
        public FighterAnimation LandsquatAnimation;
        public FighterAnimation HardKnockdownAnimation;
        public FighterAnimation SoftKnockdownAnimation;
        public FighterAnimation DeadFromAirAnimation;
        public FighterAnimation DeadFromGroundAnimation;
        public FighterAnimation KinematicReceiverAnimation;
        public FighterAnimation ThrowTechAnimation;
        
        public FighterAnimation ThrowStartupAnimation;
        public FighterAnimation ThrowWhiffAnimation;
        public Kinematics FrontThrowKinematics;
        public Kinematics BackThrowKinematics;

        public FPVector2 KinematicAttachPointOffset;

        // Actions
        public Dictionary<int, FighterAction> ActionDict;
        
        // Universal movement parameters
        public FP WalkForwardSpeed;
        public FP WalkBackwardSpeed;
        
        public FP JumpHeight;
        public int JumpTimeToHeight;
        public FP JumpForwardSpeed;
        public FP JumpBackwardSpeed;
        public int JumpCount;
        
        public FP FallSpeed;
        public int FallTimeToSpeed;

        public SectionGroup<FP> DashMovementSectionGroup;
        public SectionGroup<FP> BackdashMovementSectionGroup;
        
        
        // Hurtboxes
        public CollisionBoxCollection StandHurtboxesCollection;
        public CollisionBoxCollection CrouchHurtboxCollection;
        public CollisionBoxCollection AirHitHurtboxCollection;
        public Dictionary<int, int> InvulnerableBefore;
        
        // Pushbox
        public CollisionBox StandPushbox;
        public CollisionBox CrouchPushbox;
        public CollisionBox AirPushbox;
        public CollisionBox TallPushbox;


        // public PlayerFSM PlayerFsmTemplate;
        
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
                    return (ActionDict[source].CanCancelNow(actionParam.f, fsm) &&
                            ActionDict[destination].DoesInputMatch(param));
                }, weight);
        }
        
        protected void ConfigureGroundAction(PlayerFSM fsm, int actionState, 
            bool jumpCancellable = false, bool crouch = false, int weight = 0, bool usableRaw = true)
        {
            if (usableRaw) {
                fsm.Fsm.Configure(PlayerFSM.State.GroundActionable)
                    .PermitIf(PlayerFSM.Trigger.Action, actionState,
                        ActionDict[actionState].DoesInputMatch, weight);

                fsm.Fsm.Configure(PlayerFSM.State.Dash)
                    .PermitIf(PlayerFSM.Trigger.Action, actionState,
                        ActionDict[actionState].DoesInputMatch, weight);
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
                    return ActionDict[actionState].CanCancelNow(jumpParam.f, fsm);
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
                    ActionDict[actionState].DoesInputMatch, weight);

            fsm.Fsm.Configure(actionState)
                .SubstateOf(PlayerFSM.State.AirAction);
            
            fsm.Fsm.Configure(PlayerFSM.State.AirDash)
                .PermitIf(PlayerFSM.Trigger.Action, actionState,
                    ActionDict[actionState].DoesInputMatch, weight);
            
            if (!jumpCancellable) return;
            
            fsm.Fsm.Configure(actionState)
                .PermitIf(PlayerFSM.Trigger.Jump, PlayerFSM.State.AirActionable, param =>
                {
                    if (param is null) return false;
                    var jumpParam = (JumpParam)param;
                    return ActionDict[actionState].CanCancelNow(jumpParam.f, fsm);
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
                        ActionDict[actionState].DoesInputMatch, weight);

                fsm.Fsm.Configure(PlayerFSM.State.Dash)
                    .PermitIf(PlayerFSM.Trigger.Action, actionState,
                        ActionDict[actionState].DoesInputMatch, weight);
            }

            fsm.Fsm.Configure(actionState)
                .SubstateOf(PlayerFSM.State.AirAction);
            
            if (!jumpCancellable) return;
            
            fsm.Fsm.Configure(actionState)
                .PermitIf(PlayerFSM.Trigger.Jump, PlayerFSM.State.AirActionable, param =>
                {
                    if (param is null) return false;
                    var jumpParam = (JumpParam)param;
                    return ActionDict[actionState].CanCancelNow(jumpParam.f, fsm);
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