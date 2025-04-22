using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    public GameObject heart;
    public GameObject magicjar;

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
        Instantiate(heart, transform.position, Quaternion.identity);
    }

    public void SpawnMagicJar()
    {
        Instantiate(magicjar, transform.position, Quaternion.identity);
    }
}
