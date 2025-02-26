using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Deterministic;
using Quantum.Collections;
using UnityEngine;
using Wasp;
using Plane = Photon.Deterministic.Plane;


namespace Quantum
{
    public unsafe class InputSystem : SystemMainThreadFilter<InputSystem.Filter>
    {
        private const int BufferWindowSize = 18;

        public enum InputType
        {
            P,
            K,
            S,
            H,
            D,
            Jump,
            Dash
            
        }
        
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerLink* PlayerLink;
            public InputBuffer* InputBuffer;
        }
        
        
        public override void Update(Frame f, ref Filter filter)
        {
            if (GameFSMSystem.GetGameState(f) != GameFSM.State.Playing) return;
            
            AdvanceBuffer(filter.InputBuffer);
            BufferInputs(f, filter.Entity);
        }

        private void BufferInputs(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            Input input = *f.GetPlayerInput(playerLink->Player);
            
            
            bool resetBufferLength = false;
            if (input.P.WasPressed)
            {
                inputBuffer->type = (int)InputType.P;
                resetBufferLength = true;
            }
            if (input.K.WasPressed)
            {
                inputBuffer->type = (int)InputType.K;
                resetBufferLength = true;
            }
            if (input.S.WasPressed)
            {
                inputBuffer->type = (int)InputType.S;
                resetBufferLength = true;
            }
            if (input.H.WasPressed)
            {
                inputBuffer->type = (int)InputType.H;
                resetBufferLength = true;
            }
            if (input.D.WasPressed)
            {
                inputBuffer->type = (int)InputType.D;
                resetBufferLength = true;
            }
            if (input.Jump.WasPressed)
            {
                inputBuffer->type = (int)InputType.Jump;
                resetBufferLength = true;
            }
            if (input.Dash.WasPressed)
            {
                inputBuffer->type = (int)InputType.Dash;
                resetBufferLength = true;
            }
            

            if (resetBufferLength)
            {
                inputBuffer->length = BufferWindowSize;
                inputBuffer->direction = Numpad(f, entityRef);
            }
        }

        public static void FireFsmFromInput(Frame f, PlayerFSM fsm)
        {
            if (GameFSMSystem.GetGameState(f) != GameFSM.State.Playing) return;
            
            FireJump(f, fsm);
            FireDash(f, fsm);
            FireAction(f, fsm);
            FireMovement(f, fsm);
        }

