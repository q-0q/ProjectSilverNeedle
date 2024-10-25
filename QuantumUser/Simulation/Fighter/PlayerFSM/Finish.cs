using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        private static int HardKnockdownFrames = 60;
        private static int SoftKnockdownFrames = 23;
        private static int LandsquatFrames = 5;
        private static int ThrowStartupFrames = 3;
        private static int ThrowRecoveryFrames = 60;
        private static int ThrowTechFrames = 20;
        
        public void DoFinish(Frame f)
        {
            Character character = Characters.GetPlayerCharacter(f, EntityRef);

            bool finish = false;
            f.Unsafe.TryGetPointer<StunData>(EntityRef, out var stunData);
            var stun = stunData->stun;
            if (Fsm.IsInState(State.GroundAction) || Fsm.IsInState(State.AirAction))
            {
                if (character.ActionDict == null) return;
                FighterAnimation currentAnimation = character.ActionDict[Fsm.State()].Animation;
                finish = (FramesInCurrentState(f) >= currentAnimation.SectionGroup.Duration());
            }
            else if (Fsm.IsInState(State.Hit) || Fsm.IsInState(State.Block))
            {
                finish = (FramesInCurrentState(f) >= stun);
            }
            else if (Fsm.IsInState(State.HardKnockdown))
            {
                finish = (FramesInCurrentState(f) >= HardKnockdownFrames);
            }
            else if (Fsm.IsInState(State.SoftKnockdown))
            {
                finish = (FramesInCurrentState(f) >= SoftKnockdownFrames);
            }
            else if (Fsm.IsInState(State.Landsquat))
            {
                finish = (FramesInCurrentState(f) >= LandsquatFrames);
            }
            else if (Fsm.IsInState(State.Dash))
            {
                finish = (FramesInCurrentState(f) >= character.DashMovementSectionGroup.Duration());
            }
            else if (Fsm.IsInState(State.Backdash))
            {
                finish = (FramesInCurrentState(f) >= character.BackdashMovementSectionGroup.Duration());
            }
            else if (Fsm.IsInState(State.ThrowStartup))
            {

                finish = (FramesInCurrentState(f) >= ThrowStartupFrames + 1);
            }
            else if (Fsm.IsInState(State.ThrowWhiff))
            {
                finish = (FramesInCurrentState(f) >= ThrowRecoveryFrames);
            }
            else if (Fsm.IsInState(State.FrontThrowConnect))
            {
                finish = (FramesInCurrentState(f) >= character.FrontThrowKinematics.Animation.SectionGroup.Duration());
            }
            else if (Fsm.IsInState(State.BackThrowConnect))
            {
                finish = (FramesInCurrentState(f) >= character.BackThrowKinematics.Animation.SectionGroup.Duration());
            }
            else if (Fsm.IsInState(State.ThrowTech))
            {
                finish = (FramesInCurrentState(f) >= ThrowTechFrames);
            }
            else if (Fsm.IsInState(State.KinematicReceiver))
            {
                var opponentFsm = Util.GetPlayerFSM(f, Util.GetOtherPlayer(f, EntityRef));
                var fireAfter = 0;
                if (opponentFsm.Fsm.IsInState(State.FrontThrowConnect))
                {
                    fireAfter = Characters.GetPlayerCharacter(f, Util.GetOtherPlayer(f, EntityRef)).FrontThrowKinematics
                        .FireReceiverFinishAfter;
                }
                else if (opponentFsm.Fsm.IsInState(State.BackThrowConnect))
                {
                    fireAfter = Characters.GetPlayerCharacter(f, Util.GetOtherPlayer(f, EntityRef)).BackThrowKinematics
                        .FireReceiverFinishAfter;
                }
                finish = (PlayerFSM.FramesInCurrentState(f, Util.GetOtherPlayer(f, EntityRef)) > fireAfter);
                
            }
            // else if (Fsm.State == State.Dash) { ...

            if (finish)
            {
                var param = new CollisionHitParams() { f = f, EntityRef = EntityRef };
                Fsm.Fire(Trigger.Finish, param);
            }
            
        }


        public void CheckForOpponentThrowTech(Frame f)
        {
            EntityRef otherPlayerEntityRef = Util.GetOtherPlayer(f, EntityRef);
            var opponentFsm = Util.GetPlayerFSM(f, otherPlayerEntityRef);

            if (opponentFsm.Fsm.IsInState(State.ThrowTech))
            {
                var frameParam = new FrameParam() { f = f, EntityRef = EntityRef };
                Fsm.Fire(Trigger.ThrowTech, frameParam);
            }

        }
    }
}