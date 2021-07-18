using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : Interactable
{
    public GameObject dialogueBox;
    private bool isActive = false;
    public override void Interact()
    {
        base.Interact();
        dialogueBox.SetActive(!isActive);
    }


}
