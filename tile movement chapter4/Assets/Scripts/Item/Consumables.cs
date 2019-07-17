using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/Consumable")]

public class Consumables : Item {

    //public bool stackable;
    public int hPHealed;

    public Consumables()
    {
        itemType = ItemType.Consumables;//ties the item type to be correct
        //GameObject temp = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube));
        //temp.name = "New Consumable";
        //temp.transform.position = Vector3.zero;
        //temp.AddComponent<ItemPickup>();
        
    }

    public override void Use()
    {
        if(itemName != "Gold")
        {
            base.Use();
            //gets the player and heals him
        }
    }
}
