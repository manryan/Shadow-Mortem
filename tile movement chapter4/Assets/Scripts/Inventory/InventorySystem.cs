using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem {

    public List<Item> inventory = new List<Item>();

    const int MAX_INVENTORY_SIZE = 24;

    #region AddMethod

    public void Add(Item item,int itemCount)
    {     
        if(item.stackable == true)
        {
            if(inventory.Exists(x => x.itemName == item.itemName))
            {
                inventory.Find(x => x.itemName == item.itemName).count += itemCount;
            }
            else
            {
                //Check to see if there is any room in the inventory
                if(inventory.Count < MAX_INVENTORY_SIZE)
                {
                    inventory.Add(item);
                    item.count += itemCount;//due to it being a scriptable object the count has to be seperated
                }
                else
                {
                    //indicate that the inventory is full
                    Debug.LogWarning("Inventory is full");
                }
            }
        }
        else
        {
            if (inventory.Count < MAX_INVENTORY_SIZE)
            {
                inventory.Add(item);
            }
            else
            {
                //indicate that the inventory is full
                Debug.LogWarning("Inventory is full");
            }
        }
    }
    #endregion

    #region RemoveMethod

    public void Remove(Item item, int itemCount)
    {

        if(item.stackable == true)
        {
            Item temp = inventory.Find(x => x.itemName == item.itemName);//.count -= item.count;
            temp.count -= item.count;
            if(temp.count <= 0 && temp.defaultItem ==false)//if item isnt always in the inventory then remove the item from inventory
            {
                inventory.Remove(item);
                //put object onto the floor with correct count
            }
        }
        else
        {
            inventory.Remove(item);
            //put object onto floor
        }
    }
    #endregion

    #region Testing

    public void TestInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            Debug.Log("Slot Number " + i + "Item Name :" + inventory[i].name + " Item Count :" + inventory[i].count);
        }
    }

    #endregion

}