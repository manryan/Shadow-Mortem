using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Unit : Entity
{

    #region Variables

    public List<GameObject> enemiesInRange = new List<GameObject>();

    Vector3 touchStart;

    //Leveling
    public int level = 1;
    public float currentXp;
    public float lastLvlXp;
    public float xpToNextLvl;

    //Stats
    int _attack;
    int _accuracy;
    int _defense;
    int _agility;

    //EquipmentBonus
    float _attackAccuracyBonus;
    float _attackDamageBonus;
    float _agilityBonus;
    float _defenseBonus;

    public SearchPool sPool;

    public List<Node> burningNodes;

    Boundary camBoundary;

    public GameObject attackButton;

    public GameObject pickUpButton;

    public int movementState;

    public LayerMask enemy;

    public bool inControl = true;

    public float radius;

    public Transform xpBar;

    //Inventory System
    [System.NonSerialized]
    public InventorySystem inventory;
    Item tempItem;//For picking up items
    int tempItemCount;
    GameObject tempItemGameobject;

    //Equipment Manager
    [System.NonSerialized]
    public EquipmentManager equipmentSystem;

    //Reference to object pool
    [System.NonSerialized]
    public ObjectPoolingManager objPoolManager;

    //Test items

    public GameObject[] testItems;

    #endregion

    struct Boundary
    {
        public float Up, Down, Left, Right;

        public Boundary(float up, float down, float left, float right)
        {
            Up = up; Down = down; Left = left; Right = right;
        }
    }

    public void Start()
    {
        inventory = new InventorySystem();
        equipmentSystem = new EquipmentManager();
        equipmentSystem.Start();
        objPoolManager = GameManager.instance.objPoolManager;

        //Test Items
        for (int i = 0; i < testItems.Length; i++)
        {
            inventory.Add(testItems[i].GetComponent<ItemPickup>().item,17);
        }

        inventory.TestInventory();

        Invoke("setBounds",0.1f);

        currentPath = null;

        localScale = healthbar.localScale;
    }

    public void setBounds()
    {
        camBoundary = new Boundary(map.mapSizeY, 0, 0, map.mapSizeX);
    }

    public override void Attack()
    {
        inControl = false;

        base.Attack();

        //if enemy we attacked was asleep, wake them up
        if (targetNode.entity.GetComponent<EnemyEntity>().sleeping)       
            StartCoroutine(checkIfEnemyWasSleeping());

        if (targetNode.entity.GetComponent<EnemyEntity>().health <= 0)
            StartCoroutine(waitTillDone());
        else
            Control();
            

    }

    public IEnumerator waitTillDone()
    {
        yield return new WaitUntil(() => targetNode.entity.GetComponent<EnemyEntity>().done == true);
        targetNode.entity = null;
        targetNode.blocked = false;
        Control();
    }

    public override void Protect(int damage)
    {
        //check our/their chances of blocking

        if (Random.value * 100f < chanceToBlock)
        {
            popupPool.noDamageText(Color.white, "Blocked");
        }
        else
        {
            popupPool.ProduceText(Color.red, "-", damage);
            health -= damage;
            float p = health / maxHealth;
            float userHpBarLength = p;//P * (scale in x)
            localScale.x = userHpBarLength;
            healthbar.localScale = localScale;
            if(health<=0)
            {
                StartCoroutine(Die());
            }
            if(health>=maxHealth)
            {
                health = maxHealth;
                localScale.x = 1;
                healthbar.localScale = localScale;
            }
        }
    }

    public IEnumerator Die()
    {
        health = 0;
        float p = health / maxHealth;
        float userHpBarLength = p;//P * (scale in x)
        localScale.x = userHpBarLength;
        healthbar.localScale = localScale;
        //death animation maybe?

        //wait a bit

        //example - gameover screen etc.

        yield return null;
    }


    public IEnumerator checkIfEnemyWasSleeping()
    {
        //wait until control passes over

        yield return new WaitUntil(() => inControl);

            targetNode.entity.GetComponent<EnemyEntity>().awareness = 100;
            targetNode.entity.GetComponent<EnemyEntity>().checkAwareness();
    }


    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, 3, 11);
    }

    // Update is called once per frame
    void Update()
    {

        //Temporary to be deleted for testing purposes
        if(Input.GetKeyDown(KeyCode.X))
        {
            inventory.TestInventory();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Saving");
            Save();
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Loading");
            Load();
        }

        //Testing bs ends here

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
            Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, camBoundary.Left, camBoundary.Right), Mathf.Clamp(Camera.main.transform.position.y, camBoundary.Down, camBoundary.Up), -10);
        }
        zoom(Input.GetAxis("Mouse ScrollWheel"));



        if (currentPath != null)
        {

            int currNode = 0;

            while (currNode < currentPath.Count - 1)
            {

                Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
                    new Vector3(0, 0, -1f);
                Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
                    new Vector3(0, 0, -1f);

                Debug.DrawLine(start, end, Color.red);

                currNode++;
            }

        }
    }



    public IEnumerator move()
    {

         remainingMovement = moveSpeed;


        if (currentPath == null)
        {

            //currentNode.fireIndex = 0;

            Debug.Log(" already on our spot");
            //can use this to pick up items on our node/wait/ etc
            pickUp();


            yield break;
        }

        while (remainingMovement > 0)
        {
            if (currentPath[1].blocked == false)
            {

                Debug.Log("move");


            if (currentNode != null)
            {
                currentNode.blocked = false;
                currentNode.entity = null;
                currentNode.fireIndex = 0;
            }

                // Get cost from current tile to next tile
                remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);

                // Move us to the next tile in the sequence
                tileX = currentPath[1].x;
                tileY = currentPath[1].y;

                //  transform.up = map.TileCoordToWorldCoord(tileX, tileY) - transform.position;

                inControl = false;

                flipSprite();

                currentNode = currentPath[1];
                currentNode.blocked = true;
                currentNode.entity = gameObject;
                //   map.tileTypes[map.tiles[currentNode.x, currentNode.y]].isWalkable = false;

                // Remove the old "current" tile
                currentPath.RemoveAt(0);

                anim.Play("Walk");

                while (transform.position != map.TileCoordToWorldCoord(tileX, tileY))
                {
                    transform.position = Vector3.MoveTowards(transform.position, map.TileCoordToWorldCoord(tileX, tileY), 8 * Time.deltaTime);

                    //   transform.position = map.TileCoordToWorldCoord(tileX, tileY);   // Update our unity world position
                    Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);

                    yield return null;
                }

                // anim.Play("Idle");


                //give control to every other entity
                Control();

                if (currentPath != null)
                {
                    //if we clicked on wall, stop 1 node away

                    if (currentPath.Count == 2 && map.UnitCanEnterTile(targetNode.x, targetNode.y) == false)
                    {
                        currentPath = null;
                        anim.Play("Idle");
                        yield break;
                    }

                    if (currentPath.Count == 1)
                    {
                        // We only have one tile left in the path, and that tile MUST be our ultimate
                        // destination -- and we are standing on it!
                        // So let's just clear our pathfinding info.
                        currentPath = null;
                        anim.Play("Idle");
                        yield break;
                    }

                    //check if any nodes in our current path are blocked as long as its not our current node

                    foreach (Node node in currentPath)
                    {
                        if (node.blocked && node != currentNode)
                        {
                            if (node != targetNode)
                            {
                                anim.Play("Idle");
                                currentPath = null;
                                yield break;
                            }
                            else
                            {
                                //for handling attacking sleeping enemy if 1 node away from target

                               if (currentPath.Count < 3)
                                {
                                    anim.Play("Idle");
                                     yield return new WaitUntil(() => inControl);
                                  //  yield return new WaitUntil(() => targetNode.entity.GetComponent<EnemyEntity>().done);
                                    currentPath = null;
                                    Debug.Log("attack sleeping enemy");
                                    //call attack funt?
                                    Attack();

                                    yield break;
                                }
                            }
                        }


                    }

                }

                yield return new WaitUntil(() => inControl);
            }
            else
            {
                //if next node gets blocked, break out

                currentPath = null;
                remainingMovement = 0;
               // inControl = true;
                yield break;
            }

          //  yield return new WaitForSeconds(0.2f);
        }

    }

    public override void MoveNextTile()
    {
        StartCoroutine(move());
    }

    //check to see if we can cut grass, and make current node drop items from tiles loot table

    public void cutGrass()
    {
        if(currentNode.grass !=null)
        {
            currentNode.grass.SetActive(false);
            currentNode.cutGrass.SetActive(true);
            currentNode.grass = null;
            currentNode.lootDropItems();
        }
    }

    //passes control over to other entites, and checks current state/nodes etc

    public override void Control()
    {
        //lose control

        inControl = false;

        //check for trap and trap type :) no trap types for now other than fire trap

        if (currentNode.trap != null)
        {



            currentNode.trap.GetComponent<MeshRenderer>().enabled = true;
            //for fire trap
            currentNode.burning = true;
            //GameObject temp = Instantiate(fire, new Vector3(currentNode.x, currentNode.y, -2), Quaternion.identity);
            GameObject temp = map.pool.GetFire();
            temp.SetActive(true);
            temp.transform.position = new Vector3(currentNode.x, currentNode.y, -2);
            currentNode.holder = temp;
            currentNode.burnTime = 3;
           currentNode.fireIndex = 1;
            isBurning = true;
            burnCount = 3;
            playerFire.SetActive(true);
            currentNode.trap = null;

            currentPath = null;
            //reduce player health cause of trap
        }


        if(currentNode.stack.Count>0)
        {
            //display pickup item button + its sprite using peek?
        }
    
        //check if we're burning

        playerBurning();

        //see if we can cut grass

        cutGrass();

        //create method for controlling enemies in range, checking if sleeping or not, managing enemy awareness while asleep

        //incomplete

        StartCoroutine(enemies());

        //create method for every other entity not in range, 
        //if asleep increase awareness barely other wise make them wander to a random noeighbour node (long as its not a trap) - they can walk to an unwalkable area - making them move nowhere


        //show pickup button if there is anything to pick up in my current node

        displayPickUpButton();
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public IEnumerator enemies()
    {
        if(currentNode.burning || isBurning)
        {
            currentPath = null;
            yield return new WaitForSeconds(0.25f);
        }

        var theenemies = Physics.OverlapSphere(transform.position,radius, enemy).ToList();

         enemiesInRange = new List<GameObject>();

        foreach(Collider col in theenemies)
        {
            if(col.gameObject.GetComponent<EnemyEntity>()!=null)
            enemiesInRange.Add(col.gameObject);
        }

        //move enemies out of range


        foreach (GameObject enemy in map.thisEnemy)
        {
            EnemyEntity entity = enemy.GetComponent<EnemyEntity>();
            if (!enemiesInRange.Contains(enemy))
            {
                entity.moveOutOfRange();
               // if(entity.thoughtBubble.enabled)
              //  entity.RemoveEnemyHud();
            }
        }

            //move enemies in range

        foreach (GameObject enemy in enemiesInRange)
        {

            EnemyEntity entity = enemy.GetComponent<EnemyEntity>();

            if (entity.sleeping==false)
            {

                map.GeneratePathForEnemy(currentNode.x, currentNode.y, enemy);

                entity.MoveNextTile();

                //wait till its done moving/attacking
                if (entity.currentPath != null)
                {
                    if (entity.currentPath.Count == 2)
                        yield return new WaitUntil(() => entity.done);

                }
            }
            else
            {
                entity.awareness += (1 / Vector3.Distance(transform.position, entity.transform.position))*25;
                entity.ifBurning();
                entity.stateSprites();
            }

        }


        //check if there are enemies in neighbouring nodes, to set attack button

        displayAttButton();

        //return control

        inControl = true;
    }

    //check if we're on fire

    public void playerBurning()
    {

            burningNodes = new List<Node>();

            foreach (Node node in map.NodesToUse)
            {
                if (node.burning && node.fireIndex==0)
                {
                //if its ready to burn other tiles, add it to list of burning nodes

                    burningNodes.Add(node);
                      
                }
            }

            foreach (Node node in burningNodes)
            {
            //reduce node burntime

                    node.burnTime--;

                    if (node.burnTime <= 0)
                    {
                //set it to stop burning if burnt time is 0, and disactivate fire particle

                        node.burning = false;
                        node.holder.SetActive(false);
                    }

                    //neighbours to use are all neighbours that are walkable:)

            //use the burning nodes walkable neighbours

                foreach (Node neighbour in node.neighboursToUse)
                {
                //burn every walkable/burnable node that isn't burning/burnt and doesnt have a trap

                    if (neighbour.isBurnable == true && neighbour.burning == false && neighbour.trap == null)
                   {
                    neighbour.burning = true;
                    neighbour.isBurnable = false;
                    GameObject temp = map.pool.GetFire();
                    temp.SetActive(true);
                    temp.transform.position = new Vector3(neighbour.x, neighbour.y, -2);
                    neighbour.holder = temp;
                    neighbour.burnTime = 3;

                    //handle grass and cut grass removal, activate burnt grass sprite

                    if (neighbour.grass != null)
                      {
                        neighbour.grass.SetActive(false);
                        neighbour.burntGrass.SetActive(true);
                        neighbour.grass = null;
                      }
                      if (neighbour.cutGrass != null)
                      {

                        neighbour.cutGrass.SetActive(false);
                        neighbour.burntGrass.SetActive(true);
                        neighbour.cutGrass = null;
                       }
                    //get rid of every loot item sitting on the node
                    Stack<GameObject> nonBurnableStack = new Stack<GameObject>();
                    for (int i = 0; i < neighbour.stack.Count; i+=0)
                    {
                        if (neighbour.stack.ElementAt(i).gameObject.GetComponent<ItemPickup>().item.itemType == ItemType.Consumables || neighbour.stack.ElementAt(i).gameObject.GetComponent<ItemPickup>().item.itemType == ItemType.Water)
                        {
                            neighbour.stack.ElementAt(i).gameObject.SetActive(false);
                            neighbour.stack.Pop();
                        }
                        else
                        {
                            nonBurnableStack.Push(neighbour.stack.ElementAt(i).gameObject);
                            neighbour.stack.ElementAt(i).GetComponent<SpriteRenderer>().sortingOrder = nonBurnableStack.Count + 2;
                            neighbour.stack.Pop();
                        }
                    }
                    //nonBurnableStack.Reverse();
                    neighbour.stack = nonBurnableStack;

                }
            }
        
        }
        enemyBurning();
    }

    //search all neigbouring nodes for traps, then set them to uncovered and enable their mesh

        //incomplete - need particle effect for discovering a trap :)

    public void Search()
    {
        if (inControl)
        {
            foreach (Node node in currentNode.neighbours)
            {
                GameObject sSprite = sPool.GetSprites();
                sSprite.transform.position = new Vector3(node.x, node.y, -1);
                sSprite.SetActive(true);

                //make search sprite shrink then disable
                StartCoroutine(disableSsprites(sSprite));

                if (node.trap != null && node.uncovered == false)
                {
                    node.uncovered = true;
                    node.trap.GetComponent<MeshRenderer>().enabled = true;
                }
            }

            //pass control over
            Control();
        }
    }

    //search sprite shrink and disable

    public IEnumerator disableSsprites(GameObject sSprite)
    {
        while(sSprite.transform.localScale.x>0.3f)
        {
            sSprite.transform.localScale = new Vector3(sSprite.transform.localScale.x - Time.deltaTime, sSprite.transform.localScale.y - Time.deltaTime, 1);
            yield return null;
        }
        //return to regular size before disabling
        sSprite.transform.localScale = new Vector3(1,1, 1);
        sSprite.SetActive(false);
    }

    //pick up items

    //incomplete - need to add to inventory etc/ also need pickup item button and current node check function at the end of control function to see if there is something we can pick up

    public void pickUp()
    {
        if (currentNode.stack.Count > 0)
        {
            tempItemGameobject = currentNode.stack.Peek().gameObject;

            if(tempItemGameobject == null)
            {
                Debug.LogError("Player is picking up an empty gameobject somehow");
            }

            //check if we have enough inventory space
            //if true do this

            //check to see if we either consume it(dew), or add it to vial in inventory if health full? or add to invo

            //remove it from stack
            currentNode.stack.Pop();

            ////decrement nodes index for sprite rendering order
            //currentNode.index--;

            //check if there are still any items left on my node

            displayPickUpButton();

            //Add to inventoy Here
            objPoolManager.ObjectPoolAddition(tempItemGameobject);

            if (tempItemGameobject.GetComponent<ItemPickup>() == null)
            {
                Debug.Log(tempItemGameobject.name + "Error, Item Pickup Is not implimented onto this object");
            }
            if (tempItemGameobject.GetComponent<ItemPickup>().item == null)
            {
                Debug.Log(tempItemGameobject.name + "Error, The Item in Item Pickup component is null on this prefab object");
            }

            tempItem = tempItemGameobject.GetComponent<ItemPickup>().item;
            tempItemCount = tempItemGameobject.GetComponent<ItemPickup>().count;
            if(tempItemCount <= 0)
            {
                Debug.LogError(tempItemGameobject.name + " item pickup count is 0 or less then");
            }

            inventory.Add(tempItem, tempItemCount);

            //pass control to other entities if we picked something up?
            Control();

        }
    }

    //wait function that passes control and has a popup text with "..."

    public void Wait()
    {
        if (inControl)
        {
            popupPool.noDamageText(Color.white, "...");

            Control();
        }
    }
    
    //set attack button if there are enemies in the walkable neigbhour nodes

    public void displayAttButton()
    {
        if (attackButton.activeInHierarchy == false)
        {
            foreach (Node neighbour in currentNode.neighboursToUse)
            {
                if (neighbour.blocked)
                {
                    attackButton.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            foreach (Node neighbour in currentNode.neighboursToUse)
            {
                attackButton.SetActive(false);
                if (neighbour.blocked)
                {
                    attackButton.SetActive(true);
                    break;
                }
            }
        }
    }

    //attack button function that sets a target and attacks

    public void attButtonClicked()
    {
        if (inControl)
        {
            foreach (Node neighbour in currentNode.neighboursToUse)
            {
                if (neighbour.blocked)
                {
                    targetNode = neighbour;

                    break;
                }
            }
            Attack();
        }
    }

    //show pick up button if theres an item on my node

    public void  displayPickUpButton()
    {
        //check if currentnode.stack.count >0 then use similar code to attack button to display 
        if(currentNode.stack.Count>0)
        {
            pickUpButton.SetActive(true);

            //set its foreground image to that of the item we're standing on
            pickUpButton.transform.GetChild(1).GetComponent<Image>().sprite = currentNode.stack.Peek().GetComponent<SpriteRenderer>().sprite;
        }
        else
            pickUpButton.SetActive(false);
    }

    //pickup function for pickupbutton

    public void pickupButtonClicked()
    {
        if (inControl)
            pickUp();
    }


    public void xpGain(float Experience)
    {
        currentXp += Experience;
        popupPool.ProduceText(Color.yellow, "+", Experience);

        //check if we leveled up & update xp bar
        lvlProgress();
    }

    public void lvlProgress()
    {
        float p = (currentXp - lastLvlXp) / (xpToNextLvl - lastLvlXp);
        float userHpBarLength = p;//P * (scale in x)
        localScale.x = userHpBarLength;
        xpBar.localScale = localScale;

        if(currentXp>=xpToNextLvl)
        {
            //last lvl xp needed is updated
            lastLvlXp = xpToNextLvl;

            //increase xp to next lvl 
            xpToNextLvl += (1.5f*xpToNextLvl);



             p = (currentXp - lastLvlXp) / (xpToNextLvl - lastLvlXp);
            userHpBarLength = p;//P * (scale in x)
            localScale.x = userHpBarLength;
            xpBar.localScale = localScale;
        }
    }

    #region Getter/Setters

    //Stats
    public int attack
    {
        get { return _attack + (int)_attackDamageBonus; }

        set { _attack = value; }
    }

    public int accuracy
    {
        get { return _accuracy + (int)_attackAccuracyBonus; }

        set { _accuracy = value; }
    }

    public int defense
    {
        get { return _defense + (int)_defenseBonus; }

        set { _defense = value; }
    }

    public int agility
    {
        get { return _agility + (int)_agilityBonus; }

        set { _agility = value; }
    }

    //EquipmentBonus
    public float attackAccuracyBonus
    {
        get { return _attackAccuracyBonus; }

        set { _attackAccuracyBonus = value; }
    }

    public float attackDamageBonus
    {
        get { return _attackDamageBonus; }

        set { _attackDamageBonus = value; }
    }

    public float agilityBonus
    {
        get { return _agilityBonus; }

        set { _agilityBonus = value; }
    }

    public float defenseBonus
    {
        get { return _defenseBonus; }

        set { _defenseBonus = value; }
    }

    #endregion

    #region Saving/Loading

    public void Save()
    {
        //string saveInventory = JsonUtility.ToJson(inventory);
        //PlayerPrefs.SetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory", saveInventory);
        inventory.Save();
    }

    public void Load()
    {
        //string loadInventory = PlayerPrefs.GetString("PlayerSave" + "IDFILLEDHERE" + "/Inventory");
        //inventory = JsonUtility.FromJson<InventorySystem>(loadInventory);
        inventory.Load();
    }

    #endregion

}

