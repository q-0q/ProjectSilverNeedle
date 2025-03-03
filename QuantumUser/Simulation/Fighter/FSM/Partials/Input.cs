using System;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;

namespace Quantum
{
    public unsafe partial class FSM
    {
        public virtual bool InputIsBuffered(InputSystem.InputType type, Frame f, EntityRef entityRef)
        {
            return false;
        }
    }
}