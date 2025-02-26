using Photon.Deterministic;

namespace Quantum.Types.Collision
{
    
    public enum CollisionBoxType
    {
        Hitbox,
        Hurtbox,
        Pushbox,
    }
    
    public class CollisionBox
    {


        public bool GrowWidth = true;
        public bool GrowHeight = true;
        
        public FP Width;
        public FP Height;
        
        public FP PosX;
        public FP PosY;
    }
}