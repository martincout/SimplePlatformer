using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightFireballController : MonoBehaviour
{
    private Light2D light;
    private float maxIntensity = 4.7f;
    private float step = 5;
    private float destroyAfter = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (light.intensity < maxIntensity)
        {
            light.intensity += step * Time.deltaTime;
        }else if(light.intensity >= maxIntensity){
            Destroy(this.gameObject, destroyAfter);
        }
    }


}
