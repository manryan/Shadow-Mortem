using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;using System.IO;

public class id : MonoBehaviour {

    public static id _instance = null;

    public float health;

    public int saveIndex;

    public float x;

    public float y;

    public DateTime time;



    public void SavePlayer(string path)
    {
        SaveSystem.SavePlayer(this, path);
    }


    public void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void startNewGame()
    {
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(Application.persistentDataPath + "/gamesave.save" + (i + 1)))
            { }
            else
            {
                health = 100;
                saveIndex = i + 1;

                SavePlayer(Application.persistentDataPath + "/gamesave.save" + saveIndex);

                SceneManager.LoadScene("_SCENE_");
                break;
            }
        }
    }

    public void LoadPlayer(int index)
    {
        saveIndex = index;


        PlayerData data = SaveSystem.LoadPlayer(Application.persistentDataPath + "/gamesave.save" + saveIndex);

        health = data.health;

        saveIndex = data.saveIndex;

        x = data.x;

        y = data.y;

        SceneManager.LoadScene("_SCENE_");
    }

   /* public void loadPlayer2(string path)
    {
        saveIndex  =2;
        SceneManager.LoadScene("test 1");

        PlayerData data = SaveSystem.LoadPlayer(path);

        health = data.health;

        saveIndex = data.saveIndex;

        x = data.x;

        y = data.y;
    }*/

    public void deleteSavedFile(int num)
    {
        PlayerPrefs.DeleteKey("itemSave" + num);
        PlayerPrefs.DeleteKey("PlayerSave" + "IDFILLEDHERE" + "/Inventory" + num);
        PlayerPrefs.DeleteKey("TestSave"+ num);
        PlayerPrefs.Save();
        File.Delete(Application.persistentDataPath + "/gamesave.save" + +num);
    }

    public static id instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

}
