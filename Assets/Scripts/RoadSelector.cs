using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSelector : MonoBehaviour
{
    public Transform[] roads;

    public Transform obtainRandomRoad()
    {
        return roads[Random.Range(0, roads.Length)];
    }
}
