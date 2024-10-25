using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class CountdownAnimationEntity : AnimationEntity
    {
        public CountdownAnimationEntity()
        {
            UI = true;
            SpriteDirectory = "UI";
            SpriteSheetOffset = 0;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(3, 0),
                    new(6, 1),
                    new(33, 2),
                    new(3, 3),
                    new(6, 4),
                    new(33, 5),
                    new(3, 6),
                    new(6, 7),
                    new(33, 8),
                    new(6, 9),
                    new(6, 10),
                    new(12, 11),
                    new(6, 12),
                    new(3, 13),
                }
            };
        }
    }
}