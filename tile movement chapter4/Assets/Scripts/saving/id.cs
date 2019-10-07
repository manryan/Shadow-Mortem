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

    public int saving;
     
    public string lastSceneSaved;

    public List<string> scenes;

    public List<string> scenesUnlocked;

    public List<string> fastTravelSpots;

    public bool fastTravelling;

    public Vector2 travelPosition;

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
                saving =0;
                lastSceneSaved = "";
                scenesUnlocked = new List<string>();
                fastTravelSpots = new List<string>();

                SavePlayer(Application.persistentDataPath + "/gamesave.save" + saveIndex);

                SceneManager.LoadScene("Game");
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

        saving = data.saving;

       lastSceneSaved = data.lastSceneSaved;

        scenesUnlocked = data.scenesUnlocked;

        fastTravelSpots = data.fastTravelSpots;

        if(lastSceneSaved.Length>0)
        SceneManager.LoadScene(lastSceneSaved);
        else
        {
            SceneManager.LoadScene("Game");
        }
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
        for (int i = 0; i <= scenes.Count-1; i++)
        {
        PlayerPrefs.DeleteKey("itemSave" + scenes[i] + num);
        PlayerPrefs.DeleteKey("TestSave" + scenes[i] + num);

        }
        PlayerPrefs.DeleteKey("PlayerSave" + "IDFILLEDHERE" + "/Inventory" + num);
        PlayerPrefs.Save();
        File.Delete(Application.persistentDataPath + "/gamesave.save" + +num);
    }

    public static id instance
    {
        get { return _instance; }
        set { _instance = value; }
    }

}
