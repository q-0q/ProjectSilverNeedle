using System.Collections;
using System.Collections.Generic;
using Quantum;
using TMPro;
using UnityEngine;

public class GameFSMDebug : QuantumEntityViewComponent
{
    private TextMeshProUGUI _tmp;
    
    public override void OnInitialize()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnUpdateView()
    {

        GameFSM.State state = (GameFSM.State)PredictedFrame.Get<GameFSMData>(EntityRef).currentState;
        _tmp.text = state.ToString();
    }
}
