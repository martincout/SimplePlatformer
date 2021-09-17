using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadingGif : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(1f);
        this.gameObject.SetActive(false);
    }

}
