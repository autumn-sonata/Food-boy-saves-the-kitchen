using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    public float playerSpeed = 7f;
    public Transform destination;
    public LayerMask collisionHeavy;
    public LayerMask collisionPush;
    private float spriteSize;
    private float timeDelayPerMovement = 0.1f;
    private float timeToMoveAgain = 0f;
    private GameObject frontStack; //specifies the gameobject in the front of the queue of pushed objects
    private List<GameObject> playerToggle; //for toggling player on and off if player game object is also being pushed

    // Start is called before the first frame update
    void Start()
    {
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        destination.SetParent(null);
        frontStack = gameObject;
        playerToggle = new List<GameObject>();
    }

    void Update()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");
        if (gameObject.tag == "Player" && NoPlayerPushingBehind(horizontalMove, verticalMove))
        { 
            //only run if the opposite direction has no players
            transform.position = Vector3.MoveTowards(transform.position, destination.position, spriteSize * Time.deltaTime * playerSpeed);
            if (Vector3.Distance(destination.position, transform.position) == 0f)
            {
                NoLongerPush();
                //update destination position
                if (Mathf.Abs(horizontalMove) == 1f && Time.time >= timeToMoveAgain)
                {
                    CollideWithPush(horizontalMove, true);
                    //CheckChildIsPlayer(horizontalMove, true);
                    CollideWithHeavy(horizontalMove, 0f);
                }
                else if (Mathf.Abs(verticalMove) == 1f && Time.time >= timeToMoveAgain)
                {
                    CollideWithPush(verticalMove, false);
                    //CheckChildIsPlayer(verticalMove, false);
                    CollideWithHeavy(0f, verticalMove);
                }
                
            }
            else
            {
                timeToMoveAgain = Time.time + timeDelayPerMovement;
            }
        }
    }

    private void CollideWithHeavy(float horizontalMove, float verticalMove)
    {
        //check destination of both gameObject and frontStack destination.
        if (!Physics2D.OverlapCircle(frontStack.GetComponent<MovementPlayer>().destination.position + new Vector3(horizontalMove, verticalMove, 0f), .3f, collisionHeavy))
        {
            //update if what is in front is not heavy
            Vector3 vecCheckDir = new Vector3(horizontalMove * spriteSize, verticalMove * spriteSize, 0f);
            Vector3 checkPush = transform.position;
            while (Physics2D.OverlapPoint(checkPush, collisionPush))
            {
                Physics2D.OverlapPoint(checkPush, collisionPush).GetComponent<MovementPlayer>().destination.position += vecCheckDir;
                checkPush += vecCheckDir;
            }
        }
    }

    private void CollideWithPush(float move, bool isHorz)
    {
        //Make all pushable objects in front a child of current player
        Vector2 vecCheckDir = isHorz ? new Vector2(move, 0f) : new Vector2(0f, move);
        Vector2 checkPush = new Vector2(transform.position.x, transform.position.y) + vecCheckDir;

        while (Physics2D.OverlapPoint(checkPush, collisionPush))
        {
            Collider2D objAtPoint = Physics2D.OverlapPoint(checkPush, collisionPush);
            if (objAtPoint.tag == "Player")
            {
                //Checks if any collisionPush object is in the way and if it is a player, make it a non-player
                GameObject youTile = GameObject.Find("YouTile");
                objAtPoint.tag = youTile.GetComponent<YouTileProperties>().originalTag;
                objAtPoint.GetComponent<MovementPlayer>().destination.position = new Vector3(checkPush.x, checkPush.y, 0f);
                playerToggle.Add(objAtPoint.gameObject);
            }
            Physics2D.OverlapPoint(checkPush, collisionPush).transform.parent = gameObject.transform;
            frontStack = Physics2D.OverlapPoint(checkPush, collisionPush).gameObject;
            checkPush += vecCheckDir;
        }
    }
    private void NoLongerPush()
    {
        //Makes all children who were made non-players players again
        if (playerToggle.Count != 0)
        {
            foreach (GameObject player in playerToggle)
            {
                player.tag = "Player";
            }
            playerToggle.Clear();
        }
        transform.DetachChildren();
        frontStack = gameObject;
    }

    private bool NoPlayerPushingBehind(float horizontalMove, float verticalMove)
    {
        //checks if there is another object behind it that is a player
        Vector2 vecCheckDir = Mathf.Abs(horizontalMove) == 1 ? new Vector2(-horizontalMove, 0f) : new Vector2(0f, -verticalMove);
        Vector2 checkPush = new Vector2(transform.position.x, transform.position.y) + vecCheckDir;
        while (Physics2D.OverlapPoint(checkPush, collisionPush) && vecCheckDir != new Vector2(0f, 0f))
        {
            Collider2D objAtPoint = Physics2D.OverlapPoint(checkPush, collisionPush);
            if (objAtPoint.tag == "Player" && objAtPoint.GetComponent<MovementPlayer>().enabled == true) return false;
            checkPush += vecCheckDir;
        }
        return true;
    }
}
