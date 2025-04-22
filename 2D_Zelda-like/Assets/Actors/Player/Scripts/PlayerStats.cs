using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    //Componenet references
    private PlayerController playerController;

    //Movement variables
    public float moveSpeed = 4f;
    public float spinAttackMoveSpeed = 2f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldownTime = 0.8f;

    //Attack variables
    public float attackCooldownTime = 0.4f;
    public float attackDamage = 10f;
    public float spinAttackDamage = 20f;
    public float spinAttackCost = 20f;

    //Health variables
    public float maxHealth = 12f;
    public float currentHealth;
    public int maxHeartContainers;
    public int currentHeartContainers;
    private int heartContainerSections = 4;
    private GameObject[] heartContainers;
    private Image[] heartsImage;
    public Sprite[] heartStages; 
    public float heartSpawnChance = 0f;
    public float magicJarSpawnChance = 0f;

    //Magic variables

    public GameObject magicMeterFill;
    public GameObject tempMagicMeterFill;
    private float maxMagic = 100f;
    public float currentMagic;
    private float maxTempMagic = 100f;
    private float currentTempMagic;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        currentHealth = maxHealth;
        currentMagic = maxMagic;
        maxTempMagic = maxMagic;
        currentTempMagic = maxTempMagic;

        FindHeartContainer();
        AddHeartContainerToArray();
        AddHeartImageToArray();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (currentHealth < maxHealth)
            {
                currentHealth++;
            }
            UpdateHeartContainers();
        }
    }

    // UI Heart Container Management -------------------------------------------------------------------
    private GameObject[] FindHeartContainer()
    {
        // Find all GameObjects with the tag "HeartContainer"
        heartContainers = GameObject.FindGameObjectsWithTag("HeartContainer");

        // Sort the array by name (or another property, if applicable)
        System.Array.Sort(heartContainers, (a, b) => a.name.CompareTo(b.name));

        return heartContainers;
    }

    private void AddHeartContainerToArray()
    {
        // Determine the number of active heart containers
        foreach (GameObject heartContainer in heartContainers)
        {
            maxHeartContainers++;

            if (maxHealth / maxHeartContainers >= heartContainerSections)
            {
                currentHeartContainers++;
            }
            else
            {
                heartContainer.SetActive(false);
            }
        }
    }

    private void AddHeartImageToArray()
    {
        // Initialize the heartsUI array to match the size of currentHeartContainers
        heartsImage = new Image[currentHeartContainers];

        // Populate the heartsUI array with the Image components of active heart containers
        int i = 0;
        foreach (GameObject heartContainer in heartContainers)
        {
            if (i < currentHeartContainers)
            {
                Image heartImage = heartContainer.GetComponent<Image>();
                if (heartImage != null)
                {
                    heartsImage[i] = heartImage;
                    i++;
                }
            }
        }
    }

    private void UpdateHeartContainers()
    {
        if (heartsImage != null && heartsImage.Length > 0)
        {
            // Calculate the number of full heart sections based on current health
            int currentFullHearts = Mathf.FloorToInt(currentHealth / heartContainerSections);

            // Loop through all active heart containers
            for (int i = 0; i < currentHeartContainers; i++)
            {
                Image heartImage = heartsImage[i];

                if (heartImage != null)
                {
                    // Determine the sprite index for this heart container
                    int spriteIndex;

                    if (i < currentFullHearts)
                    {
                        // This heart container is full
                        spriteIndex = 0; // Full heart sprite
                    }
                    else if (i == currentFullHearts)
                    {
                        // This heart container is partially full
                        float remainingHealth = currentHealth % heartContainerSections;
                        spriteIndex = Mathf.Clamp(heartContainerSections - Mathf.CeilToInt(remainingHealth), 0, heartStages.Length - 1);
                    }
                    else
                    {
                        // This heart container is empty
                        spriteIndex = heartStages.Length - 1; // Empty heart sprite
                    }

                    // Update the heart container's sprite
                    heartImage.sprite = heartStages[spriteIndex];
                }
            }
        }
        else
        {
            Debug.LogWarning("No heart images found to update.");
        }
    }

    //UI Magic Meter Functions -----------------------------------------------------------------------------

    private void UpdateMagicMeter()
    {
        magicMeterFill.GetComponent<Image>().fillAmount = currentMagic / maxMagic;

        currentTempMagic = currentMagic;

        UpdateTempMagicMeter();
    }

    private void UpdateTempMagicMeter()
    {
        tempMagicMeterFill.GetComponent<Image>().fillAmount = currentTempMagic / maxTempMagic;
    }

    //Handle Damage Functions ------------------------------------------------------------------------------

    public void TakeDamage(float damage)
    {
        if (!playerController.isDead && playerController.canBeHurt)
        {
            SetCurrentHealth(currentHealth - damage);
            playerController.Hurt();
            UpdateHeartContainers();
            UpdateItemSpawnChance();

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        playerController.Die();
    }

    //Item Spawn Chance Functions ------------------------------------------------------------------------

    public void UpdateItemSpawnChance()
    {
        if (currentHealth < maxHealth)
        {
            SetHeartSpawnChance(1 - (currentHealth / maxHealth));
        }
        else if (currentHeartContainers >= maxHealth)
        {
            SetHeartSpawnChance(0);
        }

        if (currentMagic < maxMagic)
        {
            SetMagicJarSpawnChance(1 - (currentMagic / maxMagic));
        }
        else if (currentHeartContainers >= maxHealth)
        {
            SetMagicJarSpawnChance(0);
        }


        print("Heart Spawn chance: " + GetHeartSpawnChance());
        print("MagicJar Spawn chance: " + GetMagicJarSpawnChance());
    }

    //Getters and Setters for all Player Stats ------------------------------------------------------------
    //Movement getters and setters
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public float GetSpinAttackMoveSpeed()
    {
        return spinAttackMoveSpeed;
    }

    public void SetSpinAttackMoveSpeed(float newMoveSpeed)
    {
        spinAttackMoveSpeed = newMoveSpeed;
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

    public float GetSpinAttackDamage()
    {
        return spinAttackDamage;
    }

    public void SetSpinAttackDamage(float newSpinAttackDamage)
    {
        spinAttackDamage = newSpinAttackDamage;
    }

    public float GetSpinAttackCost()
    {
        return spinAttackCost;
    }

    public void SetSpinAttackCost(float newSpinAttackCost)
    {
        spinAttackCost = newSpinAttackCost;
    }

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
        currentHealth = newCurrentHealth;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHeartContainers();
    }

    public int GetMaxHeartContainers()
    {
        return maxHeartContainers;
    }

    public void SetMaxHeartContainers(int newMaxHeartContainers)
    {
        maxHeartContainers = newMaxHeartContainers;
    }

    public int GetCurrentHeartContainers()
    {
        return currentHeartContainers;
    }

    public void SetCurrentHeartContainers(int newCurrentHeartContainers)
    {
        currentHeartContainers = newCurrentHeartContainers;
        UpdateHeartContainers();
        UpdateItemSpawnChance();
    }

    //Magic getters and setters

    public float GetMaxMagic()
    {
        return maxMagic;
    }

    public void SetMaxMagic(float newMaxMagic)
    {
        maxMagic = newMaxMagic;
    }

    public float GetCurrentMagic()
    {
        return currentMagic;
    }

    public void SetCurrentMagic(float newCurrentMagic)
    {
        currentMagic = newCurrentMagic;

        if (currentMagic > maxMagic)
        {
            currentMagic = maxMagic;
        }
        else if (currentMagic < 0)
        {
            currentMagic = 0;
        }

        UpdateMagicMeter();
        UpdateItemSpawnChance();
    }

    public float GetMaxTempMagic()
    {
        return maxTempMagic;
    }

    public void SetMaxTempMagic(float newMaxTempMagic)
    {
        maxTempMagic = newMaxTempMagic;
    }

    public float GetCurrentTempMagic()
    {
        return currentTempMagic;
    }

    public void SetCurrentTempMagic(float newCurrentTempMagic)
    {
        currentTempMagic = newCurrentTempMagic;

        if (currentTempMagic > maxTempMagic)
        {
            currentMagic = maxTempMagic;
        }
        else if (currentTempMagic < 0)
        {
            currentTempMagic = 0;
        }

        UpdateTempMagicMeter();
    }

    //Item spawn chance getters and setters
    public float GetHeartSpawnChance()
    {
        return heartSpawnChance;
    }

    public void SetHeartSpawnChance(float newHeartSpawnChance)
    {
        heartSpawnChance = newHeartSpawnChance;
    }

    public float GetMagicJarSpawnChance()
    {
        return magicJarSpawnChance;
    }

    public void SetMagicJarSpawnChance(float newMagicJarSpawnChance)
    {
        magicJarSpawnChance = newMagicJarSpawnChance;
    }

}



