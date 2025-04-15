using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum.Types.Collision
{
    public class Hit
    {

        // public static List<int> AttackLevelHitstop = new() { 7, 12, 13, 14, 15 };
        public static List<int> AttackLevelHitstop = new() { 6, 11, 12, 13, 14 };
        public static List<int> AttackLevelStandHitstun = new() { 12, 14, 16, 19, 21 };
        public static List<int> AttackLevelCrouchHitstun = new() { 13, 15, 17, 20, 22 };
        public static List<int> AttackLevelGroundBlockstun = new() { 9, 11, 13, 16, 18 };
        public static List<int> AttackLevelAirBlockstun = new() { 1000, 1000, 1000, 1000, 1000 };
        public static int LandingRecoveryBlockstun = 19;
        
        public enum HitType
        {
            None,
            High,
            Mid,
            Low,
            Throw
        }

        public bool Projectile = false;

        public int Level = 0;
        public int BonusBlockstun = 0;
        public int BonusHitstun = 0;
        public int BonusHitstop = 0;

        public FP BlockPushback = FP.FromString("2");
        public FP HitPushback = 1;
        public FP TrajectoryHeight = 2;
        public FP TrajectoryXVelocity = 8;
        public bool Launches = false;
        
        public bool GroundBounce = false;
        public bool WallBounce = false;
        
        public bool HardKnockdown = false;
        public FP VisualAngle = 0;
        public FPVector2 VisualHitPositionOffset = FPVector2.One;
        public FP GravityScaling = FP.FromString("1.1");
        public FP GravityProration = FP.FromString("1.45");
        public FP DamageScaling = FP.FromString("0.85");
        public FP Damage = 20;
        public FP ProxBlockDistance = 6;
        
        public HitType Type = HitType.Mid;
        public SectionGroup<CollisionBoxCollection> HitboxCollections;

        public int TriggerCutscene = -1;

        // dont manually set this, it gets set and managed by fsmloader
        public int LookupId = -1;
    }
}