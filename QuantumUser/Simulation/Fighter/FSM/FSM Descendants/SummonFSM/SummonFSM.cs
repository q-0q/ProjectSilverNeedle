using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;
using Wasp;


namespace Quantum
{
    public abstract unsafe partial class SummonFSM : FSM
    {
        public class SummonState : FSMState
        {
            // Ground
            public static int StandActionable;
            public static int CrouchActionable;
            public static int WalkForward;
            public static int WalkBackward;
            public static int Dash;
            public static int Backdash;
            public static int GroundAction;
            public static int GroundActionable;
            public static int StandHitHigh;
            public static int StandHitLow;
            public static int CrouchHit;
            public static int StandBlock;
            public static int CrouchBlock;
            public static int GroundBlock;
            public static int HardKnockdown;
            public static int SoftKnockdown;
            public static int Landsquat;
            public static int DeadFromAir;
            public static int DeadFromGround;
            public static int ForwardThrow;
            public static int Backthrow;

            // Air
            public static int AirDash;
            public static int AirBackdash;
            public static int AirAction;
            public static int AirActionable;
            public static int AirHit;
            public static int AirBlock;
            public static int AirHitPostGroundBounce;
            public static int AirHitPostWallBounce;


            // General
            public static int Hit;
            public static int Ground;
            public static int Air;
            public static int Any;
            public static int Action;
            public static int Stand;
            public static int Crouch;
            public static int Block;
            public static int Throw;
            public static int CutsceneReactor;
        }
        
        public class SummonTrigger : Trigger
        {
            public static int Finish;
            public static int NeutralInput;
            public static int Down;
            public static int Forward;
            public static int Backward;
            public static int Dash;
            public static int Backdash;
            public static int ForwardThrow;
            public static int BackThrow;
            public static int ButtonAndDirection;
            public static int Jump;
            public static int Land;
            public static int HitWall;
            public static int JumpCancel;
            public static int HitHigh;
            public static int HitLow;
            public static int BlockHigh;
            public static int BlockLow;
            public static int Die;
        }
        
        public SummonFSM()
        {
            int currentState = SummonState.StandActionable;
            Fsm = new Machine<int, int>(currentState);
        }
        
        public override void SetupMachine()
        {
            base.SetupMachine();
            
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();

        }
    }

}