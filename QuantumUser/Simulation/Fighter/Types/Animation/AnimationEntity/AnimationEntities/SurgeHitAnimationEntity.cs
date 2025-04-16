using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class SurgeHitAnimationEntity : AnimationEntity
    {
        public SurgeHitAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 28;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(11, 0),
                    new(5, 1),
                    new(4, 2),
                }
            };
        }
    }
}