using SimplePlatformer.Player;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuContainer;
    public Button firstSelectedButton;
    private GameObject player;
    private bool disablePlayer;


    private void Start()
    {
        
        player = FindObjectOfType<PlayerController>().gameObject;
        pauseMenuContainer = transform.GetChild(0).gameObject;
    }

    internal void SetupBehaviour()
    {
        disablePlayer = false;
    }

    private void OnEnable()
    {
        EventSystems.RespawnHandler += UpdatePlayer;
        
    }

    internal void UpdateUIMenuState(bool newState)
    {
        DisablePlayer();
        pauseMenuContainer.SetActive(newState);
        firstSelectedButton.Select();
    }

    private void OnDisable()
    {
        EventSystems.RespawnHandler -= UpdatePlayer;
    }

    private void UpdatePlayer(GameObject playerGO)
    {
        player = playerGO;
    }

    private void DisablePlayer()
    {
        disablePlayer = !disablePlayer;
        player.GetComponent<PlayerMovement>().enabled = !disablePlayer;
        player.GetComponent<PlayerCombat>().enabled = !disablePlayer;
    }

    public void QuitMenu()
    {
        SceneManager.LoadScene(0);
    }
}
