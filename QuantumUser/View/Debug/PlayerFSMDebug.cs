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
        
        if (!PredictedFrame.Has<PlayerFSMData>(EntityRef)) return;

        // PlayerFSM.State state = (PlayerFSM.State)PredictedFrame.Get<PlayerFSMData>(EntityRef).currentState;
        // int numFrames = PredictedFrame.Get<PlayerFSMData>(EntityRef).framesInState;

        Characters.CharacterEnum characterEnum = (Characters.CharacterEnum)PredictedFrame.Get<PlayerLink>(EntityRef).characterId;
        var state = PlayerFsmLoader.GetPlayerFsm(PredictedFrame, EntityRef).Fsm.State();
        _tmp.text = InheritableEnum.GetFieldNameByValue(state, Characters.Get(characterEnum).StateType);// + "\n" + numFrames;
        // _tmp.text = state.ToString();
    }
}
