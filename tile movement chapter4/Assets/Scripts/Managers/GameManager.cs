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


    public void PlayerSetup(GameObject playerParameter)
    {
        player = playerParameter;
        playerInventory = playerParameter.GetComponent<Unit>().inventory;
        objPoolManager.itemmanager = playerParameter.GetComponent<ItemManager>();
    }
}