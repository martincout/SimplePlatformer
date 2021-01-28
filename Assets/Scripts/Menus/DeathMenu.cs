using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuGm;
    public GameObject deathScreenGO;
    void OnEnable() => EventSystem.DeathHandler += DisplayDeathMenu;

    void OnDisable() => EventSystem.DeathHandler -= DisplayDeathMenu;
    public void DisplayDeathMenu()
    {
        StartCoroutine(
                Fade());
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
}
