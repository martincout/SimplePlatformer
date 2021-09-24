using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Has a copy of the Game Data saved through scenes.
/// </summary>
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

    public void LoadGameData()
    {
        LocalCopyOfData = SaveSystem.LoadGame();
        if (LocalCopyOfData == null) return;
        IsSceneBeingLoaded = true;
    }

}

