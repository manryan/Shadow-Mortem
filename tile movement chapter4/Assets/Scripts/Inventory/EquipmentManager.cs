using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager
{

    //#region Singleton/Awake Function

    //static EquipmentManager _instance = null;

    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else if (instance != null)
    //    {
    //        Destroy(gameObject);
    //    }

    //    DontDestroyOnLoad(gameObject);

    //}

    //public static EquipmentManager instance
    //{
    //    get { return _instance; }
    //    set { _instance = value; }
    //}

    //#endregion

    #region Variables

    [System.NonSerialized]//accessed from the inventory ui
    public Equipment[] currentEquipment;

    [System.NonSerialized]
    public Unit player;

    const int singleObjectCount = 1;//This is for the change in inentory system.additem, just adding a single one of that back

    //used for equiping the multiple armors to their specific locations
    GameObject temp;

    #endregion

    public void Start()
    {
        int numOfSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;//will adjust to the size of the Enum Equipment type, any item placed within will go in its corresponding number within
        currentEquipment = new Equipment[numOfSlots];

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();

        #region Failsafe Starting Check

        if (player == null)
        {
            Debug.LogError("Reference to Player is missing on Equipment Manager Script");
        }

        #endregion

    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipmentType;//Changes the enum to number for refference inside of array

        Equipment oldItem = null;//creates a temp variable for the old item

        if (currentEquipment[slotIndex] != null)//double checks that if there was a piece of equipment already on in that slot
        {
            oldItem = currentEquipment[slotIndex];//stores the previous equipped item in this variable
            player.inventory.Add(oldItem, singleObjectCount);//adds the perviously equiped item back to their inventory
        }

        if (oldItem != null)
        {
            //Takes away bonuses from the old item being taken off
            //player.attack -= (int)oldItem.attackDamageBonus;
            //player.defense -= (int)oldItem.attackAccuracyBonus;
            //player.specialAttack -= (int)oldItem.defenseBonus;
            //player.specialDefense -= (int)oldItem.agilityBonus;

            player.inventory.Add(oldItem,singleObjectCount);
        }

        //Adds the bonuses of new item being equiped
        //player.attack += (int)newItem.attackDamageBonus;
        //player.defense += (int)newItem.attackAccuracyBonus;
        //player.specialAttack += (int)newItem.defenseBonus;
        //player.specialDefense += (int)newItem.agilityBonus;

        currentEquipment[slotIndex] = newItem;//adds the equipment to the array
    }

    public void Unequip(int slotIndex)
    {
        Equipment oldItem = currentEquipment[slotIndex];

        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            player.inventory.Add(oldItem, singleObjectCount);//adds the perviously equiped item back to their inventory
        }

        if (oldItem != null)
        {
            //Takes away bonuses from the old item being taken off
            //player.attack -= (int)oldItem.attackDamageBonus;
            //player.defense -= (int)oldItem.attackAccuracyBonus;
            //player.specialAttack -= (int)oldItem.defenseBonus;
            //player.specialDefense -= (int)oldItem.agilityBonus;
        }

        //makes sure the old item is now null
        currentEquipment[slotIndex] = null;
    }

    #region Testing//Delete when Finished

    public void UnEquipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Unequip called");
            UnEquipAll();
        }
    }

    #endregion

}

