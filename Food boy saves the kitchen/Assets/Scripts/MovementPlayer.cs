using System.Collections;
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
    
    // Start is called before the first frame update
    void Start()
    {
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        destination.SetParent(null);
        frontStack = gameObject;
    }

    void Update()
    {
        if (gameObject.tag == "Player")
        {
            float horizontalMove = Input.GetAxisRaw("Horizontal");
            float verticalMove = Input.GetAxisRaw("Vertical");

            transform.position = Vector3.MoveTowards(transform.position, destination.position, spriteSize * Time.deltaTime * playerSpeed);

            if (Vector3.Distance(destination.position, transform.position) == 0f)
            {
                //update destination position
                NoLongerPush();
                if (Mathf.Abs(horizontalMove) == 1f && Time.time >= timeToMoveAgain) 
                {
                    CollideWithPush(horizontalMove, true);
                    CollideWithHeavy(horizontalMove, 0f);
                } else if (Mathf.Abs(verticalMove) == 1f && Time.time >= timeToMoveAgain)
                {
                    CollideWithPush(verticalMove, false);
                    CollideWithHeavy(0f, verticalMove);
                }
            } else 
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
            Vector3 checkPush = new Vector3(transform.position.x, transform.position.y, 0f);
            while (Physics2D.OverlapPoint(checkPush, collisionPush))
            {
                Physics2D.OverlapPoint(checkPush, collisionPush).GetComponent<MovementPlayer>().destination.position =
                    Physics2D.OverlapPoint(checkPush, collisionPush).GetComponent<MovementPlayer>().transform.position + vecCheckDir;
                checkPush += vecCheckDir;
            }
            //destination.position = transform.position + vecCheckDir;
            //frontStack.GetComponent<MovementPlayer>().destination.position = 
            //    frontStack.GetComponent<MovementPlayer>().transform.position + new Vector3(horizontalMove * spriteSize, verticalMove * spriteSize, 0f);
        }
    }

    private void CollideWithPush(float move, bool isHorz)
    {
        //Make all pushable objects in front a child of current player
        Vector2 checkPush = new Vector2(transform.position.x, transform.position.y);
        Vector2 vecCheckDir;

        if (isHorz)
        {
            vecCheckDir = new Vector2(move, 0f);
        } else 
        {
            vecCheckDir = new Vector2(0f, move);
        }

        while (Physics2D.OverlapPoint(checkPush, collisionPush))
        {
            //check push layers. If they lie in vecCheckDir vectors, make them a child of player object that is pushing
            Physics2D.OverlapPoint(checkPush, collisionPush).transform.parent = gameObject.transform;
            frontStack = Physics2D.OverlapPoint(checkPush, collisionPush).gameObject;
            checkPush += vecCheckDir;
        }
    }
    private void NoLongerPush()
    {
        transform.DetachChildren();
        frontStack = gameObject;
    }
    
}
