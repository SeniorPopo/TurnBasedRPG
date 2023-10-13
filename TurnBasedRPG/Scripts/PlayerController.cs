using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
     private Animator animator;
     private Rigidbody2D rb;
     private SpriteRenderer sprite;

    [SerializeField] public float walkSpeed = 3f;


    Vector2 movement;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if( movement.x >= 1)
        {
            animator.Play("PlayerMoveE");
             sprite.flipX = false;
        }
        if( movement.x <= -1)
        {
            animator.Play("PlayerMoveW");
            sprite.flipX = true;
        }
        if( movement.y >= 1  &&  movement.x == 0)
        {
            animator.Play("PlayerMoveN");
        }
        if( movement.y <= -1 && movement.x == 0)
        {
            animator.Play("PlayerMoveS");
        }
        if( movement.y == 0  && movement.x == 0)
        {
            animator.Play("SleepyIdle");
        }
    
       
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * walkSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered");
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log(gameObject.name);
            // SceneManager.LoadScene("Necromancer Battle");
        }
    }
}
