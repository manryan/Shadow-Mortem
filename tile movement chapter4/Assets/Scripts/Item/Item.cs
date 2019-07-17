using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Descriptors for Items
public enum ItemType
{
    Equipment,
    Consumables
}

//[RequireComponent(typeof(ItemPickup))]

[System.Serializable]
public abstract partial class Item : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [System.NonSerialized]
    public ItemType itemType;
    public Sprite sprite;
    public bool defaultItem;
    public int cost;
    public bool stackable;
    public int count;

    //Destroy Item in Inventory
    public virtual void RemoveFromInventory(int removeCount)
    {
        //removes the item from inventory
    }

    public virtual void Use()
    {
        //calls the use function from whatever inherits from this
        RemoveFromInventory(1);
    }
}
