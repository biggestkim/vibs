using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    int targetPointIndex;
    public List<GameObject> travelPoints;


    public float travelSpeed;
    public bool loop;

    bool active = false;
    public GameObject activator;

    // Start is called before the first frame update
    void Start()
    {
        targetPointIndex = 0;

        //remove travel points from this so that they dont move with platform
        foreach(GameObject g in travelPoints)
        {
            //g.z = 0;
            g.transform.parent = null;
        }

        //always on if there's no switch
        if (activator == null)
            active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            if (activator.GetComponent<Activator>().active)
            {
                active = true;
            }
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, travelPoints[targetPointIndex].transform.position, travelSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, travelPoints[targetPointIndex].transform.position) < travelSpeed * Time.deltaTime)
        {
            //next target point
            if (targetPointIndex == travelPoints.Count - 1)
            {
                if (loop)
                    targetPointIndex = 0;
            }
            else
                targetPointIndex++;
        }

        
    }
}
