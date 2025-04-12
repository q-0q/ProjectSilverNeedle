using Photon.Deterministic;
using Quantum.Types;
using Wasp;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        
        private static FP _throwTechPushback = FP.FromString("6");

        private void CutsceneReactorMove(Frame f)
        {
            if (!Fsm.IsInState(PlayerState.CutsceneReactor)) return;

            var cutscene = Util.GetActiveCutscene(f, EntityRef);

            f.Unsafe.TryGetPointer<CutsceneData>(EntityRef, out var cutsceneData);
            f.Unsafe.TryGetPointer<Transform3D>(cutsceneData->initiator, out var transform3D);
            var initiatorPos = transform3D->Position.XY;
            var currentCutscenePos = cutscene.ReactorPositionSectionGroup.GetCurrentItem(f, this);
            if (!cutsceneData->initiatorFacingRight) currentCutscenePos.X *= -1;
            FPVector2 offset = KinematicAttachPointOffset;
            SetPosition(f, (initiatorPos + currentCutscenePos) - offset);
        }
        
        public override void Move(Frame f)
        {
            AddMeter(f, FP.FromString("1.667"));
            CutsceneReactorMove(f);
            SnapToGround(f);
            base.Move(f);
        }
        
        private void SnapToGround(Frame f)
        {
            if (!Fsm.IsInState(PlayerFSM.PlayerState.Ground)) return;
            f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            transform3D->Position.Y = 0;
        }
        
        public override FP GetSlowdownMod(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<SlowdownData>(entityRef, out var slowdownData);
            return slowdownData->slowdownRemaining <= 0 ? 1 : slowdownData->multiplier;
        }
        
        protected override void MomentumMove(Frame f)
        {
            f.Unsafe.TryGetPointer<MomentumData>(EntityRef, out var momentumData);
            if (momentumData->framesInMomentum >= _pushbackDuration) return;

            FPVector2 v =
                new FPVector2(GetMomentumVelocityThisFrame(momentumData->framesInMomentum,
                    momentumData->momentumAmount), 0);

            ApplyFlippedMovement(f, v, EntityRef);
        }
        
        protected override void PushbackMove(Frame f)
        {
            f.Unsafe.TryGetPointer<PushbackData>(EntityRef, out var pushbackData);

            var framesInPushback = pushbackData->framesInPushback;
            if (framesInPushback >= _pushbackDuration) return;

            FPVector2 v =
                new FPVector2(GetPushbackVelocityThisFrame(framesInPushback,
                    pushbackData->pushbackAmount), 0);

            bool inCorner = Util.IsPlayerInCorner(f, EntityRef);
            EntityRef entityRef = inCorner ? Util.GetOtherPlayer(f, EntityRef) : EntityRef;
            if (inCorner) v.X *= FP.Minus_1;

            ApplyFlippedMovement(f, v, entityRef, true);
        }
        
        private void OnEnterThrowTech(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var frameParam = (FrameParam)triggerParams;

            var distance = _throwTechPushback;
            if (IsOnLeft(frameParam.f, EntityRef)) distance *= FP.Minus_1;
            StartPushback(frameParam.f, distance);
            Util.StartDramatic(frameParam.f, EntityRef, 35);

            // only make the tech animation sprite for player 0
            if (Util.GetPlayerId(frameParam.f, EntityRef) != 0) return;

            frameParam.f.Unsafe.TryGetPointer<Transform3D>(EntityRef, out var transform3D);
            frameParam.f.Unsafe.TryGetPointer<Transform3D>(Util.GetOtherPlayer(frameParam.f, EntityRef),
                out var opponentTransform3D);

            FPVector2 attachPos = GetFirstKinematicsAttachPosition(frameParam.f, EntityRef);
            FPVector2 opponentAttachPos =
                GetFirstKinematicsAttachPosition(frameParam.f, Util.GetOtherPlayer(frameParam.f, EntityRef));

            FPVector2 pos = attachPos + transform3D->Position.XY;
            FPVector2 opponentPos = opponentAttachPos + opponentTransform3D->Position.XY;

            FPVector2 avgPos = (pos + opponentPos) * FP._0_50;

            AnimationEntitySystem.Create(frameParam.f, AnimationEntities.AnimationEntityEnum.Tech,
                avgPos, 0,
                false);
        }
    }
}