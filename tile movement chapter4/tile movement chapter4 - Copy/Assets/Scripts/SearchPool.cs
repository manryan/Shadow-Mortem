using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPool : MonoBehaviour {

    public GameObject searchSprite;

    public int pooledAmount;

    public List<GameObject> sSprites;

    public bool shouldExpand = true;

    void Start()
    {
        sSprites = new List<GameObject>();


    }

    public GameObject GetSprites()
    {
        for (int i = 0; i < sSprites.Count; i++)
        {
            //is it active in scene?
            if (!sSprites[i].activeInHierarchy)
            {
                return sSprites[i];
            }
        }
        if (shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(searchSprite);
            //  obj.SetActive(false);
            sSprites.Add(obj);
            return obj;
        }
        else
        {
            return null;
        }
    }

}