using UnityEngine;

[CreateAssetMenu(fileName = "Gold", menuName = "Item/Gold")]
public class Gold : Item
{
    public Gold()
    {
        itemName = "Gold";
        itemType = ItemType.Gold;
        defaultItem = true;
        cost = 1;
        stackable = true;
    }
}
