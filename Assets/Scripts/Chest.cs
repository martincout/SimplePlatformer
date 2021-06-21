

using UnityEngine;

public class Chest : Interactable
{
    public KeyColor keyColor;

    private void Reset()
    {
        interactOneTime = true; 
    }

    public override void Interact()
    {
        base.Interact();
        GetComponent<Animator>().Play("chestOpen");
        GameStatus.GetInstance().AddKey(keyColor);
    }
}
