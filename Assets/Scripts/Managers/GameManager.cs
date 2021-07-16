using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using SimplePlatformer.Player;

public class GameManager : MonoBehaviour
{

    // There are 3 major ways to persist this data between scene changes.
    //  1) Save the info into something persist (PlayerPrefs, a save file)
    //			- This preserves data even between game executions, not just scene changes
    //		** This is a very, very minimalist implentation. Has the benefit of
    //			true persistence. Also, you don't need a single, central class to store
    //			data in this way. Each object could save/load it's own info.
    //
    //  2) Static class data.  Very simple. Occasionally leads to weirdness from
    //		inside the Unity Editor, but not actually breaking things. You could
    //		do really messy "global" style variable with this, but you can also
    //		do really nice encapsulation as well.
    //			** This is the ideal implementation if your persist data
    //				is just that:  Data. It doesn't DO anything. It can
    //				be implemented as a purely static class.
    //
    //  3) DontDestroyOnLoad -- This flags a GameObject such that when we change
    //		from one scene to another, it doesn't get destroyed. (i.e. it is still
    //		present in the newly loaded scene. To use this to the best possible
    //		advantage, you need to use the Unity Singleton Design Pattern.
    //			** This is the ideal implementatino if this class needs to DO
    //			things, not just store things. In other words, maybe you need
    //			it to have an Update function that modifies the game data regularly
    //			and will need to do so on every scene.
    //

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
        // "Implementation #1"
        // Load data from PlayerPrefs -- this might be from the
        // previous scene, or maybe even from the previous execution (i.e. saved
        // between quitting and reloading the game)
        //score = PlayerPrefs.GetInt("score", 0);
        //numLives = PlayerPrefs.GetInt("lives", 3);

        // We want to be a Singleton (i.e. there should only ever be
        // one GameStatus instance at any given time.)


        if (instance != null)
        {
            // Someone ELSE is the singleton already.
            // So let's just destroy ourselves before we cause trouble.
            Destroy(this.gameObject);
            return;
        }

        // If we get here, the we are "the one". Let's act like it.
        instance = this;    // We are a Highlander
        DontDestroyOnLoad(this.gameObject);  // Become immortal

        InitializeKeys();

    }
    

    public void TogglePauseState()
    {
        isPaused = !isPaused;
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
