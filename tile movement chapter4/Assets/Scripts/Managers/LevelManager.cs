using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    GameManager GM;
    string levelName;

	// Use this for initialization
	void Awake () {
        GM = GameManager.instance;
        GM.PlayerSetup(GameObject.FindGameObjectWithTag("Player"));
	}



}
