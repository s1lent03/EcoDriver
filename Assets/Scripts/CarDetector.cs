using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetector : MonoBehaviour
{
    private List<Transform> cars = new List<Transform>();
    private Transform road;
    private string roadTag;

    private void Awake()
    {
        road = transform.parent.transform;
        roadTag = road.tag;
    }

    private void Update()
    {
        if (cars.Count > 0)
            road.tag = "Stop";
        else
            road.tag = roadTag;
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
