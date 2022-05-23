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
    // Start is called before the first frame update
    void Start()
    {
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        destination.SetParent(null);
        //Debug.Log(destination.position);
        //Debug.Log(transform.position);
    }
    
    void Update()
    {
        if (GetComponent<Tags>().objectTags[1] == "Player")
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
                    CollideWithHeavy(horizontalMove, 0f);
                } else if (Mathf.Abs(verticalMove) == 1f && Time.time >= timeToMoveAgain)
                {
                    CollideWithHeavy(0f, verticalMove);
                }
            } else 
            {
                timeToMoveAgain = Time.time + timeDelayPerMovement;
                //Debug.Log(timeToMoveAgain);
            }
        }
    }

    public void CollideWithHeavy(float horizontalMove, float verticalMove)
    {
        if (!Physics2D.OverlapCircle(destination.position + new Vector3(horizontalMove, verticalMove, 0f), .3f, collisionHeavy))
        {
            destination.position = transform.position + new Vector3(horizontalMove * spriteSize, verticalMove * spriteSize, 0f);
        }
    }

    public void CollideWithPush()
    {
        //Make all pushable objects that are in contact with one another as one object
    }
}
