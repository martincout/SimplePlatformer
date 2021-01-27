using UnityEngine;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuGm;
    public GameObject blackScreen;
    public GameObject[] UIElements = new GameObject[3];
    void OnEnable() => EventSystem.DeathHandler += DisplayMenu;

    void OnDisable() => EventSystem.DeathHandler -= DisplayMenu;
    public void DisplayMenu()
    {
        deathMenuGm.SetActive(true);
        LeanTween.alpha(blackScreen.GetComponent<Image>().rectTransform, 1f, 1f).setEase(LeanTweenType.linear);
        

    }
}
