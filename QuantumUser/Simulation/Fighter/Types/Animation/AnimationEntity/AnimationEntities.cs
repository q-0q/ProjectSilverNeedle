using System.Collections.Generic;

namespace Quantum.Types
{
    public static unsafe class AnimationEntities
    {
        public enum AnimationEntityEnum
        {
            Block,
            Hit,
            Counter,
            Tech,
            Countdown,
            KO,
            Dash,
            Backdash,
            GroundBounce,
            Clash,
            Break,
            BreakRed,
            Surge,
            SurgeHit,
            SurgeBlock,
        }
        
        private static Dictionary<AnimationEntityEnum, AnimationEntity> _dictionary = new()
        {
            { AnimationEntityEnum.Block, new BlockAnimationEntity() },
            { AnimationEntityEnum.Hit, new HitAnimationEntity() },
            { AnimationEntityEnum.Tech, new TechAnimationEntity() },
            { AnimationEntityEnum.Counter, new CounterHitAnimationEntity() },
            { AnimationEntityEnum.Countdown , new CountdownAnimationEntity() },
            { AnimationEntityEnum.KO , new KOAnimationEntity() },
            { AnimationEntityEnum.Dash , new DashAnimationEntity() },
            { AnimationEntityEnum.Backdash , new BackdashAnimationEntity() },
            { AnimationEntityEnum.GroundBounce , new GroundBounceAnimationEntity() },
            { AnimationEntityEnum.Clash , new ClashAnimationEntity() },
            { AnimationEntityEnum.Break , new BreakAnimationEntity() },
            { AnimationEntityEnum.BreakRed , new BreakRedAnimationEntity() },
            { AnimationEntityEnum.Surge , new SurgeAnimationEntity() },
            { AnimationEntityEnum.SurgeHit , new SurgeHitAnimationEntity() },
            { AnimationEntityEnum.SurgeBlock , new SurgeBlockAnimationEntity() },
        };
        
        public static AnimationEntity Get(AnimationEntityEnum e)
        {
            return _dictionary[e];
        } 
    }
}