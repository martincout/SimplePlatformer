using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Default")]
public class Item : ScriptableObject
{
    public string itemName;
    public float value;
    public GameObject particleGameobject;

    public virtual void DoSomething(MonoBehaviour mono)
    {
        //do power
    }

    public virtual void DoSomething()
    {
        //do power
    }

    public enum Type
    {
        COIN,
        CONSUMABLE
    }
}
