using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject backBtn;

    public GameObject selectedUnit;
    public Light slLight;

    private SideManager sideManager;
    private string unit;

    private void Start()
    {
        sideManager = FindObjectOfType<SideManager>();
        if (sideManager.isAttacking) unit = "AttackingUnit";
        else if (sideManager.isDefending) unit = "DefendingUnit";
    }

    private void Update()
    {
        if (!GetComponent<MoveAction>().isMoving)
        {
            SelectionHandler();
        }

        if (selectedUnit != null)
        {
            MarkSelection();
        }
        else
        {
            slLight.enabled = false;
        }
    }

    // one of the main function of the game
    // this function is responsible for determining the selected unit to control
    private void SelectionHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity);

            if (hit.collider.CompareTag(unit) && hit.collider.GetComponent<PlayerController>().isSelectable)
            {
                selectedUnit = hit.collider.gameObject;
                GetComponent<ActivateActions>().enabled = false;
                GetComponent<ActivateActions>().enabled = true;
                foreach (GameObject attackTargetClone in GameObject.FindGameObjectsWithTag("AttackTarget"))
                {
                    Destroy(attackTargetClone);
                }
            }
        }
    }

    // if a unit is selected, this function is used to mark its position to better visualize it
    private void MarkSelection()
    {
        slLight.enabled = true;
        slLight.transform.position = new Vector3(selectedUnit.transform.position.x, slLight.transform.position.y, selectedUnit.transform.position.z);
    }
}
