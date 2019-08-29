using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/Consumable")]

public class Consumables : Item {

    //public bool stackable;
    [Header("Consumable Info")]
    public int hPHealed;

    public Consumables()
    {
        itemType = ItemType.Consumables;//ties the item type to be correct        
    }

    public override void Use()
    {
        if(itemName != "Gold")
        {
            base.Use();
            GameManager.instance.player.GetComponent<Unit>().health += hPHealed;
            Debug.Log("Consumable used");
        }
    }
}
