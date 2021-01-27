using UnityEngine;

public enum KeyColor
{
    BLUE,
    RED,
    YELLOW,
    GRAY
}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Key")]
public class Key : Item
{
    public KeyColor color;

    public void Reset()
    {
        category = Category.KEY;
    }
}
