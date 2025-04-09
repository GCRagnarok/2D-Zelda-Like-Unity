using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Editor variables
    public Animator animator;
    private PlayerCollisions playerCollisions;
    private PlayerStats playerStats;

    //Movement variables
    private float moveDirectionX;
    private float moveDirectionY;
    private float lastMoveDirectionX = 0;
    private float lastMoveDirectionY = -1;

    public bool canMove;

    private string firstInputAxis;

    //Attack variables
    private float attackTime;
    private float nextAttackTime = 0;

    private bool isSpinAttacking;

    private Coroutine spinAttackCoroutine;

    void Start()
    {
        // Initialize Components
        playerCollisions = GetComponent<PlayerCollisions>();
        playerStats = GetComponent<PlayerStats>();

        // Initialize movement variables
        canMove = true;
        attackTime = playerStats.GetStartAttackTime();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        AttackCooldown();
    }

    //Movement Functions --------------------------------------------------------------------------------------------
    private void HandleMovementInput()
    {
        // Get the input from the player
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Check if the input is exclusivley horizontal then move & animate accordingly
        if (horizontalInput != 0 && verticalInput == 0)
        {
            moveDirectionX = horizontalInput > 0 ? 1 : -1;
            moveDirectionY = 0;
            firstInputAxis = "Horizontal";

            lastMoveDirectionX = moveDirectionX;
            lastMoveDirectionY = moveDirectionY;

            animator.SetBool("Horizontal", true);
            animator.SetBool("Vertical", false);
            animator.SetFloat("Direction", horizontalInput);
        }
        // Check if the input is exclusivley vertical then move & animate accordingly
        else if (verticalInput != 0 && horizontalInput == 0)
        {
            firstInputAxis = "Vertical";
            moveDirectionX = 0;
            moveDirectionY = verticalInput > 0 ? 1 : -1;

            lastMoveDirectionX = moveDirectionX;
            lastMoveDirectionY = moveDirectionY;

            animator.SetBool("Vertical", true);
            animator.SetBool("Horizontal", false);
            animator.SetFloat("Direction", verticalInput);
        }
        // Check if the input is both horizontal and vertical
        else if (horizontalInput != 0 && verticalInput != 0)
        {
            // Check which input was pressed first then move & animate the latter input accordingly
            if (firstInputAxis == "Horizontal")
            {
                moveDirectionX = 0;
                moveDirectionY = verticalInput > 0 ? 1 : -1;

                lastMoveDirectionY = moveDirectionY;

                animator.SetBool("Vertical", true);
                animator.SetBool("Horizontal", false);
                animator.SetFloat("Direction", verticalInput);
            }
            else if (firstInputAxis == "Vertical")
            {
                moveDirectionX = horizontalInput > 0 ? 1 : -1;
                moveDirectionY = 0;

                lastMoveDirectionX = moveDirectionX;

                animator.SetBool("Horizontal", true);
                animator.SetBool("Vertical", false);
                animator.SetFloat("Direction", horizontalInput);
            }
        }
        // Check if no input is detected then stop moving & animate accordingly
        else if (horizontalInput == 0 && verticalInput == 0)
        {
            moveDirectionX = 0;
            moveDirectionY = 0;
            firstInputAxis = "";

            animator.SetBool("Horizontal", false);
            animator.SetBool("Vertical", false);
            animator.SetFloat("Direction", 0);
        }

        // Move the player if no collision is detected
        Vector2 moveDirection = new Vector2(moveDirectionX, moveDirectionY);
        Vector2 newPosition = (Vector2)transform.position + moveDirection * playerStats.GetMoveSpeed() * Time.deltaTime;

        if (canMove && !playerCollisions.IsColliding(newPosition))
        {
            transform.position = newPosition;
        }
    }

    //Movement getters and setters
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

    public bool GetCanMove()
    {
        return canMove;
    }

    private void SetCanMoveAnimationEvent(int i)
    {
        if (i == 0)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }
    }

    //Attack Functions ----------------------------------------------------------------------------------------------
    private void AttackCooldown()
    {
        if (Time.time > nextAttackTime)
        {
            if (attackTime <= 0)
            {
                attackTime = playerStats.GetStartAttackTime();
            }
            else
            {
                attackTime -= Time.deltaTime;
                Attack();
            }
        }
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Attack"))
        {
            // Stop the existing coroutine if it's running
            if (spinAttackCoroutine != null)
            {
                StopCoroutine(spinAttackCoroutine);
                spinAttackCoroutine = null;
            }

            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + playerStats.GetAttackCooldownTime();

            playerCollisions.CheckSwordCollisions();

            // Start the coroutine to check for spin attack and set the reference
            spinAttackCoroutine = StartCoroutine(CheckForSpinAttack());
        }
    }

    private IEnumerator CheckForSpinAttack()
    {
        yield return new WaitForSeconds(0.8f);

        // Check if the attack button is still being held
        if (Input.GetButton("Attack"))
        {
            SpinAttack();
        }

        spinAttackCoroutine = null;
    }

    private void SpinAttack()
    {
        animator.SetTrigger("SpinAttack");
    }

    //Attack getters and setters
    public bool GetIsSpinAttacking()
    {
        return isSpinAttacking;
    }

    private void SetIsSpinAttackingAnimationEvent(int i)
    {
        if (i == 0)
        {
            isSpinAttacking = false;
        }
        else
        {
            isSpinAttacking = true;
        }
    }
    /*
    // Gizmos Functions ---------------------------------------------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (playerCollisions.attackPointUp != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerCollisions.attackPointUp.position, playerStats.GetAttackRadius());
        }

        if (playerCollisions.attackPointDown != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerCollisions.attackPointDown.position, playerStats.GetAttackRadius());
        }

        if (playerCollisions.attackPointLeft != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerCollisions.attackPointLeft.position, playerStats.GetAttackRadius());
        }

        if (playerCollisions.attackPointRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerCollisions.attackPointRight.position, playerStats.GetAttackRadius());
        }

        if (playerCollisions.attackPointCenter != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(playerCollisions.attackPointCenter.position, playerStats.GetSpinAttackRadius());
        }
    }
    */

}
