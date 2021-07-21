using System.Collections;
using UnityEngine;

public class CellDoor : Interactable
{
    public bool openWithLever;
    public KeyColor keyNeeded;
    public GameObject particle;
    private Renderer render;

    public void Reset()
    {
        interactOneTime = false;
        closerToInteract = true;
    }

    private void Start()
    {
        render = transform.GetChild(0).GetComponent<Renderer>();
    }

    public override void Interact()
    {
        Interacted();
        if (GameManager.GetInstance().GetKeys()[keyNeeded] > 0 || openWithLever)
        {
            PlaySound();
            StartCoroutine(PlayAnimation());
            GameManager.GetInstance().DeleteKey(keyNeeded);
            Destroy(gameObject, 1.5f);
        }
    }

    private IEnumerator PlayAnimation()
    {
        Instantiate(particle, transform.GetChild(0));
        yield return new WaitForSeconds(0.5f);
        LeanTween.alpha(gameObject, 0, 1f).setEase(LeanTweenType.linear);
    }

}
