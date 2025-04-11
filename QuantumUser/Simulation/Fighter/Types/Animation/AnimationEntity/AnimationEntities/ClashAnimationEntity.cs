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
                    new(5, 0),
                    new(11, 1),
                    new(6, 2),
                }
            };
        }
    }
}