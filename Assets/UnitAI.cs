using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    Entity entity;
    public List<Waypoint> targets;
    public float distanceToTarget;
    public float targetTollerance = 2f;

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

    // Update is called once per frame
    void Update()
    {
        if (targets.Count > 0)
        {
            targets[0].DrawLineToPoint(transform.position);

            Vector3 diff = targets[0].transform.position - transform.position;
            distanceToTarget = diff.magnitude;

            diff = diff.normalized;
            entity.orientor.desiredHeading = Mathf.Rad2Deg * Mathf.Atan2(-diff.z, diff.x);

            float dot = 1 - Mathf.Abs(Vector3.Dot(transform.forward, (targets[0].transform.position - transform.position).normalized));
            entity.movement.desiredSpeed = entity.movement.maxSpeed * dot + 0.05;

            if (distanceToTarget < targetTollerance)
            {
                Waypoint temp = targets[0];
                targets.RemoveAt(0);
                Destroy(temp.gameObject);
                entity.movement.desiredSpeed = 0;
            }
        }
    }
}
