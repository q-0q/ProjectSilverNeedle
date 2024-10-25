using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class CounterHitAnimationEntity : AnimationEntity
    {
        public CounterHitAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 4;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(12, 0),
                    new(2, 1),
                }
            };
        }
    }
}