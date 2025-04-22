using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DestroyBreakable : MonoBehaviour
{
    //Component variables
    public Animator animator;
    private BoxCollider2D boxCollider;
    private SpawnItems spawnItems;
    private PlayerStats playerStats;
    private float potBreakAnimationTime = 2.1f;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spawnItems = GetComponent<SpawnItems>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Destroy()
    {
        // Play the destroy animation
        animator.SetTrigger("Break");

        if (CompareTag("Pot"))
        {
            if (playerStats.GetCurrentHealth() <= playerStats.GetMaxHealth() / playerStats.GetCurrentHeartContainers())
            {
                CheckSpawnHeart();
            }
            else if (playerStats.GetCurrentHealth() >= playerStats.GetMaxHealth() && playerStats.GetCurrentMagic() < playerStats.GetMaxMagic())
            {
                CheckSpawnMagicJar();
            }
            else
            {
                if (Random.value < 0.5f)
                {
                    CheckSpawnHeart();
                }
                else
                {
                    CheckSpawnMagicJar();
                }
            }
        }
        // Destroy the breakable object after the animation is done
        Destroy(boxCollider);
        Destroy(gameObject, potBreakAnimationTime);
    }

    public void CheckSpawnHeart()
    {
        // Generate a random value and spawn hearts if within the spawn chance
        if (Random.value < playerStats.GetHeartSpawnChance())
        {
            spawnItems.SpawnHeart();
        }
        print("Heart spawn chance: " + playerStats.GetHeartSpawnChance());
    }

    public void CheckSpawnMagicJar()
    {
        // Generate a random value and spawn magic jars if within the spawn chance
        if (Random.value < playerStats.GetMagicJarSpawnChance())
        {
            spawnItems.SpawnMagicJar();
        }
        print("MagicJar spawn chance: " + playerStats.GetMagicJarSpawnChance());
    }
}
