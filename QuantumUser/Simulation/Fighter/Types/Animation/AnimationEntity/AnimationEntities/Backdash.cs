using System;
using System.Collections.Generic;

namespace Quantum.Types
{
    public class BackdashAnimationEntity : AnimationEntity
    {
        public BackdashAnimationEntity()
        {
            SpriteDirectory = "Environment";
            SpriteSheetOffset = 4;
            Layer = AELayer.Back;
            SectionGroup = new SectionGroup<int>()
            {
                Sections = new List<Tuple<int, int>>()
                {
                    new(3, 0),
                    new(7, 1),
                    new(9, 2),
                }
            };
        }
    }
}