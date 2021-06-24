using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Instance
    //Instance
    static UIManager instance;

    public static UIManager GetInstance()
    {
        return instance;
    }

    // Use this for initialization
    void Start()
    {
        if (instance != null)
        {
            // Someone ELSE is the singleton already.
            // So let's just destroy ourselves before we cause trouble.
            Destroy(this.gameObject);
            return;
        }
        // If we get here, the we are "the one". Let's act like it.
        instance = this;    // We are a Highlander
    }
    #endregion

    [Header("Sub-Behaviours")]
    public PauseMenu pauseMenu;
    public DeathMenu deathMenu;

    public void SetupManager()
    {
        pauseMenu.SetupBehaviour();
        deathMenu.SetupBehaviour();
    }

    public void UpdateUIMenuState(bool newState)
    {
        if(!GameManager.GetInstance().playerDeath) pauseMenu.UpdateUIMenuState(newState);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    internal void UpdateUIDeathState(bool newState)
    {
        deathMenu.UpdateUIMenuState(newState);
    }
}