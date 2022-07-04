using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    /* objectTags stores all Tags. Tags control the behaviour of the object.
     * Tags include:
     * 1) Player
     * 2) Part of winTile
     * 3) Part of youTile
<<<<<<< Updated upstream
     * 4) Is sharp
     * 5) Is cut
     * If the tag e.g Player is not found at the current moment, then disable Player behaviour.
     */
    private string[] objectTags = new string[5];
=======
     * 4) Part of hotTile
     * 5) Part of coldTile
     * 6) Part of heavyTile
     * 7) Is sharp
     * 8) Is cut
     * 9) Is hot
     * 10) Is cold
     * 11) Is cooked (cold is uncooked)
     * If the tag e.g Player is not found at the current moment, then disable Player behaviour.
     */
    private bool[] objectTags = new bool[15];

    public bool isKnife()
    {
        return gameObject.tag == "Knife";
    }
>>>>>>> Stashed changes

    public bool isKnife()
    {
        return gameObject.tag == "KnifeHilt" || gameObject.tag == "KnifeBlade";
    }
    public bool isPlayer()
    {
        return objectTags[0] == "Player";
    }

    public void disablePlayerTag()
    {
        objectTags[0] = null;
    }

    public void enablePlayerTag()
    {
        objectTags[0] = "Player";
    }

    public void enableWinTileTag()
    {
        objectTags[1] = "WinTile";
    }

    public void disableWinTileTag()
    {
        objectTags[1] = null;
    }

    public bool isInWinTile()
    {
        return objectTags[1] == "WinTile";
    }

    public void enableYouTileTag()
    {
        objectTags[2] = "YouTile";
    }

    public void disableYouTileTag()
    {
        objectTags[2] = null;
    }

    public bool isInYouTile()
    {
        return objectTags[2] == "YouTile";
    }

    public bool isInAnyTile()
    {
<<<<<<< Updated upstream
        return objectTags[1] == "WinTile" || objectTags[2] == "YouTile";
=======
        for (int i = 1; i < 6; i++)
        {
            if (objectTags[i])
                return true;
        }
        return false;
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
>>>>>>> Stashed changes
    }

    public void enableIsSharp()
    {
        objectTags[3] = "Sharp";
    }

    public void disableIsSharp()
    {
<<<<<<< Updated upstream
        objectTags[3] = null;
=======
        return objectTags[4];
    }

    public void enableHeavyTileTag()
    {
        objectTags[5] = true;
>>>>>>> Stashed changes
    }

    public void disableHeavyTileTag()
    {
        objectTags[5] = false;
    }

    public bool isInHeavyTile()
    {
        return objectTags[3] == "Sharp";
    }

    public void enableIsSharp()
    {
        objectTags[4] = "Cut";
    }

    public bool isSharp()
    {
<<<<<<< Updated upstream
        return objectTags[4] == "Cut";
=======
        return objectTags[6];
    }

    public void enableIsCut()
    {
        objectTags[7] = true;
    }

    public bool isCut()
    {
        return objectTags[7];
    }

    public void enableHot()
    {
        objectTags[8] = true;
    }

    public void disableHot()
    {
        objectTags[8] = false;
    }

    public bool isHot()
    {
        return objectTags[8];
    }

    public void enableCold()
    {
        objectTags[9] = true;
    }

    public void disableCold()
    {
        objectTags[9] = false;
    }

    public bool isCold()
    {
        return objectTags[9];
    }

    public void enableCooked()
    {
        objectTags[10] = true;
    }

    public void disableCooked()
    {
        objectTags[10] = false;
    }

    public bool isCooked()
    {
        return objectTags[10];
>>>>>>> Stashed changes
    }

    public string[] getTags()
    {
        /* To record tags for undo.
         */
        return objectTags;
    }
    
    public void setTags(string[] tags)
    {
        /* To set tag in the case of undo
         */
        objectTags = tags;
    }
}