using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleLoadingScene : MonoBehaviour
{
    public static HandleLoadingScene Instance { get; private set; }
    private int idScene;

    // Start is called before the first frame update
    void Start()
    {
        idScene = 0;
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
    }

    public void SetIdScene(int id)
    {
        this.idScene = id;
    }

    public int GetIdScene()
    {
        return idScene;
    }
    
}
