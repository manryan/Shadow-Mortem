using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node {
    [System.NonSerialized]
    public List<Node> neighbours;
    [System.NonSerialized]
    public List<Node> neighboursToUse;

    public int x;
	public int y;

    public Vector3 transform;

    public GameObject entity;

    public bool isBurnable;

    public bool burning;

    public bool blocked = false;

    public int burnTime;

    public GameObject holder;

    public bool isWater;

    public GameObject trap;

    public GameObject grass;

    public GameObject burntGrass;

    public GameObject cutGrass;

    public int fireIndex;

    public Stack<GameObject> stack = new Stack<GameObject>();

    public ClickableTile myTile;

    public int index;

    public bool uncovered;

    public Node() {
		neighbours = new List<Node>();
        neighboursToUse = new List<Node>();
    }
	

	public float DistanceTo(Node n) {
		if(n == null) {
			Debug.LogError("WTF?");
		}
		
		return Vector2.Distance(
			new Vector2(x, y),
			new Vector2(n.x, n.y)
			);
	}
	
    //public void PlayerDropItem()
    //{

    //}

    public void lootDropItems()
    {
        //manage render cue as well via a stack. push onto the stack using loot manager in myTile?
        myTile.spawnGrassLoot();

      //  foreach(GameObject item in stack)
      //  {
      //      stack.Pop();
       // }
    }


}