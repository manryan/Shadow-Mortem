using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveHandler : MonoBehaviour {

    public Button[] buttons;

   /* public List<QuestHolders> questholders;

    [System.Serializable]
    public class QuestHolders
    {
        public List<QuestHolder> quests;
    }*/

    // Use this for initialization
    void Start () {
            for (int i = 0; i < 3; i++)
            {
                 if (File.Exists(Application.persistentDataPath + "/gamesave.save" + (i + 1)))
                 {
                buttons[i].gameObject.SetActive(true);
                Text text = buttons[i].transform.GetChild(0).GetComponent<Text>();

                PlayerData data = SaveSystem.LoadPlayer(Application.persistentDataPath + "/gamesave.save" + (i + 1));

                text.text = data.time.ToString("dd/MM/yyyy") + " Time: " + data.time.ToString("HH:mm:ss tt");

                 }
            }
    }

  /*  public void substractit(DateTime date)
    {
        TimeSpan newtime = date.Subtract(DateTime.Now);
        Debug.Log(newtime.TotalSeconds);
        Debug.Log(DateTime.Now.ToString());
    }*/

    public void newGame()
    {
        id.instance.startNewGame();
    }

    public void loadPlayer(int player)
    {
        id.instance.LoadPlayer(player);
    }

    public void deleteFile(int num)
    {
        buttons[num-1].gameObject.SetActive(false);
        id.instance.deleteSavedFile(num);
    //    resetQuests(num-1);
    }

   /* public void resetQuests(int num)
    {
        foreach (QuestHolder quest in questholders[num].quests)
        {
            quest.completed = false;
            quest.started = false;
            quest.stepNum = 0;
            quest.textIndex = 0;
            foreach (npcData data in quest.npcDataStorage)
            {
                data.stepNum = 0;
            }
        }
    }*/

}
