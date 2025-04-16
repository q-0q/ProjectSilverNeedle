using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class SurgeBlockAnimationEntity : AnimationEntity
    {
        public SurgeBlockAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 25;
            Layer = AELayer.Middle;
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