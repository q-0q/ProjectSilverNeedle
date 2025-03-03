using System;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public class Fireball : SummonFSM
    {
        public class FireballState : SummonState
        {
            
        }

        public enum FireballAnimationPath
        {

        }
        
        public Fireball()
        {
            Name = "Fireball";
            StateType = typeof(FireballState);
            AnimationPathsEnum = typeof(FireballAnimationPath);
            KinematicAttachPointOffset = FPVector2.Zero;
        }

        public override void SetupStateMaps()
        {
            base.SetupStateMaps();
            
        }

        public override void SetupMachine()
        {
            base.SetupMachine();
            
        }
    }
}



        
