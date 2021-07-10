using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUp : MonoBehaviour
{
    public CinemachineVirtualCamera camera;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.77f;
        }
    }
}