        private static void FireAction(Frame f, PlayerFSM fsm)
        {
            var interactionControllerData = Util.GetInteractionControllerData(f);
            if (interactionControllerData.enabled)
            {
                FireInteractionControllerAction(f, fsm, interactionControllerData);
            }
            else if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                FireCpuAction(f, fsm);
            }
            else
            {
                FireHumanAction(f, fsm);
            }
        }

        private static void FireInteractionControllerAction(Frame f, PlayerFSM fsm, 
            InteractionControllerData interactionControllerData)
        {
            
            int playerId = Util.GetPlayerId(f, fsm.EntityRef);
            int onFrame = playerId == 0
                ? interactionControllerData.player0Frame
                : interactionControllerData.player1Frame;
            
            if (f.Number != onFrame) return;
            
            InputType type = playerId == 0
                ? (InputType)interactionControllerData.player0InputType
                : (InputType)interactionControllerData.player1InputType;
            
            int commandDirection = playerId == 0
                ? interactionControllerData.player0CommandDirection
                : interactionControllerData.player1CommandDirection;
            
            FireButtonAndDirectionTrigger(f, fsm, type, commandDirection);
        }

        private static void FireCpuAction(Frame f, PlayerFSM fsm)
        {
            foreach (var (_, cpuControllerData) in f.GetComponentIterator<CpuControllerData>())
            {
                if (!cpuControllerData.doAction) return;
                var type = (InputType)cpuControllerData.inputType;
                var commandDirection = cpuControllerData.commandDirection;
                FireThrowTrigger(f, fsm, type, commandDirection);
                FireButtonAndDirectionTrigger(f, fsm, type, commandDirection);
                return;
            }
        }

        private static void FireHumanAction(Frame f, PlayerFSM fsm)
        {
            if (GetBufferType(f, fsm.EntityRef, out var type))
            {
                if (type == InputType.Jump) return;
                int commandDirection = GetBufferDirection(f, fsm.EntityRef);
                FireThrowTrigger(f, fsm, type, commandDirection);
                FireButtonAndDirectionTrigger(f, fsm, type, commandDirection);
            }
        }
        
        
        private static void FireDash(Frame f, PlayerFSM fsm)
        {
            FrameParam param = new FrameParam() { f = f, EntityRef = fsm.EntityRef};
            
            if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                foreach (var (_, cpuControllerData) in f.GetComponentIterator<CpuControllerData>())
                {
                    if (!cpuControllerData.dash) return;
                    fsm.Fsm.Fire(PlayerFSM.Trigger.Dash, param);
                    return;
                }
                return;
            }
            
            // Debug.Log(fsm.EntityRef);
            
            int commandDirection = GetBufferDirection(f, fsm.EntityRef);
            
            if (InputIsBuffered(InputType.Dash, f, fsm.EntityRef) && (commandDirection is 1 or 4 or 7))
            {
                fsm.Fsm.Fire(PlayerFSM.Trigger.Backdash, param);
            }
            else if (InputIsBuffered(InputType.Dash, f, fsm.EntityRef))
            {
                fsm.Fsm.Fire(PlayerFSM.Trigger.Dash, param);
            }
        }
        
        

        private static void FireJump(Frame f, PlayerFSM fsm)
        {
            if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                if (fsm.Fsm.IsInState(PlayerFSM.State.Ground))
                {
                    foreach (var (_, cpuControllerData) in f.GetComponentIterator<CpuControllerData>())
                    {
                        if (!cpuControllerData.doJump) return;
                        fsm.TryToFireJump(f, PlayerFSM.JumpType.Up);
                        return;
                    }
                }
                return;
            }
            
            int numpad = Numpad(f, fsm.EntityRef);
            
            if (fsm.Fsm.IsInState(PlayerFSM.State.Ground))
            {
                switch (numpad)
                {
                    case 7:
                    {
                        fsm.TryToFireJump(f, PlayerFSM.JumpType.Backward);
                        return;
                        break;
                    }
                    case 8:
                    {
                        fsm.TryToFireJump(f, PlayerFSM.JumpType.Up);
                        return;
                        break;
                    }
                    case 9:
                    {
                        fsm.TryToFireJump(f, PlayerFSM.JumpType.Forward);
                        return;
                        break;
                    }
                }
            }
            
            if (InputIsBuffered(InputType.Jump, f, fsm.EntityRef))
            {
                switch (GetBufferDirection(f, fsm.EntityRef))
                {
                    case 7:
                    {
                        fsm.TryToFireJump(f, PlayerFSM.JumpType.Backward);
                        break;
                    }
                    case 8:
                    {
                        fsm.TryToFireJump(f, PlayerFSM.JumpType.Up);
                        break;
                    }
                    case 9:
                    {
                        fsm.TryToFireJump(f, PlayerFSM.JumpType.Forward);
                        break;
                    }
                }
            }
        }

        private static void FireMovement(Frame f, PlayerFSM fsm)
        {
            
            FrameParam param = new FrameParam() { f = f, EntityRef = fsm.EntityRef};

            if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                fsm.Fsm.Fire(PlayerFSM.Trigger.NeutralInput, param);
                return;
            }
                
            int numpad = Numpad(f, fsm.EntityRef);

            switch (numpad)
            {
                case 1:
                {
                    fsm.Fsm.Fire(PlayerFSM.Trigger.Down, param);
                    break;
                }
                case 2:
                {
                    fsm.Fsm.Fire(PlayerFSM.Trigger.Down, param);
                    break;
                }
                case 3:
                {
                    fsm.Fsm.Fire(PlayerFSM.Trigger.Down, param);
                    break;
                }
                case 4:
                {
                    fsm.Fsm.Fire(PlayerFSM.Trigger.Backward, param);
                    break;
                }
                case 5:
                {
                    fsm.Fsm.Fire(PlayerFSM.Trigger.NeutralInput, param);
                    break;
                }
                case 6:
                {
                    fsm.Fsm.Fire(PlayerFSM.Trigger.Forward, param);
                    break;
                }
            }
        }

        public static int Numpad(Frame f, EntityRef entityRef)
        {
            if (Util.EntityIsCpu(f, entityRef)) return 5;
            
            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            
            Input input = *f.GetPlayerInput(playerLink->Player);
            
            bool facingRight = PlayerDirectionSystem.IsOnLeft(f, entityRef);
            

            if (facingRight) return input.UnflippedNumpadDirection;

            switch (input.UnflippedNumpadDirection)
            {
                case 9:
                {
                    return 7;
                }
                
                case 6:
                {
                    return 4;
                }
                
                case 3:
                {
                    return 1;
                }
                
                case 7:
                {
                    return 9;
                }
                
                case 4:
                {
                    return 6;
                }
                
                case 1:
                {
                    return 3;
                }

                default:
                {
                    return input.UnflippedNumpadDirection;
                }
            }
        }
        
        
        
        public static bool InputIsBuffered(InputType type, Frame f, EntityRef entityRef)
        {
            
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            
            if (type == (InputType)inputBuffer->type)
            {
                return inputBuffer->length != 0;
            }
            
            return false;
        }

        public static bool InputIsDown(string action, Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            Input input = *f.GetPlayerInput(playerLink->Player);
            
            if (action == "P")
            {
                return input.P.IsDown;
            }
            if (action == "K")
            {
                return input.K.IsDown;
            }
            if (action == "S")
            {
                return input.S.IsDown;
            }
            if (action == "H")
            {
                return input.H.IsDown;
            }
            if (action == "D")
            {
                return input.D.IsDown;
            }
                
            if (action == Numpad(f, entityRef).ToString())
            {
                return true;
            }

            return false;
        }

        private void AdvanceBuffer(InputBuffer* inputBuffer)
        {
            inputBuffer->length = Math.Max(0, inputBuffer->length - 1);
        }
        
        private static int GetBufferDirection(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            return inputBuffer->direction;
        }
        
        private static bool GetBufferType(Frame f, EntityRef entityRef, out InputType type)
        {
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            type =  (InputType)inputBuffer->type;
            return inputBuffer->length != 0;
        }

        private static void FireButtonAndDirectionTrigger(Frame f, PlayerFSM fsm, InputType type, int commandDirection)
        {
            ButtonAndDirectionParam param = new ButtonAndDirectionParam() { f = f, Type = type, CommandDirection = commandDirection, EntityRef = fsm.EntityRef};

            fsm.Fsm.Fire(PlayerFSM.Trigger.ButtonAndDirection, param);
        }
        
        private static void FireThrowTrigger(Frame f, PlayerFSM fsm, InputType type, int commandDirection)
        {
            if (type != InputType.D) return;
            
            ButtonAndDirectionParam param = new ButtonAndDirectionParam() { f = f, Type = type, CommandDirection = commandDirection, EntityRef = fsm.EntityRef};

            var trigger = NumpadMatchesNumpad(commandDirection, 4)
                ? PlayerFSM.Trigger.BackThrow
                : PlayerFSM.Trigger.ForwardThrow;
            
            fsm.Fsm.Fire(trigger, param);
        }

        public static void ClearBufferParams(TriggerParams? triggerParams)
        {
            if (triggerParams is null) return;
            var param = (FrameParam)triggerParams;

            // if (param.EntityRef == EntityRef.None)
            // {
            //     Debug.Log("clear buffer entity null");
            // }
            
            ClearBuffer(param.f, param.EntityRef);
        }

        public static void ClearBuffer(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            inputBuffer->length = 0;
        }

        public static bool NumpadMatchesNumpad(int actual, int desired)
        {
            switch (desired)
            {
                case 5:
                {
                    return true;
                    break;
                }
                case 2:
                {
                    return actual is 1 or 2 or 3;
                    break;
                }
                case 6:
                {
                    return actual is 3 or 6 or 9;
                    break;
                }
                case 4:
                {
                    return actual is 1 or 4 or 7;
                    break;
                }
                case 8:
                {
                    return actual is 7 or 8 or 9;
                    break;
                }
            }

            return false;
        }
        
    }
    
    
    
}