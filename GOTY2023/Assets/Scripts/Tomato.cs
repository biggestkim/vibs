using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D wallDetector;
    Animator anim;

    public float moveSpeed = 3f;
    public float resetTimer = .5f;
    public string state = "patrolR";


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = "patrolR";
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            //walk to the right
            case "patrolR":
                transform.localScale = new Vector3(-1f, 1f, 1f);
                rb.velocity = new Vector2(moveSpeed, 0);

                break;
            //walk to the left
            case "patrolL":
                transform.localScale = new Vector3(1f, 1f, 1f);
                rb.velocity = new Vector2(-1 * moveSpeed, 0);

                break;
            
            case "dead":

                break;
        }
    }

    //check for wall
    void OnTriggerEnter2D()
    {
        if(state == "patrolL")
        {
            state = "patrolR";
        }
        else if (state == "patrolR")
        {
            state = "patrolL";
        }
    }


}
