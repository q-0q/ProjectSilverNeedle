using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveListEntry : MonoBehaviour
{
    private PlayerFSM.ActionConfig _actionConfig;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateActionConfig(PlayerFSM.ActionConfig actionConfig, FSM fsm)
    {
        _actionConfig = actionConfig;
        GetComponentInChildren<TextMeshProUGUI>().text = actionConfig.Name;


        // Vector3 target = (transform.position - _camera.transform.position) + transform.position;
        // transform.LookAt(target);

        string characterName = fsm.Name;
        int frame = actionConfig.AnimationDisplayFrameIndex;
        int path = fsm.StateMapConfig.FighterAnimation.Lookup(actionConfig.State, fsm).Path;
        var pathEnum = fsm.AnimationPathsEnum;
        string stringPath = Enum.ToObject(pathEnum, path).ToString();
        string fullPath = "Sprites/Characters/" + characterName + "/FrameGroups/" + stringPath + "/" + stringPath + "_" + frame;
        Sprite sprite = Resources.Load<Sprite>(fullPath);
        Image image = transform.Find("Panel").transform.Find("Image").GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
    }
}
