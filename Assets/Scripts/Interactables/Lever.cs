using UnityEngine;
using System.Collections.Generic;

public class Lever : Interactable
{
    public Sprite leverOnSprite;
    public List<GameObject> doors;
    public override void Interact()
    {
        base.Interact();
        GetComponent<SpriteRenderer>().sprite = leverOnSprite;
        if(doors != null)
        {
            foreach(GameObject door in doors)
            {
                Destroy(door);
            }
        }
    }

}
