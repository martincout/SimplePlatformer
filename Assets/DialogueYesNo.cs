using SimplePlatformer.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueYesNo : MonoBehaviour
{
    public Button firstSelected;
    private bool activated = true;
    [SerializeField] private PlayerController player;
    public int cost = 100;
    //Input
    public PlayerActions playerInput;
    private InputAction cancel;
    [SerializeField] private GameObject dontHaveEnoughGO;
    private void Awake()
    {
        playerInput = new PlayerActions();
        GameEvents.RespawnHandler += UpdatePlayer;
    }

    private void OnEnable()
    {
        cancel = playerInput.UI.Cancel;
        cancel.Enable();
        playerInput.UI.Cancel.performed += No;

        player.DisablePlayerState(true);
        //Interctable false because Select doesn't work the second time
        firstSelected.interactable = false;
        firstSelected.interactable = true;
        StartCoroutine(Animation());
    }

    private void UpdatePlayer(GameObject obj)
    {
        player = obj.GetComponent<PlayerController>();
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
        cancel.Disable();
        playerInput.UI.Cancel.performed -= No;
        transform.localScale = Vector2.zero;
    }

    public void Yes()
    {
        activated = false;
        if (!activated)
        {
            if (GameManager.GetInstance().GetScore() >= cost)
            {
                player.playerCombatBehaviour.hasBow = true;
                GameManager.GetInstance().AddScore(-cost);
            }
            else
            {
                dontHaveEnoughGO.SetActive(true);
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


    public void No(InputAction.CallbackContext obj)
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
