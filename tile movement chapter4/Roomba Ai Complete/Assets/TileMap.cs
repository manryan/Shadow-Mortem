using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileMap : MonoBehaviour {

	public GameObject selectedUnit;

    public Unit unit;

	public TileType[] tileTypes;

	int[,] tiles;
	Node[,] graph;


	int mapSizeX = 25;
	int mapSizeY = 25;

    public int garbageQuantity = 0;

    public List<Node> shortGraph;

    public int sgSize;

    public GameObject garbageBags;

	void Start() {
        // Setup the selectedUnit's variable
        unit = selectedUnit.GetComponent<Unit>();
		selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
		selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
		selectedUnit.GetComponent<Unit>().map = this;

		GenerateMapData();
		GeneratePathfindingGraph();
		GenerateMapVisual();

      //  shortGraph = graph.Cast<Node>().ToList();
	}

    public void UpdateCall()
    {
        foreach (Node node in shortGraph.ToList())
        {
            if (node.visisted == true)
                shortGraph.Remove(node);
        }

        sgSize = shortGraph.Count;

        //  finished = true;
        //  SearchedEverything();
    }

    public bool finished;

    public void SearchedEverything()
    {
        finished = true;

    }

    public int index;

	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX,mapSizeY];

		int x,y;

        int mapCnt = mapSizeX * mapSizeY;


            for (y = 0; y <mapSizeY; y++)
            {
                tiles[0, y] = 0;
            }

        for (x = 2; x < mapSizeX; x++)
        {
            for (y = 1; y < mapSizeY; y++)
            {
                int rand = Random.Range(0, 3);
                tiles[x, y] = rand;
                if (rand == 0)
                {
                    if (index == 2 && garbageQuantity < 20)
                    {
                        Instantiate(garbageBags, new Vector3(x, y, -1), Quaternion.identity);
                        index = 0;
                        garbageQuantity++;
                    }
                    else
                        index++;
                }
            }
        }

       // tiles[0, 0] = 0;
            tiles[1, 0] = 0;
          //  tiles[0, 1] = 0;
            tiles[1, 1] = 0;




		

		

        /*
		// Initialize our map tiles to be grass
		for(x=0; x < mapSizeX; x++) {
			for(y=0; y < mapSizeX; y++) {
				tiles[x,y] = 0;
			}
		}

		// Make a big swamp area
		for(x=3; x <= 5; x++) {
			for(y=0; y < 4; y++) {
				tiles[x,y] = 1;
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
        */
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

	void GeneratePathfindingGraph() {
		// Initialize the array
		graph = new Node[mapSizeX,mapSizeY];

		// Initialize a Node for each spot in the array
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {
				graph[x,y] = new Node();
				graph[x,y].x = x;
				graph[x,y].y = y;
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
        //shortGraph = graph.Cast<Node>().ToList();
        shortGraph = new List<Node>();
        foreach (Node node in graph)
        {
            if (UnitCanEnterTile(node.x, node.y) == true)
            {
                shortGraph.Add(node);
            }
        }
        StartCoroutine("Traverse");
    }

	void GenerateMapVisual() {
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeX; y++) {
				TileType tt = tileTypes[ tiles[x,y] ];
				GameObject go = (GameObject)Instantiate( tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity );

				ClickableTile ct = go.GetComponent<ClickableTile>();
				ct.tileX = x;
				ct.tileY = y;
				ct.map = this;
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

    public float baseTimer;


    public IEnumerator Traverse()
    {
        while (!finished)
        {

            foreach (Node node in shortGraph.ToList())
            {

                if (UnitCanEnterTile(node.x, node.y) == true && node.visisted == false)
                {
                        GeneratePathTo(node.x, node.y);

                       while (unit.currentPath!=null)
                      {
                        sgSize = shortGraph.Count;
                        unit.MoveNextTile();
                          yield return new WaitForSeconds(0.1f);
                      }
                }
                yield return new WaitForSeconds(baseTimer);
                baseTimer = 0;
                yield return new WaitForSeconds(0);
            }
        }
    }



	public void GeneratePathTo(int x, int y) {
		// Clear out our unit's old path.
		selectedUnit.GetComponent<Unit>().currentPath = null;

		if( UnitCanEnterTile(x,y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();
		
		Node source = graph[
		                    unit.tileX, 
		                    unit.tileY
		                    ];
		
		Node target = graph[
		                    x, 
		                    y
		                    ];
		
		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

		// Right now, currentPath describes a route from out target to our source
		// So we need to invert it!

		currentPath.Reverse();

		unit.currentPath = currentPath;
	}

}
