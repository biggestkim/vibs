using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public string nextLevelID = "oops";
    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        //find the game controller
        gc = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //tell game controller to switch the level
        if(col.transform.tag == "Player")
        {
            gc.SetLevel(nextLevelID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
