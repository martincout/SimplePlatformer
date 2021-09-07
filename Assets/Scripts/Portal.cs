using SimplePlatformer.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private AudioSource audi;
    [SerializeField] private AudioClip enterPortal;
    [SerializeField] private PlayerController playerGO;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        audi = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameEvents.OnBossDeath += PortalAppear;
        GameEvents.RespawnHandler += UpdatesPlayer;
    }

    private void OnDisable()
    {
        GameEvents.OnBossDeath -= PortalAppear;
        GameEvents.RespawnHandler += UpdatesPlayer;
    }

    private void UpdatesPlayer(GameObject player)
    {
        playerGO = player.GetComponent<PlayerController>();
    }

    private void PortalAppear()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        boxCollider.enabled = true;
        Debug.Log("play");
        audi.Play();
    }

    private IEnumerator PortalCo()
    {
        playerGO.DisablePlayerState(true);
        audi.PlayOneShot(enterPortal);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("TheEnd");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(PortalCo());
        }
    }
}
