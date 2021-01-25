using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using SimplePlatformer.Player;
public class Room : MonoBehaviour
{
    private GameObject virtualCamera;
    private bool diedFromVoid = false;

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
        EventSystem.RespawnHandler += SetFollow;
        EventSystem.SetDeathFromVoid += SetDeathFromVoid;
    }private void OnDisable()
    {
        EventSystem.RespawnHandler -= SetFollow;
        EventSystem.SetDeathFromVoid -= SetDeathFromVoid;
    }

    private void SetFollow(GameObject playerGO)
    {
        virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = playerGO.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            virtualCamera.SetActive(true);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !diedFromVoid)
        {
            virtualCamera.SetActive(false);

        }
    }

    private void SetDeathFromVoid()
    {
        diedFromVoid = !diedFromVoid;
    }

}
