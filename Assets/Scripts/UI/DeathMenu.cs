using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathScreenGO;
    public Button firstSelected;
    private CanvasGroup canvasGroup;

    private void Start()
    {
         canvasGroup = deathScreenGO.GetComponent<CanvasGroup>();
    }

    public void DisplayDeathMenu()
    {
        StartCoroutine(
                FadeIn());
        float duration = 1f;
        LeanTween.alphaCanvas(canvasGroup, 1.0f, duration);
    }


    internal void SetupBehaviour()
    {
        //throw new NotImplementedException();
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1f);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        firstSelected.Select();
    }

    private IEnumerator FadeOut()
    {
        float duration = .3f;
        LeanTween.alphaCanvas(canvasGroup, 0f, duration);
        yield return new WaitForSeconds(duration);
    }

    internal void UpdateUIMenuState(bool newState)
    {
        if (newState == true)
        {
            DisplayDeathMenu();
        }
        else 
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            StartCoroutine(
                FadeOut());
        }
    }
}
