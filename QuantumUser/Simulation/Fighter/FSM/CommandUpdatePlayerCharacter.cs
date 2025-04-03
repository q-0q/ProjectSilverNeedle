using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;

    public unsafe class CommandUpdatePlayerCharacter : DeterministicCommand
    {
        public PlayerRef player;
        public int id;
        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref id);
            stream.Serialize(ref player);
        }

        public void Execute(Frame f)
        {
            Debug.Log("Hello from Update Player Character command");
            f.Unsafe.TryGetPointer<PlayerLink>(Util.GetPlayer(f, (int)player), out var playerLink);
            playerLink->characterId = id;
            GameFsmLoader.LoadGameFSM(f).Fsm.Fire(GameFSM.Trigger.NewCharacterSelected, 
                new FrameParam() {f = f, EntityRef = GameFsmLoader.GameFsmEntityRef});
        }
    }
}