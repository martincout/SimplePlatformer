using UnityEngine;
using System.Collections.Generic;

public class Lever : Interactable
{
    public Sprite leverOnSprite;
    public List<GameObject> doors;
    public DoorMecanism triggerMecanism;
    public override void Interact()
    {
        base.Interact();
        GetComponent<SpriteRenderer>().sprite = leverOnSprite;
        if(triggerMecanism == null)
        {
            if (doors != null)
            {
                foreach (GameObject door in doors)
                {
                    door.GetComponent<CellDoor>().Interact();
                }
            }
        }
        else
        {
            triggerMecanism.Play(doors);
        }
        
        
    }

}
