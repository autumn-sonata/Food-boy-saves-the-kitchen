using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public class FoodGlowManager : MonoBehaviour
{
    /* Coordinates glow effect of cooked items.
     */

    private readonly float brightnessChangeCD = 1f;
    private readonly float minBrightness = 1f;
    private readonly float maxBrightness = 3f;
    private readonly float smooth = 5f;

    private bool allowChange;
    private float currBrightness;
    private float newBrightness;

    #region Unity specific functions

    void Start()
    {
        /* Initialisation of variables.
         */

        currBrightness = minBrightness;
        newBrightness = maxBrightness;
        allowChange = true;
    }

    void Update()
    {
        /* Keep changing the values from minBrightness to maxBrightness
         * over a fixed interval.
         */

        currBrightness = Mathf.Lerp(currBrightness, newBrightness, Time.deltaTime * smooth);
        BrightnessChange();
    }

    #endregion

    #region Brightness adjustments

    public float getCurrBrightness()
    {
        /* Gets the current brightness as stated by the interval.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * float: range from minBrightness to maxBrightness, determines
         *   the current brightness that a cooked object should be glowing at.
         */

        return currBrightness;
    }

    private void BrightnessChange()
    {
        /* Changes the brightness target,
         * from minBrightness to maxBrightness and back.
         * 
         * Changes this brightness target at a fixed time interval.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (allowChange && Mathf.Abs(Mathf.Round(currBrightness) - currBrightness) < 0.05f)
        {
            if (newBrightness == minBrightness)
            {
                newBrightness = maxBrightness;
            }
            else if (newBrightness == maxBrightness)
            {
                newBrightness = minBrightness;
            }
            allowChange = false;
            StartCoroutine(brightnessChangeDelay());
        }
    }

    private IEnumerator brightnessChangeDelay()
    {
        /* Add time delay to which the brightness 
         * target will change at.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * IEnumerator: Coroutine return.
         */

        yield return new WaitForSeconds(brightnessChangeCD);
        allowChange = true;
    }

    #endregion
}
