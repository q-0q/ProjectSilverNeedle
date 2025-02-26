using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public abstract unsafe class Character
    {
        // Metadata
        public string Name;
        public Type StateType;
        public Type AnimationPathsEnum;
        public FPVector2 KinematicAttachPointOffset;

        public FP FallSpeed;
        public int FallTimeToSpeed;
        public int JumpCount;
        
        public Trajectory UpwardJumpTrajectory;
        public Trajectory ForwardJumpTrajectory;
        public Trajectory BackwardJumpTrajectory;

        public Dictionary<int, Cutscene> Cutscenes;
        
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
        // public StateMap<InputSystem.InputType> InputTypes;
        // public StateMap<int> CommandDirection;
        public StateMap<int> CancellableAfter;
        public StateMap<bool> WhiffCancellable;
        public StateMap<int> FireReceiverFinishAfter;
        public StateMap<SectionGroup<FPVector2>> AttachPositionSectionGroup;
        public StateMap<int> InvulnerableBefore;
        
        public abstract void ConfigureCharacterFsm(PlayerFSM playerFsm);

        protected void SetupStateMaps()
        {
            FighterAnimation = new StateMap<FighterAnimation>();
            Duration = new StateMap<int>();
            Duration.DefaultValue = 0;
            Duration.SuperFuncDictionary[PlayerFSM.State.Hit] = GetStun;
            Duration.SuperFuncDictionary[PlayerFSM.State.Block] = GetStun;
            Duration.SuperFuncDictionary[PlayerFSM.State.CutsceneReactor] = GetCutsceneReactorDuration;

            Duration.Dictionary[PlayerFSM.State.HardKnockdown] = 50;
            Duration.Dictionary[PlayerFSM.State.SoftKnockdown] = 20;
            Duration.SuperDictionary[PlayerFSM.State.Throw] = 40;

            HurtboxCollectionSectionGroup = new StateMap<SectionGroup<CollisionBoxCollection>>();
            HurtTypeSectionGroup = new StateMap<SectionGroup<PlayerFSM.HurtType>>();
            HurtTypeSectionGroup.SuperDictionary[PlayerFSM.State.Throw] = new SectionGroup<PlayerFSM.HurtType>()
            {
                Sections = new List<Tuple<int, PlayerFSM.HurtType>>()
                {
                    new(100, PlayerFSM.HurtType.Counter)
                }
            };
            
            HitSectionGroup = new StateMap<SectionGroup<Hit>>();
            
            HitSectionGroup.SuperDictionary[PlayerFSM.State.ForwardThrow] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new ( 4 , null ),
                    new (2, new Hit()
                    {
                        Type = Hit.HitType.Throw,
                        TriggerCutscene = PlayerFSM.CutsceneIndexes.ForwardThrow,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new ( 2, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowWidth = false,
                                            GrowHeight = false,
                                            PosY = 4,
                                            Width = FP.FromString("4.75"),
                                            Height = 1,
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new (10, null)
                }
            };
            
            HitSectionGroup.SuperDictionary[PlayerFSM.State.Backthrow] = new SectionGroup<Hit>()
            {
                Sections = new List<Tuple<int, Hit>>()
                {
                    new ( 4 , null ),
                    new (2, new Hit()
                    {
                        Type = Hit.HitType.Throw,
                        TriggerCutscene = PlayerFSM.CutsceneIndexes.BackwardThrow,
                        HitboxCollections = new SectionGroup<CollisionBoxCollection>()
                        {
                            Sections = new List<Tuple<int, CollisionBoxCollection>>()
                            {
                                new ( 2, new CollisionBoxCollection()
                                {
                                    CollisionBoxes = new List<CollisionBox>()
                                    {
                                        new CollisionBox()
                                        {
                                            GrowWidth = false,
                                            GrowHeight = false,
                                            PosY = 4,
                                            Width = FP.FromString("4.75"),
                                            Height = 1,
                                        }
                                    }
                                })
                            }
                        }
                    }),
                    new (10, null)
                }
            };
            
            Pushbox = new StateMap<CollisionBox>();
            MovementSectionGroup = new StateMap<SectionGroup<FP>>();
            MovementSectionGroup.Dictionary[PlayerFSM.State.SoftKnockdown] = new SectionGroup<FP>()
            {
                Sections = new List<Tuple<int, FP>>()
                {
                    new Tuple<int, FP>(20, -6)
                }
            };
            
            AllowCrossupSectionGroup = new StateMap<SectionGroup<bool>>();
            TrajectorySectionGroup = new StateMap<SectionGroup<Trajectory>>();
            CancellableAfter = new StateMap<int>();
            CancellableAfter.DefaultValue = 0;
            WhiffCancellable = new StateMap<bool>();
            WhiffCancellable.DefaultValue = false;
            FireReceiverFinishAfter = new StateMap<int>();
            FireReceiverFinishAfter.DefaultValue = 10;
            AttachPositionSectionGroup = new StateMap<SectionGroup<FPVector2>>();
            InvulnerableBefore = new StateMap<int>();
            InvulnerableBefore.DefaultValue = 0;
            Cutscenes = new Dictionary<int, Cutscene>();
            return;
            
            

            int GetStun(FrameParam frameParam)
            {
                if (frameParam is null) return 0;
                
                frameParam.f.Unsafe.TryGetPointer<StunData>(frameParam.EntityRef, out var stunData);
                var stun = stunData->stun;
                return stun;
            }

            int GetCutsceneReactorDuration(FrameParam frameParam)
            {
                if (frameParam is null) return 0;
                var cutscene = Util.GetActiveCutscene(frameParam.f, frameParam.EntityRef);
                return cutscene?.ReactorDuration ?? 0;
            }
        }
        
        
        
        public class ActionConfig
        {
            public int State = -1;
            public InputSystem.InputType InputType = InputSystem.InputType.P;
            public int CommandDirection = 5;
            public bool JumpCancellable = false;
            public bool DashCancellable = false;
            public bool GroundOk = true;
            public bool AirOk = false;
            public bool RawOk = true;
            public bool Crouching = false;
            public bool Aerial = false;
            public int InputWeight = 0;
            public bool IsCutscene = false;
        }
        
        // FSM helper functions

        
        protected void ConfigureAction(PlayerFSM fsm, ActionConfig actionConfig)
        {
            fsm.Fsm.Configure(actionConfig.State)
                .SubstateOf(actionConfig.Crouching ? PlayerFSM.State.Crouch : PlayerFSM.State.Stand)
                .SubstateOf(actionConfig.Aerial ? PlayerFSM.State.AirAction : PlayerFSM.State.GroundAction);
            
            if (actionConfig.IsCutscene) return;
            
            if (actionConfig.RawOk)
            {
                if (actionConfig.GroundOk)
                {
                    AllowRawFromState(fsm, actionConfig, PlayerFSM.State.GroundActionable);
                    AllowRawFromState(fsm, actionConfig, PlayerFSM.State.Dash);
                }
                if (actionConfig.AirOk)
                {
                    AllowRawFromState(fsm, actionConfig, PlayerFSM.State.AirActionable);
                    AllowRawFromState(fsm, actionConfig, PlayerFSM.State.AirDash);
                }
            }
            
            if (actionConfig.JumpCancellable)
            {
                fsm.Fsm.Configure(actionConfig.State)
                    .PermitIf(PlayerFSM.Trigger.Jump, PlayerFSM.State.AirActionable, Util.CanCancelNow);
            }

            if (actionConfig.DashCancellable)
            {
                fsm.Fsm.Configure(actionConfig.State)
                    .PermitIf(PlayerFSM.Trigger.Dash, PlayerFSM.State.AirActionable, Util.CanCancelNow);
            }
        }

        private static void AllowRawFromState(PlayerFSM fsm, ActionConfig actionConfig, int state)
        {
            fsm.Fsm.Configure(state)
                .PermitIf(PlayerFSM.Trigger.ButtonAndDirection,
                    actionConfig.State, 
                    param =>
                        Util.DoesInputMatch(actionConfig, param), 
                    actionConfig.InputWeight);
        }
        
        protected void MakeActionCancellable(PlayerFSM fsm, ActionConfig source,
            ActionConfig destination)
        {
            fsm.Fsm.Configure(source.State)
                .PermitIf(PlayerFSM.Trigger.ButtonAndDirection,
                    destination.State, 
                    param => 
                        (Util.CanCancelNow(param) && Util.DoesInputMatch(destination, param)), 
                    destination.InputWeight);
        }

    }
}