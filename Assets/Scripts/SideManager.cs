using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is used to tell the game scene which sie you chose to play with
public class SideManager : MonoBehaviour
{
    public bool isAttacking;
    public bool isDefending;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnChooseAttack()
    {
        isDefending = false ;
        isAttacking = true;
    }

    public void OnChooseDefend()
    {
        isAttacking = false;
        isDefending = true;
    }
}
