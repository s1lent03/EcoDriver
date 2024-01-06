using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSign : MonoBehaviour
{
    public GameManager gameManager;
    public CarDetector carDetector;
    private List<Transform> cars = new List<Transform>();
    private float timer;

    private void Update()
    {
        if (cars.Count > 0)
        {
            if (!transform.CompareTag("Stop"))
                transform.tag = "Stop";
            else if (timer < gameManager.MinTimeOnStop)
                timer += Time.deltaTime;
            else
                carDetector.activate = true;
        } else
        {
            carDetector.activate = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!cars.Contains(other.transform) && other.transform.CompareTag("Car"))
            cars.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (cars.Contains(other.transform))
            cars.Remove(other.transform);
    }
}
