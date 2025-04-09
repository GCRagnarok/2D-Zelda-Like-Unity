using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Zombie1"))
        {
            maxHealth = 50f;
        }

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (CompareTag("Zombie1"))
        {
            GetComponent<Zombie1Controller>().Hurt();
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
            GetComponent<Zombie1Controller>().Die();
            Destroy(gameObject, 2.0f);
        }
    }
}