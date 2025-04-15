using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class HitAnimationEntity : AnimationEntity
    {
        public HitAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 0;
            Layer = AELayer.Middle;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(7, 0),
                    new(2, 1),
                }
            };
        }
    }
}