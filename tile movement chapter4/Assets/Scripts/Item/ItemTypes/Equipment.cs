using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Descriptors for Items
public enum EquipmentType
{
    Weapon,
    Shield,
    Armor,
    Ring
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Item/Equipment")]

public class Equipment : Item {

    [Header("Equipment Info")]
    public EquipmentType equipmentType;
    public int levelRequirement;

    [Header("Equipment Bonus Attack")]
    public int attackStatRequirement;
    public float attackAccuracyBonus;
    public float attackDamageBonus;

    [Header("Equipment Bonus Defense")]
    public int defenseStatRequirement;
    [Tooltip("Affects chance to Dodge")]
    public float agilityBonus;
    [Tooltip("affects the Max Damage Taken")]
    public float defenseBonus;

    public Equipment()
    {
        itemType = ItemType.Equipment;
    }

    public override void Use()
    {
        //equip the item, takes off old one
        base.Use();
        GameManager.instance.player.GetComponent<Unit>().equipmentSystem.Equip(this);
    }
}
