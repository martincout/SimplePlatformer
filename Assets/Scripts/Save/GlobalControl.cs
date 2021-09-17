using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalControl : MonoBehaviour
{
    internal static GlobalControl Instance;
    public GameData LocalCopyOfData;
    public bool IsSceneBeingLoaded = false;

    private void Start()
    {
        Time.timeScale = 1f;
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }

    //------
    //Loading
    //------

    public void LoadGame()
    {
        LocalCopyOfData = SaveSystem.LoadGame();
        IsSceneBeingLoaded = true;
        SceneManager.LoadScene(1);
    }

}

