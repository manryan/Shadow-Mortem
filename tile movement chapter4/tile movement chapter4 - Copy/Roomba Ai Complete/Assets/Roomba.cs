using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Roomba : MonoBehaviour {

    public int garbage = 0;

    public TileMap map;

    public Unit unit;

    public float energy = 100;

    public bool inUse = true;

    public float energyDrain;

    public Text batteryText;

    public Text garbageText;


    public void Update()
    {
        if (inUse)
        {
            energy -= Time.deltaTime * energyDrain;
            batteryText.text = "Battery: " + energy.ToString("00.0") + "%";
        }
        if (energy <= 0)
        {
            inUse = false;
            map.finished = false;
        }
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Garbage")
        {
            if (garbage < 3)
            {
                garbage++;
                Destroy(c.gameObject);
                garbageText.text = "Garbage: " + garbage.ToString();
            }
            if (garbage == 3)
            {
                Destroy(c.gameObject);
                unit.currentPath = null;
                map.GeneratePathTo(0, 0);
                garbageText.text = "Garbage: " + garbage.ToString();
            }
        }

        if (c.gameObject.tag == "Base")
        {
            if (garbage > 2)
            {
                garbage = 0;
                garbageText.text = "Garbage: " + garbage.ToString();
            }

            if (energy < 20)
            {
                inUse = false;
                map.baseTimer = 3f;
                unit.currentPath = null;
                Invoke("regen", 3f);
            }
            //   unit.currentPath = null;
            // map.GeneratePathTo(0, 0);
        }
    }
        public void regen()
        {
            energy = 100f;
        inUse = true;
        }
    
}
