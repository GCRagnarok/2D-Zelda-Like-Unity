using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollisions : MonoBehaviour
{
    public LayerMask attackableLayer;

    public BoxCollider2D attackPointUp;
    public BoxCollider2D attackPointDown;
    public BoxCollider2D attackPointLeft;
    public BoxCollider2D attackPointRight;
    public BoxCollider2D attackPointCenter;

    private PlayerController playerController;
    private PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckSwordCollisions()
    {
        BoxCollider2D activeAttackPoint = null;

        if (!playerController.GetIsSpinAttacking())
        {
            switch (playerController.GetDirection())
            {
                case "Up":
                    activeAttackPoint = attackPointUp;
                    break;
                case "Down":
                    activeAttackPoint = attackPointDown;
                    break;
                case "Left":
                    activeAttackPoint = attackPointLeft;
                    break;
                case "Right":
                    activeAttackPoint = attackPointRight;
                    break;
            }
        }
        else
        {
            activeAttackPoint = attackPointCenter;
        }

        if (activeAttackPoint != null)
        {
            print(this.name + " Checking collisions in " + activeAttackPoint.name);

            Vector2 size = activeAttackPoint.size;

            Collider2D[] hitAttackables = Physics2D.OverlapBoxAll(activeAttackPoint.bounds.center, size, 0, attackableLayer);

            foreach (Collider2D attackable in hitAttackables)
            {
                print(this.name + " hit " + attackable.name);

                if (attackable.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    if (playerController.GetIsSpinAttacking())
                    {
                        attackable.GetComponent<EnemyStats>().TakeDamage(playerStats.GetSpinAttackDamage());
                    }
                    else
                    {
                        attackable.GetComponent<EnemyStats>().TakeDamage(playerStats.GetAttackDamage());
                    }
                }
                else if (attackable.gameObject.layer == LayerMask.NameToLayer("Breakable"))
                {
                    attackable.GetComponent<DestroyBreakable>().Destroy();
                }
            }
        }
    }
}
