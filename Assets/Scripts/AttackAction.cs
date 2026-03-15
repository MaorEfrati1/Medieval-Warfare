using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject attackTarget;

    private Animator animComp;
    private GameObject selectedUnit;
    private float distanceFromPlayer;

    private string enemy;
    private GameObject[] enemyUnits;

    private void Update()
    {
        if (selectedUnit != null)
        {
            OnAttack();
        }
    }

    private void OnEnable()
    {
        selectedUnit = GetComponent<UnitSelectionHandler>().selectedUnit;
        if (selectedUnit.CompareTag("AttackingUnit")) enemy = "DefendingUnit";
        else if (selectedUnit.CompareTag("DefendingUnit")) enemy = "AttackingUnit";
        animComp = selectedUnit.GetComponent<Animator>();
        GetComponent<MoveAction>().originalPosition = selectedUnit.transform.position;
        GetComponent<MoveAction>().originalRotation = selectedUnit.transform.rotation;
        selectedUnit.GetComponent<PlayerStats>().currentMana -= 10f;
        GetEnemyUnits();
        AttackTarget();
    }

    // a function that is used to get all the enemies in the scene
    private void GetEnemyUnits()
    {
        enemyUnits = GameObject.FindGameObjectsWithTag(enemy);
    }

    // a function that is used to better visualize what enemies the player can attack
    private void AttackTarget()
    {
        foreach (GameObject enemy in enemyUnits)
        {
            distanceFromPlayer = Vector3.Distance(enemy.transform.position, selectedUnit.transform.position);
            if (distanceFromPlayer < selectedUnit.GetComponent<PlayerController>().attackRange)
            {
                GameObject attackTargetClone = Instantiate(attackTarget);
                attackTargetClone.transform.position = new Vector3(enemy.transform.position.x, 0.01f, enemy.transform.position.z);
            }
        }
    }

    // the function that is used to trigger the player attack
    private void OnAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity);

            float attackDistance = Vector3.Distance(hit.point, selectedUnit.transform.position);

            if (hit.collider.CompareTag(enemy) && attackDistance <= selectedUnit.GetComponent<PlayerController>().attackRange + 0.6f &&
                GetComponent<UnitSelectionHandler>().selectedUnit.GetComponent<PlayerStats>().currentMana + 10f >= 10f)
            {
                foreach (GameObject attackTargetClone in GameObject.FindGameObjectsWithTag("AttackTarget"))
                {
                    Destroy(attackTargetClone);
                }
                selectedUnit.transform.LookAt(new Vector3(hit.point.x, 0f, hit.point.z));
                animComp.SetTrigger("isAttacking");
                DamageEnemy(hit.collider.gameObject);
                GetComponent<UnitSelectionHandler>().selectedUnit = null;
                this.enabled = false;
            }
        }
    }

    // the function that is responsible for the enemy's actions when it is hit
    private void DamageEnemy(GameObject enemy)
    {
        enemy.GetComponent<PlayerStats>().TakeDamage(selectedUnit.GetComponent<PlayerController>().attackDamage);

        if (enemy.GetComponent<PlayerStats>().currentHealth > 0 && !selectedUnit.GetComponent<PlayerController>().isCatapult)
        {
            enemy.GetComponent<Animator>().SetTrigger("isHit");
        }
        else if (enemy.GetComponent<PlayerStats>().currentHealth <= 0)
        {
            enemy.GetComponent<Animator>().SetBool("isDead", true);
            StartCoroutine(EnemyDeath(enemy));
        }
    }

    // a function that is responsible for the death of the enemy
    IEnumerator EnemyDeath(GameObject enemyUnit)
    {
        if (enemyUnit == GetComponent<EnemyAI>().selectedEnemyUnit)
        {
            GetComponent<EnemyAI>().selectedEnemyUnit = null;
            GetComponent<EnemyAI>().selectedPlayerUnit = null;
        }
        enemyUnit.GetComponent<PlayerStats>().healthBar.value = 0f;
        enemyUnit.GetComponent<PlayerStats>().enabled = false;
        enemyUnit.GetComponent<PlayerController>().isSelectable = false;

        yield return new WaitForSeconds(3f);

        enemyUnit.SetActive(false);
        
        // reducing the number count display
        // if the number is reuced to zero, declare the win of the correct team
        if (enemy == "DefendingUnit")
        {
            int oldCount = GetComponent<UnitCount>().GetDefendingUnitCount();
            string newCount = (oldCount--).ToString();
            GetComponent<UnitCount>().SetDefendingUnitCount(newCount);
            if (newCount == "0")
            {
                StartCoroutine(GetComponent<DeclareWin>().OnTeamWin("ATTACKING TEAM"));
            }
        }
        else
        {
            int oldCount = GetComponent<UnitCount>().GetAttackingUnitCount();
            string newCount = (oldCount--).ToString();
            GetComponent<UnitCount>().SetAttackingUnitCount(newCount);
            if (newCount == "0")
            {
                StartCoroutine(GetComponent<DeclareWin>().OnTeamWin("DEFENDING TEAM"));
            }
        }
    }
}
