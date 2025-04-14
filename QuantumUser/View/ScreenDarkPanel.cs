using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using Quantum.Types;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ScreenDarkPanel : MonoBehaviour
{
    private Image _image;
    
    public void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Update()
    {
        _image.color = GetColor();
    }
    
    private Color GetColor()
    {
        var c = _image.color;
        c.a = CameraTargetController.Instance.Player0Dark || CameraTargetController.Instance.Player1Dark ? 0.8f : Mathf.Lerp(c.a, 0, Time.deltaTime * 10);

        return c;

    }
}
