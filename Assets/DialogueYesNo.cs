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
        player.playerInteractableBehaviour.enabled = false;
        player.playerCombatBehaviour.enabled = false;
        player.playerMovementBehaviour.enabled = false;
        //Interctable false because Select doesn't work the second time
        firstSelected.interactable = false;
        firstSelected.interactable = true;
        //Select
        firstSelected.Select();
    }

    public void Yes()
    {
        activated = false;
        if (!activated)
        {
            player.playerInteractableBehaviour.enabled = true;
            player.playerCombatBehaviour.enabled = true;
            player.playerMovementBehaviour.enabled = true;
            player.EnableGameplayControls();
            gameObject.SetActive(false);
        }
    }

    public void No()
    {

        activated = false;
        if (!activated)
        {
            player.playerInteractableBehaviour.enabled = true;
            player.playerCombatBehaviour.enabled = true;
            player.playerMovementBehaviour.enabled = true;
            player.EnableGameplayControls();
            gameObject.SetActive(false);
        }
    }
}
