using UnityEngine;

namespace SimplePlatformer.Player
{
    //need to change to fixedUpdate
    public class PlayerInteractable : PlayerBase
    {
        public GameObject buttonInteract;
        public Vector2 size;
        private bool btnInstantiated = false;
        private GameObject buttonRef;
        private float buttonOffset;
        // Update is called once per frame
        void Update()
        {
            Collider2D interact = Physics2D.OverlapBox(transform.position, size, 0, 1 << LayerMask.NameToLayer("Interactable"));
            if (interact != null && interact.GetComponent<Interactable>() != null && !interact.GetComponent<Interactable>().interacted)
            {
                buttonOffset = interact.GetComponent<Interactable>().buttonOffset;
                if (!btnInstantiated)
                {
                    //Position of the Button Interact
                    Vector3 pos = new Vector3(interact.transform.position.x, interact.transform.position.y + buttonOffset, -1);
                    buttonRef = Instantiate(buttonInteract, pos, Quaternion.identity, interact.transform);
                    btnInstantiated = true;
                }

                if (Input.GetButtonDown("Interact"))
                {
                    //Interact
                    interact.GetComponent<Interactable>().Interact();
                    Destroy(buttonRef);
                    btnInstantiated = false;
                }
            }
            if(interact == null && btnInstantiated)
            {
                Destroy(buttonRef);
                btnInstantiated = false;
            }


        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, size);
            Gizmos.color = Color.white;
        }
    }
}

