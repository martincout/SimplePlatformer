using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Items/Default")]
public class Item : ScriptableObject
{
    public enum Category
    {
        COIN,
        CONSUMABLE
    }

    public string itemName;
    public float value;
    public GameObject particleGameobject;
    public Category category;

    
}
