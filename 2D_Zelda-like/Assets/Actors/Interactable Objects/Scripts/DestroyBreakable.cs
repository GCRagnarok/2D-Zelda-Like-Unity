using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBreakable : MonoBehaviour
{

    //Editor variables
    public Animator animator;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Destroy()
    {
        // Play the destroy animation
        animator.SetTrigger("Break");

        // Destroy the breakable object after the animation is done
        Destroy(boxCollider);
        Destroy(gameObject, 2.0f);
    }
}
