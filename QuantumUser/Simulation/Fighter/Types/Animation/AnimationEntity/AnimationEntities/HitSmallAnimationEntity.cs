using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class HitSmallAnimationEntity : AnimationEntity
    {
        public HitSmallAnimationEntity()
        {
            SpriteDirectory = "HitSmall";
            SpriteSheetOffset = 0;
            Layer = AELayer.Back;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(4, 0),
                    new(3, 1),
                    new(10, 2),
                    new(4, 3),
                    new(3, 4),
                }
            };
        }
    }
}