using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    public PlayerInput playerInput;
    public CinemachineVirtualCamera cameraToSwitch;

    // Update is called once per frame
    void Update()
    {
        if (playerInput.actions["ChangeCamera"].triggered)
            if (cameraToSwitch.Priority == 9)
                cameraToSwitch.Priority = 11;
            else
                cameraToSwitch.Priority = 9;
    }
}
