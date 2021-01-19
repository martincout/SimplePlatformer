using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform currentRespawnPoint;
    public GameObject playerPrefab;
    public GameObject deathScreen;
    public GameObject enemyContainer;
    public List<Vector3> enemiesPositions;
    public GameObject enemyPrefab;

    internal bool isPlayerDead;

    public CinemachineVirtualCameraBase virtualCamera;

    private void Awake()
    {
        foreach (Transform child in enemyContainer.transform)
        {
            enemiesPositions.Add(child.transform.position);
        }
        
    }

    private void Start()
    {
        instance = this;
        currentRespawnPoint = RespawnManager.currentRespawn.transform;
        isPlayerDead = false;
    }

    private void OnEnable()
    {
        EventSystem.NewSpawnHandler += UpdateNewSpawn;
        EventSystem.DeathHandler += PlayerDeath;
    }

    private void OnDisable()
    {
        EventSystem.NewSpawnHandler -= UpdateNewSpawn;
        EventSystem.DeathHandler -= PlayerDeath;
    }

    private void PlayerDeath()
    {
        isPlayerDead = true;
    }

    private void PlayerAlive()
    {
        isPlayerDead = false;
    }

    public void Respawn()
    {
        EventSystem.RespawnEnemiesHandler();
        foreach(Vector3 pos in enemiesPositions)
        {
            Instantiate(enemyPrefab,pos,Quaternion.identity,enemyContainer.transform);
        }
        PlayerAlive();
        deathScreen.SetActive(false);
        GameObject player = Instantiate(playerPrefab, currentRespawnPoint.position, Quaternion.identity);
        virtualCamera.Follow = player.transform;
    } 

    private void UpdateNewSpawn()
    {
        currentRespawnPoint = RespawnManager.currentRespawn.transform;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
