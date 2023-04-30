using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameController gc;
    CameraController cam;

    //components
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    public GameObject footObject;
    public GameObject dashIndicator;
    TrailRenderer dashTrail;

    //movmeent
    public float maxXSpeed = 6f;
    public float maxYSpeed = 10f;
    public float accelleration = 8f;
    public float dragFactor = 0.2f;
    public float gravityAmount = 3f;

    //jump
    public float maxJumpDuration = 0.2f;
    float jumpTimer = 0f;
    public float jumpGravity = 0.5f;
    public float jumpSpeed = 2f; //initial jump speed
    public float airControl = 0.3f;

    public bool isGrounded = false;
    public bool onPlatform = false;

    //dash
    bool isDashing = false;
    public bool canDash = true;
    public float dashPause = 500f;
    public float dashSpeed = 8f;
    public float maxDashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    //input
    float inputH = 0f;
    float inputV = 0f;
    bool jumping = false; //jump state, used to reduce gravity
    bool jump = false; //jump trigger

    //effects
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

        //get references to some other components
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        gc = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
        cam = GetComponentInChildren<CameraController>();
        dashTrail = GetComponent<TrailRenderer>();
        dashTrail.enabled = false;

        rb.gravityScale = gravityAmount;

        dashIndicator.GetComponent<SpriteRenderer>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "HurtBox")
        {
            Debug.Log("DeathFunction");
            Death();
        }
    }

    void Death()
    {
        gc.ResetLevel();
    }

    void GetInput()
    {
        //get input
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");
        jump = Input.GetButtonDown("Jump");
    }

    //returns true and if touching a platform. Also sets parent to the platform.
    //otherwise return false and null parent.
    bool CheckPlatform()
    {
        Collider2D[] contacts = Physics2D.OverlapCircleAll(transform.position, .51f);

        foreach(Collider2D c in contacts)
        {
            if(c.gameObject.tag == "Platform")
            {               
                transform.parent = c.gameObject.transform;
                return true;
            }
        }
        transform.parent = null;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        //check if touching platform
        onPlatform = CheckPlatform();

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
        GetInput();

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

        //cap x and y speed
        if (!isDashing)
        {
            if (rb.velocity.x > maxXSpeed)
                rb.velocity = new Vector2(maxXSpeed, rb.velocity.y);
            else if (rb.velocity.x < -1 * maxXSpeed)
                rb.velocity = new Vector2(-1 * maxXSpeed, rb.velocity.y);

            if (rb.velocity.y > maxYSpeed)
                rb.velocity = new Vector2(rb.velocity.x, maxYSpeed);
            else if (rb.velocity.y < -1 * maxYSpeed)
                rb.velocity = new Vector2(rb.velocity.x, -1 * maxYSpeed);
        }

        //slow if no h input
        if(Mathf.Abs(inputH) < 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x * (1 - dragFactor), rb.velocity.y);
        }

        //move
        if (Mathf.Abs(rb.velocity.x + (inputH * accelleration * 0.01f)) < maxXSpeed)
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
                anim.SetBool("Jump", false);
                jumping = false;
                rb.gravityScale = gravityAmount;
            }
        }

        
    }

    //wait for dash released then dash in chosen direction
    IEnumerator Dash()
    {
        //setup stuff
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0f;
        float dashChoiceTimer = 0f;
        dashIndicator.GetComponent<SpriteRenderer>().enabled = true;
        dashIndicator.transform.localScale = Vector3.one * 5;

        //wait for jump to be released or timer to run out
        while (Input.GetButton("Jump") && dashChoiceTimer < dashPause)
        {
            rb.velocity = Vector2.zero;
            dashChoiceTimer += Time.deltaTime;
            dashIndicator.transform.localScale = Vector3.one * 5f * ((dashPause * 1.5f) - dashChoiceTimer) / (dashPause * 1.5f);
            yield return null;
        }

        //dash in chosen direction
        dashIndicator.GetComponent<SpriteRenderer>().enabled = false;
        GetInput();
        float dashDuration = maxDashDuration * ((dashPause - dashChoiceTimer) / (dashPause * 1.5f));
        rb.velocity = new Vector2(inputH, inputV).normalized * dashSpeed;
        dashTrail.enabled = true;
        cam.Shake(dashDuration, .1f);
        yield return new WaitForSeconds(dashDuration);
        dashTrail.enabled = false;
        //finish dashing
        rb.gravityScale = gravityAmount;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
}
