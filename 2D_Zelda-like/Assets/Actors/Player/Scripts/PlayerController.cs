using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private BoxCollider2D boxCollider;
    public LayerMask wallLayer;

    private float moveDirectionX;
    private float moveDirectionY;
    public float moveSpeed = 5.0f;

    private float attackTime;
    public float startAttackTime;
    public float attackCooldownTime;
    private float nextAttackTime = 0;

    private bool canMove;
    private bool multipleInput;

    private string firstInputAxis;

    void Start()
    {
        canMove = true;
        attackTime = startAttackTime;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        AttackCooldown();
    }

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

                animator.SetBool("Vertical", true);
                animator.SetBool("Horizontal", false);
                animator.SetFloat("Direction", verticalInput);
            }
            else if (firstInputAxis == "Vertical")
            {
                moveDirectionX = horizontalInput > 0 ? 1 : -1;
                moveDirectionY = 0;

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
        Vector2 newPosition = (Vector2)transform.position + moveDirection * moveSpeed * Time.deltaTime;

        if (!IsCollidingWithWall(newPosition) && canMove)
        {
            transform.position = newPosition;
        }
    }

    private bool IsCollidingWithWall(Vector2 newPosition)
    {
        // Perform an overlap box to check for collisions with the wall layer
        Vector2 direction = newPosition - (Vector2)transform.position;
        float distance = direction.magnitude;
        Vector2 boxSize = boxCollider.bounds.size;
        Vector2 boxCenter = (Vector2)transform.position + direction.normalized * distance;

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, wallLayer);

        return hits.Length > 0;
    }

    private void AttackCooldown()
    {
        if (Time.time > nextAttackTime)
        {
            if (attackTime <= 0)
            {
                canMove = true;
                attackTime = startAttackTime;
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
            canMove = false;
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldownTime;
        }
    }

}
