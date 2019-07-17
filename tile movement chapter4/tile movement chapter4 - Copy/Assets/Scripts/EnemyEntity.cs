using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{

    public GameObject player;

    public Unit unit;

    public bool alive = true;

    public int myTurn = 1;

    public float xpToGive;

    public float chanceToFallAsleep;

    public bool done;

    public bool sleeping;

    public float awareness;

    public Sprite sleep;

    public Sprite awake;

    public SpriteRenderer thoughtBubble;

    public GameObject healthbarBackground;

    public override void Protect(int damage)
    {
        done = false;

        //show health hud
        showHealthHud();


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
            float userHpBarLength = p;
            localScale.x = userHpBarLength;
            healthbar.localScale = localScale;
            if (health <= 0)
            {
                StartCoroutine(Die());
                return;
            }
            if (health >= maxHealth)
            {
                health = maxHealth;
                localScale.x = 1;
                healthbar.localScale = localScale;
            }
        }
        done = true;
    }

    public IEnumerator Die()
    {
        health = 0;
        float p = health / maxHealth;
        float userHpBarLength = p;//P * (scale in x)
        localScale.x = userHpBarLength;
        healthbar.localScale = localScale;

        //give player xptogive

        //death animation maybe?

        //wait a bit
        yield return new WaitForSeconds(0.5f);
        map.entity.xpGain(xpToGive);
        done = true;
        map.thisEnemy.Remove(gameObject);
        map.deadEnemies.Add(gameObject);
        gameObject.SetActive(false);

        //nullify his current node entity

      //  currentNode.entity = null;
        

        //make him drop his items? etc.
    }

    public void Update()
    {
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

    //enable thought bubble,and make it scale up and down

    public void stateSprites()
    {
        if(thoughtBubble.enabled ==true)
        {

        }
        else
        {
            if (sleeping)
            {
                thoughtBubble.sprite = sleep;
                StartCoroutine(asleep());
                thoughtBubble.enabled = true;
            }

        }
    }

    //show & hide health hud

    public void showHealthHud()
    {
        if(!healthbar.gameObject.activeInHierarchy)
        {
            healthbar.gameObject.SetActive(true);
            healthbarBackground.SetActive(true);
        }
    }

    public void hideHealthHud()
    {
        if (healthbar.gameObject.activeInHierarchy)
        {
            healthbar.gameObject.SetActive(false);
            healthbarBackground.SetActive(false);
        }
    }

    //check how aware we are while asleep

    public void checkAwareness()
    {
        if (awareness >= 100)
        {
            awareness = 0;
            sleeping = false;
            thoughtBubble.sprite = awake;
            StartCoroutine(alerted());
        }
    }

    //hide thought bubble

    public void RemoveEnemyHud()
    {
        if (thoughtBubble.enabled == true)
        {
            thoughtBubble.enabled = false;
            StopCoroutine(alerted());
        }
    }

    public IEnumerator asleep()
    {
        while (sleeping)
        {
            yield return repeatLerpSleep(new Vector3(0.65f, 0.65f, 0.65f), new Vector3(1, 1, 1), 2);
            yield return repeatLerpSleep( new Vector3(1, 1, 1), new Vector3(0.65f, 0.65f, 0.65f), 2);
        }
    }

    public IEnumerator repeatLerpSleep(Vector3 a, Vector3 b, float time)
    {
        float i = 0f;
        float rate  = (1f / time) * 2f;
        while(i<1f)
        {
            i += Time.deltaTime * rate;
            thoughtBubble.transform.localScale = Vector3.Lerp(a, b, i);
            yield return null;
        }
    }



    public IEnumerator alerted()
    {
        while (!sleeping)
        {
            yield return repeatLerp(new Vector3(0.65f, 0.65f, 0.65f), new Vector3(1, 1, 1), 1);
            yield return repeatLerp(new Vector3(1, 1, 1), new Vector3(0.65f, 0.65f, 0.65f), 1);
        }
    }

    public IEnumerator repeatLerp(Vector3 a, Vector3 b, float time)
    {
        float i = 0f;
        float rate = (1f / time) * 2f;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            thoughtBubble.transform.localScale = Vector3.Lerp(a, b, i);
            yield return null;
        }
    }
    public void moveOutOfRange()
    {
        //hide health hud
        hideHealthHud();

        if (sleeping)
        {
            awareness += Random.Range(1, 4);
            checkAwareness();

            //if current node is burning wake enemy up
            if (currentNode.burning)
            {
                awareness = 100;
                checkAwareness();
                enemyBurning();
            }
        }
        else
        { 


            if (currentPath != null)
            {
                //Debug.Log("whats going on");
                moveEnemiesOutOfRange();
            }
            else
            {
                //find a random spot

                Node randNode = map.NodesToUse[Random.Range(0, map.NodesToUse.Count)];

                //move enemies out of range only
                map.GeneratePathForEnemy(randNode.x, randNode.y, gameObject);

                moveEnemiesOutOfRange();
            }
        }

        //hide thought bubble if its visible
        if (thoughtBubble.enabled)
             RemoveEnemyHud();
    }

    //for moving enemies in range

    public override void MoveNextTile()
    {
        done = false;
                StartCoroutine(move());
    }

    public IEnumerator move()
    {
        //if thought bubble is active and enemy has a turn, hide it
        if(thoughtBubble.enabled)
        RemoveEnemyHud();

        float remainingMovement = moveSpeed;

        while (remainingMovement >0)
        {

            if (currentPath == null)
                break;
            if (currentPath.Count >= 3)
            {
                // Debug.Log("move");
             

                if (currentPath[1].blocked == false)
                {
                    if (currentNode != null)
                    {
                        currentNode.blocked = false;
                        currentNode.entity = null;
                    }

                    // Get cost from current tile to next tile
                    remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);

                    // Move us to the next tile in the sequence
                    tileX = currentPath[1].x;
                    tileY = currentPath[1].y;


                    flipSprite();

                    anim.Play("Walk");

                    currentNode = currentPath[1];

                    currentNode.blocked = true;

                    currentNode.entity = gameObject;

                    // Remove the old "current" tile
                    currentPath.RemoveAt(0);

                    while (transform.position != map.TileCoordToWorldCoord(tileX, tileY))
                    {
                        transform.position = Vector3.MoveTowards(transform.position, map.TileCoordToWorldCoord(tileX, tileY), 8 * moveSpeed * Time.deltaTime);

                        yield return null;
                    }
                    
                    if(currentPath.Count==2 || map.entity.remainingMovement < 1)
                    {
                        anim.Play("Idle");
                    }

                    //    transform.position = map.TileCoordToWorldCoord(tileX, tileY);   // Update our unity world position

                }
                else
                {
                    map.GeneratePathForEnemy(targetNode.x, targetNode.y, gameObject);
                    yield return null;
                }

            }
            else
            {


                //make attack funct with htis in it?
                //   map.entity.popupPool.ProduceText(Color.red, "-", Random.Range(0, 5));
                if (targetNode.entity != null)
                {
                    yield return new WaitForSeconds(0.3f);
                    map.entity.remainingMovement = 0;
                    map.entity.currentPath = null;
                    Attack();
                }
                // currentPath = null;

                done = true;
                break;
            }
        }
      //  anim.Play("Idle");
        enemyBurning();
        done = true;      
        // Debug.Log("check burn" + gameObject.name);

    }

   //moves enemies out of range

    public void moveEnemiesOutOfRange()
    {
        float remainingMovement = moveSpeed;


        while (remainingMovement > 0)
        {

            if (currentPath == null)
            {
                break;
            }
            if (currentPath.Count > 1)
            {
                // Debug.Log("move");


                if (currentPath[1].blocked == false)
                {

                    if (currentNode != null)
                    {
                        currentNode.blocked = false;
                        currentNode.entity = null;
                    }


                    // Get cost from current tile to next tile
                    remainingMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);

                    // Move us to the next tile in the sequence
                    tileX = currentPath[1].x;
                    tileY = currentPath[1].y;


                    flipSprite();

                    anim.Play("Walk");

                    currentNode = currentPath[1];

                    currentNode.blocked = true;

                    currentNode.entity = gameObject;

                    // Remove the old "current" tile
                    currentPath.RemoveAt(0);

                    StartCoroutine(moving());

                   // transform.position = map.TileCoordToWorldCoord(tileX, tileY);

                    if (currentPath.Count == 1 || map.entity.remainingMovement < 1)
                    {
                        anim.Play("Idle");
                    }

                    if (currentPath.Count == 1)
                    {
                        currentPath = null;
                    }

                    if (Random.value * 100f < chanceToFallAsleep)
                    {
                        sleeping = true;
                        //   currentPath = null;
                        break;
                    }

                    //    transform.position = map.TileCoordToWorldCoord(tileX, tileY);   // Update our unity world position

                }
                else
                {

                    Node randNode = map.NodesToUse[Random.Range(0, map.NodesToUse.Count)];

                    //move enemies out of range only
                    map.GeneratePathForEnemy(randNode.x, randNode.y, gameObject);
                }

            }
            else
            {




                //make attack funct with htis in it?
                //   map.entity.popupPool.ProduceText(Color.red, "-", Random.Range(0, 5));
                currentPath = null;
                //  Node randNode = map.NodesToUse[Random.Range(0, map.NodesToUse.Count)];
                //  map.GeneratePathForEnemy(randNode.x, randNode.y, gameObject);
                done = true;
                break;
            }
        }
        //  anim.Play("Idle");
        enemyBurning();
        done = true;
    }

 public IEnumerator moving()
    {
        while (transform.position != map.TileCoordToWorldCoord(tileX, tileY))
        {
             transform.position = Vector3.MoveTowards(transform.position, map.TileCoordToWorldCoord(tileX, tileY), 8 * moveSpeed * Time.deltaTime);
                 
             yield return null;
        }
    }

}