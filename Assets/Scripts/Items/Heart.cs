using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Heart")]
public class Heart : Item
{
    public override void DoSomething(MonoBehaviour mono)
    {
        HealthSystem hs = (HealthSystem) mono;
        hs.Heal(value);
    }

}
