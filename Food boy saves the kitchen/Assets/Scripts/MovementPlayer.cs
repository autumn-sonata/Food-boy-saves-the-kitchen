using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    public float playerSpeed = 7f;
    public Transform destination;
    private LayerMask collisionHeavy;
    private LayerMask collisionPush;
    private float spriteSize;
    private readonly float timeDelayPerMovement = 0.1f;
    private float timeToMoveAgain = 0f;
    private GameObject frontStack; //specifies the gameobject in the front of the queue of pushed objects

    // Start is called before the first frame update
    void Start()
    {
        destination.SetParent(null);
        collisionHeavy = LayerMask.GetMask("Heavy");
        collisionPush = LayerMask.GetMask("Push");
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        frontStack = gameObject;
    }

    void Update()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");

        if (GetComponent<FoodTags>().isPlayer() && NoPlayerPushingBehind(horizontalMove, verticalMove))
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
                    CollideWithHeavy(horizontalMove, 0f);
                }
                else if (Mathf.Abs(verticalMove) == 1f && Time.time >= timeToMoveAgain)
                {
                    CollideWithPush(verticalMove, false);
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
    public bool isAtDestination()
    {
        return destination.position == transform.position;
    }

    private void CollideWithPush(float move, bool isHorz)
    {
        //Make all pushable objects in front (relative to direction of movement) a child of the current player
        Vector2 vecCheckDir = isHorz ? new Vector2(move, 0f) : new Vector2(0f, move);
        Vector2 checkPush = getCheckPush(vecCheckDir);

        while (Physics2D.OverlapPoint(checkPush, collisionPush))
        {
            Collider2D objAtPoint = Physics2D.OverlapPoint(checkPush, collisionPush);
            if (objAtPoint.GetComponent<FoodTags>().isPlayer())
            {
                //Checks if any collisionPush object is in the way and if it is a player, make it a non-player
                objAtPoint.GetComponent<FoodTags>().disablePlayerTag();
                objAtPoint.GetComponent<MovementPlayer>().destination.position = new Vector3(checkPush.x, checkPush.y, 0f); //TODO: WHAT IS THIS FOR I FORGOR
            }
            objAtPoint.transform.parent = gameObject.transform;
            frontStack = objAtPoint.gameObject;
            checkPush += vecCheckDir;
        }
    }
    private void NoLongerPush()
    {
        /* Makes all children who were made non-players players again
         * Checks object on the WinTile. All supposed players will have their player tag enabled.
         */
        foreach (GameObject food in GameObject.FindGameObjectsWithTag(GameObject.Find("YouTile").GetComponent<YouTileProperties>()
            .getObjOnYouTile().GetComponent<FoodTags>().getFoodName()))
        {
            food.GetComponent<FoodTags>().enablePlayerTag();
        }
        transform.DetachChildren();
        frontStack = gameObject;
    }

    private bool NoPlayerPushingBehind(float horizontalMove, float verticalMove)
    {
        //checks if there is another object behind it that is a player
        Vector2 vecCheckDir = Mathf.Abs(horizontalMove) == 1 ? new Vector2(-horizontalMove, 0f) : new Vector2(0f, -verticalMove);
        Vector2 checkPush = getCheckPush(vecCheckDir);
        while (Physics2D.OverlapPoint(checkPush, collisionPush) && vecCheckDir != new Vector2(0f, 0f))
        {
            //TODO: SECOND CONDITION REMOVED ONCE INPUT.GETAXISRAW IS FIXED TO TAKE IN VALUES ONLY WHEN DESTINATION IS REACHED
            Collider2D objAtPoint = Physics2D.OverlapPoint(checkPush, collisionPush);
            if (objAtPoint.GetComponent<FoodTags>().isPlayer() && isMovementEnabled(objAtPoint)) return false;
            checkPush += vecCheckDir;
        }
        return true;
    }

    private bool isMovementEnabled(Collider2D objAtPoint)
    {
        return objAtPoint.GetComponent<MovementPlayer>().enabled;
    }

    private Vector2 getCheckPush(Vector2 vecCheckDir)
    {
        return new Vector2(transform.position.x, transform.position.y) + vecCheckDir;

    }
}