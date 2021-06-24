using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public string scene;
	public void LoadScene(string sceneName) {
		SceneManager.LoadScene( sceneName );
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadScene(scene);
        }
    }

}
