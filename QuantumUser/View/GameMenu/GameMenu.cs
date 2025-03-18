using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    private Canvas _canvas;

    public GameObject MenuTabButtonPrefab;
    private GameObject _tabList;
    private GameMenuTab _defaultTab;
    private GameMenuTab _currentTab;

    
    // Start is called before the first frame update
    private void Awake()
    {
        _defaultTab = null;
        _currentTab = null;
        _canvas = transform.Find("MainCanvas").GetComponent<Canvas>();
        _tabList = transform.Find("MainCanvas").Find("MainPanel").Find("TabList").gameObject;
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
}
