using System.Collections.Generic;
using UnityEngine;

public enum Options
{
    Road,
    Car
}

public class CarDetector : MonoBehaviour
{
    public Options option = new Options();
    public bool activate;
    [SerializeField] private List<Transform> cars = new List<Transform>();
    private Transform road;
    private string roadTag;

    private void Awake()
    {
        if (option == Options.Road)
        {
            road = transform.parent.transform;
            roadTag = road.tag;
        }
    }

    private void Update()
    {
        if (option == Options.Road)
        {
            if (activate)
            {
                if (cars.Count > 0)
                {
                    road.tag = "Stop";
                } else
                {
                    road.tag = roadTag;
                }
            }
        } else if (option == Options.Car)
        {
            transform.parent.GetComponent<AI>().forceStop = (cars.Count > 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!cars.Contains(other.transform) && (other.transform.CompareTag("Car") || other.transform.CompareTag("Player")))
            cars.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (cars.Contains(other.transform))
            cars.Remove(other.transform);
    }
}
