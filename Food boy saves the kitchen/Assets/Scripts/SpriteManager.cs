using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

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
    private SpriteGlowEffect spriteGlow;
    private readonly string original = "original";
    private readonly string cut = "cut";
    private readonly string cold = "cold";
    private readonly string hot = "hot";
    private FoodGlowManager glowManager;
    private static Color32 cookedGlowColor = new(161, 54, 54, 30);

    #region Unity specific functions

    private void Awake()
    {
        /* Disable all other sprites except for original when first loaded
         * Original is enabled by default.
         */

        glowManager = GameObject.Find("Main Camera").GetComponent<FoodGlowManager>();
        spriteGlow = GetComponent<SpriteGlowEffect>();
        spriteGlow.GlowColor = cookedGlowColor;
        spriteGlow.enabled = false;
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

    private void Update()
    {
        //Lerp brightness to new brightness
        if (spriteGlow.enabled) spriteGlow.GlowBrightness = glowManager.getCurrBrightness();
    }

    #endregion

    #region Updating sprites

    public void UpdateSprites()
    {
        /* Updates the sprites based on the tags on the gameObjects.
         * If the object is supposed to be inactive, disable the gameObject.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        Tags tag = GetComponent<Tags>();
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (tag.isCut())
        {
            render.sprite = sprites[cut].sprite;
        }
        else
        {
            render.sprite = sprites[original].sprite;
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
            //enable glow effect.
            if (!tag.isKnife()) spriteGlow.enabled = true;
        }
        else
        {
            //disable glow effect.
            spriteGlow.enabled = false;
        }
    }

    private void enableSprite(string name)
    {
        /* Enables the sprite with the specific name.
         * 
         * Parameters
         * ----------
         * 1) name: "cut", "cold", "hot", "original"
         * 
         * Return
         * ------
         * 
         */

        sprites[name].enabled = true;
    }

    private void disableSprite(string name)
    {
        /* Disables the sprite with the specific name.
         * 
         * Parameters
         * ----------
         * 1) name: "cut", "cold", "hot", "original"
         * 
         * Return
         * ------
         * 
         */

        sprites[name].enabled = false;
    }

    #endregion
}
