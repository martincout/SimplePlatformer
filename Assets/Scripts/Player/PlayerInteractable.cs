﻿using System;
using UnityEngine;

namespace SimplePlatformer.Player
{
    //need to change to fixedUpdate
    public class PlayerInteractable : MonoBehaviour
    {
        public GameObject buttonInteract;
        public Vector2 size;
        private bool btnInstantiated = false;
        private GameObject buttonRef;
        private float buttonOffset;
        private Collider2D interact;
        private PlayerVariables pv;

        public void Setup(PlayerVariables pv)
        {
            this.pv = pv;
        }

        private void Start()
        {
            this.pv = new PlayerVariables();
        }

        // Update is called once per frame
        void Update()
        {
            if (!pv.canInteract) return;
            interact = Physics2D.OverlapBox(transform.position, size, 0, 1 << LayerMask.NameToLayer("Interactable"));
            if (interact != null && interact.GetComponent<Interactable>() != null && !interact.GetComponent<Interactable>().interacted)
            {

                buttonOffset = interact.GetComponent<Interactable>().buttonOffset;
                //Instantiate the Button UI (closer to interact doesn't instantiate the button UI)
                if (!btnInstantiated && !interact.GetComponent<Interactable>().closerToInteract)
                {
                    //Position of the Button Interact
                    Vector3 pos = new Vector3(interact.transform.position.x, interact.transform.position.y + buttonOffset, -1);
                    buttonRef = Instantiate(buttonInteract, pos, Quaternion.identity, interact.transform);
                    btnInstantiated = true;
                }
                if (interact.GetComponent<Interactable>().closerToInteract)
                {
                    interact.GetComponent<Interactable>().Interact();
                }
            }
            //Destroy UI BUTTON 
            if (interact == null && btnInstantiated)
            {
                DestroyButton(buttonRef);
                btnInstantiated = false;
            }


        }

        public void Interact()
        {
            if (!pv.canInteract) return;
            //If there is an Interactable object near
            if (interact != null)
            {
                interact.GetComponent<Interactable>().Interact();
                DestroyButton(buttonRef);
                btnInstantiated = false;
            }

        }

        private void DestroyButton(GameObject btn)
        {
            if (buttonRef != null)
            {
                Destroy(btn);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, size);
            Gizmos.color = Color.white;
        }
    }
}

