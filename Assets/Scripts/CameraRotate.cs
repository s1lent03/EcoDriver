using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    public Transform follow;
    public float rotationSpeed;

    private void Update()
    {
        transform.position = follow.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, follow.rotation, rotationSpeed * Time.deltaTime);
    }
}
