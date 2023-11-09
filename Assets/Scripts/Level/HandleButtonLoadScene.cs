using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleButtonLoadScene : MonoBehaviour
{
    [SerializeField] private int nextIdScene;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        HandleLoadingScene.Instance.LoadingSceneAndSetNextId(nextIdScene);
    }

}
