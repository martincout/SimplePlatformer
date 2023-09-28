using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFireballController : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D light;
    private float targetIntensity = 4.7f;
    private float speed = 10;
    private float destroyAfter = 1f;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        light.intensity = 0f;
        StartCoroutine(StartFadeIn());
        Destroy(this.gameObject, destroyAfter);
    }

    public IEnumerator StartFadeIn()
    {
        float currentTime = 0;
        float currentIntensity = 0;
        float duration = 0.6f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(currentIntensity, targetIntensity, currentTime / duration);
            light.intensity = newIntensity;
            yield return null;
        }
        currentTime = 0f;
        duration = 0.2f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newIntensity = Mathf.Lerp(targetIntensity, 0, currentTime / duration);
            light.intensity = newIntensity;
            yield return null;
        }
        yield break;
    }


}
