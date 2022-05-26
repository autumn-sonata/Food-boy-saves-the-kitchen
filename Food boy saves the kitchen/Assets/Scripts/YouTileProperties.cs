using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouTileProperties : MonoBehaviour
{
    private GameObject[] foodSameTag;
    public string originalTag;
    private Collider2D col;

    private void Start()
    {
        foodSameTag = new List<GameObject>().ToArray();
    }
    private void Update()
    {
        if (col != null && col.GetComponent<MovementPlayer>().destination.position == col.transform.position)
        {
            //Alter tag of player and food
            foreach (GameObject food in foodSameTag)
            {
                food.tag = originalTag;
            }

            foodSameTag = GameObject.FindGameObjectsWithTag(col.tag);
            originalTag = col.tag;
            foreach (GameObject food in foodSameTag)
            {
                food.tag = "Player";
            }

            //Disable movement of gameObject on the You tile
            col.GetComponent<MovementPlayer>().enabled = false;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (col != null)
        {
            col.GetComponent<MovementPlayer>().enabled = true;
        }
        col = collision;
    }
}
