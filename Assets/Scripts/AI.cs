using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [Header("Variables")]
    public float speed;
    public float turningSpeed;
    public float stopMultiplier;

    [Header("Toggles")]
    public bool forceStop;

    private float currentSpeed;
    private Vector3 direction;
    private bool doJunction;
    private bool doRoundabout;
    private List<Transform> paths = new List<Transform>();
    private Transform road;
    private Transform[] roundabouts;
    private Transform closestRoundabout;
    private Transform previousRoundabout;
    private Transform destination;
    private string sideOfRoundabout;
    private float distance;

    private void Awake()
    {
        currentSpeed = speed;
    }

    private void Update()
    {
        direction = Vector3.zero;

        if (doJunction)
        {
            Junction();
        } else if (doRoundabout)
        {
            Roundabout();
        }
        else if (road != null)
        {
            Road();
        } else if (paths.Count > 0)
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
        }

        if (forceStop)
            currentSpeed -= Time.deltaTime;

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
        Move();

        if (!doRoundabout)
        {
            doRoundabout = true;
            roundabouts = paths[paths.Count - 1].GetComponent<RoadSelector>().roads;
            destination = paths[paths.Count - 1].GetComponent<RoadSelector>().obtainRandomRoad();
            GetClosestRoundabout(paths[paths.Count - 1]);
        }

        if (paths.Count <= 0 || paths[0].parent.name != sideOfRoundabout)
        {
            direction = transform.forward * currentSpeed * Time.deltaTime;
        }
        else
        {
            if (destination != closestRoundabout)
            {
                if (DidAIPassRoundabout())
                {
                    GetClosestRoundabout(closestRoundabout);
                } else
                {
                    Road();
                }
            } else
            {
                if (Mathf.Round(transform.position.x - destination.position.x) != 0 && Mathf.Round(transform.position.z - destination.position.z) != 0)
                {
                    Road();
                } else
                {
                    doRoundabout = false;
                    road = destination;
                    closestRoundabout = null;
                    previousRoundabout = null;
                }
            }
        }
    }

    private bool DidAIPassRoundabout()
    {
        float temp = (transform.position - closestRoundabout.position).magnitude;

        if (temp <= distance)
        {
            distance = temp;
            return false;
        } else
        {
            return true;
        }
    }

    private void GetClosestRoundabout(Transform current)
    {
        closestRoundabout = null;

        for (int i = 0; i < roundabouts.Length; i++)
        {
            if (roundabouts[i].parent == current.parent || (previousRoundabout != null && roundabouts[i].parent == previousRoundabout.parent))
            {
                continue;
            }
            
            if (closestRoundabout == null || (transform.position - roundabouts[i].position).magnitude < (transform.position - closestRoundabout.position).magnitude)
            {
                closestRoundabout = roundabouts[i];
            }
        }

        previousRoundabout = current;

        if (destination == closestRoundabout)
            sideOfRoundabout = "Outside";
        else
            sideOfRoundabout = "Inside";

        distance = (transform.position - closestRoundabout.position).magnitude;
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
