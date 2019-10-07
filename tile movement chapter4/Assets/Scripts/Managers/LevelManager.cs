using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    GameManager GM;

	// Use this for initialization
	void Start () {
        GM = GameManager.instance;
        GM.PlayerSetup(GameObject.FindGameObjectWithTag("Player"));
	}

}
