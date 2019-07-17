using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{


    public GameObject fire;

    public int pooledAmount;

    public List<GameObject> fires;

    public bool shouldExpand = true;

    void Start()
    {
        fires = new List<GameObject>();


    }

    public GameObject GetFire()
    {
        for (int i = 0; i < fires.Count; i++)
        {
            //is it active in scene?
            if (!fires[i].activeInHierarchy)
            {
                return fires[i];
            }
        }
        if (shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(fire);
            //  obj.SetActive(false);
            fires.Add(obj);
            return obj;
        }
        else
        {
            return null;
        }
    }

}