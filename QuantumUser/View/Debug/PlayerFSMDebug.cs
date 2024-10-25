using System.Collections;
using System.Collections.Generic;
using Quantum;
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
        
        if (!PredictedFrame.Has<PlayerFSMData>(EntityRef)) return;

        // PlayerFSM.State state = (PlayerFSM.State)PredictedFrame.Get<PlayerFSMData>(EntityRef).currentState;
        // int numFrames = PredictedFrame.Get<PlayerFSMData>(EntityRef).framesInState;


        var state = PlayerFsmLoader.GetPlayerFsm(PredictedFrame, EntityRef).Fsm.State();
        _tmp.text = state.ToString();// + "\n" + numFrames;
    }
}
