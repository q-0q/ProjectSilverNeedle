namespace Quantum.Types
{
    public abstract unsafe class AnimationEntity
    {
        public SectionGroup<int> SectionGroup;
        public string SpriteDirectory;
        public int SpriteSheetOffset;
        public bool UI = false;
        public AELayer Layer = AELayer.Middle;

        public virtual void SetSpriteForFsm(Frame f, PlayerFSM fsm)
        {
            int frame = SectionGroup.GetCurrentItem(f, fsm);
            f.Unsafe.TryGetPointer<AnimationData>(fsm.EntityRef, out var animationData);
            animationData->frame = frame + SpriteSheetOffset;
        }
    }
}