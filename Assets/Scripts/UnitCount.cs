using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// a script that has various functions to query how many attacking and defending units are left in the game
public class UnitCount : MonoBehaviour
{
    [SerializeField] private Text attackingCount;
    [SerializeField] private Text defendingCount;

    // Start is called before the first frame update
    void Start()
    {
        GetUnitCount();
    }

    private void GetUnitCount()
    {
        GameObject[] attackingUnits = GameObject.FindGameObjectsWithTag("AttackingUnit");
        GameObject[] defendingUnits = GameObject.FindGameObjectsWithTag("DefendingUnit");

        attackingCount.text = (attackingUnits.Length).ToString();
        defendingCount.text = (defendingUnits.Length).ToString();
    }

    public int GetAttackingUnitCount()
    {
        int count = int.Parse(attackingCount.text) - 1;
        return count;
    }

    public int GetDefendingUnitCount()
    {
        int count = int.Parse(defendingCount.text) - 1;
        return count;
    }

    public void SetAttackingUnitCount(string newCount)
    {
        attackingCount.text = newCount;
    }

    public void SetDefendingUnitCount(string newCount)
    {
        defendingCount.text = newCount;
    }
}
