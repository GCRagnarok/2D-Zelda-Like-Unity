using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    private float dashDurationTime;
    private float nextDashTime;

    private bool canMove;
    private bool isDashing;
    private bool isDashOnCooldown;

    private Vector2 dashDirection;

    private string firstInputAxis;

    //Attack variables
    private float attackTime;
    private float nextAttackTime;

    private bool isAttacking;
    private bool isSpinAttacking;
    private bool isAttackOnCooldown;

    private Coroutine spinAttackHeldCoroutine;
    private Coroutine spinAttackReleasedCoroutine;

    void Start()
    {
        // Initialize Components
        playerCollisions = GetComponent<PlayerCollisions>();
        playerStats = GetComponent<PlayerStats>();

        // Initialize movement variables
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        Dash();
        Attack();
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
        Vector2 moveDirection = isDashing ? dashDirection : new Vector2(moveDirectionX, moveDirectionY);
        float currentSpeed = isDashing ? playerStats.GetDashSpeed() : playerStats.GetMoveSpeed();
        Vector2 newPosition = (Vector2)transform.position + moveDirection * currentSpeed * Time.deltaTime;

        if (canMove && !playerCollisions.IsColliding(newPosition))
        {
            transform.position = newPosition;
        }
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && !isDashing && !isAttacking && !isDashOnCooldown)
        {
            isDashing = true;
            dashDurationTime = playerStats.GetDashDuration();

            // Get the input from the player
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            float desiredMoveDirectionX = lastMoveDirectionX;
            float desiredMoveDirectionY = lastMoveDirectionY;

            // Check if the input is both horizontal and vertical
            if (horizontalInput != 0 && verticalInput != 0)
            {
                // Check which input was pressed first then move & animate the latter input accordingly
                if (firstInputAxis == "Horizontal")
                {
                    desiredMoveDirectionX = 0;
                    desiredMoveDirectionY = verticalInput > 0 ? 1 : -1;

                }
                else if (firstInputAxis == "Vertical")
                {
                    desiredMoveDirectionX = horizontalInput > 0 ? 1 : -1;
                    desiredMoveDirectionY = 0;
                }
            }
                dashDirection = new Vector2(desiredMoveDirectionX, desiredMoveDirectionY);

            // Set the cooldown
            isDashOnCooldown = true;
            nextDashTime = Time.time + playerStats.GetDashCooldownTime();
        }

        if (isDashing)
        {
            if (dashDurationTime > 0)
            {
                dashDurationTime -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
            }
        }

        // Check if the cooldown has expired
        if (isDashOnCooldown && Time.time >= nextDashTime)
        {
            isDashOnCooldown = false;
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
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }


    //Attack Functions ----------------------------------------------------------------------------------------------

    private void Attack()
    {
        if (Input.GetButtonDown("Attack") && !isAttacking && !isDashing && !isAttackOnCooldown)
        {
            isAttacking = true;
            // Stop the existing coroutine if it's running
            if (spinAttackHeldCoroutine != null)
            {
                StopCoroutine(spinAttackHeldCoroutine);
                spinAttackHeldCoroutine = null;
            }

            animator.SetTrigger("Attack");

            //Set the cooldown
            isAttackOnCooldown = true;
            nextAttackTime = Time.time + playerStats.GetAttackCooldownTime();

            playerCollisions.CheckSwordCollisions();

            // Start the coroutine to check for spin attack and set the reference
            spinAttackHeldCoroutine = StartCoroutine(CheckForSpinAttack());
        }

        // Check if the cooldown has expired
        if (isAttackOnCooldown && Time.time >= nextAttackTime)
        {
            isAttackOnCooldown = false;
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

        spinAttackHeldCoroutine = null;
    }

    private void SpinAttack()
    {
        animator.SetTrigger("SpinAttackHeld");

        spinAttackReleasedCoroutine = StartCoroutine(CheckForSpinAttackReleased());
    }

    private IEnumerator CheckForSpinAttackReleased()
    {
        print("waiting");
        // Wait until the attack button is released
        yield return new WaitUntil(() => Input.GetButtonUp("Attack"));

        // Trigger the release animation
        animator.SetTrigger("SpinAttackReleased");
    }


    //Attack getters and setters
    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    private void SetIsAttackingAnimationEvent(int i)
    {
        if (i == 0)
        {
            isAttacking = false;
        }
        else
        {
            isAttacking = true;
        }
    }

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
            Gizmos.DrawWireCube(playerCollisions.attackPointUp.position, new Vector2(0.3f, 1));
        }

        if (playerCollisions.attackPointDown != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(playerCollisions.attackPointDown.position, new Vector2(0.3f, 1));
        }

        if (playerCollisions.attackPointLeft != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(playerCollisions.attackPointLeft.position, new Vector2(1, 0.3f));
        }

        if (playerCollisions.attackPointRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(playerCollisions.attackPointRight.position, new Vector2(1, 0.3f));
        }

        if (playerCollisions.attackPointCenter != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(playerCollisions.attackPointCenter.position, new Vector2(2, 2f));
        }
    }
    */
}
