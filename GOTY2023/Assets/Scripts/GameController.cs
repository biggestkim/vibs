using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //singleton pattern
        //ensures that there's only ever one gamecontroller active
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("GameController"))
        {
            if (g != this.gameObject)
            {
                Destroy(gameObject);
                return;
            }     
        }
        //keep this between scenes
        DontDestroyOnLoad(gameObject);

        //find player
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    //load and reset new level
    public void SetLevel(string newID)
    {
        SceneManager.LoadScene(newID);    
    }

    public void ResetLevel()
    {
        SetLevel(SceneManager.GetActiveScene().name);
        //find player
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
