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
    public GameObject MoveListTagPrefab;
    
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
        transform.Find("Title").GetComponent<TextMeshProUGUI>().text = actionConfig.Name;
        
        transform.Find("Panel").Find("Input").GetComponent<TextMeshProUGUI>().text = 
            Enum.GetName(typeof(InputSystem.InputType), actionConfig.InputType);

        if (actionConfig.CommandDirection != 5)
        {
            transform.Find("Panel").Find("Input").GetComponent<TextMeshProUGUI>().text +=
                " + " + actionConfig.CommandDirection;
        }
        
        if (actionConfig.WhileIn != null)
        {
            transform.Find("Panel").Find("Input").GetComponent<TextMeshProUGUI>().text +=
                " while in " + actionConfig.WhileIn.Name;
        }
        
        
        transform.Find("Panel").Find("Text").Find("Description").GetComponent<TextMeshProUGUI>().text
            = actionConfig.Description;
        
        transform.Find("Panel").Find("Text").Find("Flavor").GetComponent<TextMeshProUGUI>().text
            = actionConfig.FlavorText;

        string data = "Frame Advantage:\n";
        data += "Ground block:   " +
                GetRangeString(actionConfig.MinGroundBlockFrameAdvantage, actionConfig.MaxGroundBlockFrameAdvantage);
        data += "Standing hit:   " +
                GetRangeString(actionConfig.MinStandHitFrameAdvantage, actionConfig.MaxStandHitFrameAdvantage);
        data += "Crouching hit:  " +
                GetRangeString(actionConfig.MinCrouchHitFrameAdvantage, actionConfig.MaxCrouchHitFrameAdvantage);
        // data += "Air block:      " +
        //         GetRangeString(actionConfig.MinAirBlockFrameAdvantage, actionConfig.MaxAirBlockFrameAdvantage);
        
        transform.Find("Panel").Find("Text").Find("Data").GetComponent<TextMeshProUGUI>().text
            = data;

        // Vector3 target = (transform.position - _camera.transform.position) + transform.position;
        // transform.LookAt(target);

        string characterName = fsm.Name;
        int frame = actionConfig.AnimationDisplayFrameIndex;
        string path = fsm.StateMapConfig.FighterAnimation.Lookup(actionConfig.State, fsm, null).Path;
        string fullPath = "Sprites/Characters/" + characterName + "/FrameGroups/" + path + "/" + path + "_" + frame;
        Sprite sprite = Resources.Load<Sprite>(fullPath);
        Image image = transform.Find("Panel").transform.Find("Image").GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
        if (actionConfig.Aerial)
        {
            var anchoredPosition = image.GetComponent<RectTransform>().anchoredPosition;
            image.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(anchoredPosition.x, anchoredPosition.y + 70);
        }
    }

    private string GetRangeString(int min, int max)
    {
        string s_min = min.ToString();
        if (min >= 0)
        {
            s_min = "+" + s_min;
            s_min = "<color=\"green\">" + s_min + "</color>";
        }
        else
        {
            s_min = "<color=\"red\">" + s_min + "</color>";
        }
        
        string s_max = max.ToString();
        if (max >= 0)
        {
            s_max = "+" + s_max;
            s_max = "<color=#08b500>" + s_max + "</color>";
        }
        else
        {
            s_max = "<color=#b50000>" + s_max + "</color>";
        }

        return s_min + " - " + s_max + "\n";
    }

    private void AddTag()
    {
        
    }
}
