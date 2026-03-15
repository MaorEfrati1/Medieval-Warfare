using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
    [HideInInspector] public bool isMoving;
    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Quaternion originalRotation;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject moveTarget;

    private GameObject selectedUnit;
    private float distanceFromPlayer;
    private Vector3 destination;

    private void OnEnable()
    {
        selectedUnit = GetComponent<UnitSelectionHandler>().selectedUnit;
        originalPosition = selectedUnit.transform.position;
        originalRotation = selectedUnit.transform.rotation;
        selectedUnit.GetComponent<PlayerStats>().currentMana -= 10f;
    }

    private void Update()
    {
        selectedUnit = GetComponent<UnitSelectionHandler>().selectedUnit;
        if (selectedUnit != null) {
            OnMove(); 
        }
        if (isMoving)
        {
            selectedUnit.GetComponent<PlayerController>().Move(destination);
        }
    }

    // that is the function that is used to trigger the unit movement
    private void OnMove()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity);

        // getting the destination information to apply in the movement later
        if (hit.collider.CompareTag("Ground"))
        {
            distanceFromPlayer = Vector3.Distance(hit.point, selectedUnit.transform.position);
            moveTarget.SetActive(true);
            MoveTarget(hit.point);
            if (Input.GetMouseButtonDown(0) && distanceFromPlayer < selectedUnit.GetComponent<PlayerController>().moveRange &&
                GetComponent<UnitSelectionHandler>().selectedUnit.GetComponent<PlayerStats>().currentMana + 10f >= 10f)
            {
                // making sure the destination can't be changed mid-movement
                if (!isMoving)
                {
                    originalPosition = selectedUnit.transform.position;
                    originalRotation = selectedUnit.transform.rotation;
                    isMoving = true;
                    destination = hit.point;
                }
            }
        }
    }

    // this function is used to better visualize to where the player is able to move
    private void MoveTarget(Vector3 mousePos)
    {
        if (distanceFromPlayer < selectedUnit.GetComponent<PlayerController>().moveRange)
        {
            if (!isMoving)
            {
                moveTarget.transform.position = new Vector3(mousePos.x, moveTarget.transform.position.y, mousePos.z);
            }
            else
            {
                moveTarget.transform.position = new Vector3(destination.x, moveTarget.transform.position.y, destination.z);
            }
        }
    }
}
