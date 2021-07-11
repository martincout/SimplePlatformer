using Cinemachine;
using SimplePlatformer.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStart : MonoBehaviour
{
    public CinemachineVirtualCamera camera;
    public BossBehaviour boss;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.77f;
            boss.ChangeState(BossBehaviour.State.START);
        }
    }
}
