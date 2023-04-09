using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//patrols left and right until it finds the player, then executes a punch
//position on left side of patrol path and specify patrol distance to suit.

public class Carrot : MonoBehaviour
{
    Rigidbody2D rb;
    public Collider2D punchCollider;
    public Collider2D hitBox;
    public Collider2D hurtBox;
    public Animator anim;
    public GameObject player;

    public float moveSpeed = 2f;
    public float punchDist = 1f;
    public float punchResetDelay = 1f;
    public float punchDashSpeed = 15f;
    float punchResetTimer = 0;
    public float patrolDist = 5f;

    Vector2 initialPos;
    string prevState;
    public bool moveRight = true;

    string state = "patrolR";

    // Start is called before the first frame update
    void Start()
    {
        hitBox = gameObject.GetComponent<Collider2D>();
        state = "patrolR";
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        anim = transform.GetComponent<Animator>();
        initialPos = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {

        CheckForDeath();

        switch (state)
        {
            //walk to the right
            case "patrolR":
                transform.localScale = new Vector3(-1f, 1f, 1f);
                rb.velocity = new Vector2(moveSpeed, 0);
                if(transform.position.x >= initialPos.x + patrolDist)
                {
                    state = "patrolL";
                }

                CheckForPunch();
                break;
            //walk to the left
            case "patrolL":
                transform.localScale = new Vector3(1f, 1f, 1f);
                rb.velocity = new Vector2(-1*moveSpeed, 0);
                if (transform.position.x <= initialPos.x)
                {
                    state = "patrolR";
                }
                CheckForPunch();
                break;
            //punch and recover
            case "punch":
                //dash fwd while punching
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CarrotPunch")
                    rb.velocity = new Vector2(-1 * transform.localScale.x * punchDashSpeed, 0f);
                else
                    rb.velocity = Vector2.zero;

                //go back to patrolling when done
                punchResetTimer += Time.deltaTime;
                if(punchResetTimer > punchResetDelay)
                {
                    state = prevState;
                }
                break;
            case "dead":
                if(transform.position.y > initialPos.y - 1.5f)
                {
                    break;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    rb.gravityScale = 0f;
                    transform.position = new Vector2(transform.position.x, initialPos.y - 1.5f);
                    this.enabled = false;
                }
                break;
        }
    }

    //check if the player is in punching range
    void CheckForPunch()
    {
        if (punchCollider.IsTouching(player.GetComponent<Collider2D>()))
        {
            prevState = state;
            state = "punch";
            anim.SetTrigger("Punch");
            punchResetTimer = 0;
        }
    }

    //if player bounces on head, kill this carrot
    void CheckForDeath()
    {
        if (hitBox.IsTouching(player.GetComponent<Collider2D>()))
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            rb.gravityScale = 2f;
            hitBox.enabled = false;
            hurtBox.enabled = false;
            transform.GetComponent<Collider2D>().isTrigger = true;
            punchCollider.enabled = false;
            //rb.velocity = Vector2.zero;
            state = "dead";
            anim.SetTrigger("Dead");
        }
    }

}

