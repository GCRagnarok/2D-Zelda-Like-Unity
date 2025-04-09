using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Movement variables
    public float moveSpeed = 5f;

    //Attack variables
    public float startAttackTime = 0.1f;
    public float attackCooldownTime = 0.4f;
    public float attackRadius = 0.6f;
    public float spinAttackRadius = 1.2f;
    public float attackDamage = 10f;
    public float spinAttackDamage = 30f;

    //Health variables
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    //Movement getters and setters
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    //Attack getters and setters
    public float GetStartAttackTime()
    {
        return startAttackTime;
    }

    public void SetStartAttackTime(float newStartAttackTime)
    {
        startAttackTime = newStartAttackTime;
    }

    public float GetAttackCooldownTime()
    {
        return attackCooldownTime;
    }

    public void SetAttackCooldownTime(float newAttackCooldownTime)
    {
        attackCooldownTime = newAttackCooldownTime;
    }

    public float GetAttackRadius()
    {
        return attackRadius;
    }

    public void SetAttackRadius(float newAttackRadius)
    {
        attackRadius = newAttackRadius;
    }

    public float GetSpinAttackRadius()
    {
        return spinAttackRadius;
    }

    public void SetSpinAttackRadius(float newSpinAttackRadius)
    {
        spinAttackRadius = newSpinAttackRadius;
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void SetAttackDamage(float newAttackDamage)
    {
        attackDamage = newAttackDamage;
    }
    public float GetSpinAttackDamage()
    {
        return spinAttackDamage;
    }

    public void SetSpinAttackDamage(float newSpinAttackDamage)
    {
        spinAttackDamage = newSpinAttackDamage;
    }


}
