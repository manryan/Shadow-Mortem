using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicInventorySizeAdjustor : MonoBehaviour {

    GameObject inventoryPanel;

    float inventoryPanelWidth;
    float inventoryPanelHeight;

    GameObject inventoryBackgroundBorder;
    GameObject EquipmentBackground;


	// Use this for initialization
	void Start () {
        inventoryPanel = this.gameObject;//Gets the inventory Panel and plugs it in

        inventoryPanelWidth = inventoryPanel.GetComponent<RectTransform>().rect.width;
        inventoryPanelHeight = inventoryPanel.GetComponent<RectTransform>().rect.height;

        inventoryBackgroundBorder = transform.Find("InventoryBackground").gameObject;

        //This is where the math will go to get the size of the inventory working correctly 
	}

}
