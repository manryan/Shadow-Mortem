using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]//For Saving
public class InventorySystem {

    public List<Item> inventory = new List<Item>();

    const int MAX_INVENTORY_SIZE = 16;    

    #region AddMethod

    public bool checkIfWeCanAdd(Item item)
    {
        if (item.stackable == true)
        {
            if (inventory.Exists(x => x.itemName == item.itemName))
            {
                //     inventory.Find(x => x.itemName == item.itemName).count += itemCount;
                return true;
            }
            else
            {
                //Check to see if there is any room in the inventory
                if (inventory.Count < MAX_INVENTORY_SIZE)
                {
                    return true;
                    //inventory.Add(item);
                   // item.count += itemCount;//due to it being a scriptable object the count has to be seperated
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            if (inventory.Count < MAX_INVENTORY_SIZE)
            {
                return true;
            }
            else
            {
                //indicate that the inventory is full
                return false;
            }
        }
    }

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

    public void Remove(Item item, int itemCount = 1)
    {

        if(item.stackable == true)
        {
            Item temp = inventory.Find(x => x.itemName == item.itemName);//.count -= item.count;
            temp.count -= itemCount;
            if(temp.count <= 0 && temp.defaultItem ==false)//if item isnt always in the inventory then remove the item from inventory
            {
                if(temp.itemType != ItemType.Gold)
                {
                    inventory.Remove(item);
                    //If player drops item this is done through the inventory UI since this removes the item even if used
                }
            }
        }
        else
        {
            inventory.Remove(item);
            //If player drops item this is done through the inventory UI since this removes the item even if used
        }
    }
    #endregion

    //make a bool to check to see if this item can be added to the inventory;

    #region Saving/Loading

    public void Save()
    {
        string inventorySave = "";

        for (int i = 0; i < inventory.Count; i++)
        {
            if (i != inventory.Count - 1)
                inventorySave += inventory[i].itemName.ToString() + "^" + inventory[i].count.ToString() + "^" + inventory[i].itemType + "*";
            else
                inventorySave += inventory[i].itemName.ToString() + "^" + inventory[i].count.ToString() + "^" + inventory[i].itemType;
        }

        PlayerPrefs.SetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory" + id.instance.saveIndex, inventorySave);




        //InventorySave inventorySave = new InventorySave();
        //inventorySave.saveList = new List<ItemAndCount>();

        //for (int i = 0; i < inventory.Count; i++)
        //{
        //    ItemAndCount current = new ItemAndCount();
        //    current.item = inventory[i];
        //    current.count = inventory[i].count;
        //    inventorySave.saveList.Add(current);
        //}

        //string saveInventory = JsonUtility.ToJson(inventorySave);
        //PlayerPrefs.SetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory", saveInventory);
    }

    public void Load()
    {
        string temp = PlayerPrefs.GetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory" + id.instance.saveIndex);
        string[] inventoryLoad = temp.Split("*".ToCharArray());
        string[] tempItemAndCount;

        inventory = new List<Item>();
        if(temp.Length>0)
        for (int i = 0; i < inventoryLoad.Length; i++)
        {
            tempItemAndCount = inventoryLoad[i].Split("^".ToCharArray());
            Item tempItem = Resources.Load<Item>("Items/ScriptableObjects/" + tempItemAndCount[2] + "/" + tempItemAndCount[0]);
            tempItem.count = 0;
            Add(tempItem, System.Int32.Parse(tempItemAndCount[1]));
        }



        //string loadInventory = PlayerPrefs.GetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory");
        //InventorySave inventoryLoad = JsonUtility.FromJson<InventorySave>(loadInventory);

        //inventory = new List<Item>();

        //for (int i = 0; i < inventoryLoad.saveList.Count; i++)
        //{
        //    Add(inventoryLoad.saveList[i].item, inventoryLoad.saveList[i].count);
        //}
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