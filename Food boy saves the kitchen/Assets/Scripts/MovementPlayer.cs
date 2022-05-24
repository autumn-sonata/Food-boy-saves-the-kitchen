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
        //Debug.Log(destination.position);
        //Debug.Log(transform.position);
    }

    void Update()
    {
        if (gameObject.tag == "Player")
        {
            float horizontalMove = Input.GetAxisRaw("Horizontal");
            float verticalMove = Input.GetAxisRaw("Vertical");
            // Debug.Log(horizontalMove);
            // Debug.Log(verticalMove);
            // Debug.Log(destination.position);
            // Debug.Log(transform.position);

            transform.position = Vector3.MoveTowards(transform.position, destination.position, spriteSize * Time.deltaTime * playerSpeed);

            if (Vector3.Distance(destination.position, transform.position) <= 0.05f)
            {
                //update destination position
                if (Mathf.Abs(horizontalMove) == 1f && Time.time >= timeToMoveAgain) 
                {
                    NoLongerPush();
                    CollideWithPush(horizontalMove, true);
                    CollideWithHeavy(horizontalMove, 0f);
                } else if (Mathf.Abs(verticalMove) == 1f && Time.time >= timeToMoveAgain)
                {
                    NoLongerPush();
                    CollideWithPush(verticalMove, false);
                    CollideWithHeavy(0f, verticalMove);
                }
            } else 
            {
                timeToMoveAgain = Time.time + timeDelayPerMovement;
                //Debug.Log(timeToMoveAgain);
            }
        }
    }
    
    private void CollideWithHeavy(float horizontalMove, float verticalMove)
    {
        //check destination of both gameObject and frontStack destination.
        if (!Physics2D.OverlapCircle(destination.position + new Vector3(horizontalMove, verticalMove, 0f), .3f, collisionHeavy) &&
        !Physics2D.OverlapCircle(frontStack.GetComponent<MovementPlayer>().destination.position + new Vector3(horizontalMove, verticalMove, 0f), .3f, collisionHeavy))
        {   
            //update if what is in front is not heavy
            destination.position = transform.position + new Vector3(horizontalMove * spriteSize, verticalMove * spriteSize, 0f);
            //Debug.Log(destination.position);
            frontStack.GetComponent<MovementPlayer>().destination.position = 
                frontStack.GetComponent<MovementPlayer>().transform.position + new Vector3(horizontalMove * spriteSize, verticalMove * spriteSize, 0f);
        }
        // Debug.Log("FrontStack " + frontStack.GetComponent<MovementPlayer>().destination.position);
        // Debug.Log("Destination " + destination.position);
    }

    private void CollideWithPush(float move, bool isHorz)
    {
        //Make all pushable objects in front a child of current player
        Vector2 checkPush = new Vector2(transform.position.x, transform.position.y);
        //Debug.Log(checkPush);
        Vector2 vecCheckDir;

        if (isHorz)
        {
            vecCheckDir = new Vector2(move, 0f);
        } else 
        {
            vecCheckDir = new Vector2(0f, move);
        }

        //Debug.Log(vecCheckDir);
        while (Physics2D.OverlapPoint(checkPush, collisionPush))
        {
            //check push layers. If they lie in vecCheckDir vectors, make them a child of player object that is pushing
            Physics2D.OverlapPoint(checkPush, collisionPush).gameObject.transform.parent = gameObject.transform;
            frontStack = Physics2D.OverlapPoint(checkPush, collisionPush).gameObject;
            checkPush += vecCheckDir;
        }
        //Debug.Log(frontStack);
    }
    private void NoLongerPush()
    {
        transform.DetachChildren();
        frontStack = gameObject;
    }
    
}
