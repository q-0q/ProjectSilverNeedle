using System.Collections.Generic;
using UnityEngine;

namespace Quantum
{
    public static class FsmLoader
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

            FSMs = new Dictionary<EntityRef, FSM>()
            {
                { Util.GetPlayer(f, 0), p0 },
                { Util.GetPlayer(f, 1), p1 }
            };
            

        }

        public static FSM GetPlayerFsm(EntityRef entityRef)
        {
            return FSMs?[entityRef];
        }
        
        
    }
}