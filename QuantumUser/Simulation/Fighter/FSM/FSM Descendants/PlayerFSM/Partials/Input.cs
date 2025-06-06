using System;
using JetBrains.Annotations;
using Photon.Deterministic;
using Quantum.Types;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe partial class PlayerFSM
    {
        public override bool InputIsBuffered(InputSystem.InputType type, Frame f, EntityRef entityRef)
        {
            
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            
            if (type == (InputSystem.InputType)inputBuffer->type)
            {
                return inputBuffer->length != 0;
            }
            
            return false;
        }
        
        public override void AdvanceBuffer(InputBuffer* inputBuffer)
        {
            inputBuffer->length = Math.Max(0, inputBuffer->length - 1);
        }
        
        public override int GetBufferDirection(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            return inputBuffer->direction;
        }
        
        public override bool GetBufferType(Frame f, EntityRef entityRef, out InputSystem.InputType type)
        {
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            type =  (InputSystem.InputType)inputBuffer->type;
            return inputBuffer->length != 0;
        }
    }
}