using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum.Types.Collision
{
    public class Kinematics

    {
        public int FireReceiverFinishAfter = 10;
        public FighterAnimation Animation;
        public SectionGroup<FPVector2> GrabPositionSectionGroup;
        public SectionGroup<Hit> HitSectionGroup;
    }
}