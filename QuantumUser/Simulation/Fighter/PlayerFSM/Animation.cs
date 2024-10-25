using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        public void Animation(Frame f)
        {
            Character character = Characters.GetPlayerCharacter(f, EntityRef);
            FighterAnimation currentAnimation;
            
            if (Fsm.State() == State.StandActionable)
            {
                currentAnimation = character.StandAnimation;
            }
            else if (Fsm.State() == State.CrouchActionable)
            {
                currentAnimation = character.CrouchAnimation;
            }
            else if (Fsm.State() == State.WalkForward)
            {
                currentAnimation = character.WalkForwardAnimation;
            }
            else if (Fsm.State() == State.WalkBackward)
            {
                currentAnimation = character.WalkBackwardAnimation;
            }
            else if (Fsm.State() == State.AirActionable)
            {
                if (IsRising(f)) currentAnimation = character.AirActionableRisingAnimation;
                else currentAnimation = character.AirActionableFallingAnimation;
            }
            else if (Fsm.IsInState(State.Action))
            {
                if (character.ActionDict == null) currentAnimation = character.StandAnimation;
                else currentAnimation = character.ActionDict[Fsm.State()].Animation;
            }
            else if (Fsm.IsInState(State.ThrowStartup))
            {
                currentAnimation = character.ThrowStartupAnimation;
            }
            else if (Fsm.State() == State.ThrowWhiff)
            {
                currentAnimation = character.ThrowWhiffAnimation;
            }
            else if (Fsm.State() == State.FrontThrowConnect)
            {
                currentAnimation = character.FrontThrowKinematics.Animation;
            }
            else if (Fsm.State() == State.BackThrowConnect)
            {
                currentAnimation = character.BackThrowKinematics.Animation;
            }
            else if (Fsm.State() == State.ThrowTech)
            {
                currentAnimation = character.ThrowTechAnimation;
            }
            else if (Fsm.IsInState(State.KinematicReceiver))
            {
                currentAnimation = character.KinematicReceiverAnimation;
            }
            else if (Fsm.State() == State.StandHitHigh)
            {
                currentAnimation = character.StandHitHighAnimation;
            }
            else if (Fsm.State() == State.StandHitLow)
            {
                currentAnimation = character.StandHitLowAnimation;
            }
            else if (Fsm.State() == State.CrouchHit)
            {
                currentAnimation = character.CrouchHitAnimation;
            }
            else if (Fsm.State() == State.AirHit || Fsm.State() == State.AirHitPostWallBounce)
            {
                if (IsRising(f)) currentAnimation = character.AirHitRisingAnimation;
                else currentAnimation = character.AirHitFallingAnimation;
            }
            else if (Fsm.State() == State.AirHitPostGroundBounce)
            {
                if (IsRising(f)) currentAnimation = character.AirHitPostGroundBounceRisingAnimation;
                else currentAnimation = character.AirPostHitGroundBounceFallingAnimation;
            }
            else if (Fsm.State() == State.StandBlock)
            {
                currentAnimation = character.StandBlockAnimation;
            }
            else if (Fsm.State() == State.CrouchBlock)
            {
                currentAnimation = character.CrouchBlockAnimation;
            }
            else if (Fsm.State() == State.AirBlock)
            {
                currentAnimation = character.AirBlockAnimation;
            }
            else if (Fsm.State() == State.HardKnockdown)
            {
                currentAnimation = character.HardKnockdownAnimation;
            }
            else if (Fsm.State() == State.SoftKnockdown)
            {
                currentAnimation = character.SoftKnockdownAnimation;
            }
            else if (Fsm.State() == State.DeadFromAir)
            {
                currentAnimation = character.DeadFromAirAnimation;
            }
            else if (Fsm.State() == State.DeadFromGround)
            {
                currentAnimation = character.DeadFromGroundAnimation;
            }
            else if (Fsm.State() == State.Landsquat)
            {
                currentAnimation = character.LandsquatAnimation;
            }
            else if (Fsm.State() == State.Dash)
            {
                currentAnimation = character.DashAnimation;
            }
            else if (Fsm.State() == State.Backdash)
            {
                currentAnimation = character.BackdashAnimation;
            }
            else if (Fsm.State() == State.AirDash)
            {
                currentAnimation = character.AirdashAnimation;
            }
            else if (Fsm.State() == State.AirBackdash)
            {
                currentAnimation = character.AirBackdashAnimation;
            }
            else
            {
                currentAnimation = character.StandAnimation;
            }

            currentAnimation.SetSpriteForFsm(f, this);
        }
    }
}