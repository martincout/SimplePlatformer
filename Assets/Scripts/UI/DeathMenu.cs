using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathScreenGO;
    public void DisplayDeathMenu()
    {
        StartCoroutine(
                FadeIn());
    }

    internal void SetupBehaviour()
    {
        //throw new NotImplementedException();
    }

    private IEnumerator FadeIn()
    {
        CanvasGroup canvasGroup = deathScreenGO.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        float duration = 1f;
        LeanTween.alphaCanvas(canvasGroup, 1.0f, duration);
        yield return new WaitForSeconds(duration);
        canvasGroup.interactable = true;
    }

    private IEnumerator FadeOut()
    {
        CanvasGroup canvasGroup = deathScreenGO.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        float duration = .3f;
        LeanTween.alphaCanvas(canvasGroup, 0f, duration);
        yield return new WaitForSeconds(duration);
        canvasGroup.interactable = false;
    }

    internal void UpdateUIMenuState(bool newState)
    {
        if (newState == true)
        {
            DisplayDeathMenu();
        }
        else 
        {
            StartCoroutine(
                FadeOut());
        }
    }
}
