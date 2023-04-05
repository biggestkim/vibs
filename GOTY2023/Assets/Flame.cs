using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//bounces back and forth in an arc. set jumpRight in editor to pick starting direction
public class Flame : MonoBehaviour
{

    float jumpDelay = 1.5f;
    float jumpTimer = 0f;
    public float jumpSpeed = 4f;

    Rigidbody2D rb;

    public bool jumpRight = true; //which way the flame starts jumping

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(jumpTimer > jumpDelay)
        {
            rb.velocity = new Vector2((jumpSpeed / 4), jumpSpeed);
            if (!jumpRight)
                rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
            jumpTimer = 0f; //reset timer
            jumpRight = !jumpRight; //switch direction
        }
        else
        {
            jumpTimer += Time.deltaTime;
        }

    }
}
