using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuGm;
    public GameObject deathScreenGO;
    public void DisplayDeathMenu()
    {
        StartCoroutine(
                Fade());
    }

    internal void SetupBehaviour()
    {
        //throw new NotImplementedException();
    }

    private IEnumerator Fade()
    {
        CanvasGroup canvasGroup = deathScreenGO.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        float duration = 1f;
        LeanTween.alphaCanvas(canvasGroup, 1.0f, duration);
        yield return new WaitForSeconds(duration);
        canvasGroup.interactable = true;
    }

    internal void UpdateUIMenuState(bool newState)
    {
        throw new NotImplementedException();
    }
}
