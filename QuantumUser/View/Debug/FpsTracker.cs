using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsTracker
    : MonoBehaviour
{
    private float fps = 60f;
    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float newFPS = 1.0f / Time.unscaledDeltaTime;
        fps = Mathf.Lerp(fps, newFPS, 0.005f);
        _tmp.text = "FPS: " + ((int)fps);
    }
}
