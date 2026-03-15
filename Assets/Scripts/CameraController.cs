using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float borderThickness;
    public float zoomSpeed;
    public Vector2 moveLimit;
    public float minZoomLimit, maxZoomLimit;
    public bool isReversed;


    // Update is called once per frame
    void Update()
    {
        CameraMovement();
    }

    // the logic behind the camera movement
    void CameraMovement()
    {
        // getting the position of the camera
        Vector3 pos = transform.position;

        // moving the camera according to mouse position and keyboard keys
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - borderThickness)
        {
            if (isReversed) pos.z -= moveSpeed * Time.deltaTime;
            else pos.z += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= borderThickness)
        {
            if (isReversed) pos.z += moveSpeed * Time.deltaTime;
            else pos.z -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - borderThickness)
        {
            if (isReversed) pos.x -= moveSpeed * Time.deltaTime;
            else pos.x += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= borderThickness)
        {
            if (isReversed) pos.x += moveSpeed * Time.deltaTime;
            else pos.x -= moveSpeed * Time.deltaTime;
        }

        // zooming the camera in and out using the mouse scroll wheel
        Vector3 zoom = transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (pos.y == minZoomLimit)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                pos += zoom;
            }
        }
        else if (pos.y == maxZoomLimit)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                pos += zoom;
            }
        }
        else pos += zoom;

        // making sure the camera movement and zoom stay within limits
        pos.x = Mathf.Clamp(pos.x, -moveLimit.x, moveLimit.x);
        pos.y = Mathf.Clamp(pos.y, minZoomLimit, maxZoomLimit);
        pos.z = Mathf.Clamp(pos.z, -moveLimit.y, moveLimit.y);

        // actually applying the new position to the camera
        transform.position = pos;
    }
}
