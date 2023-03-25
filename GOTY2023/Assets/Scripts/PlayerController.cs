using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameController gc;

    Rigidbody2D rb;
    SpriteRenderer sr;
    public GameObject footObject;

    public float maxSpeed = 3f;
    public float accelleration = 8f;
    public float dragFactor = 0.2f;
    public float jumpSpeed = 2f;
    public float airControl = 0.3f;

    public bool isGrounded = false;

    float inputH = 0f;
    bool jump = false;

    // Start is called before the first frame update
    void Start()
    {
        //singleton pattern
        //ensures there's only ever 1 player object
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(g != this.gameObject)
            {
                Destroy(gameObject);
                return;
            }
        }
        //keep this between scenes
        DontDestroyOnLoad(gameObject);

        //get references to some other components
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        gc = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "HurtBox")
        {
            Death();
        }
    }

    void Death()
    {
        gc.ResetLevel();
    }

    // Update is called once per frame
    void Update()
    {
        //check for grounded
        if(footObject.GetComponent<Collider2D>().GetContacts(new List<Collider2D>()) > 0)
            isGrounded = true;
        else
            isGrounded = false;


        //get input
        inputH = Input.GetAxis("Horizontal");
        jump = Input.GetButton("Jump");

        //cap x speed
        if (rb.velocity.x < -1 * maxSpeed)
            rb.velocity = new Vector2(-1 * maxSpeed, rb.velocity.y);
        else if (rb.velocity.x > maxSpeed)
            rb.velocity = new Vector2(maxSpeed, rb.velocity.y);

        //slow if no h input
        if(Mathf.Abs(inputH) < 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x * (1 - dragFactor), rb.velocity.y);
        }

        //move
        if (Mathf.Abs(rb.velocity.x + (inputH * accelleration * 0.01f)) < maxSpeed)
            if (isGrounded)
                rb.AddForce(new Vector2(inputH * accelleration, 0));
            else
                rb.AddForce(new Vector2(inputH * accelleration * airControl, 0));

        //facing direction
        if (inputH < -0.1f)
            sr.flipX = true;
        else if(inputH > 0.1f)
            sr.flipX = false;

        //jumping
        if (jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    

    }
}
