using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using TMPro;
using UnityEngine;
using Input = UnityEngine.Input;

public class GameMenu : MonoBehaviour
{
    private Canvas _canvas;

    public GameObject MenuTabButtonPrefab;
    private GameObject _tabList;
    private GameMenuTab _defaultTab;
    private GameMenuTab _currentTab;
    public GameObject MoveListEntryPrefab;

    
    // Start is called before the first frame update
    private void Awake()
    {
        _defaultTab = null;
        _currentTab = null;
        _canvas = transform.Find("MainCanvas").GetComponent<Canvas>();
        _tabList = transform.Find("MainCanvas").Find("MainPanel").Find("TabList").gameObject;
        
        QuantumEvent.Subscribe(listener: this, handler: (EventGameFinishLoading e) => PopulateMoveList());

    }

    private void Start()
    {
        foreach (var tab in GetComponentsInChildren<GameMenuTab>())
        {
            var obj = Instantiate(MenuTabButtonPrefab, _tabList.transform);
            obj.GetComponent<MenuTabButton>().GameMenuTab = tab;
            tab.gameObject.SetActive(false);
            if (_defaultTab is null)
            {
                _defaultTab = tab;
            }
        } 
        
        UpdateCurrentTab(_defaultTab);
        _canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) _canvas.gameObject.SetActive(!_canvas.gameObject.activeInHierarchy);
    }
    
    private void OnEnable()
    {
        MenuTabButton.OnMenuTabButtonClicked += UpdateCurrentTab;
    }

    private void OnDisable()
    {
        MenuTabButton.OnMenuTabButtonClicked -= UpdateCurrentTab;
    }

    private void UpdateCurrentTab(GameMenuTab tab)
    {
        if (_currentTab is not null) _currentTab.gameObject.SetActive(false);
        tab.gameObject.SetActive(true);
        _currentTab = tab;
        transform.Find("MainCanvas").Find("MainPanel").Find("ContentPanel").Find("CurrentTabTitle").GetComponent<TextMeshProUGUI>().text = tab.TabName;
    }

    private void PopulateMoveList()
    {
        int playerId = 0; //todo
        
        
    }
}
