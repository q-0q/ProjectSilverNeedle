using Photon.Deterministic;
using Quantum.QuantumUser.Simulation.Fighter.Types;
using Quantum.Types;
using Quantum.Types.Collision;

namespace Quantum.StateMap
{
    public class StateMapConfig
    {
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
        public StateMap<int> InvulnerableBefore;
        public StateMap<SectionGroup<bool>> ProjectileInvulnerable;
        public StateMap<SectionGroup<SummonPool>> UnpoolSummonSectionGroup;

    }
}