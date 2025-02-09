namespace Quantum.Types
{
    public unsafe class FighterAnimation
    {
        public SectionGroup<int> SectionGroup;
        public int Path;

        public virtual void SetAnimationPathForFsm(Frame f, PlayerFSM fsm)
        {
            int frame = SectionGroup.GetCurrentItem(f, fsm);
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame;
            animationData->path = Path;
        }
    }
}