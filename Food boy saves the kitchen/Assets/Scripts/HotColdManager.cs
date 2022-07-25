using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class HotColdManager : TileManager
{
    /* Manager for the hot and cold tiles.
     */

    #region Routines
    protected override void OldColRoutine()
    {
        /* Update foodSameTag and oldCol tags.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        DisableFoodSameTagProperty();
        disableTileProperty();
    }

    protected override void NewColRoutine()
    {
        /* Update to new collider
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        enableTileProperty();
    }

    #endregion

    protected override void PlayerAdjustment()
    {
        //Nothing. Does not need to tell coordinator
        //any information.
    }

    protected override void MoveExecuted()
    {
        //No additional work needs to be done.
    }
}
