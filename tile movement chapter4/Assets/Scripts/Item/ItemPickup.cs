using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Item))]
public class ItemPickup : MonoBehaviour {

    public Item item;
    public int count;


    public bool madeFromScratch;

    public ItemPickup(Item CurrentItem,int CurrentCount = 1)
    {
        item = CurrentItem;
        count = CurrentCount;
    }

    private void Awake()
    {
        if(item.itemPrefab == null)
        {
            if(item.itemType != ItemType.Equipment)
            {
                item.itemPrefab = Resources.Load<GameObject>("Items/Prefabs/" + item.itemType.ToString() + "/" + item.name);
            }
            else
            {
                Equipment equipment = (Equipment)item;
                item.itemPrefab = Resources.Load<GameObject>("Items/Prefabs/" + item.itemType.ToString() + "/" + equipment.equipmentType.ToString() + "/" + item.name);
            }
        }
    }

    void Start ()
    {
        if(item == null)
        {
            Debug.LogError(gameObject.name + " Missing its item Plugin", gameObject);
        }
        if(item.itemPrefab == null)
        {
            Debug.Log("Item Prefab is not being set, check to see if the name is matching or the equipment type",gameObject);
        }
        if(item.name != item.itemPrefab.name)
        {
            Debug.Log("Prefab and Item Names do not match, is something missing?", gameObject);
        }
        if(count<=0)
        {
            count = 1;
        }
        if(item.sprite = GetComponent<SpriteRenderer>().sprite)
        {
            item.sprite = GetComponent<SpriteRenderer>().sprite;
        }

        if (gameObject.CompareTag("Item") == false)
        {
            Debug.LogWarning("This GameObject Prefab Is Missing The Item Tag");
        }

    }

    void Reset()//Called when this component is added
    {
        gameObject.tag = "Item";
    }
}
