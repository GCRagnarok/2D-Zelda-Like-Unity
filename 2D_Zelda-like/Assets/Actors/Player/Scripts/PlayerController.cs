using Pathfinding;
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
    private PlayerStats playerStats;
    private Rigidbody2D rb2d;
    private PlayerAttackCollisions playerAttackCollisions;

    //Movement variables
    private float moveDirectionX;
    private float moveDirectionY;
    private float lastMoveDirectionX = 0;
    private float lastMoveDirectionY = -1;
    private float dashDurationTime;
    private float nextDashTime;
    private float originalMoveSpeed;

    private bool canMove = true;
    private bool canDash = true;
    private bool isDashing;
    private bool isDashOnCooldown;

    private Vector2 moveDirection;
    private Vector2 walkDirection;
    private Vector2 dashDirection;

    private string firstInputAxis;

    //Attack variables
    private float attackTime;
    private float nextAttackTime;

    private bool isAttacking;
    private float activateSpinAttackTime = 0.8f;
    private bool isChargingSpinAttack;
    private bool isSpinAttacking;
    private bool isAttackOnCooldown;

    private Coroutine spinAttackHeldCoroutine;

    //Hurt variables
    private bool isHurt;
    private Coroutine ResetHurtCouroutine;
    public bool isDead;
    public bool canBeHurt = true;

    //Animation variables
    private float attackAnimationTime = 0.8f;
    private float startSpinAttackAnimationTime = 0.9f;
    private float spinAttackReleaseAnimationTime = 1.1f;
    private float hurtAnimationTime = 0.6f;
    private float dieAnimationTime = 2.1f;

    void Start()
    {
        // Initialize Components
        rb2d = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        playerAttackCollisions = GetComponent<PlayerAttackCollisions>();

        // Initialize movement variables
        originalMoveSpeed = playerStats.GetMoveSpeed();

        //initialize direction variables
        animator.SetBool("Down", true);
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
        if (!canMove)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }

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

            animator.SetBool("Idle", false);
            animator.SetBool("Walking", true);
            animator.SetBool("Horizontal", true);
            animator.SetBool("Vertical", false);
            animator.SetFloat("Direction", horizontalInput);

            GetDirection();
        }
        // Check if the input is exclusivley vertical then move & animate accordingly
        else if (verticalInput != 0 && horizontalInput == 0)
        {
            firstInputAxis = "Vertical";
            moveDirectionX = 0;
            moveDirectionY = verticalInput > 0 ? 1 : -1;

            lastMoveDirectionX = moveDirectionX;
            lastMoveDirectionY = moveDirectionY;

            animator.SetBool("Idle", false);
            animator.SetBool("Walking", true);
            animator.SetBool("Horizontal", false);
            animator.SetBool("Vertical", true);
            animator.SetFloat("Direction", verticalInput);

            GetDirection();
        }
        // Check if the input is both horizontal and vertical
        else if (horizontalInput != 0 && verticalInput != 0)
        {
            // Check which input was pressed first then move & animate the latter input accordingly
            if (firstInputAxis == "Horizontal")
            {
                moveDirectionX = 0;
                moveDirectionY = verticalInput > 0 ? 1 : -1;

                lastMoveDirectionX = moveDirectionX;
                lastMoveDirectionY = moveDirectionY;

                animator.SetBool("Idle", false);
                animator.SetBool("Walking", true);
                animator.SetBool("Horizontal", false);
                animator.SetBool("Vertical", true);
                animator.SetFloat("Direction", verticalInput);
            }
            else if (firstInputAxis == "Vertical")
            {
                moveDirectionX = horizontalInput > 0 ? 1 : -1;
                moveDirectionY = 0;

                lastMoveDirectionX = moveDirectionX;
                lastMoveDirectionY = moveDirectionY;

                animator.SetBool("Idle", false);
                animator.SetBool("Walking", true);
                animator.SetBool("Horizontal", true);
                animator.SetBool("Vertical", false);
                animator.SetFloat("Direction", horizontalInput);
            }

            GetDirection();
        }
        // Check if no input is detected then stop moving & animate accordingly
        else if (horizontalInput == 0 && verticalInput == 0 || canMove == false)
        {
            moveDirectionX = 0;
            moveDirectionY = 0;
            firstInputAxis = "";

            animator.SetBool("Idle", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Horizontal", false);
            animator.SetBool("Vertical", false);
            animator.SetFloat("Direction", 0);
        }

        walkDirection = new Vector2(moveDirectionX, moveDirectionY).normalized;
        moveDirection = isDashing ? dashDirection : walkDirection;
        float currentSpeed = isDashing ? playerStats.GetDashSpeed() : playerStats.GetMoveSpeed();

        rb2d.velocity = moveDirection * currentSpeed;
    }


    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && canMove && canDash && !isDashing && !isAttacking && !isDashOnCooldown)
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

            animator.SetBool("Dashing", true);
            canBeHurt = false;
            dashDirection = new Vector2(desiredMoveDirectionX, desiredMoveDirectionY).normalized;

            // Set the cooldown
            isDashOnCooldown = true;
            nextDashTime = Time.time + (playerStats.GetDashDuration() + playerStats.GetDashCooldownTime());
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
                canBeHurt = true;
                animator.SetBool("Dashing", false);
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
                animator.SetBool("Right", true);
                animator.SetBool("Left", false);
                animator.SetBool("Up", false);
                animator.SetBool("Down", false);
                break;
            case (0, 1):
                direction = "Up";
                animator.SetBool("Up", true);
                animator.SetBool("Down", false);
                animator.SetBool("Right", false);
                animator.SetBool("Left", false);
                break;
            case (0, -1):
                direction = "Down";
                animator.SetBool("Down", true);
                animator.SetBool("Up", false);
                animator.SetBool("Right", false);
                animator.SetBool("Left", false);
                break;
            case (-1, 0):
                direction = "Left";
                animator.SetBool("Left", true);
                animator.SetBool("Right", false);
                animator.SetBool("Up", false);
                animator.SetBool("Down", false);
                break;
        }
        return direction;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    //Attack Functions ----------------------------------------------------------------------------------------------
    private void Attack()
    {
        if (Input.GetButtonDown("Attack") && !isAttacking && !isDashing && !isAttackOnCooldown && !isHurt)
        {
            // Stop spinAttackHeldCoroutine if it's running
            if (spinAttackHeldCoroutine != null)
            {
                StopCoroutine(spinAttackHeldCoroutine);
                spinAttackHeldCoroutine = null;
            }

            isAttacking = true;
            canMove = false;

            animator.SetTrigger("Attack");

            isAttackOnCooldown = true;
            nextAttackTime = Time.time + playerStats.GetAttackCooldownTime();

            StartCoroutine(ResetAttackBooleans());

            playerAttackCollisions.CheckSwordCollisions();

            if (playerStats.GetCurrentMagic() >= playerStats.GetSpinAttackCost())
            {
                spinAttackHeldCoroutine = StartCoroutine(CheckForSpinAttack());
            }
        }

        // Check if the cooldown has expired
        if (isAttackOnCooldown && Time.time >= nextAttackTime)
        {
            isAttackOnCooldown = false;
        }
    }

    private IEnumerator ResetAttackBooleans()
    {
        yield return new WaitForSeconds(attackAnimationTime - 0.2f);

        isAttacking = false;
        canMove = true;
    }

    private IEnumerator CheckForSpinAttack()
    {
        yield return new WaitForSeconds(activateSpinAttackTime);

        // Check if the attack button is still being held
        if (Input.GetButton("Attack") && !isHurt)
        {
            SpinAttack();
        }

        spinAttackHeldCoroutine = null;;
    }

    private void SpinAttack()
    {
        isSpinAttacking = true;
        canMove = false;
        playerStats.SetMoveSpeed(playerStats.GetSpinAttackMoveSpeed());

        animator.SetTrigger("SpinAttackHeld");
        animator.SetBool("HoldingSpinAttack", true);

        playerStats.SetCurrentTempMagic(playerStats.GetCurrentTempMagic() - playerStats.GetSpinAttackCost());

        StartCoroutine(CheckForSpinAttackReleased());
        StartCoroutine(SetFinishedChargingSpinAttackBooleans());
    }

    private IEnumerator SetFinishedChargingSpinAttackBooleans()
    {
        isChargingSpinAttack = true;

        // Wait for start spin attack animation time and enable movement
        yield return new WaitForSeconds(startSpinAttackAnimationTime - 0.4f);

        // Check if the spin attack was successfully charged
        if (isSpinAttacking)
        {
            canMove = true;
            canDash = false;
            isChargingSpinAttack = false;
        }
    }

    private IEnumerator CheckForSpinAttackReleased()
    {
        // While the spin attack is charged, check for exit conditions
        while (true)
        {
            // If the player is hurt or attack button is released while charging, cancel the spin attack and reset variables
            if (isHurt || Input.GetButtonUp("Attack") && isChargingSpinAttack)
            {
                playerStats.SetCurrentTempMagic(playerStats.GetCurrentMagic());

                animator.SetBool("HoldingSpinAttack", false);
                isSpinAttacking = false;
                canMove = true;
                canDash = true;
                playerStats.SetMoveSpeed(originalMoveSpeed);

                yield break; // Exit the coroutine
            }

            // If the attack button is released and charging has finished, release the spin attack
            if (Input.GetButtonUp("Attack") && !isChargingSpinAttack)
            {
                canMove = false;

                animator.SetTrigger("SpinAttackReleased");

                playerAttackCollisions.CheckSwordCollisions();

                playerStats.SetCurrentMagic(playerStats.GetCurrentMagic() - playerStats.GetSpinAttackCost());

                StartCoroutine(ResetSpinAttackVariables());

                yield break; // Exit the coroutine
            }
         
            // Wait until the next frame before checking again
            yield return null;
        }
    }

    private IEnumerator ResetSpinAttackVariables()
    {
        yield return new WaitForSeconds(spinAttackReleaseAnimationTime - 0.5f);

        animator.SetBool("HoldingSpinAttack", false);
        isSpinAttacking = false;
        canMove = true;
        canDash = true;
        playerStats.SetMoveSpeed(originalMoveSpeed);
    }

    //Attack getters and setters
    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    public bool GetIsSpinAttacking()
    {
        return isSpinAttacking;
    }

    //Hurt Functions ------------------------------------------------------------------------------------------------

    public void Hurt()
    {
        canMove = false;
        isHurt = true;
        animator.SetTrigger("Hurt");

        StartCoroutine(ResetHurtBooleans());
    }

    public IEnumerator ResetHurtBooleans()
    {
        yield return new WaitForSeconds(hurtAnimationTime);

        if (!isDead)
        {
            canMove = true;
            isHurt = false;
        }
    }

    public void Die()
    {
        isDead = true;
        canMove = false;
        isDashing = true;
        isAttacking = true;

        animator.SetTrigger("Die");

        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(dieAnimationTime + 1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
