using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class BreakRedAnimationEntity : AnimationEntity
    {
        public BreakRedAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 22;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(5, 0),
                    new(11, 1),
                    new(6, 2),
                }
            };
        }
    }
}