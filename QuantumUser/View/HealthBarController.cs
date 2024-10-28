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

public class HealthBarController : MonoBehaviour
{
    public static HealthBarController Instance;

    public RectTransform Player0HealthBar;
    public RectTransform Player1HealthBar;
    
    public RectTransform Player0ComboBar;
    public RectTransform Player1ComboBar;

    private float _maxBarLength;
    
    private Vector3 _0defaultLocalPosition;
    private Vector3 _1defaultLocalPosition;

    private TextMeshProUGUI _p0Score;
    private TextMeshProUGUI _p1Score;

    private float prev0Hp = 500f;
    private float prev1Hp = 500f;

    private const string RoundsPrefix = "ROUNDS: ";

    private void Awake()
    {
        Instance = this;
        
        Player0HealthBar = transform.Find("HealthBar0").Find("Holder").Find("Red").GetComponent<RectTransform>();
        Player1HealthBar = transform.Find("HealthBar1").Find("Holder").Find("Red").GetComponent<RectTransform>();

        _0defaultLocalPosition = transform.Find("HealthBar0").Find("Holder").localPosition;
        _1defaultLocalPosition = transform.Find("HealthBar1").Find("Holder").localPosition;
        
        Player0ComboBar = transform.Find("HealthBar0").Find("Holder").Find("Combo").GetComponent<RectTransform>();
        Player1ComboBar = transform.Find("HealthBar1").Find("Holder").Find("Combo").GetComponent<RectTransform>();

        _p0Score = transform.Find("HealthBar0").GetComponentInChildren<TextMeshProUGUI>();
        _p1Score = transform.Find("HealthBar1").GetComponentInChildren<TextMeshProUGUI>();
        
        _maxBarLength = Player0HealthBar.localScale.x;

    }

    public void UpdatePlayerHealth(int playerId, float health, int comboLength, int score)
    {

        if (health < (playerId == 0 ? prev0Hp : prev1Hp))
        {
            Vibrate(playerId, 15f, 0.4f, 20);
        }

        if (playerId == 0) prev0Hp = health;
        if (playerId == 1) prev1Hp = health;

        if (playerId == 0) _p0Score.text = RoundsPrefix + score;
        if (playerId == 1) _p1Score.text = RoundsPrefix + score;

        
        RectTransform healthRectTransform = playerId == 0 ? Player0HealthBar : Player1HealthBar;
        RectTransform comboRectTransform = playerId == 0 ? Player0ComboBar : Player1ComboBar;
        
        
        var healthLocalScale = healthRectTransform.localScale;
        healthLocalScale = new Vector3(GetWidth(health), healthLocalScale.y, healthLocalScale.z);
        healthRectTransform.localScale = healthLocalScale;
        

        if (comboLength != 0 && health > 0) return;
        
        var comboLocalScale = comboRectTransform.localScale;
        comboLocalScale = new Vector3(healthLocalScale.x, comboLocalScale.y, comboLocalScale.z);
        comboRectTransform.localScale = Vector3.Lerp(comboRectTransform.localScale, comboLocalScale, Time.deltaTime * 10f);

    }

    private float GetWidth(float health)
    {
        var maxBarLength = (health / 500f) * _maxBarLength;
        return Mathf.Max(maxBarLength, 0);
    }
    
    
    private void Vibrate(int player, float strength, float duration, int vibrato)
    {
        Transform t = player == 0 ? transform.Find("HealthBar0").Find("Holder") : transform.Find("HealthBar1").Find("Holder");
        t.localPosition = player == 0 ? _0defaultLocalPosition : _1defaultLocalPosition;
        t.DOShakePosition(duration, strength, vibrato, 90f, false, true, ShakeRandomnessMode.Full);
    }
}