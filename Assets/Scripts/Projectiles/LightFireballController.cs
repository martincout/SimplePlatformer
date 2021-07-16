using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFireballController : MonoBehaviour
{
    private Light2D light;
    private float maxIntensity = 4.7f;
    private float speed = 10;
    private float destroyAfter = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        light.intensity = 0f;
        Destroy(this.gameObject, destroyAfter);
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity = Mathf.PingPong(Time.time * speed, maxIntensity);
        //light.intensity += step * Time.deltaTime;
        //light.intensity = magnitude + Mathf.Sin(Time.timeSinceLevelLoad * frequency) * magnitude;

        //light.intensity = Mathf.Lerp(0,4.7f,t);
        //Debug.Log(Mathf.Lerp(0, 4.7f, t));
        //if(light.intensity >= 4.7f)
        //{
        //    Destroy(gameObject,destroyAfter);
        //}
    }


}
