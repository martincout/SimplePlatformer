using UnityEngine;
using Cinemachine;
public class Room : MonoBehaviour
{
    private GameObject virtualCamera;
    //Don't disable the camera when we fall
    [HideInInspector] public bool diedFromVoid;

    private void Awake()
    {
        foreach (Transform eachChild in transform)
        {
            if (eachChild.GetComponent<CinemachineVirtualCamera>())
            {
                virtualCamera = eachChild.gameObject;
            }
        }
    }

    private void OnEnable()
    {
        GameEvents.RespawnHandler += SetFollow;
    }private void OnDisable()
    {
        GameEvents.RespawnHandler -= SetFollow;
    }

    private void SetFollow(GameObject playerGO)
    {
        virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = playerGO.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LevelManager.instance.UpdateCurrentRoom(gameObject);
            virtualCamera.SetActive(true);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !diedFromVoid)
        {
            virtualCamera.SetActive(false);
        }
        //set to false because we already had died by the time
        diedFromVoid = false;
    }


}
