using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shake(float shakeTime, float shakeStrength)
    {
        StartCoroutine(Shaking(shakeTime, shakeStrength));
    }

    public IEnumerator Shaking(float shakeTime, float shakeStrength)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = transform.localPosition;

        while (timeElapsed < shakeTime)
        {
            timeElapsed += Time.deltaTime;
            transform.localPosition = new Vector3(startPosition.x + Random.Range(-1,1)*shakeStrength, startPosition.y + Random.Range(-1, 1) * shakeStrength, startPosition.z);
            yield return 0;
        }
        transform.localPosition = startPosition;
    }
}
