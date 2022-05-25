using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouTileProperties : MonoBehaviour
{
    private readonly int pushLayer = 7;
    // Start is called before the first frame update
    private GameObject parentFood;
    private GameObject[] playerFoods;
    private string beforeTag;
    
    IEnumerator OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("Exit trigger!");
        Debug.Log(col.gameObject.GetComponent<MovementPlayer>().destination.position);
        yield return new WaitUntil(() => col.gameObject.transform.position == col.gameObject.GetComponent<MovementPlayer>().destination.position);
        parentFood.GetComponent<MovementPlayer>().enabled = true;
        foreach (GameObject food in playerFoods)
        {
            food.tag = beforeTag;
            Debug.Log("returned original food tag");
        }
    }
    IEnumerator OnTriggerEnter2D(Collider2D col)
    {
        //Make all corresponding items which are the same foods players.
        Debug.Log("Enter triggered!");
        if (col.gameObject.layer == pushLayer)
        {
            Debug.Log(col.gameObject);
            Debug.Log(col.gameObject.GetComponent<MovementPlayer>().destination.position);
            yield return new WaitUntil(() => col.gameObject.transform.position == col.gameObject.GetComponent<MovementPlayer>().destination.position);
            Debug.Log("Should be position " + col.gameObject.GetComponent<MovementPlayer>().destination.position);
            parentFood = col.gameObject;
            //Debug.Log(parentFood);
            playerFoods = GameObject.FindGameObjectsWithTag(parentFood.gameObject.tag);
            beforeTag = parentFood.gameObject.tag;

            foreach (GameObject food in playerFoods)
            {
                food.tag = "Player";
            }
            //Prevent parentFood from moving
            parentFood.GetComponent<MovementPlayer>().enabled = false;
        }    
    }
}
