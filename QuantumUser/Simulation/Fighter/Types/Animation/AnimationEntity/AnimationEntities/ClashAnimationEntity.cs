using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class ClashAnimationEntity : AnimationEntity
    {
        public ClashAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 16;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(9, 0),
                    new(9, 1),
                    new(4, 2),
                }
            };
        }
    }
}