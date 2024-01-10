using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    public float rotationPower;
    public PlayerInput playerInput;

    private void Update()
    {
        Vector2 mouseMovement = playerInput.actions["Look"].ReadValue<Vector2>();
        transform.rotation *= Quaternion.AngleAxis(mouseMovement.x * rotationPower, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(mouseMovement.y * rotationPower, Vector3.up);
    }
}
