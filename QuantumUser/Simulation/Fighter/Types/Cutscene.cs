using Photon.Deterministic;

namespace Quantum.Types
{
    public class Cutscene
    {
        public int state;
        public SectionGroup<FPVector2> reactorPositionSectionGroup;
        public SectionGroup<int> reactorStateSectionGroup;
        public int reactorDuration;
    }
}