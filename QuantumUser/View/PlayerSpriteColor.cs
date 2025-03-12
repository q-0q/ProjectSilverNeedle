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
using LayerMask = UnityEngine.LayerMask;

public class PlayerSpriteColor : QuantumEntityViewComponent
{
    private SpriteRenderer _renderer;
    private SpriteRenderer _shadowCasterRenderer;
    private Color _colorA;
    private Color _colorB;
    readonly float saturation = 0.85f;

    
    // public override void OnInitialize()
    // {
    //     _renderer = GetComponent<SpriteRenderer>();
    //     _colorA = new Color(1, saturation, saturation);
    //     _colorB = new Color(saturation, saturation, 1);
    // }
    //
    // public override void OnUpdateView()
    // {
    //     
    //     _renderer.color = PredictedFrame.Get<PlayerLink>(EntityRef).Player == 0 ? _colorA  :  _colorB;
    // }
}
