using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class BlockAnimationEntity : AnimationEntity
    {
        public BlockAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 8;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(8, 0),
                    new(5, 1),
                    new(3, 2),
                    new(3, 3),
                }
            };
        }
    }
}