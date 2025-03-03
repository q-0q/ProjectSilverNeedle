using System.Collections.Generic;
using UnityEngine;

namespace Quantum
{
    public static unsafe class FsmLoader
    {
        public static Dictionary<EntityRef, FSM> FSMs;
        

        public static void InitializeFsms(Frame f)
        {
            
            InheritableEnum.InheritableEnum.Initialize();
            
            // Todo: load player-selected FSM and summons also
            
            var p0 = new StickTwo();
            p0.SetupMachine();
            p0.SetupStateMaps();
            
            var p1 = new StickTwo();
            p1.SetupMachine();
            p1.SetupStateMaps();

            var summonEntity =CreateSummonEntity(f, 0);
            var s1 = new Fireball();
            s1.playerOwnerEntity = Util.GetPlayer(f, 0);
            s1.SetupMachine();
            s1.SetupStateMaps();

            FSMs = new Dictionary<EntityRef, FSM>()
            {
                { Util.GetPlayer(f, 0), p0 },
                { Util.GetPlayer(f, 1), p1 },
                { summonEntity, s1 }
            };
            

        }

        public static FSM GetFsm(EntityRef entityRef)
        {
            return FSMs?[entityRef];
        }

        private static EntityRef CreateSummonEntity(Frame f, int playerOwner)
        {
            EntityRef entity =
                f.Create(f.FindAsset<EntityPrototype>("QuantumUser/Resources/SummonEntityPrototype"));

            f.Unsafe.TryGetPointer<SummonData>(entity, out var summonData);
            summonData->owner = Util.GetPlayer(f, playerOwner);
            summonData->player = playerOwner;
            
            return entity;
        }
        
        
    }
}