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
    public GameObject enemyContainer;
    public GameObject bossContainer;
    //Enemies positions and prefabs
    private List<RespawnEntityData> enemies;
    private List<RespawnEntityData> bosses;
    public Room currentRoom;
    public CanvasGroup bossHealthbar;

    public CinemachineVirtualCameraBase virtualCamera;

    private void OnEnable()
    {
        EventSystems.NewSpawnHandler += UpdateNewSpawn;
        EventSystems.OnPlayerDeath += FadeBossHealthBar;
    }

    private void OnDisable()
    {

        EventSystems.NewSpawnHandler -= UpdateNewSpawn;
        EventSystems.OnPlayerDeath -= FadeBossHealthBar;
    }

    public void UpdateCurrentRoom(GameObject room)
    {
        currentRoom = room.GetComponent<Room>();
    }

    private void Start()
    {
        //Gets enemies
        enemies = new List<RespawnEntityData>();
        foreach (Transform child in enemyContainer.transform)
        {
            enemies.Add(new RespawnEntityData(child.GetComponent<Enemy>(),child.position));
        }
        //Gets bosses
        bosses = new List<RespawnEntityData>();
        foreach(Transform child in bossContainer.transform)
        {
            bosses.Add(new RespawnEntityData(child.GetComponent<BossBehaviour>(), child.position));
        }
        //Instance
        instance = this;
        //Current spawnpoint
        currentRespawnPoint = RespawnManager.currentRespawn.transform;
    }
   

    public void Respawn()
    {
        //Set an event to respawn all enemies because every enemy handles his own destruction
        EventSystems.RespawnEnemiesHandler();
        foreach (RespawnEntityData ed in enemies)
        {
            GameObject instance = Instantiate(ed.enemy._enemyData.prefab, ed.position, Quaternion.identity, enemyContainer.transform);
            instance.GetComponent<Enemy>().dropItem = ed.enemy.dropItem;
            instance.GetComponent<Enemy>().dropChance = ed.enemy.dropChance;
            instance.GetComponent<Enemy>().patrollingEnabled = ed.enemy.patrollingEnabled;
        }
        foreach(RespawnEntityData bs in bosses)
        {
            GameObject instance = Instantiate(bs.boss._bossData.prefab, bs.position, Quaternion.identity, bossContainer.transform);
        }
        //PlayerAlive();
        GameObject player = Instantiate(playerPrefab, currentRespawnPoint.position, Quaternion.identity);
        //Set the follow to the actual virtual camera (don't know why I did this)
        virtualCamera.Follow = player.transform;
        //Set the follow to all virtual cameras with the respawn Event
        EventSystems.RespawnHandler?.Invoke(player);

    }

    public void FadeBossHealthBar()
    {
        float duration = 1f;
        LeanTween.alphaCanvas(bossHealthbar, 0f, duration);
    }

    private void UpdateNewSpawn()
    {
        currentRespawnPoint = RespawnManager.currentRespawn.transform;
    }

    
}
