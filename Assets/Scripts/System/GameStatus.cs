using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStatus : MonoBehaviour
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

    static GameStatus instance;

    public static GameStatus GetInstance()
    {
        return instance;
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
        GameObject.DontDestroyOnLoad(this.gameObject);  // Become immortal

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
}
