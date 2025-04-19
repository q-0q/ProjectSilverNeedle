using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class SurgeHitAnimationEntity : AnimationEntity
    {
        public SurgeHitAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 31;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(11, 0),
                    new(2, 1),
                    new(2, 2),
                }
            };
        }
    }
}