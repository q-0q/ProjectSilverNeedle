using System;
using System.Collections.Generic;
using Quantum.QuantumUser.Simulation.Fighter.Types;
using UnityEngine;

namespace Quantum
{
    public static unsafe class FsmLoader
    {
        public static Dictionary<EntityRef, FSM> FSMs;


        public static void InitFsmLoader()
        {
            FSMs = new Dictionary<EntityRef, FSM>();
        }

        public static void InitializeFsms(Frame f)
        {
            
            InheritableEnum.InheritableEnum.Initialize();
            
            // Todo: load player-selected FSM
            
            var p0 = new StickTwoFSM();
            p0.SetupMachine();
            p0.SetupStateMaps();
            p0.EntityRef = Util.GetPlayer(f, 0);
            
            var p1 = new StickTwoFSM();
            p1.SetupMachine();
            p1.SetupStateMaps();
            p1.EntityRef = Util.GetPlayer(f, 1);
            
            FSMs = new Dictionary<EntityRef, FSM>()
            {
                { Util.GetPlayer(f, 0), p0 },
                { Util.GetPlayer(f, 1), p1 },
            };
            
            InitializeSummonPools(f, p0, 0);
            InitializeSummonPools(f, p1, 1);

        }

        public static FSM GetFsm(EntityRef entityRef)
        {
            return FSMs?[entityRef];
        }

        private static EntityRef CreateSummonEntity(Frame f, int playerOwner, int idInPool)
        {
            EntityRef entity =
                f.Create(f.FindAsset<EntityPrototype>("QuantumUser/Resources/SummonEntityPrototype"));

            f.Unsafe.TryGetPointer<SummonData>(entity, out var summonData);
            summonData->owner = Util.GetPlayer(f, playerOwner);
            summonData->player = playerOwner;
            summonData->counter = idInPool;
            
            f.Unsafe.TryGetPointer<Transform3D>(entity, out var transform3D);
            return entity;
        }

        private static void InitializeSummonPools(Frame f, FSM ownerFsm, int ownerPlayerId)
        {
            var summonPools = ownerFsm.SummonPools;
            if (summonPools is null) return;

            foreach (var summonPool in summonPools)
            {
                summonPool.EntityRefs = new List<EntityRef>();
                for (int i = 0; i < summonPool.Size; i++)
                {
                    var summonEntity = CreateSummonEntity(f, ownerPlayerId, i);
                    if (Activator.CreateInstance(summonPool.SummonFSMType) is not SummonFSM summonFsm)
                    {
                        Debug.LogError("You tried to instantiate a SummonFSM pool on a type that is not a SummonFSM");
                        return;
                    }

                    summonFsm.playerOwnerEntity = Util.GetPlayer(f, ownerPlayerId);
                    summonFsm.EntityRef = summonEntity;
                    summonFsm.SetupMachine();
                    summonFsm.SetupStateMaps();
                    FSMs[summonEntity] = summonFsm;
                    summonPool.EntityRefs.Add(summonEntity);
                    Debug.Log("Successfully instantiated player " + ownerPlayerId + " " + summonPool.SummonFSMType + " [" + i + "]");
                }
            }
        }
        
    }
}