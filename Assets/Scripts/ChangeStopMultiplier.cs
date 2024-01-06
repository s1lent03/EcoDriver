using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStopMultiplier : MonoBehaviour
{
    public float newStopMultiplier;
    private float stopMultiplier;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Car"))
        {
            stopMultiplier = other.GetComponent<AI>().stopMultiplier;
            other.GetComponent<AI>().stopMultiplier = newStopMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Car"))
        {
            other.GetComponent<AI>().stopMultiplier = stopMultiplier;
        }
    }
}
