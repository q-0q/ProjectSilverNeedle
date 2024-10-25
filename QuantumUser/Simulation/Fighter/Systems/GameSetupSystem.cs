using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;

namespace Quantum
{
    public unsafe class GameSetupSystem : SystemSignalsOnly
    {
        public struct Filter
        {
            public EntityRef Entity;
            
        }

        public override void OnInit(Frame f)
        {
            EntityRef gameFsmEntity =
                f.Create(f.FindAsset<EntityPrototype>("QuantumUser/Resources/GameFSMEntityPrototype"));

            f.Unsafe.TryGetPointer<GameFSMData>(gameFsmEntity, out var gameFsmData);
            gameFsmData->currentState = (int)GameFSM.State.Waiting;
            gameFsmData->framesInState = 0;


            EntityRef hitstopEntity = f.Create();
            var hitstopData = new HitstopData()
            {
                hitstopRemaining = 0,
                queued = false,
            };

            f.Add(hitstopEntity, hitstopData);
        }
    }
}