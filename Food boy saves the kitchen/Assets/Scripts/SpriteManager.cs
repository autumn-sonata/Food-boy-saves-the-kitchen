using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    /* Manages all the sprites. To be connected to all movable foods and items.
     * Every object which is a push will have a possibility of a sprite change.
     * 
     * spriteObjects are in the Sprite layer, and should not ever be detached.
     * 
     * 1) Original state -> normal food without properties
     * 2) Cut state -> food sprite change
     * 3) Cold state -> food border change
     * 4) Hot state -> food border change
     * 5) Cooked state -> outline of food
     */

    public Transform[] spriteObjects;
    private Dictionary<string, SpriteRenderer> sprites;
    private readonly string original = "original";
    private readonly string cut = "cut";
    private readonly string cold = "cold";
    private readonly string hot = "hot";
    private readonly string cooked = "cooked";

    private void Awake()
    {
        /* Disable all other sprites except for original when first loaded
         * Original is enabled by default.
         */
        sprites = new Dictionary<string, SpriteRenderer>();

        for (int i = 0; i < spriteObjects.Length; i++)
        {
            spriteObjects[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        //Load sprites into hashmap for quick access
        foreach (Transform spriteObj in spriteObjects)
        {
            sprites.Add(spriteObj.name, spriteObj.GetComponent<SpriteRenderer>());
        }
    }

    public void UpdateSprites()
    {
        /* Updates the sprites based on the tags.
         * If the object is supposed to be inactive, disable the gameObject.
         */
        Tags tag = GetComponent<Tags>();

        if (tag.isCut())
        {
            GetComponent<SpriteRenderer>().sprite = sprites[cut].sprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = sprites[original].sprite;
        }

        //Hot or cold?
        if (tag.isCold() && tag.isHot() && !tag.isInactive())
        {
            Debug.LogError(gameObject + " was supposed to be destroyed.");
        }
        else if (tag.isCold())
        {
            enableSprite(cold);
            disableSprite(hot);
        }
        else if (tag.isHot())
        {
            enableSprite(hot);
            disableSprite(cold);
        }
        else
        {
            disableSprite(hot);
            disableSprite(cold);
        }

        //Cooked or uncooked?
        if (tag.isCooked())
        {
            enableSprite(cooked);
        }
        else
        {
            disableSprite(cooked);
        }
    }

    private void enableSprite(string name)
    {
        /* Enables the sprite with the specific name.
         */
        sprites[name].enabled = true;
    }

    private void disableSprite(string name)
    {
        /* Disables the sprite with the specific name.
         */
        sprites[name].enabled = false;
    }
}
