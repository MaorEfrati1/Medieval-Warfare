using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public GameObject selectedEnemyUnit;
    public GameObject selectedPlayerUnit;

    [SerializeField] private float cooldownTimer;
    private float minResetCooldownTimer = 2f;
    private float maxResetCooldownTimer = 8f;

    private SideManager sideManager;
    private string enemyTeam;
    private string playerTeam;
    private Vector3 originalPos = Vector3.zero;
    private bool isAttacking;

    private GameObject[] enemyTeamUnits;
    private GameObject[] unitsToAttack;

    private void Awake()
    {
        sideManager = FindObjectOfType<SideManager>();
        if (sideManager.isAttacking)
        {
            enemyTeam = "DefendingUnit";
            playerTeam = "AttackingUnit";
        }
        else if (sideManager.isDefending)
        {
            enemyTeam = "AttackingUnit";
            playerTeam = "DefendingUnit";
        }

        GetEnemyTeamUnits();
        GetUnitsToAttack();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetCooldownTimer();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (selectedEnemyUnit == null)
        {
            selectedEnemyUnit = GetRandomEnemyUnit();

            if (selectedEnemyUnit.GetComponent<PlayerStats>().currentMana < 10f)
            {
                selectedEnemyUnit = null;
            }
        }

        if (selectedPlayerUnit == null)
        {
            selectedPlayerUnit = GetRandomPlayerUnit();
            
            if (selectedPlayerUnit.GetComponent<PlayerController>().isCatapult)
            {
                selectedPlayerUnit = null;
            }
            else
            {
                selectedPlayerUnit.GetComponent<PlayerController>().isSelectable = false;
            }
        }

        if (selectedEnemyUnit != null && selectedPlayerUnit != null &&
            !selectedEnemyUnit.GetComponent<PlayerController>().isCatapult && cooldownTimer <= 0 && !isAttacking)
        {
            MoveToPlayerUnit(selectedEnemyUnit, selectedPlayerUnit);
        }

        else if (selectedEnemyUnit.GetComponent<PlayerController>().isCatapult && cooldownTimer <= 0)
        {
            AttackPlayerUnit(selectedEnemyUnit);
        }
    }

    private void ResetCooldownTimer()
    {
        cooldownTimer = Random.Range(minResetCooldownTimer, maxResetCooldownTimer);
    }

    public void GetEnemyTeamUnits()
    {
        enemyTeamUnits = GameObject.FindGameObjectsWithTag(enemyTeam);
    }

    public void GetUnitsToAttack()
    {
        unitsToAttack = GameObject.FindGameObjectsWithTag(playerTeam);
    }

    private GameObject GetRandomEnemyUnit()
    {
        int indx = Random.Range(0, enemyTeamUnits.Length);
        GameObject randomUnit = enemyTeamUnits[indx];

        return randomUnit;
    }

    private GameObject GetRandomPlayerUnit()
    {
        int indx = Random.Range(0, unitsToAttack.Length);
        GameObject randomUnit = unitsToAttack[indx];

        return randomUnit;
    }

    private void MoveToPlayerUnit(GameObject enemyUnit, GameObject playerUnit)
    {
        // getting the direction to which the unit should face
        Vector3 moveDirection = (playerUnit.transform.position - enemyUnit.transform.position).normalized;

        if (originalPos == Vector3.zero)
        {
            originalPos = enemyUnit.transform.position;
        }

        float distanceMoved = Vector3.Distance(enemyUnit.transform.position, originalPos);
        float distanceFromPlayer = Vector3.Distance(enemyUnit.transform.position, playerUnit.transform.position);

        // getting the new position information for the unit movement
        if (distanceMoved < enemyUnit.GetComponent<PlayerController>().moveRange &&
            distanceFromPlayer > 1f)
        {
            enemyUnit.GetComponent<PlayerController>().isRunning = true;
            enemyUnit.transform.forward = Vector3.Lerp(enemyUnit.transform.forward, moveDirection, enemyUnit.GetComponent<PlayerController>().rotateSpeed);
            enemyUnit.transform.position += moveDirection * enemyUnit.GetComponent<PlayerController>().moveSpeed * Time.deltaTime;
            enemyUnit.GetComponent<PlayerController>().animComp.SetBool("isRunning", true);
        }
        // when the unit reaches its destination
        else
        {
            enemyUnit.GetComponent<PlayerController>().isRunning = false;
            enemyUnit.GetComponent<PlayerController>().animComp.SetBool("isRunning", false);
            enemyUnit.GetComponent<PlayerStats>().currentMana -= 10f;
            originalPos = Vector3.zero;
            isAttacking = true;
            AttackPlayerUnit(enemyUnit);
        }
    }

    private void AttackPlayerUnit(GameObject enemyUnit)
    {
        GetAttackTarget(enemyUnit);
        
        if (selectedPlayerUnit != null)
        {
            enemyUnit.transform.LookAt(new Vector3(selectedPlayerUnit.transform.position.x, 0f, selectedPlayerUnit.transform.position.z));
            enemyUnit.GetComponent<Animator>().SetTrigger("isAttacking");
            DamageEnemy(selectedPlayerUnit);
            enemyUnit.GetComponent<PlayerStats>().currentMana -= 10f;
            selectedPlayerUnit.GetComponent<PlayerController>().isSelectable = true;
            selectedEnemyUnit = null;
            selectedPlayerUnit = null;
            isAttacking = false;
            ResetCooldownTimer();
        }

        else
        {
            selectedEnemyUnit = null;
            isAttacking = false;
            ResetCooldownTimer();
        }
    }

    private void GetAttackTarget(GameObject enemyUnit)
    {
        List<GameObject> unitsInRange = new List<GameObject>();

        foreach (GameObject playerUnit in unitsToAttack)
        {
            float distanceFromUnit = Vector3.Distance(playerUnit.transform.position, enemyUnit.transform.position);
            if (distanceFromUnit < enemyUnit.GetComponent<PlayerController>().attackRange)
            {
                unitsInRange.Add(playerUnit);
            }
        }

        if (unitsInRange.Count > 0)
        {
            selectedPlayerUnit = unitsInRange[Random.Range(0, unitsInRange.Count)];
        }
        else
        {
            selectedPlayerUnit.GetComponent<PlayerController>().isSelectable = true;
            selectedPlayerUnit = null;
        }
    }

    private void DamageEnemy(GameObject enemy)
    {
        enemy.GetComponent<PlayerStats>().TakeDamage(selectedEnemyUnit.GetComponent<PlayerController>().attackDamage);

        if (enemy.GetComponent<PlayerStats>().currentHealth > 0 && !selectedEnemyUnit.GetComponent<PlayerController>().isCatapult)
        {
            enemy.GetComponent<Animator>().SetTrigger("isHit");
        }
        else if (enemy.GetComponent<PlayerStats>().currentHealth <= 0)
        {
            enemy.GetComponent<Animator>().SetBool("isDead", true);
            StartCoroutine(EnemyDeath(enemy));
        }
    }

    IEnumerator EnemyDeath(GameObject enemyUnit)
    {
        GetUnitsToAttack();
        enemyUnit.GetComponent<PlayerStats>().healthBar.value = 0f;
        enemyUnit.GetComponent<PlayerStats>().enabled = false;
        enemyUnit.GetComponent<PlayerController>().isSelectable = false;

        yield return new WaitForSeconds(3f);

        enemyUnit.SetActive(false);

        if (playerTeam == "DefendingUnit")
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
