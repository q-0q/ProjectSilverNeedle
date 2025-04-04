using System.Collections;
using System.Collections.Generic;
using Quantum;
using Quantum.Types.Collision;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class PSNGameSettings : MonoBehaviour
{
    private Light _light;
    // Start is called before the first frame update
    void Start()
    {
        _light = FindObjectOfType<Light>();
    }

    public void ToggleCollisionBoxes()
    {
        CollisionBoxViewer.OnCollisionBoxesToggled?.Invoke();
    }
    
    public void ToggleFrameMeter()
    {
        FrameMeterReporter.OnFrameMeterToggled?.Invoke();
    }
    
    public void ToggleShadows()
    {
        _light.shadows = _light.shadows == LightShadows.Hard ? LightShadows.None : LightShadows.Hard;
    }
    
    public void UpdateCharacter(TMPro.TMP_Dropdown dropdown)
    {
        CommandUpdatePlayerCharacter command = new CommandUpdatePlayerCharacter()
        {
            id = dropdown.value,
            player = QuantumRunner.Default.Game.GetLocalPlayers()[0]
        };
        QuantumRunner.Default.Game.SendCommand(command);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
