using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Determines different types of status for level.
 * 
 * Level can either be: 
 * 1) Locked - The player cannot access this level at all
 * 2) Unlocked - The player can access this level, but has 
 *               not beaten the level
 * 3) Completed - The player can access this level and 
 *                has beaten this level already
 */
public enum LevelStatus
{
    Locked,
    Unlocked,
    Completed
}
