using System.Collections;
using System.Collections.Generic;
using Quantum;
using Quantum.InheritableEnum;
using TMPro;
using UnityEngine;

public class PlayerFSMDebug : QuantumEntityViewComponent
{
    private TextMeshProUGUI _tmp;
    
    public override void OnInitialize()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    public override void OnUpdateView()
    {
        // return;
        if (!PredictedFrame.Has<FSMData>(EntityRef)) return;

        // PlayerFSM.State state = (PlayerFSM.State)PredictedFrame.Get<PlayerFSMData>(EntityRef).currentState;
        // int numFrames = PredictedFrame.Get<FSMData>(EntityRef).framesInState;
        
         
        if (FsmLoader.GetFsm(EntityRef) is not PlayerFSM fsm) return;

        int crossup = fsm.GetFramesSinceCrossupProtectionStart(PredictedFrame);
        int throww = fsm.GetFramesSinceThrowProtectionStart(PredictedFrame);

        _tmp.text = "Crossup: " + crossup + "\nThrow: " + throww;
    }
}
