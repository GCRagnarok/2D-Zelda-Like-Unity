using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie1Controller : MonoBehaviour
{
    public Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }

    public void Die()
    {
        animator.SetBool("Dead", true);
        Destroy(boxCollider);
    }

    public void SetSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }
}
