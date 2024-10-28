using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;


public class ComboCounter : QuantumEntityViewComponent
{
    private Transform _ejected;
    
    private TextMeshProUGUI _mainTmp;
    private TextMeshProUGUI _gravityScalingTmp;
    private TextMeshProUGUI _damageScalingTmp;
    
    private Vector3 _defaultLocalPosition;

    private float _mainFontSize;
    private int length = 0;
    private int shownLength = 0;

    
    public override void OnInitialize()
    {
        _defaultLocalPosition = transform.localPosition;
        QuantumEvent.Subscribe(listener: this, handler: (EventEntityVibrate e) => ComboCounterVibrate(e.entityRef, e.strength, e.duration, e.vibrato));
    }

    private void Awake()
    {
        _ejected = transform.Find("EjectedComboCanvas");
        _mainTmp = _ejected.Find("Main").GetComponent<TextMeshProUGUI>();
        _gravityScalingTmp = _ejected.Find("GravityScaling").GetComponent<TextMeshProUGUI>();
        _damageScalingTmp = _ejected.Find("DamageScaling").GetComponent<TextMeshProUGUI>();
        _mainFontSize = _mainTmp.fontSize;
    }

    private void Start()
    {
        // _ejected.SetParent(null, true);
    }

    public override void OnUpdateView()
    {
        if (!PredictedFrame.Has<ComboData>(EntityRef)) return;
        
        length = PredictedFrame.Get<ComboData>(EntityRef).length;
        FP gravityScaling = PredictedFrame.Get<ComboData>(EntityRef).gravityScaling;
        FP damageScaling = PredictedFrame.Get<ComboData>(EntityRef).damageScaling;

        shownLength = Math.Max(shownLength, length);
        if (length < shownLength && length != 0) shownLength = length;

        _mainTmp.text = shownLength > 1 ? shownLength.ToString() : "";
        
        _gravityScalingTmp.text = length > 1 ? Truncate(gravityScaling.ToString()) : "";
        _damageScalingTmp.text = length > 1 ? Truncate(damageScaling.ToString()) : "";
        
        // _tmp.text = length.ToString();

        UpdateFlip();
    }

    private void Update()
    {
        _mainTmp.fontSize = Mathf.Lerp(_mainTmp.fontSize, length <= 1 ? 0 : _mainFontSize, Time.deltaTime * 40f);
    }

    private void UpdateFlip()
    {
        if (!PredictedFrame.Get<PlayerDirection>(EntityRef).FacingRight)
        {
            Vector3 flippedLocalPosition =
                new Vector3(-_defaultLocalPosition.x, _defaultLocalPosition.y, _defaultLocalPosition.z);
            
            transform.localPosition = flippedLocalPosition;
        }
        else
        {
            transform.localPosition = _defaultLocalPosition;
        }
    }

    private void ComboCounterVibrate(EntityRef entityRef, FP strength, FP duration, int vibrato)
    {
        if (entityRef != EntityRef) return;
        transform.localPosition = _defaultLocalPosition;
        UpdateFlip();
        transform.DOShakePosition(duration.AsFloat, strength.AsFloat * 0.5f, vibrato, 90f, false, true, ShakeRandomnessMode.Full);
    }

    private string Truncate(string s)
    {
        int l = Math.Min(s.Length, 5);
        return s[..l];
    }
}
