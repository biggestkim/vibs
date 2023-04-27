using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.gameObject.tag == "Player")
        {
            active = true;
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }
}
