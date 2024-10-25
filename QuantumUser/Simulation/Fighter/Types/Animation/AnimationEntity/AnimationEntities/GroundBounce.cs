using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class GroundBounceAnimationEntity : AnimationEntity
    {
        public GroundBounceAnimationEntity()
        {
            SpriteDirectory = "Environment";
            SpriteSheetOffset = 7;
            Layer = AELayer.Back;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(9, 0),
                    new(3, 1),
                    new(3, 2),
                }
            };
        }
    }
}