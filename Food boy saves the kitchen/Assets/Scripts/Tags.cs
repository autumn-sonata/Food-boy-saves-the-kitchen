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
     * 4) Is sharp
     * 5) Is cut
     * If the tag e.g Player is not found at the current moment, then disable Player behaviour.
     */
    private string[] objectTags = new string[5];

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
        return objectTags[1] == "WinTile" || objectTags[2] == "YouTile";
    }

    public void enableIsSharp()
    {
        objectTags[3] = "Sharp";
    }

    public void disableIsSharp()
    {
        objectTags[3] = null;
    }

    public bool isSharp()
    {
        return objectTags[3] == "Sharp";
    }

    public void enableIsCut()
    {
        objectTags[4] = "Cut";
    }

    public bool isCut()
    {
        return objectTags[4] == "Cut";
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