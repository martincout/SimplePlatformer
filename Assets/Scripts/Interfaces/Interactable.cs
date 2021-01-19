
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool interactOneTime;
    public bool interacted;
    public float buttonOffset;
    public AudioClip sound;

    public virtual void Interact()
    {
        PlaySound();
        Interacted();
    }
    public virtual void Interacted()
    {
        if (interactOneTime)
        {
            interacted = !interacted;
        }
    }

    public virtual void PlaySound()
    {
        if (sound != null)
        {
            GameObject audioGm = new GameObject("Interactable Sound");
            AudioSource source = audioGm.AddComponent<AudioSource>();
            source.PlayOneShot(sound);
            Destroy(audioGm, 2f);
        }

    }
}
