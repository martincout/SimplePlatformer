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
    private List<RespawnEntityData> enemies;
    public Room currentRoom;

    internal bool isPlayerDead;

    public CinemachineVirtualCameraBase virtualCamera;

    public void UpdateCurrentRoom(GameObject room)
    {
        currentRoom = room.GetComponent<Room>();
    }

    private void Start()
    {
        enemies = new List<RespawnEntityData>();
        foreach (Transform child in enemyContainer.transform)
        {
            enemies.Add(new RespawnEntityData(child.GetComponent<Enemy>(),child.position));
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
        foreach (RespawnEntityData ed in enemies)
        {
            GameObject instance = Instantiate(ed.enemy._enemyData.prefab, ed.position, Quaternion.identity, enemyContainer.transform);
            instance.GetComponent<Enemy>().dropItem = ed.enemy.dropItem;
            instance.GetComponent<Enemy>().dropChance = ed.enemy.dropChance;
            instance.GetComponent<Enemy>().patrollingEnabled = ed.enemy.patrollingEnabled;
        }
        PlayerAlive();
        GameObject player = Instantiate(playerPrefab, currentRespawnPoint.position, Quaternion.identity);
        //Set the follow to the actual virtual camera (don't know why I did this)
        virtualCamera.Follow = player.transform;
        //Set the follow to all virtual cameras with the respawn Event
        EventSystem.RespawnHandler(player);
        //Load Screen TODO
        deathScreen.GetComponent<CanvasGroup>().alpha = 0f;
        deathScreen.GetComponent<CanvasGroup>().interactable = false;

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
