using System.Collections.Generic;
using UnityEngine;

namespace Quantum
{
    public static class PlayerFsmLoader
    {
        public static List<PlayerFSM> PlayerFsms;

        public static void InitializePlayerFsms(Frame f)
        {
            
            // Force static initialization of InheritableEnum class
            Debug.Log("Trying to initialize...");
            var _ = PlayerFSM.State.GroundActionable;
            
            var p0 = new PlayerFSM();
            var p0Character = Characters.GetPlayerCharacter(f, Util.GetPlayer(f, 0));
            p0Character.ConfigureCharacterFsm(p0);
            
            var p1 = new PlayerFSM();
            var p1Character = Characters.GetPlayerCharacter(f, Util.GetPlayer(f, 1));
            p1Character.ConfigureCharacterFsm(p1);

            PlayerFsms = new List<PlayerFSM>
            {
                p0,
                p1
            };
        }

        public static PlayerFSM GetPlayerFsm(Frame f, EntityRef entityRef)
        {
            return PlayerFsms[Util.GetPlayerId(f, entityRef)];
        }
    }
}