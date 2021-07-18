using UnityEngine;

public class CampFire : Interactable
{
    private Animator anim;
    private readonly string CAMPFIRE_ON = "campFireIdle";
    public GameObject lightGO;

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Start()
    {
        if (interacted)
        {
            anim.Play(CAMPFIRE_ON);
        }
    }

    public override void Interact()
    {
        if (!interacted)
        {
            base.Interact();
            anim.Play(CAMPFIRE_ON);
            RespawnManager.currentRespawn = gameObject;
            EventSystems.NewSpawnHandler();
            Instantiate(lightGO, transform.position, Quaternion.identity);
        }
    }
}
