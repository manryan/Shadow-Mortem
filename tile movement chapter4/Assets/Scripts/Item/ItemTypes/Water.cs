using UnityEngine;

[CreateAssetMenu(fileName = "Water", menuName = "Item/Water")]
public class Water : Item
{
    public int hPHealed;
    public Water()
    {
        itemName = "Water";
        itemType = ItemType.Water;
        defaultItem = true;
        //cost = 1;
        stackable = true;
        hPHealed = 3;
    }

    public override void Use()
    {
        if (itemName != "Gold")
        {
            base.Use();
            GameManager.instance.player.GetComponent<Unit>().health += hPHealed;
            Debug.Log("Drank water mother fucker");
        }
    }
}
