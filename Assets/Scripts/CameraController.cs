using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 focusPos;
    private Vector2 mouseStartPos;
    private Vector2 cameraOrbitAngles = new Vector2(180, 90);
    private float orbitAngleChangeRate = 145;
    private Vector3 posInSphere;
    private float distanceFromFocus = 5f;
    public float scrollSpeed = 10f;
    public float minHeight = 1f;
    public float cameraMoveSpeed = 5f;
    private Vector3 cameraStartEulerAngles;
    private Vector3 desiredCameraAngles;

    private void Awake()
    {

    }

    private void OnMouseDrag()
    {
        float diffx = Input.mousePosition.x - mouseStartPos.x;
        float diffy = Input.mousePosition.y - mouseStartPos.y;

        // Calculate xDiff and yDiff as a percentage of window size
        Vector2 movePercent;
        movePercent.x = diffx / Screen.width;
        movePercent.y = diffy / Screen.height;

        desiredCameraAngles = cameraStartEulerAngles;
        desiredCameraAngles.y += movePercent.x * orbitAngleChangeRate;
        desiredCameraAngles.x -= movePercent.y * orbitAngleChangeRate;
        desiredCameraAngles.x = Mathf.Clamp(desiredCameraAngles.x, -90, 90);

        transform.eulerAngles = desiredCameraAngles;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            distanceFromFocus = (transform.position - focusPos).magnitude;
            mouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            cameraStartEulerAngles = transform.eulerAngles;
        }

        if (Input.GetMouseButton(1))
        {
            OnMouseDrag();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            transform.position += transform.forward * -scrollSpeed * Time.deltaTime;

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            transform.position += transform.forward * scrollSpeed * Time.deltaTime;

        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * cameraMoveSpeed * Time.deltaTime ;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * cameraMoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * cameraMoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * cameraMoveSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            cameraMoveSpeed = 2 * cameraMoveSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            cameraMoveSpeed = cameraMoveSpeed / 2;
        }
/*
        Vector3 diff = (desiredCameraAngles - transform.eulerAngles);
        transform.eulerAngles += (diff) * Time.deltaTime * 15;*/

        if (transform.position.y < minHeight)
        {
            Vector3 pos = transform.position;
            pos.y = minHeight;
            transform.position = pos;
        }

    }
}
