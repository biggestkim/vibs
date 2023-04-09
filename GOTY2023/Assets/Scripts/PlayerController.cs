using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameController gc;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    public GameObject footObject;

    public float maxSpeed = 3f;
    public float accelleration = 8f;
    public float dragFactor = 0.2f;
    public float gravityAmount = 3f;

    public float maxJumpDuration = 0.5f;
    float jumpTimer = 0f;
    public float jumpGravity = 0.5f;
    public float jumpSpeed = 2f; //initial jump speed
    public float airControl = 0.3f;

    public bool isGrounded = false;

    bool isDashing = false;
    public bool canDash = true;
    public float dashSpeed = 8f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;


    float inputH = 0f;
    float inputV = 0f;
    bool jumping = false; //jump state, used to reduce gravity
    bool jump = false; //jump trigger

    public GameObject footDust;
    public float footDustDelay = 0.25f;
    float footDustTimer = 0;

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
        anim = GetComponentInChildren<Animator>();
        gc = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

        rb.gravityScale = gravityAmount;
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
        //ignore input while dashing
        if (isDashing)
        {
            return;
        }

        //check for grounded
        if(footObject.GetComponent<Collider2D>().GetContacts(new List<Collider2D>()) > 0)
            isGrounded = true;
        else
            isGrounded = false;

        //get input
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");
        jump = Input.GetButtonDown("Jump");

        //dash trigger
        if(!isGrounded && canDash && jump)
        {
            StartCoroutine(Dash());
        }

        if (isGrounded)
        {
            //animation stuff
            anim.SetBool("Jump", false);
            anim.SetFloat("Speed", rb.velocity.magnitude);

            //foot dust
            footDustTimer += Time.deltaTime;
            if (footDustTimer > footDustDelay && inputH != 0)
            {
                footDustTimer = 0;
                Instantiate(footDust, footObject.transform.position, transform.rotation);
            }
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

        //cap x speed
        if (!isDashing)
        {
            if (rb.velocity.magnitude > maxSpeed)
                rb.velocity = rb.velocity.normalized * maxSpeed;
        }

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

        //jump
        if (jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            jumping = true;
            jumpTimer = 0;
        }

        //reduce gravity while jumping
        if (jumping)
        {
            anim.SetBool("Jump", true);

            rb.gravityScale = jumpGravity;

            //limit length of jump
            jumpTimer += Time.deltaTime;
            
            //when jump button released or max jump reached, reset gravity
            if (Input.GetButtonUp("Jump") || jumpTimer >= maxJumpDuration)
            {
                Debug.Log("Jump released");
                anim.SetBool("Jump", false);
                jumping = false;
                rb.gravityScale = gravityAmount;
            }
        }

        //sets speed and eliminates gravity while dashing
        IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(inputH, inputV).normalized * dashSpeed;
            yield return new WaitForSeconds(dashDuration);
            rb.gravityScale = gravityAmount;
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
            
        }
    }
}
