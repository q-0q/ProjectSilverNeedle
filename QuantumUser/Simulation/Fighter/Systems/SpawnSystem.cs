using Photon.Deterministic;
using Quantum.Collections;
using UnityEngine;


namespace Quantum
{
    unsafe class SpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame frame, PlayerRef player)
        {
            
            var data = frame.GetPlayerData(player);
            var prototype = frame.FindAsset<EntityPrototype>(data.PlayerAvatar);
            var entity = frame.Create(prototype);
            
            // PlayerLink
            var playerLink = new PlayerLink()
            {
                Player = player,
                characterId = (int)Characters.CharacterEnum.StickTwo
            };
            frame.Add(entity, playerLink);

            var gameFsm = GameFSMSystem.LoadGameFsm(frame);
            FrameParam frameParam = new FrameParam() { f = frame, EntityRef = entity };
            gameFsm.Fsm.Fire(GameFSM.Trigger.PlayerJoin, frameParam);
            GameFSMSystem.WritebackGameFsm(frame, gameFsm);

        }
    }
    
    
}