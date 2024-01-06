using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLights : MonoBehaviour
{
    public GameObject sign;
    private string roadTag;

    private void Awake()
    {
        roadTag = transform.tag;
    }

    private void Update()
    {
        bool canPass = sign.GetComponentInChildren<TraficWarningsDetector>().CanPassByTrafficLight();

        if (!canPass)
            transform.tag = "Stop";
        else
            transform.tag = roadTag;
    }
}
