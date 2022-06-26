using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSprite : MonoBehaviour
{
    /* Stores the cut state of the food item.
     */
    public Sprite cutObject;
    private bool called;

    private void Awake()
    {
        called = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!called && GetComponent<Tags>().isCut())
        {
            called = true; //called only once during gameObject's lifetime
            GetComponent<SpriteRenderer>().sprite = cutObject;
        }
    }
}
