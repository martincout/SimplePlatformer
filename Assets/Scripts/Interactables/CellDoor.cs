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
        }
    }

    private IEnumerator PlayAnimation()
    {
        GameObject p = Instantiate(particle, transform.GetChild(0).position,Quaternion.identity);
        Destroy(p, 0.5f);
        yield return new WaitForSeconds(0.5f);
        LeanTween.alpha(gameObject, 0, 1f).setEase(LeanTweenType.linear);
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

}
