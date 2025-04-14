using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class SurgeAnimationEntity : AnimationEntity
    {
        public SurgeAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 22;
            Layer = AELayer.Back;
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