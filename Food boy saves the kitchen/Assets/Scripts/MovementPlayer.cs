using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    public float playerSpeed = 7f;
    public Transform destination;
    public LayerMask collisionType;
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
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");
        // Debug.Log(horizontalMove);
        // Debug.Log(verticalMove);
        // Debug.Log(destination.position);
        // Debug.Log(transform.position);

        transform.position = Vector3.MoveTowards(transform.position, destination.position, spriteSize * Time.deltaTime * playerSpeed);
        //TODO ADD A DELAY AFTER REACHING DEST
        if (Vector3.Distance(destination.position, transform.position) <= 0.05f)
        {
            //update destination position
            if (Mathf.Abs(horizontalMove) == 1f && 
            !Physics2D.OverlapCircle(destination.position + new Vector3(horizontalMove * spriteSize, 0f, 0f), .3f, collisionType) &&
            Time.time >= timeToMoveAgain) 
            {
                destination.position += new Vector3(horizontalMove * spriteSize, 0f, 0f);
            } else if (Mathf.Abs(verticalMove) == 1f && 
            !Physics2D.OverlapCircle(destination.position + new Vector3(0f, verticalMove * spriteSize, 0f), .3f, collisionType) &&
            Time.time >= timeToMoveAgain)
            {
                destination.position += new Vector3(0f, verticalMove * spriteSize, 0f);
            }
        } else 
        {
            timeToMoveAgain = Time.time + timeDelayPerMovement;
            //Debug.Log(timeToMoveAgain);
        }
    }
}
