using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe partial class FSM
    {
        public static FPVector2 GetFirstKinematicsAttachPosition(Frame f, EntityRef entityRef)
        {
            
            // TODO: maybe have a character value for this
            
            return FPVector2.Zero;
        }
    }
}