using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DormantSprite : MonoBehaviour
{
    /* Stores the dormant sprite of Level tiles.
     */

    [SerializeField]
    private Sprite levelSprite;

    public void ChangeToActiveSprite()
    {
        /* Changes the SpriteRenderer sprite to levelSprite.
         */
        if (!levelSprite) 
            Debug.LogError("LevelSprite in DormantSprite not initialised.");
        GetComponent<SpriteRenderer>().sprite = levelSprite;
    }
}
