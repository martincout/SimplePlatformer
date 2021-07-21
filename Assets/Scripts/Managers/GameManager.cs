using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using SimplePlatformer.Player;

public class GameManager : MonoBehaviour
{
    protected int score = 0;
    public bool isPaused = false;
    public bool playerDeath = false;
    public PlayerController player;

    /// <summary>
    /// Type of key and quantity
    /// </summary>
    protected Dictionary<KeyColor,int> keys;

    //Instance
    static GameManager instance;

    public static GameManager GetInstance()
    {
        return instance;
    }

    public Dictionary<KeyColor,int> GetKeys()
    {
        return keys;
    }

    // Use this for initialization
    void Start()
    {
        instance = this;    

        InitializeKeys();

    }
    //------
    //Save
    //------

    public void SaveGame()
    {
        SaveSystem.SaveGame(player, score);
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();

        score = data.score;
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        player.transform.position = position;
        player.playerCombatBehaviour.hasBow = data.hasBow;
        player.healthSystem.SetHealth(data.health);
    }

    public void TogglePauseState()
    {
        isPaused = !isPaused;
        player.DisablePlayerState(isPaused);
        ToggleTimeScale();
        SwitchFocusedPlayerControlScheme();
        UpdateUIMenu();

    }

    void SwitchFocusedPlayerControlScheme()
    {
        switch (isPaused)
        {
            case true:
                //player.EnablePauseMenuControls();
                break;

            case false:
                //player.EnableGameplayControls();
                break;
        }
    }

    private void UpdateUIMenu()
    {
        UIManager.GetInstance().UpdateUIMenuState(isPaused);
    }

    private void UpdateUIDeath()
    {
        UIManager.GetInstance().UpdateUIDeathState(playerDeath);
    }


    public void TogglePlayerDeath(bool death)
    {
        if(death == true)
        {
            playerDeath = true;
            UpdateUIDeath();
        }
        else
        {
            playerDeath = false;
            UpdateUIDeath();
        }
    }

    /// <summary>
    /// Initialize the keys Dictionary.
    /// Adds every KeyColor into the list and set it to zero (amount of keys)
    /// </summary>
    private void InitializeKeys()
    {
        if (keys == null)
        {
            keys = new Dictionary<KeyColor, int>();
        }

        foreach (KeyColor kc in (KeyColor[]) Enum.GetValues(typeof(KeyColor)))
        {
            keys.Add(kc,0);
        }
    }

    /// <summary>
    /// Increments the amount of keys in the color passed
    /// </summary>
    /// <param name="kc"></param>
    public void AddKey(KeyColor kc)
    {
        if (keys == null)
        {
            keys = new Dictionary<KeyColor, int>();
        }

        if (keys.ContainsKey(kc))
        {
            keys[kc] += 1;
        }
        EventSystems.UpdateKeysHandler();
        
    }

    /// <summary>
    /// Decreases the amount of keys of that Color by 1.
    /// </summary>
    /// <param name="kc"></param>
    /// <returns bool ></returns> Returns true if not zero
    public bool DeleteKey(KeyColor kc)
    {
        if (keys.ContainsKey(kc) && keys[kc] > 0)
        {
            keys[kc] -= 1;
            EventSystems.UpdateKeysHandler();
            return true;
        }
        else
        {
            return false;
        }
        
    }

    private void OnEnable() => EventSystems.RespawnHandler += UpdatePlayer;

    private void OnDisable() => EventSystems.RespawnHandler -= UpdatePlayer;


    public void UpdatePlayer(GameObject p_player)
    {
        player = p_player.GetComponent<PlayerController>();
    }

    void OnDestroy()
    {
        //Debug.Log("GameStatus was destroyed.");

        // Before we get destroyed, let's save our data to our save file.
        // This is "Implementation #1".
        // This will happen whenever this object is destroyed, which
        // includes scene changes as well as simply exiting the program.
        //PlayerPrefs.SetInt("score", score);
        //PlayerPrefs.SetInt("lives", numLives);
    }

    public void AddScore(int s)
    {
        score += s;
        // We could take this opportunity to save the score in a file/UserPrefs
        // But depending on implentation, this could be slow/inneficient.
        // On the other hand, if we are constantly saving the data on 
        // the fly, it means that we pretty never lose anything to a crash.
    }

    public int GetScore()
    {
        return score;
    }

    //Pause Utilities ----

    void ToggleTimeScale()
    {
        float newTimeScale = 0f;

        switch (isPaused)
        {
            case true:
                newTimeScale = 0f;
                break;

            case false:
                newTimeScale = 1f;
                break;
        }

        Time.timeScale = newTimeScale;
    }


}
