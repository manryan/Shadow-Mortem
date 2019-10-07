using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPoolingManager : MonoBehaviour {

    //public static ObjectPoolingManager _instance;//Reference

    //public static ObjectPoolingManager instance
    //{
    //    get { return _instance; }
    //    set { _instance = value; }
    //}

    GameObject temp;//used for everything pretty much in object pooling
    GameManager GM;

    //Gold Pooling
    [Header("Gold")]
    public int goldPoolingAmount;

    //Water Pooling
    [Header("Water")]
    public int waterPoolingAmount;

    //Standard Pooling
    Dictionary<string, GameObject> itemCache = new Dictionary<string, GameObject>();

    //Temp
    Transform playerCurrentLocation;

    //Audio SFX
    [Header("Audio SFX")]
    public AudioClip goldSpawnSFX;
    public AudioClip waterSpawnSFX;
    public AudioClip equipmentSpawnSFX;
    public AudioClip consumablesSpawnSFX;

    void Start()
    {

        #region Pooling Creation Amount Checker

        if (goldPoolingAmount <= 0)
        {
            Debug.LogWarning("goldPoolingAmount is not set, setting to 3");
            goldPoolingAmount = 3;
        }

        if (waterPoolingAmount <= 0)
        {
            Debug.LogWarning("waterPoolingAmount is not set, setting to 5");
            waterPoolingAmount = 5;
        }

        #endregion

        #region All Items Pooling Creation

        itemCache = Resources.LoadAll<GameObject>("Items").ToDictionary(item => item.name, item => item);

        for (int i = 0; i < itemCache.Count; i++)
        {

            if (itemCache.ElementAt(i).Value.GetComponent<ItemPickup>() == null)
            {
                Debug.LogWarning(itemCache.ElementAt(i).Value.name + "Error, Item Pickup Is not implimented onto this object");
            }
            if (itemCache.ElementAt(i).Value.GetComponent<ItemPickup>().item == null)
            {
                Debug.LogWarning(itemCache.ElementAt(i).Value.name + "Error, The Item in Item Pickup component is null on this prefab object");
            }
            if (itemCache.ElementAt(i).Value.GetComponent<ItemPickup>().item.itemName == "")
            {
                Debug.LogWarning(itemCache.ElementAt(i).Value.name + "Error, The Item in Item Pickup Scriptable Object name is null on this prefab object");
            }
            //if (transform.GetChild(i).GetComponent<ItemPickup>().item.itemGameObject == null)
            //{
            //    Debug.LogWarning(transform.GetChild(i).name + "Error, The Item Game Object in Item Pickup Scriptable Object name is null on this prefab object");
            //}

            //This resets all starting values to 0 just incase
            itemCache.ElementAt(i).Value.GetComponent<ItemPickup>().item.count = 0;

            if (itemCache.ElementAt(i).Value.GetComponent<ItemPickup>().item.itemType == ItemType.Gold)
            {
                for (int j = 0; j < goldPoolingAmount; j++)
                {
                    temp = Instantiate(itemCache.ElementAt(i).Value, this.transform);
                    temp.SetActive(false);
                }
            }
            else if(itemCache.ElementAt(i).Value.GetComponent<ItemPickup>().item.itemType == ItemType.Water)
            {
                for (int j = 0; j < waterPoolingAmount; j++)
                {
                    temp = Instantiate(itemCache.ElementAt(i).Value, this.transform);
                    temp.SetActive(false);
                }
            }
            else
            { 
                temp = Instantiate(itemCache.ElementAt(i).Value, this.transform);
                temp.SetActive(false);
            }

        }

        //This cleans up everything and makes the location in the dictionary have a number

        itemCache = new Dictionary<string, GameObject>();

        int counterAidGold = 0;
        int counterAidWater = 0;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if(this.transform.GetChild(i).GetComponent<ItemPickup>().item.itemType == ItemType.Gold) //|| this.transform.GetChild(i).GetComponent<ItemPickup>().item.itemName != waterPrefab.name)
            {
                itemCache.Add(this.transform.GetChild(i).GetComponent<ItemPickup>().item.itemName + counterAidGold.ToString(), this.transform.GetChild(i).gameObject);
                counterAidGold++;
            }
            else if(this.transform.GetChild(i).GetComponent<ItemPickup>().item.itemType == ItemType.Water)
            {
                itemCache.Add(this.transform.GetChild(i).GetComponent<ItemPickup>().item.itemName + counterAidWater.ToString(), this.transform.GetChild(i).gameObject);
                counterAidWater++;
            }
            else
            {
                //This will add one of each item in the scene and give it a key reference in the object pool with a number
                itemCache.Add(this.transform.GetChild(i).GetComponent<ItemPickup>().item.itemName + "0", this.transform.GetChild(i).gameObject);
            }
        }

        Debug.Log(itemCache.Count + "How many items in the Object Pool");

        #endregion

        GM = GameManager.instance;
        GM.objPoolManager = this;
    }

    #region Equiping Object Pool Item

    //public GameObject EquipMethod(string equipmentName)
    //{

    //    for (int i = 0; i < 99; i++)
    //    {
    //        if (itemCache.ContainsKey(equipmentName + i))
    //        {
    //            temp = itemCache[equipmentName + i];
    //            if (temp.activeInHierarchy)
    //            {
    //                break;
    //            }
    //            else
    //            {
    //                //temp.GetComponent<Rigidbody>().isKinematic = true;
    //                //temp.GetComponent<Rigidbody>().useGravity = false;
    //                if (temp.GetComponent<MeshCollider>())
    //                {
    //                    temp.GetComponent<MeshCollider>().enabled = false;
    //                }
    //                if (temp.GetComponent<BoxCollider>())//just for the sword and other bs that dont have a mesh collider at the time
    //                {
    //                    temp.GetComponent<BoxCollider>().enabled = false;
    //                }
    //                temp.GetComponent<ItemPickup>().enabled = false;
    //                temp.SetActive(true);
    //                return temp;
    //            }
    //        }
    //        else
    //        {
    //            temp = Instantiate(itemCache[equipmentName + "0"], this.transform);
    //            itemCache.Add(temp.GetComponent<ItemPickup>().item.itemName + i, temp);
    //            temp.GetComponent<Rigidbody>().detectCollisions = false;
    //            temp.GetComponent<MeshCollider>().enabled = false;
    //            temp.GetComponent<ItemPickup>().enabled = false;
    //        }
    //    }

    //    return temp;
    //}

    #endregion

    #region UnEquiping Object Pool Item

    //public void UnEquipMethod(GameObject unEquipedItem)
    //{
    //    temp = unEquipedItem;
    //    if (itemCache.ContainsValue(temp))
    //    {
    //        temp.GetComponent<Rigidbody>().isKinematic = false;
    //        temp.GetComponent<Rigidbody>().useGravity = true;
    //        if (temp.GetComponent<MeshCollider>())
    //        {
    //            temp.GetComponent<MeshCollider>().enabled = true;
    //        }
    //        if (temp.GetComponent<BoxCollider>())//just for the sword and other bs that dont have a mesh collider at the time
    //        {
    //            temp.GetComponent<BoxCollider>().enabled = true;
    //        }
    //        temp.GetComponent<ItemPickup>().enabled = true;
    //        temp.transform.parent = this.transform;
    //        temp.SetActive(false);
    //        //Debug.Log("Hit", temp);
    //    }
    //    else
    //    {
    //        Debug.Log("You fucked up");
    //    }
    //}

    #endregion

    #region Add To Object Pool

    public void ObjectPoolAddition(GameObject newItem)//Function to be called when picking up a item
    {
        int counterAssistForDictionary = 0;
        bool exists = false;

        for (int i = 0; i < 99; i++)
        {
            if (newItem.GetComponent<ItemPickup>() == null)
            {
                Debug.Log(newItem.name + "Error, Item Pickup Is not implimented onto this object");
            }
            if (newItem.GetComponent<ItemPickup>().item == null)
            {
                Debug.Log(transform.GetChild(i).name + "Error, The Item in Item Pickup component is null on this prefab object");
            }

            if (itemCache.ContainsValue(newItem))
            {
                newItem.transform.parent = this.transform;
                newItem.SetActive(false);
                Debug.Log("Item is already part of the item cache, Readding" + newItem.GetComponent<ItemPickup>().item.itemName + i);
                break;
            }
            else
            {
                exists = itemCache.ContainsKey(newItem.GetComponent<ItemPickup>().item.itemName + counterAssistForDictionary.ToString());
                //This next line is to help identify if there are errors with the itemcache
                //Debug.Log(newItem.GetComponent<ItemPickup>().item.itemName + counterAssistForDictionary.ToString());
                //exists = itemCache.Any(x => (x.Value.transform.GetComponent<ItemPickup>().item.itemName + counterAssistForDictionary.ToString() != null));
                if (exists)
                {
                    counterAssistForDictionary++;
                }
                else
                {
                    itemCache.Add(newItem.GetComponent<ItemPickup>().item.itemName + counterAssistForDictionary.ToString(), newItem);
                    newItem.transform.parent = this.transform;
                    newItem.SetActive(false);
                    Debug.Log("Item being added to the item cache, adding " + newItem.GetComponent<ItemPickup>().item.itemName + i);
                    counterAssistForDictionary = 0;
                    break;
                }

            }
        }

        //for (int i = 0; i < 99; i++)
        //{
        //    if (itemCache.ContainsValue(newItem))
        //    {
        //        newItem.transform.parent = this.transform;
        //        newItem.SetActive(false);
        //        Debug.Log("Item is already part of the item cache, Readding" + newItem.GetComponent<ItemPickup>().item.itemName + i);
        //        break;
        //    }
        //    else
        //    {
        //        exists = itemCache.ContainsKey(transform.GetChild(i).GetComponent<ItemPickup>().item.itemName + counterAssistForDictionary.ToString());
        //        if (itemCache.ContainsKey(newItem.name + i))
        //        {
        //            //Debug.Log("item cache slot filled at index " + i,newItem);
        //            //slot filled so it will go back through the loop
        //        }
        //        else
        //        {
        //            newItem.transform.parent = this.transform;
        //            newItem.SetActive(false);
        //            Debug.Log("Item being added to the item cache, adding " + newItem.GetComponent<ItemPickup>().item.itemName + i);
        //            itemCache.Add(newItem.GetComponent<ItemPickup>().item.itemName + i, newItem);
        //            break;
        //        }
        //    }
        //}

    }

    #endregion

    #region Removing Object From Object Pool

    public void ObjectPoolPlayerDrop(GameObject droppingItem, int droppingHowMany)
    {

        for (int i = 0; i < 99; i++)
        {
            if (itemCache.ContainsKey(droppingItem.GetComponent<ItemPickup>().item.itemName + i))
            {
                temp = itemCache[droppingItem.GetComponent<ItemPickup>().item.itemName + i];
                if (!temp.activeInHierarchy)
                {
                    temp.transform.parent = GM.player.GetComponent<Unit>().currentNode.myTile.transform.Find("loot");
                    temp.transform.localPosition = new Vector3(0, 0, -1);//This is to make the item pop up on the Z axis
                    temp.GetComponent<ItemPickup>().count = droppingHowMany;
                    temp.SetActive(true);
                    GM.player.GetComponent<Unit>().currentNode.stack.Push(temp);
                    GM.player.GetComponent<Unit>().displayPickUpButton();
                    Debug.Log("Found inactive object ", temp);
                    break;
                }
                else
                {
                    Debug.Log("This item is active" + droppingItem.GetComponent<ItemPickup>().item.itemName + i);
                }

            }
            else
            {
                temp = Instantiate(droppingItem.GetComponent<ItemPickup>().item.itemPrefab, Vector3.zero, Quaternion.identity);
                temp.transform.parent = GM.player.GetComponent<Unit>().currentNode.myTile.transform.Find("loot");
                temp.transform.localPosition = new Vector3(0, 0, -1);//This is to make the item pop up on the Z axis
                temp.GetComponent<ItemPickup>().count += droppingHowMany;
                GM.player.GetComponent<Unit>().currentNode.stack.Push(temp);
                GM.player.GetComponent<Unit>().displayPickUpButton();
                break;
            }
        }

    }

    public GameObject ObjectPoolFindNReturn(GameObject droppingItem, Vector3 location, int droppingHowMany = 1)
    {

        for (int i = 0; i < Mathf.Infinity; i++)
        {
            if (itemCache.ContainsKey(droppingItem.GetComponent<ItemPickup>().item.itemName + i))
            {
                temp = itemCache[droppingItem.GetComponent<ItemPickup>().item.itemName + i];
                if (!temp.activeInHierarchy)
                {
                    temp.transform.position = location;
                    temp.GetComponent<ItemPickup>().count = droppingHowMany;
                    temp.transform.parent = null;
                    temp.SetActive(true);
                    Debug.Log("Found inactive object ", temp);
                    return temp;
                }
            }
            else
            {
                temp = Instantiate(droppingItem, location, Quaternion.identity);
                temp.GetComponent<ItemPickup>().count += droppingHowMany;
                temp.transform.parent = null;
                Debug.Log("Inactive object not Found, Creating New Object", temp);
                return temp;
            }
        }
        Debug.Log("Unreachable Code Detected, fucked up somewhere in object pooling");
        return null;
    }

    #endregion

}