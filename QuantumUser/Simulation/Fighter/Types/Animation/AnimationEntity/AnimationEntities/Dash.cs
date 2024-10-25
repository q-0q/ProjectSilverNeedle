using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class DashAnimationEntity : AnimationEntity
    {
        public DashAnimationEntity()
        {
            SpriteDirectory = "Environment";
            SpriteSheetOffset = 0;
            Layer = AELayer.Back;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(3, 0),
                    new(7, 1),
                    new(7, 2),
                    new(9, 3),
                }
            };
        }
    }
}