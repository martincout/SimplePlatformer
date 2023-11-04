using SimplePlatformer.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button firstSelectedButton;
    private GameObject player;
    private bool disablePlayer;
    private CanvasGroup Container;
    public CanvasGroup Menu;
    public CanvasGroup Settings;


    private void Start()
    {

        player = FindObjectOfType<PlayerController>().gameObject;
        Container = GetComponent<CanvasGroup>();
    }

    internal void SetupBehaviour()
    {
        disablePlayer = false;
    }

    private void OnEnable()
    {
        GameEvents.RespawnHandler += UpdatePlayer;

    }

    internal void UpdateUIMenuState(bool newState)
    {
        DisablePlayer();
        switch (newState)
        {
            case true:
                SetActive(Container);
                SetActive(Menu);
                SetButtonsInteractable(true);
                firstSelectedButton.Select();
                break;
            case false:
                SetInactive(Container);
                SetInactive(Menu);
                SetButtonsInteractable(false);
                SetInactive(Settings);
                break;
        }
    }

    private void OnDisable()
    {
        GameEvents.RespawnHandler -= UpdatePlayer;
    }

    private void UpdatePlayer(GameObject playerGO)
    {
        player = playerGO;
    }

    public void SetActive(CanvasGroup cg)
    {
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    public void SetInactive(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Transform t in Menu.transform)
        {
            if (t.GetComponent<Button>())
            {
                t.GetComponent<Button>().interactable = interactable;
            }
        }
        foreach (Transform t in Settings.transform)
        {
            if (t.GetComponent<Button>())
            {
                t.GetComponent<Button>().interactable = interactable;
            }
        }
    }

    private void DisablePlayer()
    {
        disablePlayer = !disablePlayer;
        //player.GetComponent<PlayerMovement>().enabled = !disablePlayer;
        //player.GetComponent<PlayerCombat>().enabled = !disablePlayer;
    }

}
