using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    //Health variables ----------------------------------------------------------------------------------------------
    public float maxHealth = 50f;
    public float currentHealth;

    //Zombie1 variables ---------------------------------------------------------------------------------------------

    //Component variables
    private Zombie1Controller z1Controller;
    private AIPath aiPath;

    //Ai Movement variables
    public float chaseSpeed = 3f;
    public float patrolSpeed = 2f;
    public float searchRadius = 5f;
    public float endDistanceOffset = 0.2f;

    //Engage variables
    public float attackDamage = 10f;
    public float engageCooldownMinTime = 0.5f;
    public float engageCooldownMaxTime = 3f;
    public float attackChance = 0.7f;
    public float evadeDistance = 1f;

    //End of variables ----------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Zombie1"))
        {
            maxHealth = 50f;
            z1Controller = GetComponent<Zombie1Controller>();
            aiPath = z1Controller.GetComponent<AIPath>();
        }

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        if (CompareTag("Zombie1"))
        {
            if (z1Controller.canBeHurt)
            {
                SetCurrentHealth(currentHealth - damage);
                z1Controller.Hurt();
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (CompareTag("Zombie1"))
        {
            z1Controller.Die();
        }
    }

    //Getters and Setters for all Enemies ------------------------------------------------------------------------

    //Health getters and setters
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(float newCurrentHealth)
    {
        if (newCurrentHealth >= maxHealth)
        {
            newCurrentHealth = maxHealth;
        }
        else if (newCurrentHealth < 0)
        {
            newCurrentHealth = 0;
        }

        currentHealth = newCurrentHealth;
    }

    //Zombie1 getters and setters -----------------------------------------------------------------------------------

    //Attack getters and setters
    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void SetAttackDamage(float newAttackDamage)
    {
        attackDamage = newAttackDamage;
    }

    public float GetAttackCooldownMinTime()
    {
        return engageCooldownMinTime;
    }

    public void SetAttackCooldownMinTime(float newAttackCooldownMinTime)
    {
        engageCooldownMinTime = newAttackCooldownMinTime;
    }

    public float GetAttackCooldownMaxTime()
    {
        return engageCooldownMaxTime;
    }

    public void SetAttackCooldownMaxTime(float newAttackCooldownMaxTime)
    {
        engageCooldownMaxTime = newAttackCooldownMaxTime;
    }

    //Zombie1 Ai Movement getters and setters

    public float GetChaseSpeed()
    {
        return chaseSpeed;
    }
    public void SetChaseSpeed(float newChaseSpeed)
    {
        chaseSpeed = newChaseSpeed;
    }

    public float GetPatrolSpeed()
    {
        return patrolSpeed;
    }

    public void SetPatrolSpeed(float newPatrolSpeed)
    {
        patrolSpeed = newPatrolSpeed;
    }

    public float GetSearchRadius()
    {
        return searchRadius;
    }

    public void SetSearchRadius(float newSearchRadius)
    {
        searchRadius = newSearchRadius;
    }

    public float GetEndDistanceOffset()
    {
        return endDistanceOffset;
    }

    public void SetEndDistanceOffset(float newEndDistanceOffset)
    {
        endDistanceOffset = newEndDistanceOffset;
    }
}