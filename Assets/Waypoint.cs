using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Entity entity;
    public static float angle = 0;
    public float speed = 25;

    private LineRenderer lr;

    private void Awake()
    {
        lr = transform.GetComponent<LineRenderer>();
    }

    public void DrawLineToPoint(Vector3 point)
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, point);
    }

    public void SetFocus(bool state)
    {
        if (state)
        {
            Renderer renderer = transform.GetComponent<Renderer>();
            renderer.material = entity.focusedMat;
            lr.material = entity.focusedMat;
            Vector3 pos = transform.position;
            pos.y = 0.01f;
            transform.position = pos;
            DrawLineToPoint(lr.GetPosition(1) + new Vector3(0, 0.01f, 0));
        }
        else
        {
            Renderer renderer = transform.GetComponent<Renderer>();
            renderer.material = entity.unfocusedMat;
            lr.material = entity.unfocusedMat;
            Vector3 pos = transform.position;
            pos.y = 0.0f;
            transform.position = pos;
            DrawLineToPoint(lr.GetPosition(1) + new Vector3(0, -0.01f, 0));
        }
    }
    // Update is called once per frame
    void Update()
    {
        angle = speed * Time.realtimeSinceStartup;
        if (angle > 360)
        {
            angle -= 360;
        }
        Vector3 angles = transform.eulerAngles;
        angles.y = angle;
        transform.eulerAngles = angles;
    }
}
