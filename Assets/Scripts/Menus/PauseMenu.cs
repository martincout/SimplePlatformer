using SimplePlatformer.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuGO;
    private GameObject player;
    private bool disablePlayer;

    private void Start()
    {
        disablePlayer = false;
        player = FindObjectOfType<PlayerBase>().gameObject;
    }

    private void OnEnable()
    {
        EventSystem.RespawnHandler += UpdatePlayer;
    }
    
    private void OnDisable()
    {
        EventSystem.RespawnHandler -= UpdatePlayer;
    }

    private void UpdatePlayer()
    {
        player = FindObjectOfType<PlayerBase>().gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                
                showPaused();
            }
            else if (Time.timeScale == 0)
            {
                
                unPause();
            }
        }
    }

    public void showPaused()
    {
        DisablePlayer();
        Time.timeScale = 0;
        pauseMenuGO.SetActive(true);
    }

    private void DisablePlayer()
    {
        disablePlayer = !disablePlayer;
        player.GetComponent<PlayerMovement>().enabled = !disablePlayer;
        player.GetComponent<PlayerCombat>().enabled = !disablePlayer;
    }

    public void unPause()
    {
        DisablePlayer();
        Time.timeScale = 1;
        pauseMenuGO.SetActive(false);
    }

    public void QuitMenu()
    {
        SceneManager.LoadScene(0);
    }
}
