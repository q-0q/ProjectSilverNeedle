using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Deterministic;
using Quantum;
using Quantum.Types.Collision;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Input;

public class CollisionBoxViewer : QuantumEntityViewComponent
{
    private bool _boxesEnabled = false;
    private SpriteRenderer _spriteRenderer;
    private static float _alpha = 1f;
    private static Color _hurtboxColor = new(0, 0f, 255f, _alpha);
    private static Color _hitboxColor = new(255f, 0, 0, _alpha);
    private static Color _pushboxColor = new(255f, 255f, 0, _alpha);

    private GameObject _lineRendererPrefab;
    private List<LineRenderer> _lineRenderers;
    
    public static Action OnCollisionBoxesToggled;


    private void OnEnable()
    {
        OnCollisionBoxesToggled += Toggle;
    }
    
    private void OnDisable()
    {
        OnCollisionBoxesToggled -= Toggle;
    }

    public void Toggle()
    {
        _boxesEnabled = !_boxesEnabled;
        Debug.Log("Toggle: " + _boxesEnabled);
        if (_boxesEnabled) return;
        foreach (var t in _lineRenderers)
        {
            t.enabled = false;
        }
    }

    
    public override void OnInitialize()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _lineRendererPrefab = Resources.Load<GameObject>("Prefabs/CollisionBoxLineRenderer");
        _lineRenderers = new List<LineRenderer>();
    }

    private void Start()
    {
        // Debug.Log(FrameMeterReporter.CollisionBoxViewEnabled);
        // _alpha = FrameMeterReporter.CollisionBoxViewEnabled ? 0.3f : 0;
        // _spriteRenderer.color = GetColor();
    }

    public override void OnUpdateView()
    {
        if (!_boxesEnabled) return;
        if (_alpha <= 0.005f) return; 
        if (!PredictedFrame.Has<FSMData>(EntityRef)) return;

        var internals = new List<PlayerFSM.CollisionBoxInternal>();
        var pushboxInternals =
            PlayerFSM.GetCollisionBoxInternalsOfType(PredictedFrame, EntityRef, CollisionBoxType.Pushbox);

        var hurtboxInternals =
            PlayerFSM.GetCollisionBoxInternalsOfType(PredictedFrame, EntityRef, CollisionBoxType.Hurtbox);

        var hitboxInternals = PlayerFSM.GetCollisionBoxInternalsOfType(PredictedFrame, EntityRef,
            CollisionBoxType.Hitbox);

        if (pushboxInternals is not null) internals.AddRange(pushboxInternals);
        if (hurtboxInternals is not null) internals.AddRange(hurtboxInternals);
        if (hitboxInternals is not null) internals.AddRange(hitboxInternals);
        
        int requiredLineRenderers = internals.Count;

        // Make sure there are enough LineRenderers in the pool
        while (_lineRenderers.Count < requiredLineRenderers)
        {
            LineRenderer newLr = Instantiate(_lineRendererPrefab, null).GetComponent<LineRenderer>();
            _lineRenderers.Add(newLr);
        }

        // Disable unused LineRenderers
        for (int i = requiredLineRenderers; i < _lineRenderers.Count; i++)
        {
            _lineRenderers[i].enabled = false;
        }

        // Draw each rectangle
        for (int i = 0; i < requiredLineRenderers; i++)
        {
            var _internal = internals[i];
            var lr = _lineRenderers[i];
            lr.enabled = true;

            var color = _internal.type switch
            {
                CollisionBoxType.Hitbox => _internal.HitType == Hit.HitType.Throw ? Color.green : Color.red,
                CollisionBoxType.Hurtbox => Color.blue,
                _ => Color.yellow
            };

            var pos = new Vector3(_internal.pos.X.AsFloat, _internal.pos.Y.AsFloat, 0);
            DrawRectangle(lr, pos, _internal.width.AsFloat, _internal.height.AsFloat, color);
        }
    }
    
    void DrawRectangle(LineRenderer lr, Vector3 position, float width, float height, Color color)
    {
        // Define the four corners of the rectangle
        Vector3[] positions = new Vector3[5];
        positions[0] = position + new Vector3(-width / 2, -height / 2, 0); // Bottom-left
        positions[1] = position + new Vector3(width / 2, -height / 2, 0);  // Bottom-right
        positions[2] = position + new Vector3(width / 2, height / 2, 0);   // Top-right
        positions[3] = position + new Vector3(-width / 2, height / 2, 0);  // Top-left
        positions[4] = positions[0]; // Close the rectangle (Back to the bottom-left)

        lr.positionCount = 5;
        
        lr.material.SetColor("_Color", color);
        // lr.startColor = color;
        // lr.endColor = color;
        lr.SetPositions(positions);
    }
    
}
    
    // public override void OnUpdateView()
    // {
    //     FP width = PredictedFrame.Get<CollisionBoxData>(EntityRef).width;
    //     FP height = PredictedFrame.Get<CollisionBoxData>(EntityRef).height;
    //
    //     _spriteRenderer.transform.localScale = new Vector3(width.AsFloat, height.AsFloat, 1f);
    //     _spriteRenderer.transform.localPosition = new Vector3(0, 0, -0.2f);
    // }

    // private Color GetColor()
    // {
    //     
    //     var type = 
    //         (Quantum.Types.Collision.CollisionBox.CollisionBoxType)PredictedFrame.Get<CollisionBoxData>(EntityRef).type;
    //     
    //     // if (type != Quantum.Types.Collision.CollisionBoxType.Hitbox 
    //     //     && type != Quantum.Types.Collision.CollisionBoxType.Hurtbox) return Color.clear;
    //
    //     Color color;
    //     if (type == Quantum.Types.Collision.CollisionBoxType.Hitbox)
    //         color = _hitboxColor;
    //     else if (type == Quantum.Types.Collision.CollisionBoxType.Hurtbox)
    //         color = _hurtboxColor;
    //     else if (type == Quantum.Types.Collision.CollisionBoxType.Pushbox)
    //         color = _pushboxColor;
    //     else if (type == Quantum.Types.Collision.CollisionBoxType.Throwbox)
    //         color = Color.green;
    //     else
    //         color = Color.black;
    //
    //     color.a = _alpha;
    //     return color;
    // }
    
    

