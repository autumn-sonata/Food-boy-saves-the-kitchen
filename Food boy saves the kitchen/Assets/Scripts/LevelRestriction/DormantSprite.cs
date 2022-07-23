using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DormantSprite : MonoBehaviour
{
    /* Stores the dormant sprite of Level tiles.
     */

    [SerializeField]
    private Sprite levelSprite;

    public Sprite GetActiveSprite()
    {
        if (!levelSprite) 
            Debug.LogError("LevelSprite in DormantSprite not initialised.");
        return levelSprite;
    }
}
