using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum.Types.Collision
{
    public class Kinematics

    {
        
        // Subtle but important, this number corresponds to the number of frames the grabbed opponent has to be
        // in the grabbed state for before it's fired, in other words the number of frames they will be grabbed for.
        
        // That means it wont always match the number of frames that the attacker spends grabbing, since the opponent
        // might not get grabbed on the very first active frame of the the throw hitbox
        public int FireReceiverFinishAfter = 10;
        public FighterAnimation Animation;
        public SectionGroup<FPVector2> GrabPositionSectionGroup;
        public SectionGroup<Hit> HitSectionGroup;
    }
}