using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUpdateTile : MonoBehaviour
{
    /* Sets the next scene that the gameObject will
     * warp the player to.
     */

    public int nextScene;

    private void OnTriggerEnter2D(Collider2D col)
    {
        /* Load scene. +5 allows nextScene to correspond
         * to the level number
         * 
         * Eg Lvl 1: nextScene == 1
         */

        SceneManager.LoadScene(nextScene + 5);
    }
}
