using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Item))]
public class ItemPickup : MonoBehaviour {

    public Item item;
    public int count;

	void Start () {
        if(item == null)
        {
            Debug.LogError(gameObject.name + " Missing its item Plugin", gameObject);
        }
	}

    public void Pickup()
    {
        GameManager.instance.playerInventory.Add(item, count);
    }
}
