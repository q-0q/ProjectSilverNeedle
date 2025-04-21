using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
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
            // AddMeter(f, FP.FromString("0.1667"));
            
            // if (Fsm.IsInState(PlayerState.Surge))
            // {
            //     f.Unsafe.TryGetPointer<SlowdownData>(EntityRef, out var slowdownData);
            //     slowdownData->slowdownRemaining = 0;
            //     StartMomentum(f, 0);
            // }
            if (Fsm.IsInState(PlayerState.Break))
            {
                AddMeter(f, FP.FromString("-0.7"));
            }
            
            
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
            ;
            var framesInMomentum = Util.FramesFromVirtualTime(momentumData->virtualTimeInMomentum);
            if (framesInMomentum >= _pushbackDuration) return;

            FPVector2 v =
                new FPVector2(GetMomentumVelocityThisFrame(framesInMomentum,
                    momentumData->momentumAmount), 0);

            ApplyFlippedMovement(f, v, EntityRef);
        }
        
        protected override void PushbackMove(Frame f)
        {
            f.Unsafe.TryGetPointer<PushbackData>(EntityRef, out var pushbackData);

            // return;
            
            var framesInPushback = Util.FramesFromVirtualTime(pushbackData->virtualTimeInPushback);
            if (framesInPushback >= _pushbackDuration) return;

            var pushbackVelocityThisFrame = GetPushbackVelocityThisFrame(framesInPushback,
                pushbackData->pushbackAmount);
            if (Fsm.IsInState(PlayerState.Block))
            {
                Debug.Log(pushbackVelocityThisFrame);
            }
            FPVector2 v =
                new FPVector2(pushbackVelocityThisFrame, 0);

            bool inCorner = Util.IsPlayerInCorner(f, EntityRef);
            EntityRef entityRef = inCorner ? Util.GetOtherPlayer(f, EntityRef) : EntityRef;
            
            // This is like the hackiest workaround ever for the problem related to corner pushback
            // reversion while under a time modifier causing the other player to get 1/x times pushback
            // due to reasons that I really honestly cannot understand for the life of me but this seems
            // to nullify it in most cases
            FP slowMod = inCorner ? GetSlowdownMod(f, EntityRef) : 1;
            if (inCorner) v.X *= (FP.Minus_1 * slowMod);

            ApplyFlippedMovement(f, v, entityRef, true);
        }
    }
}