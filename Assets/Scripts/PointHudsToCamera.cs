using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHudsToCamera : MonoBehaviour
{
    [SerializeField] private Transform cam;

    private GameObject[] huds;

    private void Start()
    {
        huds = GameObject.FindGameObjectsWithTag("HUD");
    }

    private void LateUpdate()
    {
        PointToCam();
    }

    // a function that is used to point the camera display to the camera so the player can see it better
    private void PointToCam()
    {
        foreach (GameObject hud in huds)
        {
            hud.transform.LookAt(hud.transform.position + cam.forward);
        }
    }
}
