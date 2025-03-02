
using System.Collections.Generic;
using UnityEngine;
using Wasp;

namespace Quantum
{
    public unsafe static class GameFsmLoader
    {
        private static GameFSM GameFsm;
        public static EntityRef GameFsmEntityRef;

        public static void InitializeGameFsm(Frame f)
        {
            
            InheritableEnum.InheritableEnum.Initialize();
            GameFsmEntityRef = GetGameFsmEntityRef(f);
            GameFsm = new GameFSM(0, GameFsmEntityRef);
        }

        private static EntityRef GetGameFsmEntityRef(Frame f)
        {
            foreach (var (entityRef, _) in f.GetComponentIterator<GameFSMData>())
            {
                return entityRef;
            }
        
            return EntityRef.None;
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