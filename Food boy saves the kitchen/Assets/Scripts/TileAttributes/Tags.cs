using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    /* objectTags stores all Tags. Tags control the behaviour of the object.
     * Tags include:
     * 1) Is player
     * 2) Part of winTile
     * 3) Part of youTile
     * 4) Part of hotTile
     * 5) Part of coldTile
     * 6) Part of heavyTile
     * 7) Is heavy
     * 8) Is sharp
     * 9) Is cut
     * 10) Is hot
     * 11) Is cold
     * 12) Is cooked (cold is uncooked)
     * 13) Is inactive (in the level)
     * If the tag e.g Player is not found at the current moment, then disable Player behaviour.
     */

    private bool[] objectTags = new bool[15];

    public bool isKnife()
    {
        return gameObject.tag == "Knife";
    }

    #region Player
    public bool isPlayer()
    {
        return objectTags[0];
    }

    public void disablePlayerTag()
    {
        objectTags[0] = false;
    }

    public void enablePlayerTag()
    {
        objectTags[0] = true;
    }

    #endregion

    #region Tiles
    public void enableWinTileTag()
    {
        objectTags[1] = true;
    }

    public void disableWinTileTag()
    {
        objectTags[1] = false;
    }

    public bool isInWinTile()
    {
        return objectTags[1];
    }

    public void enableYouTileTag()
    {
        objectTags[2] = true;
    }

    public void disableYouTileTag()
    {
        objectTags[2] = false;
    }

    public bool isInYouTile()
    {
        return objectTags[2];
    }

    public bool isInAnyTile()
    {
        for (int i = 1; i < 6; i++)
        {
            if (objectTags[i])
                return true;
        }
        return false;
    }

    public void notOnAnyTile()
    {
        /* Disables all on tile tags.
         */
        for (int i = 1; i < 6; i++)
        {
            objectTags[i] = false;
        }
    }

    public void enableHotTileTag()
    {
        objectTags[3] = true;
    }

    public void disableHotTileTag()
    {
        objectTags[3] = false;
    }

    public bool isInHotTile()
    {
        return objectTags[3];
    }

    public void enableColdTileTag()
    {
        objectTags[4] = true;
    }

    public void disableColdTileTag()
    {
        objectTags[4] = false;
    }

    public bool isInColdTile()
    {
        return objectTags[4];
    }

    public void enableHeavyTileTag()
    {
        objectTags[5] = true;
    }

    public void disableHeavyTileTag()
    {
        objectTags[5] = false;
    }

    public bool isInHeavyTile()
    {
        return objectTags[5];
    }

    #endregion

    #region Object Attributes
    public void enableHeavy()
    {
        objectTags[6] = true;
    }

    public void disableHeavy()
    {
        objectTags[6] = false;
    }

    public bool isHeavy()
    {
        return objectTags[6];
    }

    public void enableIsSharp()
    {
        objectTags[7] = true;
    }

    public bool isSharp()
    {
        return objectTags[7];
    }

    public void enableIsCut()
    {
        objectTags[8] = true;
    }

    public bool isCut()
    {
        return objectTags[8];
    }

    public void enableHot()
    {
        objectTags[9] = true;
    }

    public void disableHot()
    {
        objectTags[9] = false;
    }

    public bool isHot()
    {
        return objectTags[9];
    }

    public void enableCold()
    {
        objectTags[10] = true;
    }

    public void disableCold()
    {
        objectTags[10] = false;
    }

    public bool isCold()
    {
        return objectTags[10];
    }

    public void enableCooked()
    {
        objectTags[11] = true;
    }

    public void disableCooked()
    {
        objectTags[11] = false;
    }

    public bool isCooked()
    {
        return objectTags[11];
    }

    public void enableInactive()
    {
        objectTags[12] = true;
    }

    public void disableInactive()
    {
        objectTags[12] = false;
    }

    public bool isInactive()
    {
        return objectTags[12];
    }

    #endregion

    #region Tag interactions
    public bool[] getTags()
    {
        /* To record tags for undo.
         */
        return objectTags;
    }
    
    public void setTags(bool[] tags)
    {
        /* To set tag in the case of undo
         */

        objectTags = tags;
    }

    public void printTags()
    {
        /* Prints out the tags for debugging purposes.
         */

        Debug.Log(name +
                  "\n1) Is player " + objectTags[0] +
                  "\n2) Part of winTile " + objectTags[1] +
                  "\n3) Part of youTile " + objectTags[2] +
                  "\n4) Part of hotTile " + objectTags[3] +
                  "\n5) Part of coldTile " + objectTags[4] +
                  "\n6) Part of heavyTile " + objectTags[5] +
                  "\n7) Is heavy " + objectTags[6] +
                  "\n8) Is sharp " + objectTags[7] +
                  "\n9) Is cut " + objectTags[8] +
                  "\n10) Is hot " + objectTags[9] +
                  "\n11) Is cold " + objectTags[10] +
                  "\n12) Is cooked " + objectTags[11] +
                  "\n13) Is inactive " + objectTags[12]);
    }

    #endregion
}