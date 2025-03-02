
using System.Collections.Generic;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe static class GameFsmLoader
    {
        private static GameFSM GameFsm;
        public static EntityRef GameFsmEntityRef;

        public static void InitializeGameFsm(EntityRef entityRef)
        {
            InheritableEnum.InheritableEnum.Initialize();
            GameFsmEntityRef = entityRef;
            GameFsm = new GameFSM(0, GameFsmEntityRef);
        }
        
        public static GameFSM LoadGameFSM(Frame f)
        {
            f.Unsafe.TryGetPointer<GameFSMData>(GameFsmEntityRef, out var gameFsmData);
            GameFsm.Fsm.Assume((GameFSM.State)gameFsmData->currentState);
            return GameFsm;
        }

        public static void WritebackGameFSM(Frame f)
        {
            f.Unsafe.TryGetPointer<GameFSMData>(GameFsmEntityRef, out var gameFsmData);
            gameFsmData->currentState = (int)GameFsm.Fsm.State();
        }

        
        
    }
}