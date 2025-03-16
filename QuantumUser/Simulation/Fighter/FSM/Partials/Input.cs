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
        
        public virtual void AdvanceBuffer(InputBuffer* inputBuffer) { }
        
        public virtual int GetBufferDirection(Frame f, EntityRef entityRef) { return 5; }
        
        public virtual bool GetBufferType(Frame f, EntityRef entityRef, out InputSystem.InputType type)
        {
            type = InputSystem.InputType.L;
            return false;
        }
    }
}