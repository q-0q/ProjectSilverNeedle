using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Deterministic;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Input;

public class CollisionBox : QuantumEntityViewComponent
{
    private SpriteRenderer _spriteRenderer;
    private static float _alpha = 0.4f;
    private static Color _hurtboxColor = new(0, 0f, 255f, _alpha);
    private static Color _hitboxColor = new(255f, 0, 0, _alpha);
    private static Color _pushboxColor = new(255f, 255f, 0, _alpha);
    
    
    
    public override void OnInitialize()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Debug.Log(FrameMeterReporter.CollisionBoxViewEnabled);
        // _alpha = FrameMeterReporter.CollisionBoxViewEnabled ? 0.3f : 0;
        _spriteRenderer.color = GetColor();
    }
    
    public override void OnUpdateView()
    {
        FP width = PredictedFrame.Get<CollisionBoxData>(EntityRef).width;
        FP height = PredictedFrame.Get<CollisionBoxData>(EntityRef).height;

        _spriteRenderer.transform.localScale = new Vector3(width.AsFloat, height.AsFloat, 1f);
        _spriteRenderer.transform.localPosition = new Vector3(0, 0, -0.2f);
    }

    private Color GetColor()
    {
        
        var type = 
            (Quantum.Types.Collision.CollisionBox.CollisionBoxType)PredictedFrame.Get<CollisionBoxData>(EntityRef).type;
        
        // if (type != Quantum.Types.Collision.CollisionBox.CollisionBoxType.Hitbox 
        //     && type != Quantum.Types.Collision.CollisionBox.CollisionBoxType.Hurtbox) return Color.clear;

        Color color;
        if (type == Quantum.Types.Collision.CollisionBox.CollisionBoxType.Hitbox)
            color = _hitboxColor;
        else if (type == Quantum.Types.Collision.CollisionBox.CollisionBoxType.Hurtbox)
            color = _hurtboxColor;
        else if (type == Quantum.Types.Collision.CollisionBox.CollisionBoxType.Pushbox)
            color = _pushboxColor;
        else if (type == Quantum.Types.Collision.CollisionBox.CollisionBoxType.Throwbox)
            color = Color.green;
        else
            color = Color.black;

        color.a = _alpha;
        return color;
    }
    
    
}
