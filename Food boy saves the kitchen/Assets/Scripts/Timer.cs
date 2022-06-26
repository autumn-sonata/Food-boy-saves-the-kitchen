using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float countdown;
    private bool currentlyCounting;

    // Update is called once per frame
    void Update()
    {
        if (countdown > 0) countdown -= Time.deltaTime;
    }

    public void startTimer(float duration)
    {
        if (!currentlyCounting)
        {
            countdown = duration;
            currentlyCounting = true;
        }
    }

    public bool countdownFinished()
    {
        currentlyCounting = false;
        return countdown <= 0f;
    }
}
