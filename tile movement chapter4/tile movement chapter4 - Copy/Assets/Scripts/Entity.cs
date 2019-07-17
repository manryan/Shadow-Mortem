using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public TileMap map;

    public List<Node> currentPath = null;

    public int moveSpeed = 1;

    [SerializeField]
    public Node currentNode;

    [SerializeField]
    public Node targetNode;


    public virtual void MoveNextTile()
    { }

    public virtual void Control()
    { }

    public float health;

    public int maxHealth;

    public Vector3 localScale;

    public Transform healthbar;

    public PopupPool popupPool;

    public GameObject playerFire;

    public float remainingMovement;

    public float chanceToBlock;

    public float chanceToHit;

    public int maxHit;

    public int Damage;


    public SpriteRenderer playerSprite;

    public Animator anim;


    public bool isBurning;

    public int burnCount;


    public virtual void Attack()
    {
        //handle sprite flipping to face target

        faceAttackedEnemy();

        //play slash animation
        anim.Play("Slash");

        //check our odds of hitting - otherwise we missed

        if (Random.value * 100f < chanceToHit)
        {
            Damage = Random.Range(0, maxHit);

            targetNode.entity.GetComponent<Entity>().Protect(Damage);
        }
        else
        {
            targetNode.entity.GetComponent<Entity>().popupPool.noDamageText(Color.white, "Missed");
        }
        
        //pass control over

       // Control();
    }


    public abstract void Protect(int damage);


    //handle flip towards target attacked

    public void faceAttackedEnemy()
    {
        if (targetNode.x > currentNode.x)
        {
            if (playerSprite.flipX == true)
            {
                playerSprite.flipX = false;
            }
        }
        if (targetNode.x < currentNode.x)
        {
            if (playerSprite.flipX == false)
            {
                playerSprite.flipX = true;
            }
        }
    }

    //handle flip towards next node in path

    public void flipSprite()
    {
        if (currentPath[1].x > currentPath[0].x)
        {
            if (playerSprite.flipX == true)
            {
                playerSprite.flipX = false;
            }
        }
        if (currentPath[1].x < currentPath[0].x)
        {
            if (playerSprite.flipX == false)
            {
                playerSprite.flipX = true;
            }
        }
    }



    public void enemyBurning()
    {
        //if my current node is burning, reset my burnt Count

        if (currentNode.burning)
        {
            burnCount = 4;
            playerFire.SetActive(true);
            isBurning = true;
        }



        if (isBurning)
        {
            //if burning and we touch water, remove burn

            if (map.tiles[currentNode.x, currentNode.y] == 3)
            {
                isBurning = false;
                playerFire.SetActive(false);
                return;
            }

            //if our burn Count is greater than 0, handle damage+ text popup
            //otherwise, remove burn and dont deal damage


            if (burnCount > 0)
            {

                //plsy idle if burnt
                anim.Play("Idle");

                burnCount--;
                //reduce health

                int burnDamage = Random.Range(1, 4);


                Debug.Log("gotBurnt");
                //show hitsplat
                popupPool.ProduceText(Color.red, "-", burnDamage);

                // stop unit by resetting out path

                remainingMovement = 0;

                //currentPath = null;

            }
            else
            {
                isBurning = false;
                playerFire.SetActive(false);

                //return so we cant burn other nodes
                return;
            }

            //if our node can get burnt and we're burning, let it catch fire, and set its burnt time and my burn count to 3

            if (currentNode.isBurnable)
            {
                currentNode.burning = true;
                currentNode.isBurnable = false;
                GameObject temp = map.pool.GetFire();
                temp.SetActive(true);
                temp.transform.position = new Vector3(currentNode.x, currentNode.y, -2);
                currentNode.holder = temp;
                currentNode.burnTime = 3;
                burnCount = 3;

                //activate burnt grass sprite and remove all other grass
                if (currentNode.burntGrass != null)
                {
                    currentNode.grass.SetActive(false);
                    currentNode.burntGrass.SetActive(true);
                    currentNode.grass = null;
                }
            }


        }
    }

}
