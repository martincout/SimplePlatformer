﻿using System.Collections;
using TMPro;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Item item;
    //Waits a certain amount of time before pickup, this is for let player see the item
    private bool canPickUp = false;
    private bool alreadyCollided = false;

    private void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (item.animation != null && anim != null) { anim.Play(item.animation); }
        StartCoroutine(WaitForPickup());
    }

    /// <summary>
    /// Checks the player collision to pick up the item
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If already collided. To check after if it needs to pickup the item once.
        if (collision.CompareTag("Player"))
        {
            alreadyCollided = true;
        }

        if (collision.CompareTag("Player") && canPickUp)
        {
            PickUpItem(collision);
        }
    }
    /// <summary>
    /// Checks if the player it's still colliding. This method is to pick up the item after the timer goes off and so the player
    /// can see the item it's picking up.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canPickUp && alreadyCollided)
        {
            alreadyCollided = false;
            PickUpItem(collision);
        }
    }

    private void PickUpItem(Collider2D collision)
    {
        GameObject particles = Instantiate(item.particleGameobject, transform.position, Quaternion.identity);
        SoundManager.instance.Play("Coin");
        //Particles
        Destroy(particles, 1f);

        switch (item.category)
        {
            case Item.Category.COIN:
                GameManager.GetInstance().AddScore((int)item.value);
                Destroy(gameObject);
                break;
            case Item.Category.CONSUMABLE:
                //Just adding the value of the item as a health amount
                collision.GetComponent<IItem>().ItemInteraction(item);
                Destroy(gameObject);
                break;
            case Item.Category.KEY:
                GameManager.GetInstance().AddKey(((Key)item).color);
                gameObject.SetActive(false);
                break;
        }

    }

    
    private IEnumerator WaitForPickup()
    {
        yield return new WaitForSeconds(0.2f);
        canPickUp = true;
    }
}
