using Pathfinding;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Zombie1Controller : MonoBehaviour
{
    //Component variables
    public Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private BoxCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    private AIPath aiPath;
    private AIDestinationSetter aiDestinationSetter;
    private EnemyStats enemyStats;
    private EnemyAttackCollisions enemyAttackCollisions;
    private PlayerController playerController;

    //Target variables
    private Transform currentTarget;
    private GameObject waypoint1;
    private GameObject waypoint2;
    public Vector2 waypoint1Offset;
    public Vector2 waypoint2Offset;

    //StateMachine variables
    public enum StateMachine
    {
        Patrol,
        Chase,
        Attack,
        Dead
    }

    public StateMachine currentState;

    private bool isDead;
    private bool playerSeen;
    private bool isAttacking;
    public bool isHurt;
    private bool canBeHurt;

    //Attack variables
    private float attackTime;
    private float nextAttackTime;
    private bool isAttackOnCooldown;
    private float currentAttackCooldownTime;

    //Ai Movement variables
    private Vector2 previousPosition;
    private int lastMoveDirectionX;
    private int lastMoveDirectionY;

    //Animation variables
    private float attackAnimationTime = 0.8f;
    private float dieAnimationTime = 1f;
    private float hurtAnimationTime = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        //Own Components
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        enemyStats = GetComponent<EnemyStats>();
        enemyAttackCollisions = GetComponent<EnemyAttackCollisions>();

        //Player Components
        playerController = FindObjectOfType<PlayerController>();
        playerCollider = playerController.GetComponent<BoxCollider2D>();

        //Waypoint Components
        waypoint1 = new GameObject(this.name + " Waypoint1");
        waypoint1.transform.position = new Vector2(transform.position.x + waypoint1Offset.x, transform.position.y + waypoint1Offset.y);

        waypoint2 = new GameObject(this.name + " Waypoint2");
        waypoint2.transform.position = new Vector2(transform.position.x + waypoint2Offset.x, transform.position.y + waypoint2Offset.y);

        //StateMachine variables
        currentState = StateMachine.Patrol;
        aiDestinationSetter.target = waypoint1.transform;
        currentTarget = aiDestinationSetter.target;
        aiPath.maxSpeed = enemyStats.patrolSpeed;
        canBeHurt = true;
    }

    // Update is called once per frame
    void Update()
    {
        SetPreviousPosition();
        SetDirection();
        checkPlayerSeen();
        SetCurrentState();
    }

    //State Machine -------------------------------------------------------------------------------------------------
    private void SetCurrentState()
    {
        switch (currentState)
        {
            case StateMachine.Patrol:
                Patrol();
                break;
            case StateMachine.Chase:
                Chase();
                break;
            case StateMachine.Attack:
                Attack();
                break;
            case StateMachine.Dead:
                Dead();
                break;
        }

        if (isDead && currentState != StateMachine.Dead)
        {
            currentState = StateMachine.Dead;
        }
        else if (!playerSeen && currentState != StateMachine.Patrol)
        {
            aiPath.maxSpeed = enemyStats.patrolSpeed;
            aiPath.endReachedDistance = 0f;

            SetCurrentTarget(waypoint1.transform);

            currentState = StateMachine.Patrol;
        }
        else if (playerSeen && !aiPath.reachedDestination && !isDead && currentState != StateMachine.Chase)
        {
            aiPath.maxSpeed = enemyStats.chaseSpeed;

            float enemyColliderRadius = Mathf.Max(boxCollider.bounds.size.x / 2, boxCollider.bounds.size.y / 2);
            float playerColliderRadius = Mathf.Max(playerCollider.bounds.size.x / 2, playerCollider.bounds.size.y / 2);
            aiPath.endReachedDistance = enemyColliderRadius + playerColliderRadius + enemyStats.endDistanceOffset;

            currentState = StateMachine.Chase;
        }
        else if (playerSeen && aiPath.reachedDestination && !isDead && currentState != StateMachine.Attack)
        {
            currentState = StateMachine.Attack;
        }
    }

    private void Patrol()
    {
        //print("Patrol State Initiated");

        if (transform.position == waypoint1.transform.position)
        {
            SetCurrentTarget(waypoint2.transform);
        }
        else if (transform.position == waypoint2.transform.position)
        {
            SetCurrentTarget(waypoint1.transform);
        }
    }

    private void Chase()
    {
        //print("Chase State Initiated");

        if (currentTarget != playerController.transform)
        {
            SetCurrentTarget(playerController.transform);
        }
        else if (!playerSeen)
        {
            SetCurrentTarget(waypoint1.transform);
        }
    }

    private void Attack()
    {
        FacePlayer();

        //print("Attack State Initiated");
        if (!isAttackOnCooldown && !isHurt)
        {
            canBeHurt = false;
            isAttacking = true;
            aiPath.canMove = false;
            animator.SetTrigger("Attack");
            animator.SetBool("Attacking", true);
            currentAttackCooldownTime = Random.Range(attackAnimationTime + enemyStats.attackCooldownMinTime, attackAnimationTime + enemyStats.attackCooldownMaxTime);
            nextAttackTime = Time.time + currentAttackCooldownTime;
            isAttackOnCooldown = true;

            StartCoroutine(ResetAttackBooleans());
        }

        // Check if the cooldown has expired
        if (isAttackOnCooldown && Time.time >= nextAttackTime)
        {
            isAttackOnCooldown = false;
        }
    }

    private IEnumerator ResetAttackBooleans()
    {
        yield return new WaitForSeconds(attackAnimationTime);

        canBeHurt = true;
        isAttacking = false;
        aiPath.canMove = true;
        animator.SetBool("Attacking", false);
    }

    private void Dead()
    {
        print("Dead State Initiated");
        aiPath.canMove = false;
        currentTarget = null;
        animator.SetBool("Dead", true);
        boxCollider.enabled = false;
        Destroy(gameObject, dieAnimationTime);
    }

    //Pathfinding Custom Functions ----------------------------------------------------------------------------------

    private void SetPreviousPosition()
    {
        previousPosition = transform.position;
    }

    private void SetCurrentTarget(Transform target)
    {
        aiDestinationSetter.target = target;
        currentTarget = aiDestinationSetter.target;

        //print("Current Target: " + currentTarget.name);
    }

    private void checkPlayerSeen()
    {
        if (!playerController.isDead)
        {
            playerSeen = Vector2.Distance(transform.position, playerController.transform.position) < enemyStats.searchRadius;
        }
        else
        {
            playerSeen = false;
        }
    }

    //Animation Functions -------------------------------------------------------------------------------------------
    private void SetDirection()
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = aiPath.desiredVelocity;
        float directionThreshold = 0.5f;

        if (Mathf.Abs(direction.x) > directionThreshold && Mathf.Abs(direction.y) <= directionThreshold)
        {
            //print("moving horizontal");
            animator.SetBool("Idle", false);
            animator.SetBool("Horizontal", true);
            animator.SetBool("Vertical", false);
            animator.SetFloat("Direction", direction.x > 0 ? 1 : -1);
            lastMoveDirectionX = direction.x > 0 ? 1 : -1;
            lastMoveDirectionY = 0;
        }
        else if (Mathf.Abs(direction.y) > directionThreshold && Mathf.Abs(direction.x) <= directionThreshold)
        {
            //print("moving vertical");
            animator.SetBool("Idle", false);
            animator.SetBool("Horizontal", false);
            animator.SetBool("Vertical", true);
            animator.SetFloat("Direction", direction.y > 0 ? 1 : -1);
            lastMoveDirectionX = 0;
            lastMoveDirectionY = direction.y > 0 ? 1 : -1;
        }
        else if (currentPosition == previousPosition)
        {
            animator.SetBool("Idle", true);
        }
    }

    public string GetDirection()
    {
        string direction = "";

        switch (lastMoveDirectionX, lastMoveDirectionY)
        {
            case (1, 0):
                direction = "Right";
                break;
            case (0, 1):
                direction = "Up";
                break;
            case (0, -1):
                direction = "Down";
                break;
            case (-1, 0):
                direction = "Left";
                break;
        }
        return direction;
    }

    private void FacePlayer()
    {
        // Calculate the direction to the player
        Vector2 directionToPlayer = (playerController.transform.position - transform.position).normalized;

        // Determine the primary direction (horizontal or vertical)
        if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
        {
            // Horizontal direction
            animator.SetBool("Horizontal", true);
            animator.SetBool("Vertical", false);
            animator.SetFloat("Direction", directionToPlayer.x > 0 ? 1 : -1);
            lastMoveDirectionX = directionToPlayer.x > 0 ? 1 : -1;
            lastMoveDirectionY = 0;
        }
        else
        {
            // Vertical direction
            animator.SetBool("Horizontal", false);
            animator.SetBool("Vertical", true);
            animator.SetFloat("Direction", directionToPlayer.y > 0 ? 1 : -1);
            lastMoveDirectionX = 0;
            lastMoveDirectionY = directionToPlayer.y > 0 ? 1 : -1;
        }
    }

    public void Hurt()
    {
        isHurt = true;
        aiPath.canMove = false;
        animator.SetTrigger("Hurt");

        StartCoroutine(ResetHurtBooleans());
    }

    public IEnumerator ResetHurtBooleans()
    {
        yield return new WaitForSeconds(hurtAnimationTime);

        isHurt = false;
        aiPath.canMove = true;
    }

    public void Die()
    {
        isDead = true;
    }

    public void SetSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }
}
