using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    public GameObject heart;
    public GameObject magicjar;
    public int maxHealthPickups = 3;
    public int maxMagicPickups = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnHeart()
    {
        RestoreHealth[] healthPickups = FindObjectsByType<RestoreHealth>(FindObjectsSortMode.None);
        int healthPickupCount = healthPickups.Length;

        if (healthPickupCount < maxHealthPickups)
        {
            Instantiate(heart, transform.position, Quaternion.identity);
        }
    }

    public void SpawnMagicJar()
    {
        RestoreMagic[] magicPickups = FindObjectsByType<RestoreMagic>(FindObjectsSortMode.None);
        int magicPickupCount = magicPickups.Length;

        if (magicPickupCount < maxMagicPickups)
        {
            Instantiate(magicjar, transform.position, Quaternion.identity);
        }
    }
}
