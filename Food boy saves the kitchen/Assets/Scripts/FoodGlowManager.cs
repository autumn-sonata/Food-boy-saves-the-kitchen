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

    void Start()
    {
        currBrightness = minBrightness;
        newBrightness = maxBrightness;
        allowChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        currBrightness = Mathf.Lerp(currBrightness, newBrightness, Time.deltaTime * smooth);
        BrightnessChange();
    }

    public float getCurrBrightness()
    {
        return currBrightness;
    }

    private void BrightnessChange()
    {
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
        yield return new WaitForSeconds(brightnessChangeCD);
        allowChange = true;
    }
}
