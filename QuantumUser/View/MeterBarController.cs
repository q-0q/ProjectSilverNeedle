using System;
using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Input = UnityEngine.Input;

public class MeterBarController : MonoBehaviour
{
    public static MeterBarController Instance;

    public RectTransform Player0MeterBar;
    public RectTransform Player1MeterBar;
    
    public RectTransform Player0ComboBar;
    public RectTransform Player1ComboBar;

    private float _maxBarLength;
    
    private Vector3 _0defaultLocalPosition;
    private Vector3 _1defaultLocalPosition;
    
    private float prev0Meter = 0f;
    private float prev1Meter = 0f;
    

    private void Awake()
    {
        Instance = this;
        
        Player0MeterBar = transform.Find("MeterBar0").Find("Holder").Find("Red").GetComponent<RectTransform>();
        Player1MeterBar = transform.Find("MeterBar1").Find("Holder").Find("Red").GetComponent<RectTransform>();

        _0defaultLocalPosition = transform.Find("MeterBar0").Find("Holder").localPosition;
        _1defaultLocalPosition = transform.Find("MeterBar1").Find("Holder").localPosition;
        
        Player0ComboBar = transform.Find("MeterBar0").Find("Holder").Find("Combo").GetComponent<RectTransform>();
        Player1ComboBar = transform.Find("MeterBar1").Find("Holder").Find("Combo").GetComponent<RectTransform>();
        
        _maxBarLength = Player0MeterBar.localScale.x;

    }

    public void UpdatePlayerMeter(int playerId, float meter)
    {

        if (meter < (playerId == 0 ? prev0Meter : prev1Meter))
        {
            Vibrate(playerId, 15f, 0.4f, 20);
        }

        if (playerId == 0) prev0Meter = meter;
        if (playerId == 1) prev1Meter = meter;

        
        RectTransform healthRectTransform = playerId == 0 ? Player0MeterBar : Player1MeterBar;
        RectTransform comboRectTransform = playerId == 0 ? Player0ComboBar : Player1ComboBar;
        
        
        var healthLocalScale = healthRectTransform.localScale;
        healthLocalScale = new Vector3(GetWidth(meter), healthLocalScale.y, healthLocalScale.z);
        healthRectTransform.localScale = healthLocalScale;
        
        var comboLocalScale = comboRectTransform.localScale;
        comboLocalScale = new Vector3(healthLocalScale.x, comboLocalScale.y, comboLocalScale.z);
        comboRectTransform.localScale = Vector3.Lerp(comboRectTransform.localScale, comboLocalScale, Time.deltaTime * 10f);

    }

    private float GetWidth(float health)
    {
        var maxBarLength = (health / 100f) * _maxBarLength;
        return Mathf.Max(maxBarLength, 0);
    }
    
    
    private void Vibrate(int player, float strength, float duration, int vibrato)
    {
        Transform t = player == 0 ? transform.Find("MeterBar0").Find("Holder") : transform.Find("MeterBar1").Find("Holder");
        t.localPosition = player == 0 ? _0defaultLocalPosition : _1defaultLocalPosition;
        t.DOShakePosition(duration, strength, vibrato, 90f, false, true, ShakeRandomnessMode.Full);
    }
}