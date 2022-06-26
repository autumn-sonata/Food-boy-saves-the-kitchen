using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSprite : MonoBehaviour
{
    /* Stores the cut state of the food item.
     */
    public Sprite cutObject;
    private Sprite startState; //Remember the sprite when level begins.

    private void Awake()
    {
        startState = GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Tags>().isCut())
        {
            GetComponent<SpriteRenderer>().sprite = cutObject;
        } else
        {
            GetComponent<SpriteRenderer>().sprite = startState;
        }
    }
}
