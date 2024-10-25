using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class KOAnimationEntity : AnimationEntity
    {
        public KOAnimationEntity()
        {
            UI = true;
            SpriteDirectory = "UI";
            SpriteSheetOffset = 14;
            Layer = AELayer.Front;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(6, 0),
                    new(6, 1),
                    new(6, 2),
                    new(6, 3),
                    new(6, 4),
                    new(6, 5),
                    new(6, 6),
                    new(108, 7),
                }
            };
        }
    }
}