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

        HomePlayer();
    }

    //reload current level and send player to start position
    void HomePlayer()
    {
        GameObject playerStart = GameObject.FindGameObjectsWithTag("PlayerStart")[0];
        player.transform.position = playerStart.transform.position;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    //load and reset new level
    public void SetLevel(string newID)
    {
        SceneManager.LoadScene(newID);
        StartCoroutine(HomePlayerPause(.01f));       
    }

    public void ResetLevel()
    {
        SetLevel(SceneManager.GetActiveScene().name);
    }

    IEnumerator HomePlayerPause(float t)
    {

        //Wait for 4 seconds
        yield return new WaitForSeconds(t);

        HomePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
