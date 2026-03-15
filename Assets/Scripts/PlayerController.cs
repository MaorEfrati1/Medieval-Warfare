using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed = 0.25f;
    public float moveRange;
    public float attackRange;
    public float attackDamage;
    public bool isCatapult;
    
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isSelectable = true;
    [HideInInspector] public Animator animComp;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject moveTarget;

    private Vector3 targetPos;
    private float distanceFromTarget;
    private float stoppingDistance = 0.1f;

    private void Start()
    {
        animComp = GetComponent<Animator>();
        targetPos = transform.position;
    }

    // the function that is responsible for the movement of the player unit
    public void Move(Vector3 destination)
    {
        // getting the direction to which the unit should face
        Vector3 moveDirection = (targetPos - transform.position).normalized;
        // getting the distance between the player and its destination
        distanceFromTarget = Vector3.Distance(destination, transform.position);

        // getting the new position information for the unit movement
        if (distanceFromTarget > stoppingDistance)
        {
            isRunning = true;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed);
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            animComp.SetBool("isRunning", true);
        }
        // when the unit reaches its destination
        else
        {
            isRunning = false;
            animComp.SetBool("isRunning", false);
            moveTarget.SetActive(false);
            gameManager.GetComponent<MoveAction>().isMoving = false;
            gameManager.GetComponent<MoveAction>().enabled = false;
            gameManager.GetComponent<ActivateActions>().enabled = false;
            gameManager.GetComponent<ActivateActions>().enabled = true;
        }

        // actually applying the new position to the player unit
        targetPos = destination;
    }
}
