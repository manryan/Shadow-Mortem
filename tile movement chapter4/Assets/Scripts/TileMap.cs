using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileMap : MonoBehaviour {

	public GameObject selectedUnit;

    public List<GameObject> thisEnemy = new List<GameObject>();

    public List<GameObject> deadEnemies = new List<GameObject>();

    public TileType[] tileTypes;

	public int[,] tiles;
	public Node[,] graph;


    public List<Node> NodesToUse;


	public int mapSizeX = 10;
	public int mapSizeY = 10;

    public EffectPool pool;

    public Unit entity;

    void Start() {

     //   Time.timeScale = 0.2f;

        if(!GameManager.instance.turnLightsOn)
        RenderSettings.ambientLight = Color.black;

		// Setup the selectedUnit's variable


		GenerateMapData();
		GeneratePathfindingGraph();
		GenerateMapVisual();

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //add all enemies in game to list
        thisEnemy = enemies.ToList();

        entity.tileX = (int)selectedUnit.transform.position.x;
        entity.tileY = (int)selectedUnit.transform.position.y;
        entity.map = this;
        entity.currentNode = graph[entity.tileX, entity.tileY];
        entity.currentNode.entity = selectedUnit;
       // selectedUnit.GetComponent<Entity>().currentNode.blocked = true;

        foreach (GameObject obj in thisEnemy)
        {
            Entity enemyEntity = obj.GetComponent<EnemyEntity>();

            enemyEntity.tileX = (int)obj.transform.position.x;
            enemyEntity.tileY = (int)obj.transform.position.y;
            enemyEntity.map = this;
            enemyEntity.currentNode = graph[enemyEntity.tileX, enemyEntity.tileY];
            enemyEntity.currentNode.blocked = true;
            enemyEntity.currentPath = null;
        }

    }

	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX,mapSizeY];
		
		int x,y;
		
		// Initialize our map tiles to be grass
		for(x=0; x < mapSizeX; x++) {
			for(y=0; y < mapSizeX; y++) {
				tiles[x,y] = 0;
			}
		}

		// Make a big swamp area
		for(x=3; x <= 5; x++) {
			for(y=0; y < 4; y++) {
				tiles[x,y] = 3;
			}
		}
		
		// Let's make a u-shaped mountain range
		tiles[4, 4] = 2;
		tiles[5, 4] = 2;
		tiles[6, 4] = 2;
		tiles[7, 4] = 2;
		tiles[8, 4] = 2;

		tiles[4, 5] = 2;
		tiles[4, 6] = 2;
		tiles[8, 5] = 2;
		tiles[8, 6] = 2;

        tiles[7,9] = 1;
        tiles[7,8] = 1;
        tiles[7, 7] = 1;
        tiles[6,9] = 1;
        tiles[6,8] = 1;
        tiles[6, 7] = 1;
        tiles[5,9] = 1;
        tiles[5,8] = 1;
        tiles[5, 7] = 1;

        tiles[8, 8] = 4;

    }

	public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY) {

		TileType tt = tileTypes[ tiles[targetX,targetY] ];

		if(UnitCanEnterTile(targetX, targetY) == false)
			return Mathf.Infinity;

		float cost = tt.movementCost;

		if( sourceX!=targetX && sourceY!=targetY) {
			// We are moving diagonally!  Fudge the cost for tie-breaking
			// Purely a cosmetic thing!
			cost += 0.001f;
		}

		return cost;

    }

    public float UnitCostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {

        TileType tt = tileTypes[tiles[targetX, targetY]];
        if (UnitCanEnterTile(targetX, targetY) == false)
        {
            if (entity.targetNode != graph[targetX, targetY])
            {
                return Mathf.Infinity;
            }
            else
            {
                float cost = tt.movementCost;

                if (sourceX != targetX && sourceY != targetY)
                {
                    // We are moving diagonally!  Fudge the cost for tie-breaking
                    // Purely a cosmetic thing!
                    cost += 0.001f;
                }

                return cost;
            }
        }

        else
        {
            float cost = tt.movementCost;

            if (sourceX != targetX && sourceY != targetY)
            {
                // We are moving diagonally!  Fudge the cost for tie-breaking
                // Purely a cosmetic thing!
                cost += 0.001f;
            }

            return cost;
        }

    }

	void GeneratePathfindingGraph() {
		// Initialize the array
		graph = new Node[mapSizeX,mapSizeY];

		// Initialize a Node for each spot in the array
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {
				graph[x,y] = new Node();
				graph[x,y].x = x;
				graph[x,y].y = y;
                graph[x, y].transform = new Vector2(x, y);
                if(tiles[x,y] ==0)
                graph[x, y].isBurnable = true;
             //   if(x==9 && y==9)
             //   {
                   // GameObject temp = Instantiate(firetrap, new Vector3(x, y, -0.75f), Quaternion.identity);
                  //  graph[x, y].trap = temp;
                 //   temp.GetComponent<MeshRenderer>().enabled = false;
             //   }
			}
		}

		// Now that all the nodes exist, calculate their neighbours
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {

				// This is the 4-way connection version:
/*				if(x > 0)
					graph[x,y].neighbours.Add( graph[x-1, y] );
				if(x < mapSizeX-1)
					graph[x,y].neighbours.Add( graph[x+1, y] );
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );
*/

				// This is the 8-way connection version (allows diagonal movement)
				// Try left
				if(x > 0) {
					graph[x,y].neighbours.Add( graph[x-1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x-1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x-1, y+1] );
				}

				// Try Right
				if(x < mapSizeX-1) {
					graph[x,y].neighbours.Add( graph[x+1, y] );
					if(y > 0)
						graph[x,y].neighbours.Add( graph[x+1, y-1] );
					if(y < mapSizeY-1)
						graph[x,y].neighbours.Add( graph[x+1, y+1] );
				}

				// Try straight up and down
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );

				// This also works with 6-way hexes and n-way variable areas (like EU4)
			}
		}
        foreach(Node node in graph)
        {
            if(node.trap ==null && UnitCanEnterTile(node.x,node.y) ==true)
            {
                NodesToUse.Add(node);
            }
            foreach(Node neighbour in node.neighbours)
            {
                if (UnitCanEnterTile(neighbour.x, neighbour.y) == true)
                {
                    node.neighboursToUse.Add(neighbour);
                }
            }
        }
	}

	void GenerateMapVisual() {
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {
				TileType tt = tileTypes[ tiles[x,y] ];
				GameObject go = (GameObject)Instantiate( tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity );
				ClickableTile ct = go.GetComponent<ClickableTile>();
                if(ct.grass !=null)
                {
                    graph[x, y].grass = go.transform.GetChild(0).gameObject;
                    graph[x, y].burntGrass = go.transform.GetChild(1).gameObject;
                    graph[x, y].cutGrass = go.transform.GetChild(2).gameObject;
                }
                if (ct.holdsTraps)
                {
                    int checkTrapNum = Random.Range(0, 1);
                    if (ct.traps[checkTrapNum] != null)
                        graph[x, y].trap = ct.traps[checkTrapNum];
                  //  graph[x,y].uncovered = true;
                    Debug.Log("assign trap" + checkTrapNum);
                }
                    ct.tileX = x;
				ct.tileY = y;
				ct.map = this;
                ct.myNode = graph[x, y];
                graph[x, y].myTile = ct;
			}
		}
	}

	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3(x, y, 0);
	}

	public bool UnitCanEnterTile(int x, int y) {

		// We could test the unit's walk/hover/fly type against various
		// terrain flags here to see if they are allowed to enter the tile.

		return tileTypes[ tiles[x,y] ].isWalkable;
	}



    public void GeneratePathTo(int x, int y)
    {
        // Clear out our unit's old path.
        entity.currentPath = null;
       // selectedUnit.GetComponent<Entity>().currentNode.blocked = false;

       /* if (UnitCanEnterTile(x, y) == false)
        {
            // We probably clicked on a mountain or something, so just quit out.
            return;
        }*/

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        // Setup the "Q" -- the list of nodes we haven't checked yet.
        List<Node> unvisited = new List<Node>();

        Node source = graph[
                            entity.tileX,
                            entity.tileY
                            ];

        Node target = graph[
                            x,
                            y
                            ];
        entity.targetNode = target;

        dist[source] = 0;
        prev[source] = null;

        // Initialize everything to have INFINITY distance, since
        // we don't know any better right now. Also, it's possible
        // that some nodes CAN'T be reached from the source,
        // which would make INFINITY a reasonable value
        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            if (v.blocked == false || v == source)
            {
                if (v.trap != null)
                {
                    if(v.uncovered==false)
                    unvisited.Add(v);
                }
                else
                    unvisited.Add(v);

                if (v == target)
                    unvisited.Add(v);
            }
        }

        while (unvisited.Count > 0)
        {
            // "u" is going to be the unvisited node with the smallest distance.
            Node u = null;

            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;  // Exit the while loop!
            }

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                //float alt = dist[u] + u.DistanceTo(v);
                float alt = dist[u] + UnitCostToEnterTile(u.x, u.y, v.x, v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        // If we get there, the either we found the shortest route
        // to our target, or there is no route at ALL to our target.

        if (prev[target] == null)
        {
            // No route between our target and the source
            if (target == source)
            {
                Debug.Log("try to pick up");
                entity.moveSpeed = 1;
                entity.MoveNextTile();
            }
            return;
        }

        List<Node> currentPath = new List<Node>();

        Node curr = target;

        // Step through the "prev" chain and add it to our path
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }

        // Right now, currentPath describes a route from out target to our source
        // So we need to invert it!

        currentPath.Reverse();

        entity.currentPath = currentPath;

        if(UnitCanEnterTile(x,y))
        {
            entity.moveSpeed = currentPath.Count-1;
        }
        else
        {
            Debug.Log("not walkable");
            entity.moveSpeed = currentPath.Count - 2;
            if(entity.moveSpeed<=0)
            {
                entity.currentPath = null;
                return;
            }
        }

        if (target.blocked)
        {
            if (currentPath.Count < 3)
            {
                Debug.Log("player attacks");
                //call player att funct
                entity.currentPath = null;
                entity.Attack();

               // entity.currentPath = null;
            }
            else
            {
                Debug.Log("moves closer to enemy");
                entity.MoveNextTile();
            }
        }
        else
        {
            entity.MoveNextTile();
        }
    }



    public void GeneratePathForEnemy(int x, int y, GameObject enemy) {



          Entity entity = enemy.GetComponent<Entity>();

           entity.currentPath = null;
        

		if( UnitCanEnterTile(x,y) == false) {
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();
            Node source = graph[
                            entity.tileX,
                            entity.tileY
                            ];


            Node target = graph[
                                x,
                                y
                                ];

            entity.targetNode = target;

            dist[source] = 0;
            prev[source] = null;


            // Initialize everything to have INFINITY distance, since
            // we don't know any better right now. Also, it's possible
            // that some nodes CAN'T be reached from the source,
            // which would make INFINITY a reasonable value
            foreach (Node v in graph)
            {
                if (v != source)
                {
                    dist[v] = Mathf.Infinity;
                    prev[v] = null;
                }

                if (v.blocked == false || v == source || v == target)
                {
                    if(v.trap ==null)
                    unvisited.Add(v);
                }
            }

            while (unvisited.Count > 0)
            {
                // "u" is going to be the unvisited node with the smallest distance.
                Node u = null;

                foreach (Node possibleU in unvisited)
                {
                    if (u == null || dist[possibleU] < dist[u])
                    {
                        u = possibleU;
                    }
                }

                if (u == target)
                {
                    break;  // Exit the while loop!
                }

                unvisited.Remove(u);

                foreach (Node v in u.neighbours)
                {
                    //float alt = dist[u] + u.DistanceTo(v);
                    float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            // If we get there, the either we found the shortest route
            // to our target, or there is no route at ALL to our target.

            if (prev[target] == null)
            {
                // No route between our target and the source
                return;
            }

            List<Node> currentPath = new List<Node>();

            Node curr = target;

            // Step through the "prev" chain and add it to our path
            while (curr != null)
            {
                currentPath.Add(curr);
                curr = prev[curr];
            }

            // Right now, currentPath describes a route from out target to our source
            // So we need to invert it!

            currentPath.Reverse();

            entity.currentPath = currentPath;
    }

}
