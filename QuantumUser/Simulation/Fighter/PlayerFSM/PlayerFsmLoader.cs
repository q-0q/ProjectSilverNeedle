using System.Collections.Generic;

namespace Quantum
{
    public static class PlayerFsmLoader
    {
        public static List<PlayerFSM> PlayerFsms;

        public static void InitializePlayerFsms(Frame f)
        {
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