using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Item))]
public class ItemPickup : MonoBehaviour {

    public Item item;
    public int count;

    private void Awake()
    {
        if(item.itemPrefab == null)
        {
            item.itemPrefab = Resources.Load<GameObject>("Items/Prefabs/" + item.itemType.ToString() + "/" + item.name);
        }
    }

    void Start ()
    {
        if(item == null)
        {
            Debug.LogError(gameObject.name + " Missing its item Plugin", gameObject);
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

    }

    //public void Pickup() Implimented on the unit script on pick up
    //{
    //    GameManager.instance.playerInventory.Add(item, count);
    //}
}
