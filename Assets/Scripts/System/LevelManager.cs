using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using SimplePlatformer.Enemy;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform currentRespawnPoint;
    public GameObject playerPrefab;
    public GameObject deathScreen;
    public GameObject enemyContainer;
    //Enemies positions and prefabs
    private Dictionary<Vector3, GameObject> enemies;

    internal bool isPlayerDead;

    public CinemachineVirtualCameraBase virtualCamera;

    private void Start()
    {
        enemies = new Dictionary<Vector3, GameObject>();
        foreach (Transform child in enemyContainer.transform)
        {
            enemies.Add(child.transform.position, child.GetComponent<Enemy>()._enemyData.prefab);
        }
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
        //Set an event to respawn all enemies because every enemy handles his own destruction
        EventSystem.RespawnEnemiesHandler();
        foreach (KeyValuePair<Vector3, GameObject> e in enemies)
        {
            Instantiate(e.Value, e.Key, Quaternion.identity, enemyContainer.transform);
        }
        PlayerAlive();
        GameObject player = Instantiate(playerPrefab, currentRespawnPoint.position, Quaternion.identity);
        //Set the follow to the actual virtual camera (don't know why I did this)
        virtualCamera.Follow = player.transform;
        //Set the follow to all virtual cameras with the respawn Event
        EventSystem.RespawnHandler(player);
        //Set active here to not show transitions
        deathScreen.SetActive(false);
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
