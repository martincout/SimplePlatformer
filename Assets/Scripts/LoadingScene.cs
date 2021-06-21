using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public Slider _progressBar;
    AsyncOperation gameLevel;
    private void Start()
    {
        StartCoroutine(LoadAsyncOperations());
    }

    private void Update()
    {
        _progressBar.value = gameLevel.progress;
    }

    IEnumerator LoadAsyncOperations()
    {
        gameLevel = SceneManager.LoadSceneAsync(2);
        if (gameLevel.progress < 1)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
