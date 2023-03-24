using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];

        ResetLevel();
    }

    void ResetLevel()
    {
        GameObject playerStart = GameObject.FindGameObjectsWithTag("PlayerStart")[0];
        player.transform.position = playerStart.transform.position;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
