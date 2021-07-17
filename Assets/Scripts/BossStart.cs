using Cinemachine;
using SimplePlatformer.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStart : MonoBehaviour
{
    public CinemachineVirtualCamera camera;
    public CinemachineVirtualCamera bossCamera;
    public BossBehaviour boss;
    public GameObject celldoor;
    public Transform celldoorPosition;
    public AudioClip BGM;
    public AudioSource BGMGO;
    private bool oneTime = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !oneTime)
        {
            oneTime = true;
            camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.77f;
            StartCoroutine(FocusBoss(2f));
            boss.ChangeState(BossBehaviour.State.START);
            boss.DisplayHealthBar();
            Instantiate(celldoor, celldoorPosition,true);
            BGMGO.clip = BGM;
            BGMGO.Play();
        }
    }
    private IEnumerator FocusBoss(float _sec)
    {
        bossCamera.gameObject.SetActive(true);
        camera.gameObject.SetActive(false);
        yield return new WaitForSeconds(_sec);
        camera.gameObject.SetActive(true);
        bossCamera.gameObject.SetActive(false);
    }
}
