using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    public float playerSpeed = 7f;
    public Transform destination;
    public LayerMask collisionType;
    private float spriteSize;
    // Start is called before the first frame update
    void Start()
    {
        spriteSize = gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
        destination.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");

        //moves object to destination
        transform.position = Vector3.MoveTowards(transform.position, destination.position, playerSpeed * Time.deltaTime);
        
        //checks for input and moves destination accordingly
        if (Vector3.Distance(destination.position, transform.position) == 0f)
        {
            if (Mathf.Abs(horizontalMove) == 1f) 
            {

                if (!Physics2D.OverlapCircle(destination.position + new Vector3(horizontalMove, 0f, 0f), .1f, collisionType))
                {
                    destination.position = transform.position + new Vector3(horizontalMove * spriteSize, 0f, 0f);
                }
            } else if (Mathf.Abs(verticalMove) == 1f)
            {
                if (!Physics2D.OverlapCircle(destination.position + new Vector3(0f, verticalMove * spriteSize, 0f), .1f, collisionType))
                {
                    destination.position = transform.position + new Vector3(0f, verticalMove * spriteSize, 0f);
                }

            } 
        }
    }
}
