using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum.Types.Collision
{
    public class Hit
    {

        public static List<int> AttackLevelHitstop = new() { 11, 12, 13, 14, 15 };
        public static List<int> AttackLevelStandHitstun = new() { 12, 14, 16, 19, 21 };
        public static List<int> AttackLevelCrouchHitstun = new() { 13, 15, 17, 20, 22 };
        public static List<int> AttackLevelGroundBlockstun = new() { 9, 11, 13, 16, 18 };
        public static List<int> AttackLevelAirBlockstun = new() { 19, 19, 19, 19, 19 };
        
        public enum HitType
        {
            None,
            High,
            Mid,
            Low,
            Throw
        }

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
        public FP GravityScaling = FP.FromString("1.04");
        public FP DamageScaling = FP.FromString("0.85");
        public FP Damage = 20;
        
        public HitType Type = HitType.Mid;
        public SectionGroup<CollisionBoxCollection> HitboxCollections;

        public int TriggerCutscene = -1;
    }
}