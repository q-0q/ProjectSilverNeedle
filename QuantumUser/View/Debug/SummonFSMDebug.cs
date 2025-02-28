using System.Collections;
using System.Collections.Generic;
using Quantum;
using Quantum.InheritableEnum;
using TMPro;
using UnityEngine;

public class SummonFSMDebug : QuantumEntityViewComponent
{
    private TextMeshProUGUI _tmp;
    
    public override void OnInitialize()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    public override void OnUpdateView()
    {
        
        if (!PredictedFrame.Has<FSMData>(EntityRef)) return;
        int numFrames = PredictedFrame.Get<FSMData>(EntityRef).framesInState;

        var fsm = FsmLoader.GetPlayerFsm(PredictedFrame, EntityRef);
        if (fsm is null) return;
        var state = fsm.Fsm.State();
        
        _tmp.text = InheritableEnum.GetFieldNameByValue(state, typeof(SummonFSM.State)) + "\n" + numFrames;
        // _tmp.text = state.ToString();
    }
}
