using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    Entity entity;
    public List<Waypoint> targets;
    public float distanceToTarget;
    public float targetTollerance = 2f;
    public float targetSpeed = 0;
    public float stoppingDistance;


    // Start is called before the first frame update
    void Awake()
    {
        entity = transform.GetComponent<Entity>();
    }

    public void AddTask(Waypoint wp)
    {
        if (targets.Count > 0)
        {
            wp.DrawLineToPoint(targets[targets.Count - 1].transform.position);
        } else
        {
            wp.DrawLineToPoint(wp.transform.position);
        }
        targets.Add(wp);
    }

    public void SetFocus(bool state)
    {
        foreach (Waypoint waypoint in targets)
        {
            waypoint.SetFocus(state);
        }

    }

    public void ClearTasks()
    {
        foreach (Waypoint waypoint in targets)
        {
            Destroy(waypoint.gameObject);
        }
        targets.Clear();
    }

    public Vector3 GetLastWaypointPosition()
    {
        if (targets.Count > 0)
        {
            return targets[targets.Count - 1].transform.position;
        }
        return transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (targets.Count > 0)
        {
            targets[0].DrawLineToPoint(transform.position);

            Vector3 diff = targets[0].transform.position - transform.position;
            distanceToTarget = diff.magnitude;

            Vector3 dir = diff.normalized;
            entity.orientor.directionVector += dir;
            /*entity.orientor.desiredHeading = Mathf.Rad2Deg * Mathf.Atan2(-dir.z, dir.x);*/

            // Computing desired speed

            float dot;
            if (targets.Count > 1)
            {
                dot = 1 - Mathf.Abs(Vector3.Dot(transform.forward, (targets[1].transform.position - targets[0].transform.position).normalized));
                float approachSpeed = entity.movement.maxSpeed * dot + 0.05f;
                if (entity.movement.speed > approachSpeed)
                {
                    float timeToStop = (entity.movement.speed - approachSpeed) / (entity.movement.accelRate);
                    stoppingDistance = (0.5f * entity.movement.speed * timeToStop);/* + (0.01f * entity.movement.accelRate * Mathf.Pow(timeToStop, 2f));*/

                    if (distanceToTarget < stoppingDistance)
                    {
                        entity.movement.desiredSpeed = approachSpeed ; //entity.movement.desiredSpeed - entity.movement.accelRate * Time.deltaTime;
                    }
                }
                else
                {
                    dot = 1 - Mathf.Abs(Vector3.Dot(transform.forward, (targets[0].transform.position - transform.position).normalized));
                    entity.movement.desiredSpeed = entity.movement.maxSpeed * dot + 0.05f;
                }
            } else if (targets.Count == 1)
            {

                if (entity.movement.speed > 0)
                {
                    float timeToStop = (entity.movement.speed) / (entity.movement.accelRate);
                    stoppingDistance = (0.5f * entity.movement.speed * timeToStop);/* + (0.01f * entity.movement.accelRate * Mathf.Pow(timeToStop, 2f));*/
                } else
                {
                    stoppingDistance = 0;
                }


                if (distanceToTarget < stoppingDistance)
                {
                    entity.movement.desiredSpeed = 0; //entity.movement.desiredSpeed - entity.movement.accelRate * Time.deltaTime;
                } else
                {
                    dot = 1 - Mathf.Abs(Vector3.Dot(transform.forward, (targets[0].transform.position - transform.position).normalized));
                    entity.movement.desiredSpeed = entity.movement.maxSpeed * dot + 0.05f;
                }
            }



            if (distanceToTarget < targetTollerance)
            {
                Waypoint temp = targets[0];
                targets.RemoveAt(0);
                Destroy(temp.gameObject);
                entity.movement.desiredSpeed = 0;
                if (targets.Count > 0)
                {
                    targets[0].DrawLineToPoint(targets[0].transform.position);
                }
            }
        }
    }
}
