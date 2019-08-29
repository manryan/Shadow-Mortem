using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;//For item reset and to load items to dictionary

public class GameManager : MonoBehaviour {

    #region Variables

    [Header("Editor Tools")]
    public bool turnLightsOn;

    public GameObject player;

    [System.NonSerialized]
    public InventorySystem playerInventory;

    //[System.NonSerialized]
    public ObjectPoolingManager objPoolManager;

    #endregion

    #region GameManager Singleton/Awake

    static GameManager _instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public static GameManager instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

    #endregion

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInventory = player.GetComponent<Unit>().inventory;
	}

    //void ResetScriptableObjectsCount()  Moved to object pool manager
    //{
    //    //Resets the count of scriptable objects (Items) due to the fact that they stay regardless if the game is stopped or not
    //    Dictionary<string, Item> itemCache = new Dictionary<string, Item>();

    //    itemCache = Resources.LoadAll<Item>("Items").ToDictionary(item => item.name, item => item);

    //    //Debug.Log(itemCache.Count);

    //    for (int i = 0; i < itemCache.Count; i++)
    //    {
    //        itemCache.ElementAt(i).Value.count = 0;
    //    }

    //    //Debug.Log("Item Total in Resources Folder " + itemCache.Count.ToString());
    //}

}