using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupPool : MonoBehaviour {

    public GameObject popup;

    public int pooledAmount;

    public List<GameObject> popups;

    public bool shouldExpand = true;

    void Start()
    {
        popups = new List<GameObject>();


    }

    public GameObject GetPopup()
    {
        for (int i = 0; i < popups.Count; i++)
        {
            //is it active in scene?
            if (!popups[i].activeInHierarchy)
            {
                return popups[i];
            }
        }
        if (shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(popup);
            //  obj.SetActive(false);
            popups.Add(obj);
            return obj;
        }
        else
        {
            return null;
        }
    }

    public void ProduceText(Color color , string display ,float Damage)
    {
        GameObject hit = GetPopup();
        hit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, -2);
        hit.transform.SetParent(transform);
        hit.SetActive(true);
        TextMeshPro mesh = hit.GetComponent<TextMeshPro>();

        //Set Text And Color Of Popup
        mesh.text = display + Damage;
        mesh.color = color;
        mesh.fontSize = 6.5f;
        mesh.alpha = 1;

                mesh.alignment = TMPro.TextAlignmentOptions.Center;
        mesh.sortingOrder = 200;

        //Add Clone To List And Start The Wait Coroutine
        popups.Add(hit);
        StartCoroutine(Wait(hit));
        StartCoroutine(Move(hit));
    }


    public void noDamageText(Color color, string display)
    {
        GameObject hit = GetPopup();
        hit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, -2);
        hit.transform.SetParent(transform);
        hit.SetActive(true);
        TextMeshPro mesh = hit.GetComponent<TextMeshPro>();

        //Set Text And Color Of Popup
        mesh.text = display;
        mesh.color = color;
        mesh.fontSize = 6.5f;
        mesh.alpha = 1;
        mesh.alignment = TMPro.TextAlignmentOptions.Center;
        mesh.sortingOrder = 200;

        //Add Clone To List And Start The Wait Coroutine
        popups.Add(hit);
        StartCoroutine(Wait(hit));
        StartCoroutine(Move(hit));
    }

    public IEnumerator Move(GameObject hit)
    {
        while(hit.activeInHierarchy)
        {
            hit.transform.position = new Vector3(transform.position.x, hit.transform.position.y + 0.01f, -2);
            TextMeshPro mesh = hit.GetComponent<TextMeshPro>();
            mesh.fontSize -= 0.05f;
            mesh.alpha -= 0.015f;
            yield return null;
        }
    }

    IEnumerator Wait(GameObject clone)
    {
        yield return new WaitForSeconds(1f);
        // popups.Remove(clone);
        clone.SetActive(false);
        clone.transform.SetParent(null);
       // Destroy(clone);
    }

}
