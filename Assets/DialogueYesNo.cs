using SimplePlatformer.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueYesNo : MonoBehaviour
{
    public Button firstSelected;
    private bool activated = true;
    public PlayerController player;
    public int cost = 50;
    private void OnEnable()
    {
        player.DisablePlayerState(true);
        //Interctable false because Select doesn't work the second time
        firstSelected.interactable = false;
        firstSelected.interactable = true;

        StartCoroutine(Animation());
        
        
    }

    private IEnumerator Animation()
    {
        float sec = 0.2f;
        LeanTween.scale(gameObject, Vector2.one, sec).setEaseLinear();
        yield return new WaitForSeconds(sec);
        //Select
        firstSelected.Select();

    }

    private void OnDisable()
    {
        transform.localScale = Vector2.zero;
    }

    public void Yes()
    {
        activated = false;
        if (!activated)
        {
            if (GameManager.GetInstance().GetScore() >= 50)
            {
                player.playerCombatBehaviour.hasBow = true;
                GameManager.GetInstance().AddScore(-50);
            }
            player.DisablePlayerState(false);
            player.EnableGameplayControls();
            gameObject.SetActive(false);
        }
    }

    public void No()
    {

        activated = false;
        if (!activated)
        {
            player.DisablePlayerState(false);
            player.EnableGameplayControls();
            gameObject.SetActive(false);
        }
    }
}
