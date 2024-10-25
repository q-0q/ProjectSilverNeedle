using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Client.StructWrapping;
using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEventsCanvas : QuantumEntityViewComponent
{
    private TextMeshProUGUI _tmp;
    private float timeOnUpdate = 0;
    private float totalDisplayTime = 2.65f;
    private Vector3 _tmpBaseLocalPos;
    
    public override void OnInitialize()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
        QuantumEvent.Subscribe(listener: this, handler: (EventGameEvent e) => UpdateText(e.entityRef, e.type));
        _tmpBaseLocalPos = _tmp.transform.localPosition;

    }

    public override void OnUpdateView()
    {
        PlayerLink playerLink = PredictedFrame.Get<PlayerLink>(EntityRef);
        if ((int)playerLink.Player != 0) _tmp.alignment = TextAlignmentOptions.Right;
    }

    private void Update()
    {
        if (Time.time - timeOnUpdate > totalDisplayTime)
        {
            _tmp.text = "";
        }
    }
    
    private void UpdateText(EntityRef entityRef, GameEventType type)
    {
        if (entityRef != EntityRef) return;
        _tmp.text = type.ToString();
        _tmp.color = GetColor(type);
        timeOnUpdate = Time.time;

        float duration = 0.3f;
        float strength = type is GameEventType.Counter ? 20 : 15;
        int vibrato = type is GameEventType.Counter ? 35 : 20;
        
        _tmp.transform.localPosition = _tmpBaseLocalPos;
        _tmp.transform.DOShakePosition(duration, strength, vibrato, 90f, false, true, ShakeRandomnessMode.Full);
    }

    private Color GetColor(GameEventType type)
    {
        switch (type)
        {
            case GameEventType.Counter:
            {
                return Color.yellow;
            }
            case GameEventType.Punish:
            {
                return Color.red;
            }
            case GameEventType.Knockdown:
            {
                return Color.magenta;
            }
            default:
            {
                return Color.white;
            }
        }
    }
    
}
