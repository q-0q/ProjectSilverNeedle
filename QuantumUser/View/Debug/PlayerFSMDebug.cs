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
        
        if (!PredictedFrame.Has<FSMData>(EntityRef)) return;

        // PlayerFSM.State state = (PlayerFSM.State)PredictedFrame.Get<PlayerFSMData>(EntityRef).currentState;
        int numFrames = PredictedFrame.Get<FSMData>(EntityRef).framesInState;

        Characters.CharacterEnum characterEnum = (Characters.CharacterEnum)PredictedFrame.Get<PlayerLink>(EntityRef).characterId;
        var fsm = FsmLoader.GetPlayerFsm(EntityRef);
        if (fsm is null) return;
        var state = fsm.Fsm.State();
        
        _tmp.text = InheritableEnum.GetFieldNameByValue(state, Characters.Get(characterEnum).StateType) + "\n" + numFrames;
        // _tmp.text = state.ToString();
    }
}
