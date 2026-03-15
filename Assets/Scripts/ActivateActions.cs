using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateActions : MonoBehaviour
{
    [SerializeField] private GameObject moveBtn, attackBtn, backBtn;

    private GameObject selectedUnit;

    private void OnEnable()
    {
        
        backBtn.SetActive(false);
        selectedUnit = GetComponent<UnitSelectionHandler>().selectedUnit;
    }

    private void Update()
    {
        ShowButtons();
    }

    // a function that is used to determine which action buttons should be enabled
    private void ShowButtons()
    {
        if (GetComponent<UnitSelectionHandler>().selectedUnit != null && !backBtn.activeSelf)
        {
            if (!selectedUnit.GetComponent<PlayerController>().isCatapult) moveBtn.SetActive(true);
            attackBtn.SetActive(true);
        }

        else if (GetComponent<UnitSelectionHandler>().selectedUnit == null)
        {
            moveBtn.SetActive(false);
            attackBtn.SetActive(false);
            backBtn.SetActive(false);
        }

        else
        {
            moveBtn.SetActive(false);
            attackBtn.SetActive(false);
        }
    }

    // a function that enables the move action
    public void OnMoveAction()
    {
        GetComponent<MoveAction>().enabled = true;
        backBtn.SetActive(true);
    }

    // a function that enables the attack action
    public void OnAttackAction()
    {
        GetComponent<AttackAction>().enabled = true;
        backBtn.SetActive(true);
    }

    // a function that resets the logic and lets the player go back to the action selection
    public void OnBackAction()
    {
        selectedUnit.GetComponent<PlayerStats>().currentMana += 10f;
        selectedUnit.GetComponent<PlayerController>().Move(GetComponent<UnitSelectionHandler>().selectedUnit.transform.position);
        selectedUnit.transform.position = GetComponent<MoveAction>().originalPosition;
        selectedUnit.transform.rotation = GetComponent<MoveAction>().originalRotation;
        GetComponent<MoveAction>().enabled = false;
        GetComponent<AttackAction>().enabled = false;
        foreach (GameObject attackTargetClone in GameObject.FindGameObjectsWithTag("AttackTarget"))
        {
            Destroy(attackTargetClone);
        }
        backBtn.SetActive(false);
    }
}