using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float delay = 8f;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(canvasGroup, 0f, 3f).setDelay(delay);
    }
}
