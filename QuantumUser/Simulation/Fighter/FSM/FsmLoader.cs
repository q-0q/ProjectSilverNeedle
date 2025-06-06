using System;
using System.Collections.Generic;
using Quantum.QuantumUser.Simulation.Fighter.Types;
using Quantum.Types;
using Quantum.Types.Collision;
using UnityEngine;

namespace Quantum
{
    public static unsafe class FsmLoader
    {
        public static Dictionary<EntityRef, FSM> FSMs;
        public static List<Hit> HitTable;

        public static List<Type> CharacterTypes = new List<Type>()
        {
            typeof(StickTwoFSM),
            typeof(GirlShotoFSM),
            typeof(PriestessFSM),
        };
        
        public static void InitFsmLoader()
        {
            FSMs = new Dictionary<EntityRef, FSM>();
            HitTable = new List<Hit>();
        }

        public static void InitializeFsms(Frame f)
        {
            
            InheritableEnum.InheritableEnum.Initialize();
            
            // Setup players
            var p0 = SetupPlayerFsm(f, 0);
            var p1 = SetupPlayerFsm(f, 1);
            
            FSMs = new Dictionary<EntityRef, FSM>()
            {
                { Util.GetPlayer(f, 0), p0 },
                { Util.GetPlayer(f, 1), p1 },
            };
            
            // Clear existing summons
            foreach (var (entityRef, _) in f.GetComponentIterator<SummonData>())
            {
                f.Destroy(entityRef);
            }
            
            // Setup summons
            InitializePlayerSummonPools(f, p0, 0);
            InitializePlayerSummonPools(f, p1, 1);
            
            // Fill in HitTable
            HitTable = new List<Hit>();
            foreach (var (_, fsm) in FSMs)
            {
                FillHitTableFromFSM(f, fsm);
            }

        }

        private static PlayerFSM SetupPlayerFsm(Frame f, int playerId)
        {
            var entityRef = Util.GetPlayer(f, playerId);
            f.Unsafe.TryGetPointer<PlayerLink>(entityRef, out var playerLink);
            var fsm = Activator.CreateInstance(CharacterTypes[playerLink->characterId]);
            if (fsm is not PlayerFSM playerFSM) return null;
            playerFSM.SetupStateMaps();
            playerFSM.SetupMachine();
            playerFSM.EntityRef = entityRef;
            
            return playerFSM;
        }

        public static FSM GetFsm(EntityRef entityRef)
        {
            if (FSMs is null) return null;
            if (FSMs.ContainsKey(entityRef)) return FSMs?[entityRef];
            return null;
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

        private static void InitializePlayerSummonPools(Frame f, PlayerFSM ownerFsm, int ownerPlayerId)
        {
            
            InitializeSummonPool(f, ownerPlayerId, ownerFsm.JumpGameFXSummonPool);
            InitializeSummonPool(f, ownerPlayerId, ownerFsm.LandGameFXSummonPool);
            InitializeSummonPool(f, ownerPlayerId, ownerFsm.DashGameFXSummonPool);
            InitializeSummonPool(f, ownerPlayerId, ownerFsm.GroundBounceGameFXSummonPool);
            
            var summonPools = ownerFsm.SummonPools;
            if (summonPools is null) return;

            foreach (var summonPool in summonPools)
            {
                InitializeSummonPool(f, ownerPlayerId, summonPool);
            }
        }

        private static void InitializeSummonPool(Frame f, int ownerPlayerId, SummonPool summonPool)
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
                summonFsm.SetupStateMaps();
                summonFsm.SetupMachine();
                FSMs[summonEntity] = summonFsm;
                summonPool.EntityRefs.Add(summonEntity);
                // Debug.Log("Successfully instantiated player " + ownerPlayerId + " " + summonPool.SummonFSMType + " [" + i + "]");
            }
        }


        public static void ReadAllFSMsFromNetwork(Frame f)
        {
            foreach (var (entityRef, fsm) in FSMs)
            {
                f.Unsafe.TryGetPointer<FSMData>(entityRef, out var fsmData);
                FSMs[entityRef].Fsm.Assume(fsmData->currentState);
            }
        }
        
        public static void WriteAllFSMsToNetwork(Frame f)
        {
            foreach (var (entityRef, fsm) in FSMs)
            {
                f.Unsafe.TryGetPointer<FSMData>(entityRef, out var fsmData);
                fsmData->currentState = fsm.Fsm.State();
            }
        }

        private static void FillHitTableFromFSM(Frame f, FSM fsm)
        {
            var stateMapConfig = fsm.StateMapConfig;
            var hitSectionGroup = stateMapConfig?.HitSectionGroup;
            if (hitSectionGroup is null) return;
            
            foreach (var (_, sectionGroup) in hitSectionGroup.Dictionary)
            {
                FillHitTableFromSectionGroup(sectionGroup);
            }
            
            foreach (var (_, sectionGroup) in hitSectionGroup.SuperDictionary)
            {
                FillHitTableFromSectionGroup(sectionGroup);
            }
            
            foreach (var (_, func) in hitSectionGroup.FuncDictionary)
            {
                FillHitTableFromSectionGroup(func(new FrameParam() { f = f, EntityRef = fsm.EntityRef }));
            }
            
            foreach (var (_, func) in hitSectionGroup.SuperFuncDictionary)
            {
                FillHitTableFromSectionGroup(func(new FrameParam() { f = f, EntityRef = fsm.EntityRef }));
            }
        }
        
        private static void FillHitTableFromSectionGroup(SectionGroup<Hit> sectionGroup)
        {
            if (sectionGroup.Sections is null) return;
            
            foreach (var (_, hit) in sectionGroup.Sections)
            {
                if (hit is null) continue;
                hit.LookupId = HitTable.Count;
                HitTable.Add(hit);
            }
        }
        
    }
}