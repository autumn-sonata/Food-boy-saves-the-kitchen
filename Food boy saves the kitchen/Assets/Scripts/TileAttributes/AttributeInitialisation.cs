using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeInitialisation : MonoBehaviour
{
    /* Determine whether an food item should be cut and/or
     * cooked at the start of the level.
     */

    public bool isCutAtStart;
    public bool isCookedAtStart;

    public void Initialise()
    {
        /* Initialise the current food item to be either 
         * cooked and/or cut.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        if (isCutAtStart) GetComponent<Tags>().enableIsCut();
        if (isCookedAtStart) GetComponent<Tags>().enableCooked();
    }
}
