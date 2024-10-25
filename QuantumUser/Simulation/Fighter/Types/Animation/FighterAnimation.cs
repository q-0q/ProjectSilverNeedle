namespace Quantum.Types
{
    public unsafe class FighterAnimation
    {
        public SectionGroup<int> SectionGroup;
        public int SpriteSheetOffset;

        public virtual void SetSpriteForFsm(Frame f, PlayerFSM fsm)
        {
            int frame = SectionGroup.GetCurrentItem(f, fsm);
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame + SpriteSheetOffset;
        }
    }
}