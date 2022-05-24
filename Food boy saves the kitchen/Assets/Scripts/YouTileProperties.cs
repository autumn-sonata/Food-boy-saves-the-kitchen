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

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Make all corresponding items which are the same foods players.
        if (col.gameObject.layer == pushLayer)
        {
            parentFood = col.gameObject;
            //Debug.Log(parentFood);
            playerFoods = GameObject.FindGameObjectsWithTag(parentFood.gameObject.tag);
            beforeTag = parentFood.gameObject.tag;

            foreach (GameObject food in playerFoods)
            {
                food.tag = "Player";
            }
            Debug.Log(beforeTag);
        }    

        //Prevent parentFood from moving
        parentFood.GetComponent<MovementPlayer>().enabled = false;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log(beforeTag);
        parentFood.GetComponent<MovementPlayer>().enabled = true;
        foreach (GameObject food in playerFoods)
        {
            food.tag = beforeTag;
        }
    }
}
