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
    public GameObject MoveListSectionHeaderPrefab;

    private bool _enqueueDisabled = false;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _defaultTab = null;
        _currentTab = null;
        _canvas = transform.Find("MainCanvas").GetComponent<Canvas>();
        _tabList = transform.Find("MainCanvas").Find("MainPanel").Find("TabList").gameObject;
        QuantumEvent.Subscribe(listener: this, handler: (EventGameFinishLoading e) => PopulateMoveList(e.f));

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
    }

    void Update()
    {
        if (_enqueueDisabled)
        {
            _canvas.gameObject.SetActive(false);
            _enqueueDisabled = false;
        }
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

    private void PopulateMoveList(Frame f)
    {
        
        int playerId = 0; //todo
        
        if (FsmLoader.FSMs[Util.GetPlayer(f, playerId)] is not PlayerFSM fsm)
        {
            Debug.LogError("Tried to PopulateMoveList from a non-Player FSM");
            return;
        }

        var scrollContent = transform.Find("MainCanvas").Find("MainPanel").Find("ContentPanel").Find("CurrentTabContent")
            .Find("MoveList").Find("ScrollContent");

        var normHeader = Instantiate(MoveListSectionHeaderPrefab, scrollContent);
        normHeader.GetComponent<TextMeshProUGUI>().text = "Normal moves";
        foreach (var move in fsm.NormalMoveList)
        {
            var obj = Instantiate(MoveListEntryPrefab,
                scrollContent);
            obj.GetComponent<MoveListEntry>().UpdateActionConfig(move, fsm);
        }
        
        var commandHeader = Instantiate(MoveListSectionHeaderPrefab, scrollContent);
        commandHeader.GetComponent<TextMeshProUGUI>().text = "Command normals";
        foreach (var move in fsm.CommandNormalMoveList)
        {
            var obj = Instantiate(MoveListEntryPrefab,
                scrollContent);
            obj.GetComponent<MoveListEntry>().UpdateActionConfig(move, fsm);
        }

        _enqueueDisabled = true;
        
        var specialHeader = Instantiate(MoveListSectionHeaderPrefab, scrollContent);
        specialHeader.GetComponent<TextMeshProUGUI>().text = "Special moves";
        foreach (var move in fsm.SpecialMoveList)
        {
            var obj = Instantiate(MoveListEntryPrefab,
                scrollContent);
            obj.GetComponent<MoveListEntry>().UpdateActionConfig(move, fsm);
        }

        _enqueueDisabled = true;


    }
}
