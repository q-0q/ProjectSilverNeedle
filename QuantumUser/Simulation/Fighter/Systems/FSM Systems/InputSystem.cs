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
            L,
            M,
            H,
            S,
            T,
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
            if (GameFsmLoader.LoadGameFSM(f).Fsm.State() != GameFSM.State.Playing) return;
            
            var fsm = FsmLoader.FSMs[filter.Entity];
            if (fsm is null) return;
                
            fsm.AdvanceBuffer(filter.InputBuffer);
            BufferInputs(f, filter.Entity);
        }

        private void BufferInputs(Frame f, EntityRef entityRef)
        {
            f.Unsafe.TryGetPointer<PlayerLink>(FsmLoader.FSMs[entityRef].GetPlayer(), out var playerLink);
            f.Unsafe.TryGetPointer<InputBuffer>(entityRef, out var inputBuffer);
            Input input = *f.GetPlayerInput(playerLink->Player);
            
            
            bool resetBufferLength = false;
            if (input.L.WasPressed)
            {
                inputBuffer->type = (int)InputType.L;
                resetBufferLength = true;
            }
            if (input.M.WasPressed)
            {
                inputBuffer->type = (int)InputType.M;
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
            if (input.T.WasPressed)
            {
                inputBuffer->type = (int)InputType.T;
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

        public static void FireFsmFromInput(Frame f, FSM fsm)
        {
            if (GameFsmLoader.LoadGameFSM(f).Fsm.State() != GameFSM.State.Playing) return;
            
            FireJump(f, fsm);
            FireDash(f, fsm);
            FireButtonAndDirection(f, fsm);
            FireDirection(f, fsm);
        }

        private static void FireButtonAndDirection(Frame f, FSM fsm)
        {
            var interactionControllerData = Util.GetInteractionControllerData(f);
            if (interactionControllerData.enabled)
            {
                FireInteractionControllerButtonAndDirection(f, fsm, interactionControllerData);
            }
            else if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                FireCpuButtonAndDirection(f, fsm);
            }
            else
            {
                FireHumanButtonAndDirection(f, fsm);
            }
        }

        private static void FireInteractionControllerButtonAndDirection(Frame f, FSM fsm, 
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

        private static void FireCpuButtonAndDirection(Frame f, FSM fsm)
        {
            foreach (var (_, cpuControllerData) in f.GetComponentIterator<CpuControllerData>())
            {
                if (!cpuControllerData.doAction) return;
                Debug.Log("fire cpu action");
                var type = (InputType)cpuControllerData.inputType;
                var commandDirection = cpuControllerData.commandDirection;
                FireThrowTrigger(f, fsm, type, commandDirection);
                FireButtonAndDirectionTrigger(f, fsm, type, commandDirection);
                return;
            }
        }

        private static void FireHumanButtonAndDirection(Frame f, FSM fsm)
        {
            if (fsm.GetBufferType(f, fsm.EntityRef, out var type))
            {
                if (type == InputType.Jump) return;
                int commandDirection = fsm.GetBufferDirection(f, fsm.EntityRef);
                FireThrowTrigger(f, fsm, type, commandDirection);
                FireButtonAndDirectionTrigger(f, fsm, type, commandDirection);
            }
        }
        
        
        private static void FireDash(Frame f, FSM fsm)
        {
            FrameParam param = new FrameParam() { f = f, EntityRef = fsm.EntityRef};
            
            if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                foreach (var (_, cpuControllerData) in f.GetComponentIterator<CpuControllerData>())
                {
                    if (!cpuControllerData.dash) return;
                    fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Dash, param);
                    return;
                }
                return;
            }
            
            // Debug.Log(fsm.EntityRef);
            
            int commandDirection = fsm.GetBufferDirection(f, fsm.EntityRef);
            
            if (fsm.InputIsBuffered(InputType.Dash, f, fsm.EntityRef) && (commandDirection is 1 or 4 or 7))
            {
                fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Backdash, param);
            }
            else if (fsm.InputIsBuffered(InputType.Dash, f, fsm.EntityRef))
            {
                fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Dash, param);
            }
        }
        
        

        private static void FireJump(Frame f, FSM fsm)
        {
            if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                if (fsm.Fsm.IsInState(PlayerFSM.PlayerState.Ground))
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
            
            if (fsm.Fsm.IsInState(PlayerFSM.PlayerState.Ground))
            {
                switch (numpad)
                {
                    case 7:
                    {
                        fsm.TryToFireJump(f, FSM.JumpType.Backward);
                        return;
                        break;
                    }
                    case 8:
                    {
                        fsm.TryToFireJump(f, FSM.JumpType.Up);
                        return;
                        break;
                    }
                    case 9:
                    {
                        fsm.TryToFireJump(f, FSM.JumpType.Forward);
                        return;
                        break;
                    }
                }
            }
            
            if (fsm.InputIsBuffered(InputType.Jump, f, fsm.EntityRef))
            {
                switch (fsm.GetBufferDirection(f, fsm.EntityRef))
                {
                    case 7:
                    {
                        fsm.TryToFireJump(f, FSM.JumpType.Backward);
                        break;
                    }
                    case 8:
                    {
                        fsm.TryToFireJump(f, FSM.JumpType.Up);
                        break;
                    }
                    case 9:
                    {
                        fsm.TryToFireJump(f, FSM.JumpType.Forward);
                        break;
                    }
                }
            }
        }

        private static void FireDirection(Frame f, FSM fsm)
        {
            FrameParam param = new FrameParam() { f = f, EntityRef = fsm.EntityRef};

            if (Util.EntityIsCpu(f, fsm.EntityRef))
            {
                fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.NeutralInput, param);
                return;
            }
                
            int numpad = Numpad(f, fsm.EntityRef);

            switch (numpad)
            {
                case 1:
                {
                    fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Down, param);
                    break;
                }
                case 2:
                {
                    fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Down, param);
                    break;
                }
                case 3:
                {
                    fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Down, param);
                    break;
                }
                case 4:
                {
                    fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Backward, param);
                    break;
                }
                case 5:
                {
                    fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.NeutralInput, param);
                    break;
                }
                case 6:
                {
                    fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.Forward, param);
                    break;
                }
            }
        }

        public static int Numpad(Frame f, EntityRef entityRef)
        {
            if (Util.EntityIsCpu(f, entityRef)) return 5;
            
            f.Unsafe.TryGetPointer<PlayerLink>(FsmLoader.FSMs[entityRef].GetPlayer(), out var playerLink);
            
            Input input = *f.GetPlayerInput(playerLink->Player);
            
            bool facingRight = FSM.IsOnLeft(f, entityRef);
            

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
        

        private static void FireButtonAndDirectionTrigger(Frame f, FSM fsm, InputType type, int commandDirection)
        {
            ButtonAndDirectionParam param = new ButtonAndDirectionParam() { f = f, Type = type, CommandDirection = commandDirection, EntityRef = fsm.EntityRef};

            try
            {
                fsm.Fsm.Fire(PlayerFSM.PlayerTrigger.ButtonAndDirection, param);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        private static void FireThrowTrigger(Frame f, FSM fsm, InputType type, int commandDirection)
        {
            if (type != InputType.T) return;
            
            ButtonAndDirectionParam param = new ButtonAndDirectionParam() { f = f, Type = type, CommandDirection = commandDirection, EntityRef = fsm.EntityRef};

            var trigger = NumpadMatchesNumpad(commandDirection, 4)
                ? PlayerFSM.PlayerTrigger.BackThrow
                : PlayerFSM.PlayerTrigger.ForwardThrow;
            
            fsm.Fsm.Fire(trigger, param);
            
            // THROW TECHING
            if (!fsm.Fsm.IsInState(PlayerFSM.PlayerState.CutsceneReactor)) return;
            var cutscene = Util.GetActiveCutscene(f, fsm.EntityRef);
            if (!cutscene.Techable) return;
            fsm.Fsm.Fire(FSM.Trigger.Tech, param);
            f.Unsafe.TryGetPointer<CutsceneData>(fsm.EntityRef, out var cutsceneData);
            FsmLoader.FSMs[cutsceneData->initiator].Fsm.Fire(FSM.Trigger.Tech, param);
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