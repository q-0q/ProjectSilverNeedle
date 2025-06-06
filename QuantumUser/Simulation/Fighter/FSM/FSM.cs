using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum.QuantumUser.Simulation.Fighter.Types;
using Quantum.StateMap;
using Quantum.Types;
using Quantum.Types.Collision;
using Wasp;

namespace Quantum
{
    public abstract unsafe partial class FSM
    {
        public class FSMState : InheritableEnum.InheritableEnum { }

        public class Trigger : InheritableEnum.InheritableEnum
        {
            public static int Finish;
            public static int NeutralInput;
            public static int Down;
            public static int Forward;
            public static int Backward;
            public static int Dash;
            public static int NotDash;
            public static int Backdash;
            public static int ForwardThrow;
            public static int BackThrow;
            public static int Tech;
            public static int ButtonAndDirection;
            public static int Jump;
        }
        
        public FPVector2 KinematicAttachPointOffset;
        public string Name;

        
        public EntityRef EntityRef;
        public Machine<int, int> Fsm;
        public StateMapConfig StateMapConfig;
        public FP DamageDealtModifier = 1;
        public Type StateType;
        
        public Dictionary<int, Cutscene> Cutscenes;
        public List<SummonPool> SummonPools;
        public bool sendToBack = false;
        public bool IgnoreHitstop = false;
        public FP SpriteScale = 1;

        
        public enum JumpType
        {
            Up,
            Forward,
            Backward
        }


        public virtual void SetupStateMaps()
        {
            StateMapConfig = new StateMapConfig();
            StateMapConfig.FighterAnimation = new StateMap<FighterAnimation>();
            StateMapConfig.Duration = new StateMap<int>();
            StateMapConfig.HurtboxCollectionSectionGroup = new StateMap<SectionGroup<CollisionBoxCollection>>();
            StateMapConfig.HurtTypeSectionGroup = new StateMap<SectionGroup<PlayerFSM.HurtType>>();
            StateMapConfig.HitSectionGroup = new StateMap<SectionGroup<Hit>>();
            StateMapConfig.Pushbox = new StateMap<CollisionBox>();
            StateMapConfig.MovementSectionGroup = new StateMap<SectionGroup<FP>>();
            StateMapConfig.AllowCrossupSectionGroup = new StateMap<SectionGroup<bool>>();
            StateMapConfig.TrajectorySectionGroup = new StateMap<SectionGroup<Trajectory>>();
            StateMapConfig.TrajectoryYVelocityMod = new StateMap<SectionGroup<FP>>();
            StateMapConfig.TrajectoryXVelocityMod = new StateMap<SectionGroup<FP>>();
            StateMapConfig.CancellableAfter = new StateMap<int>();
            StateMapConfig.WhiffCancellable = new StateMap<bool>();
            StateMapConfig.FireReceiverFinishAfter = new StateMap<int>();
            StateMapConfig.InvulnerableBefore = new StateMap<int>();
            StateMapConfig.ProjectileInvulnerable = new StateMap<SectionGroup<bool>>();
            StateMapConfig.UnpoolSummonSectionGroup = new StateMap<SectionGroup<SummonPool>>();
            StateMapConfig.SmearFrame = new StateMap<SectionGroup<int>>();
            Cutscenes = new Dictionary<int, Cutscene>();
            
        }

        public virtual void SetupMachine()
        {
            Fsm.OnTransitioned(OnStateChanged);
        }
        
        protected virtual void OnStateChanged(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;
            ResetStateEnteredFrame(param.f);
            // Util.WritebackFsm(param.f, EntityRef);
        }

        protected void ResetStateEnteredFrame(Frame f)
        {
            f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var playerFsmData);
            playerFsmData->framesInState = 0;
            playerFsmData->virtualTimeInState = 0;
        }

        public int FramesInCurrentState(Frame f)
        {
            // if (EntityRef == EntityRef.None) return 0;
            f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var playerFsmData);
            return Util.FramesFromVirtualTime(playerFsmData->virtualTimeInState);
        }

        public virtual bool IsTimeStopped(Frame f)
        {
            return HitstopSystem.IsHitstopActive(f) && !IgnoreHitstop;
        }
        
        public int RealtimeFramesInCurrentState(Frame f)
        {
            // if (EntityRef == EntityRef.None) return 0;
            f.Unsafe.TryGetPointer<FSMData>(EntityRef, out var playerFsmData);
            return playerFsmData->framesInState;
        }

        public static int FramesInCurrentState(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<FSMData>(entityRef, out var playerFsmData);
            return Util.FramesFromVirtualTime(playerFsmData->virtualTimeInState);
        }
        
        public virtual void TryToFireJump(Frame f, JumpType type)
        {
        }
        
        public virtual void CheckForLand(Frame f)
        {
        }
        
        public virtual void TrajectoryArc(Frame f)
        {
        }
        
        public virtual void ReportFrameMeterType(Frame f)
        {
        }

        public abstract EntityRef GetPlayer();

        public FSM GetPlayerFsm()
        {
            return FsmLoader.FSMs[GetPlayer()];
        }
        
        public virtual void IncrementClockByAmount(Frame f, EntityRef entityRef, FP amount)
        {
            // if (EntityRef == EntityRef.None) return;
            FP virtualTimeIncrement = amount;
            f.Unsafe.TryGetPointer<FSMData>(entityRef, out var playerFsmData);

            var firstFrameOfHitBefore = IsOnFirstFrameOfHit(f);
            playerFsmData->framesInState++;
            playerFsmData->virtualTimeInState += virtualTimeIncrement;
            var firstFrameOfHitAfter = IsOnFirstFrameOfHit(f);
            if (!firstFrameOfHitBefore && firstFrameOfHitAfter)
            {
                ClearHitEntities(f);
            }
        }
    }
}