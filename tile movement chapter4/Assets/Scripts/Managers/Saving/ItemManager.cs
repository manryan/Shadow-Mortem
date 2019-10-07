using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
public class ItemManager : MonoBehaviour {

    [System.Serializable]
    public class itemData
    {
        public int counter;

        public Vector3 pos;

        public Item item;
    }

    [System.Serializable]
    public class itemDataContainer
    {
        public List<itemData> dataHolder;

        public List<itemData> newObjectsData;

        public List<string> alive;

        public List<string> dead;

        public List<string> pool;

        public List<string> objsToInstantiate;
    }
   public List<GameObject> items;

    public itemDataContainer tester;

    // Use this for initialization
    void Awake () {
        items = GameObject.FindGameObjectsWithTag("Items").ToList();

        string jsonString = PlayerPrefs.GetString("itemSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex);
        tester = JsonUtility.FromJson<itemDataContainer>(jsonString);
    }


    public void adjustList(string enemy)
    {
        tester.alive.Remove(enemy);
        tester.dead.Add(enemy);
    }

    public void initializeEnemies()
    {
        tester = new itemDataContainer();
        tester.dead = new List<string>();
        tester.alive = new List<string>();
        tester.pool = new List<string>();
        tester.newObjectsData = new List<itemData>();
        tester.objsToInstantiate = new List<string>();
        tester.dataHolder = new List<itemData>();

        foreach (GameObject enemy in items)
        {
            tester.alive.Add(enemy.name);
            itemData storage = new itemData { pos = enemy.transform.position, counter = enemy.GetComponent<ItemPickup>().count};
            tester.dataHolder.Add(storage);
        }
        string json = JsonUtility.ToJson(tester);
        PlayerPrefs.SetString("itemSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex, json);

        PlayerPrefs.Save();
    }

    public void addToPool(GameObject obj)
    {
        tester.pool.Add(obj.name);
        tester.alive.Remove(obj.name);
    }

    public void removeFromPool(GameObject obj)
    {
        tester.alive.Add(obj.name);
        tester.pool.Remove(obj.name);
    }

    public void saveEm()
    {
        // string jsonString = PlayerPrefs.GetString("TestSave" + saveIndex);
        //tester = JsonUtility.FromJson<dataContainer>(jsonString);

        tester.dataHolder.Clear();

        tester.newObjectsData.Clear();

        foreach (GameObject obj in items)
        {
            if (tester.alive.Contains(obj.name))
            {
                itemData storage = new itemData { pos = obj.transform.position, counter = obj.GetComponent<ItemPickup>().count };
                tester.dataHolder.Add(storage);
            }
            if(tester.objsToInstantiate.Contains(obj.name))
            {
                itemData storage = new itemData { pos = obj.transform.position, counter = obj.GetComponent<ItemPickup>().count, item = obj.GetComponent<ItemPickup>().item };
                tester.newObjectsData.Add(storage);
            }

        }
        string json = JsonUtility.ToJson(tester);
        PlayerPrefs.SetString("itemSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex, json);
        PlayerPrefs.Save();
    }

    public ObjectPoolingManager pooler;


    public void loadEnemies()
    {
        int i = 0;

        string jsonString = PlayerPrefs.GetString("itemSave" + SceneManager.GetActiveScene().name + id.instance.saveIndex);
        tester = JsonUtility.FromJson<itemDataContainer>(jsonString);

        foreach (GameObject obj in items)
        {
            if (tester.pool.Contains(obj.name))
            {
                obj.SetActive(false);
                obj.transform.parent = pooler.transform;
                continue;
            }

            if (tester.alive.Contains(obj.name))
            {
                //   Enemy enemy = obj.GetComponent<Enemy>();
                //   enemy.health = tester.dataHolder[i].enemyHealth;


                    obj.transform.position = tester.dataHolder[i].pos;
                obj.GetComponent<ItemPickup>().count = tester.dataHolder[i].counter;
                    i++;
                
            }
            else
            {
                obj.SetActive(false);
            }
        }

                
        i = 0;
        foreach(string name in tester.objsToInstantiate)
        {
            GameObject enemy = new GameObject();
            enemy.name = tester.objsToInstantiate[i];
            enemy.transform.position = tester.newObjectsData[i].pos;
           SpriteRenderer sprite=  enemy.AddComponent<SpriteRenderer>();
           CircleCollider2D cc= enemy.AddComponent<CircleCollider2D>();
            cc.isTrigger = true;
            ItemPickup item= enemy.AddComponent<ItemPickup>();
            item.item = tester.newObjectsData[i].item;
            item.count = tester.newObjectsData[i].counter;
            item.madeFromScratch = true;
         //   item.item.itemPrefab = Resources.Load<GameObject>("Items/Prefabs/" + item.item.itemType.ToString() + "/" + item.item.name);
            sprite.sprite = item.item.defaultSprite;
            sprite.sortingOrder = 5;
            item.enabled = true;
            enemy.tag = "Items";
            items.Add(enemy);
            enemy.layer = LayerMask.NameToLayer("Item");
            i++;
        }
        tester.pool.Clear();
        pooler.itemCache.Clear();
        pooler.sendit();

    }
}
