using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    private Canvas _canvas;

    public GameObject MenuTabButtonPrefab;
    private GameObject _tabList;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _canvas = transform.Find("MainCanvas").GetComponent<Canvas>();
        _tabList = transform.Find("MainCanvas").Find("TabList").gameObject;
    }

    private void Start()
    {
        foreach (var tab in GetComponentsInChildren<GameMenuTab>())
        {
            Debug.Log(tab.TabName);
            Instantiate(MenuTabButtonPrefab, _tabList.transform);
        }
        _canvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) _canvas.gameObject.SetActive(!_canvas.gameObject.activeInHierarchy);
    }
}
