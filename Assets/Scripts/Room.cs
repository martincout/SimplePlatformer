using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using SimplePlatformer.Player;
public class Room : MonoBehaviour
{
    private GameObject virtualCamera;

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
    }private void OnDisable()
    {
        EventSystem.RespawnHandler -= SetFollow;
    }

    private void SetFollow()
    {
        Debug.Log("working");
        GameObject playerGm = FindObjectOfType<PlayerBase>().gameObject;
        virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = playerGm.transform;
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
        if (collision.CompareTag("Player"))
        {
            virtualCamera.SetActive(false);

        }
    }


}
