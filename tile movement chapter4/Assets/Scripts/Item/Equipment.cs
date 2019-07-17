using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Item/Equipment")]

public class Equipment : Item {

    [Header("Equipment Info")]
    public int levelRequirement;

    //other stat bonuses

    public Equipment()
    {
        itemType = ItemType.Equipment;
    }

    public override void Use()
    {
        //equip the item, takes off old one
    }
}
