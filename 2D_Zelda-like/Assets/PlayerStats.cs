using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Movement variables
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldownTime = 0.8f;

    //Attack variables
    public float attackCooldownTime = 0.4f;
    public float attackDamage = 10f;
    public Vector2 swordHorizontalHitBox = new Vector2(0.3f, 0.5f);
    public Vector2 swordVerticalHitBox = new Vector2(0.5f, 0.3f);
    public Vector2 swordSpinHitBox = new Vector2(2f, 2f);

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

    public float GetDashSpeed()
    {
        return dashSpeed;
    }

    public void SetDashSpeed(float newDashSpeed)
    {
        dashSpeed = newDashSpeed;
    }

    public float GetDashDuration()
    {
        return dashDuration;
    }

    public void SetDashDuration(float newDashDuration)
    {
        dashDuration = newDashDuration;
    }

    public float GetDashCooldownTime()
    {
        return dashCooldownTime;
    }

    public void SetDashCooldownTime(float newDashCooldownTime)
    {
        dashCooldownTime = newDashCooldownTime;
    }

    //Attack getters and setters
    public float GetAttackCooldownTime()
    {
        return attackCooldownTime;
    }

    public void SetAttackCooldownTime(float newAttackCooldownTime)
    {
        attackCooldownTime = newAttackCooldownTime;
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void SetAttackDamage(float newAttackDamage)
    {
        attackDamage = newAttackDamage;
    }
}
