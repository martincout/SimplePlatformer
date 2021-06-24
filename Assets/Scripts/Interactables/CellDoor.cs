using System.Collections;
using UnityEngine;

public class CellDoor : Interactable
{
    public bool openWithLever;
    public KeyColor keyNeeded;
    public GameObject particle;

    public void Reset()
    {
        interactOneTime = false;
        closerToInteract = true;
    }

    public override void Interact()
    {
        Interacted();
        if (GameManager.GetInstance().GetKeys()[keyNeeded] > 0 || openWithLever)
        {
            PlaySound();
            StartCoroutine( PlayAnimation());
            Destroy(gameObject, 1f);
            GameManager.GetInstance().DeleteKey(keyNeeded);
        }
    }

    private IEnumerator PlayAnimation()
    {
        Instantiate(particle, transform.GetChild(0));
        yield return new WaitForSeconds(0.5f);
        LeanTween.alpha(gameObject, 0f, 1f).setEase(LeanTweenType.linear);

    }
}
