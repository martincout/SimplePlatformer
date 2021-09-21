

using UnityEngine;

public class Chest : Interactable
{
    public KeyColor keyColor;
    private SpriteRenderer spr;
    [SerializeField] private Sprite openChest;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    private void Reset()
    {
        interactOneTime = true;
    }

    public override void Interacted()
    {
        base.Interacted();
        GetComponent<Animator>().Play("chestOpen");
    }

    public override void Interact()
    {
        if (!interacted)
        {
            base.Interact();
            GetComponent<Animator>().Play("chestOpen");
            GameManager.GetInstance().AddKey(keyColor);
        }
    }
}
