using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerStats playerStats;

    //Movement collision variables
    public LayerMask collisionLayer;
    private Rigidbody2D rb2d;

    //Attack collision variables
    public LayerMask attackableLayer;
    public Transform attackPointUp;
    public Transform attackPointDown;
    public Transform attackPointLeft;
    public Transform attackPointRight;
    public Transform attackPointCenter;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    //Movement Collision Functions -----------------------------------------------------------------------------------
    public bool IsColliding(Vector2 newPosition)
    {
        Vector2 direction = newPosition - (Vector2)transform.position;
        float distance = direction.magnitude;

        RaycastHit2D[] hits = new RaycastHit2D[10];
        int hitCount = rb2d.Cast(direction, hits, distance);

        for (int i = 0; i < hitCount; i++)
        {
            if (((1 << hits[i].collider.gameObject.layer) & collisionLayer) != 0)
            {
                return true;
            }
        }

        return false;
    }

    //Attack Collision Functions -------------------------------------------------------------------------------------------
    public void CheckSwordCollisions()
    {
        //directional attack
        if (!playerController.GetIsSpinAttacking())
        {
            switch (playerController.GetDirection())
            {
                case "Up":
                    Collider2D[] hitAttackablesUp = Physics2D.OverlapBoxAll(attackPointUp.position, playerStats.swordHorizontalHitBox, attackableLayer);

                    foreach (Collider2D attackable in hitAttackablesUp)
                    {
                        print("Hit:" + attackable.name);

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Breakable"))
                        {
                            attackable.GetComponent<DestroyBreakable>().Destroy();
                        }

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            attackable.GetComponent<EnemyStats>().TakeDamage(playerStats.GetAttackDamage());
                        }
                    }
                    break;

                case "Down":
                    Collider2D[] hitAttackablesDown = Physics2D.OverlapBoxAll(attackPointDown.position, playerStats.swordHorizontalHitBox, attackableLayer);

                    foreach (Collider2D attackable in hitAttackablesDown)
                    {
                        print("Hit:" + attackable.name);

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Breakable"))
                        {
                            attackable.GetComponent<DestroyBreakable>().Destroy();
                        }

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            attackable.GetComponent<EnemyStats>().TakeDamage(playerStats.GetAttackDamage());
                        }
                    }
                    break;

                case "Left":
                    Collider2D[] hitAttackablesLeft = Physics2D.OverlapBoxAll(attackPointLeft.position, playerStats.swordVerticalHitBox, attackableLayer);

                    foreach (Collider2D attackable in hitAttackablesLeft)
                    {
                        print("Hit:" + attackable.name);

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Breakable"))
                        {
                            attackable.GetComponent<DestroyBreakable>().Destroy();
                        }

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            attackable.GetComponent<EnemyStats>().TakeDamage(playerStats.GetAttackDamage());
                        }
                    }
                    break;

                case "Right":

                    Collider2D[] hitAttackablesRight = Physics2D.OverlapBoxAll(attackPointRight.position, playerStats.swordVerticalHitBox, attackableLayer);

                    foreach (Collider2D attackable in hitAttackablesRight)
                    {
                        print("Hit:" + attackable.name);

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Breakable"))
                        {
                            attackable.GetComponent<DestroyBreakable>().Destroy();
                        }

                        if (attackable.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                        {
                            attackable.GetComponent<EnemyStats>().TakeDamage(playerStats.GetAttackDamage());
                        }
                    }
                    break;
            }
        }
        //Spin attack
        else
        {
            Collider2D[] hitAttackablesCenter = Physics2D.OverlapBoxAll(attackPointCenter.position, playerStats.swordSpinHitBox, attackableLayer);

            foreach (Collider2D attackable in hitAttackablesCenter)
            {
                print("Hit:" + attackable.name);

                if (attackable.gameObject.layer == LayerMask.NameToLayer("Breakable"))
                {
                    attackable.GetComponent<DestroyBreakable>().Destroy();
                }

                if (attackable.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    attackable.GetComponent<EnemyStats>().TakeDamage(playerStats.GetAttackDamage());
                }
            }
        }

    }
}
