using UnityEngine;

public class CampFire : Interactable
{
    private Animator anim;
    private readonly string CAMPFIRE_ON = "campFireIdle";

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
        base.Interact();
        anim.Play(CAMPFIRE_ON);
        RespawnManager.currentRespawn = gameObject;
        EventSystem.NewSpawnHandler();
    }
}
