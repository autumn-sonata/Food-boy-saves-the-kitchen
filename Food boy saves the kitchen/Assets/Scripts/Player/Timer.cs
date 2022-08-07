using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float countdown;
    private bool currentlyCounting;

    void Update()
    {
        if (countdown > 0) countdown -= Time.deltaTime;
    }

    public void startTimer(float duration)
    {
        /* Start counting the time before another player movement
         * by the user/player can be executed.
         * 
         * Parameters
         * ----------
         * 1) duration: How long the user/player should wait
         *   until another move can be made in the level.
         * 
         * Return
         * ------
         * 
         */

        if (!currentlyCounting)
        {
            countdown = duration;
            currentlyCounting = true;
        }
    }

    public bool countdownFinished()
    {
        /* Checks if the duration that has been
         * specified has been fully waited.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if user/player can now make another movement,
         *   False otherwise.
         */

        currentlyCounting = false;
        return countdown <= 0f;
    }
}
