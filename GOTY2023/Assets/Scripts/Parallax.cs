using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    public float parallaxFactor = .2f; //0 to stay still, 1 to fully follow cam
    public Vector2 startPos;
    GameObject cam;

    Vector2 moveAmount = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectsWithTag("MainCamera")[0];
        startPos = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        moveAmount = new Vector2(cam.transform.position.x, cam.transform.position.y) * parallaxFactor;
        transform.position = new Vector3(startPos.x + moveAmount.x, startPos.y + moveAmount.y, transform.position.z);
    }
}
