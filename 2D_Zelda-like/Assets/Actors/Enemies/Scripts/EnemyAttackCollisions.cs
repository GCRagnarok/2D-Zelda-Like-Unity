using System.Collections;using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollisions : MonoBehaviour
{
    public LayerMask attackableLayer;
    private EnemyStats enemyStats;

    //zombie1 variables ---------------------------------------------------------------------------------------------
    private Zombie1Controller z1Controller;

    public BoxCollider2D z1AttackPointUp;
    public BoxCollider2D z1AttackPointDown;
    public BoxCollider2D z1AttackPointLeft;
    public BoxCollider2D z1AttackPointRight;

    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Zombie1"))
        {
            z1Controller = GetComponent<Zombie1Controller>();
        }

        enemyStats = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckAttackCollisions()
    {
        BoxCollider2D activeAttackPoint = null;

        //Zombie1 attack collision check
        if (CompareTag("Zombie1"))
        {
            switch (z1Controller.GetDirection())
            {
                case "Up":
                    activeAttackPoint = z1AttackPointUp;
                    break;
                case "Down":
                    activeAttackPoint = z1AttackPointDown;
                    break;
                case "Left":
                    activeAttackPoint = z1AttackPointLeft;
                    break;
                case "Right":
                    activeAttackPoint = z1AttackPointRight;
                    break;
            }

            if (activeAttackPoint != null)
            {
                print(this.name + " Checking collisions in " + activeAttackPoint.name);

                Vector2 size = activeAttackPoint.size;

                Collider2D[] hitAttackables = Physics2D.OverlapBoxAll(activeAttackPoint.bounds.center, size, 0, attackableLayer);

                foreach (Collider2D attackable in hitAttackables)
                {
                    print(this.name + " hit " + attackable.name);
                    if (attackable.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        attackable.GetComponent<PlayerStats>().TakeDamage(enemyStats.GetAttackDamage());
                    }
                }
            }
        }
    }
}
