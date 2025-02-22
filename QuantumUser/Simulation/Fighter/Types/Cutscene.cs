using Photon.Deterministic;

namespace Quantum.Types
{
    public class Cutscene
    {
        public int InitiatorState = -1;
        public SectionGroup<FPVector2> ReactorPositionSectionGroup = null;
        public int ReactorDuration = 0;
        public bool Techable = false;
    }
}