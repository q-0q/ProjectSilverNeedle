using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class TechAnimationEntity : AnimationEntity
    {
        public TechAnimationEntity()
        {
            SpriteDirectory = "Collisions";
            SpriteSheetOffset = 13;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(3, 0),
                    new(7, 1),
                    new(9, 2)
                }
            };
        }
    }
}