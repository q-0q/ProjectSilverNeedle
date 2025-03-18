using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabButton : MonoBehaviour
{
    public GameMenuTab GameMenuTab;
    
    public static Action<GameMenuTab> OnMenuTabButtonClicked;
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = GameMenuTab.TabName;
        GetComponent<Button>().onClick.AddListener(OnClick);

    }
    
    private void OnClick()
    {
        OnMenuTabButtonClicked?.Invoke(GameMenuTab);
    }
    
}
