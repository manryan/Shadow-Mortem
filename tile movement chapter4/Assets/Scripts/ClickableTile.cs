using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ClickableTile : MonoBehaviour
{

    public int tileX;
    public int tileY;
    public TileMap map;

    public bool left = false;

    public bool clicked;

    public GameObject grass;

    public GameObject trap;

    public GameObject[] traps;

    public bool holdsTraps;

    public Node myNode;

    public List<Loot> loot = new List<Loot>();

    ObjectPoolingManager obj;


    public void Start()
    {
        if (holdsTraps)
        {
            trap = traps[Random.Range(0, traps.Length)];
        }
        obj = GameManager.instance.objPoolManager;
    }

    public void OnMouseDown()
    {
            if (IsPointerOverUIObject()) 
                return;


        left = false;
        clicked = true;
    }

    public void OnMouseUp()
    {

        if (left == false && clicked)
        {

            Debug.Log("Click!");

            if (map.entity.currentPath != null)
            {
                    map.entity.remainingMovement = 0;
                    map.entity.currentPath = null;
            }
            else
            {
                if(map.entity.inControl)
                map.GeneratePathTo(tileX, tileY);

               // map.selectedUnit.GetComponent<Entity>().MoveNextTile();
            }
        }
        clicked = false;
        left = false;
    }

    void OnMouseExit()
    {
        left = true;
    }


    public void spawnGrassLoot()
    {
        myNode.index = myNode.stack.Count;
        for (int i=0; i<loot.Count; i++)
        {
            if(Random.value * 100f < loot[i].spawnChance)
            {
                //GameObject temp = Instantiate(loot[i].itemPrefab, new Vector3(tileX, tileY, -1), Quaternion.identity);
                GameObject temp = obj.ObjectPoolFindNReturn(loot[i].itemPrefab, new Vector3(tileX, tileY, -1));
                //transform.GetChild(3).GetChild(i).gameObject.SetActive(true);
                //transform.GetChild(3).GetChild(i).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2 + myNode.index;
                temp.transform.parent = transform.GetChild(3);
                temp.GetComponent<SpriteRenderer>().sortingOrder = 2 + myNode.index;
                myNode.stack.Push(temp);
                myNode.index++;
            }
           
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    [System.Serializable]
    public class Loot
        {
        public GameObject itemPrefab;

        public float spawnChance;
        }

}