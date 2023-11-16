using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [Header("Variables")]
    public float speed;
    public float turningSpeed;
    public float stopMultiplier;

    private float currentSpeed;
    private Vector3 direction;
    private bool doJunction;
    private List<Transform> paths = new List<Transform>();
    private Transform road;

    private void Awake()
    {
        currentSpeed = speed;
    }

    private void Update()
    {
        direction = Vector3.zero;

        if (paths.Count > 0)
        {
            if (paths[paths.Count - 1].CompareTag("Road"))
            {
                Road();
            } else if (paths[paths.Count - 1].CompareTag("Stop"))
            {
                Stop();
            } else if (paths[paths.Count - 1].CompareTag("Junction"))
            {
                Junction();
            } else if (paths[paths.Count - 1].CompareTag("Roundabout"))
            {
                Roundabout();
            }
        } else if (doJunction)
        {
            Junction();
        } else if (road != null)
        {
            Road();
        }

        transform.position += direction;
    }

    private void Road()
    {
        Move();

        if (road != null)
        {
            transform.forward = Vector3.Slerp(transform.forward, road.forward, Time.deltaTime * turningSpeed);

            if (paths.Count > 0 && paths[paths.Count - 1] == road)
                road = null;
        } else
        {
            transform.forward = Vector3.Slerp(transform.forward, paths[paths.Count - 1].forward, Time.deltaTime * turningSpeed);
        }

        direction = transform.forward * currentSpeed * Time.deltaTime;
    }

    private void Stop()
    {
        if (currentSpeed > 0)
            currentSpeed -= Time.deltaTime * stopMultiplier;
        else
            currentSpeed = 0;

        direction = transform.forward * currentSpeed * Time.deltaTime;
    }

    private void Move()
    {
        if (currentSpeed < speed)
            currentSpeed += Time.deltaTime * stopMultiplier;
        else
            currentSpeed = speed;
    }

    private void Junction()
    {
        Move();

        if (!doJunction)
        {
            doJunction = true;
            road = paths[paths.Count - 1].GetComponent<RoadSelector>().obtainRandomRoad();
        }

        if (Mathf.Round(transform.position.x - road.position.x) != 0 && Mathf.Round(transform.position.z - road.position.z) != 0)
            direction = transform.forward * currentSpeed * Time.deltaTime;
        else
            doJunction = false;
    }

    private void Roundabout()
    {
        Debug.Log(paths[paths.Count - 1].GetComponent<RoadSelector>().roads[0].name + "|" + (transform.position - paths[paths.Count - 1].GetComponent<RoadSelector>().roads[0].position).magnitude);
        Debug.Log(paths[paths.Count - 1].GetComponent<RoadSelector>().roads[1].name + "|" + (transform.position - paths[paths.Count - 1].GetComponent<RoadSelector>().roads[1].position).magnitude);
        Debug.Log(paths[paths.Count - 1].GetComponent<RoadSelector>().roads[2].name + "|" + (transform.position - paths[paths.Count - 1].GetComponent<RoadSelector>().roads[2].position).magnitude);
        /*Move();

        if (!doJunction)
        {
            doJunction = true;
            road = paths[paths.Count - 1].GetComponent<RoadSelector>().obtainRandomRoad();
        }

        if (Mathf.Round(transform.position.x - road.position.x) != 0 && Mathf.Round(transform.position.z - road.position.z) != 0)
            direction = transform.forward * currentSpeed * Time.deltaTime;
        else
            doJunction = false;*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!paths.Contains(other.transform) && !other.transform.CompareTag("Untagged"))
            paths.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (paths.Contains(other.transform))
            paths.Remove(other.transform);
    }
}
