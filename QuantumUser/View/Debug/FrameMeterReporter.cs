using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using Quantum.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Input = UnityEngine.Input;

public class FrameMeterReporter : QuantumEntityViewComponent
{
    private bool _frameMeterEnabled = false;

    // object pooling for Frame prefabs
    public List<GameObject> pooledObjects;
    public GameObject FrameMeterFramePrefab;
    public int poolSize = 50;
    // public static bool CollisionBoxViewEnabled;
    public static Action OnFrameMeterToggled;


    private void OnEnable()
    {
        OnFrameMeterToggled += Toggle;
    }
    
    private void OnDisable()
    {
        OnFrameMeterToggled -= Toggle;
    }

    public void Toggle()
    {
        _frameMeterEnabled = !_frameMeterEnabled;
        ClearCurrentFrameMeter();
    }

    public override void OnInitialize()
    {
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < poolSize; i++)
        {
            var tmp = Instantiate(FrameMeterFramePrefab, transform);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    
    public void ClearCurrentFrameMeter()
    {
        if (_frameMeterEnabled) return;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public override void OnUpdateView()
    {
        
        if (!_frameMeterEnabled) return;
        
        if (!PredictedFrame.Has<FrameMeterData>(EntityRef)) return;
        
        PlayerLink playerLink = PredictedFrame.Get<PlayerLink>(EntityRef);
        if ((int)playerLink.Player != 0) transform.GetComponent<HorizontalLayoutGroup>().padding.bottom = 60;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        
        var frameMeterData = PredictedFrame.Get<FrameMeterData>(EntityRef);
        QList<int> types = PredictedFrame.ResolveList(frameMeterData.types);
        QList<int> frames = PredictedFrame.ResolveList(frameMeterData.frames);
        
        for (int i = 0; i < types.Count; i++)
        {
            GameObject obj = GetPooledObject(); 
            if (obj != null) {
                var img = obj.GetComponent<Image>();
                obj.GetComponentInChildren<TextMeshProUGUI>().text = frames[i].ToString();
                var c = FrameColor(types[i]);
                c.a = 0.85f;
                img.color = c;
                obj.SetActive(true);
            }
        }
    }
    
    private Color FrameColor(int type)
    {
        switch (type)
        {
            case (int)PlayerFSM.FrameMeterType.None:
            {
                return Color.black;
            }
            case (int)PlayerFSM.FrameMeterType.Startup:
                return Color.cyan;
            case (int)PlayerFSM.FrameMeterType.Active:
                return Color.red;
            case (int)PlayerFSM.FrameMeterType.Recovery:
                return Color.blue;
            case (int)PlayerFSM.FrameMeterType.HitStun:
                return Color.yellow;
            case (int)PlayerFSM.FrameMeterType.BlockStun:
                return Color.green;
            case (int)PlayerFSM.FrameMeterType.Oki:
                return Color.magenta;
            default:
                return Color.black;
        }
    }
    
    public GameObject GetPooledObject()
    {
        for(int i = 0; i < poolSize; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
