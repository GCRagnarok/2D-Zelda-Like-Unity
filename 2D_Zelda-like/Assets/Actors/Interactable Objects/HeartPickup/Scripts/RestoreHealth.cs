using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreHealth : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private PlayerStats playerStats;
    public int healthToRestore = 1;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float newCurrentHealth = playerStats.GetCurrentHealth() + healthToRestore;
            playerStats.SetCurrentHealth(newCurrentHealth);
            playerStats.UpdateItemSpawnChance();
            Destroy(gameObject);
        }
    }
}
