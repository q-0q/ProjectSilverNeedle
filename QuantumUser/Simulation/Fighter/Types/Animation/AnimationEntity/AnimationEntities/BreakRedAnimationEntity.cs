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
                    new(6, 0),
                    new(14, 1),
                    new(18, 2),
                }
            };
        }
    }
}