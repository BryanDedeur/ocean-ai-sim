using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public double accelRate;
    public double desiredSpeed;
    public double speed;
    [Range(-1,0)]
    public float minSpeed = 0f;
    [Range(0, 5)]
    public float maxSpeed = 2f;

    public bool lockAtHeight = true;
    private float height;

    public ParticleSystem wake1;
    public ParticleSystem wake2;

    // Start is called before the first frame update
    void Awake()
    {
        height = transform.position.y;
        wake1.Stop();
        wake2.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (desiredSpeed > maxSpeed)
        {
            desiredSpeed = maxSpeed;
        } else if (desiredSpeed < minSpeed)
        {
            desiredSpeed = minSpeed;
        }

        if (desiredSpeed != speed)
        {
            if (desiredSpeed < speed)
            {
                speed -= accelRate * (double) Time.deltaTime;
                if (speed <= 0)
                {
                    wake1.Stop();
                    wake2.Stop();
                }
            }
            else
            {
                if (speed == 0)
                {
                    wake1.Play();
                    wake2.Play();
                }
                speed += accelRate * (double) Time.deltaTime;
            }
        }

        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        } else if (speed < minSpeed)
        {
            speed = minSpeed;
        }

        transform.position += transform.right * (float) speed * Time.deltaTime;

        if (lockAtHeight)
        {
            Vector3 pos = transform.position;
            pos.y = height;
            transform.position = pos;
        }
    }
}
