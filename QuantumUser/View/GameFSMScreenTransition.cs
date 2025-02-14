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

public class GameFSMScreenTransition : QuantumEntityViewComponent
{
    private Image _image;
    public Color Color = Color.black;

    private int FadeInDuration = 5;
    private int DistortionDuration = 65;
    private float DistortionAmount = -0.35f;
    private int FadeOutDuration = 5;

    public static event Action OnCountdownStart;
    public static event Action OnResetStart;

    private LensDistortion _lensDistortion;
    
    public override void OnInitialize()
    {
        _image = GetComponent<Image>();
    }

    public override void OnUpdateView()
    {
        _image.color = GetColor();
    }

    private void Start()
    {
        FindObjectOfType<Volume>().profile.TryGet<LensDistortion>(out _lensDistortion);
        OnCountdownStart?.Invoke();
    }

    private Color GetColor()
    {
        var gameFsmData = PredictedFrame.Get<GameFSMData>(EntityRef);
        var state = (GameFSM.State)PredictedFrame.Get<GameFSMData>(EntityRef).currentState;
        var frames = PredictedFrame.Get<GameFSMData>(EntityRef).framesInState;
        float alpha = 0;

        if (state is GameFSM.State.Countdown)
        {
            OnCountdownStart?.Invoke();
            alpha = Mathf.InverseLerp(FadeInDuration, 0, frames);
            var distortion = Mathf.InverseLerp(DistortionDuration, 0, frames) * DistortionAmount;
            if (_lensDistortion is not null) _lensDistortion.intensity.value = distortion;
        }
        else if (state is GameFSM.State.RoundEnd)
        {
            alpha = Mathf.InverseLerp(GameFSMSystem.RoundEndDuration - FadeOutDuration, GameFSMSystem.RoundEndDuration, frames);
        }
        else if (state is GameFSM.State.RoundResetting)
        {
            OnResetStart?.Invoke();
            alpha = 1f;
        }

        var c = Color;
        c.a = alpha;

        return c;

    }
}
