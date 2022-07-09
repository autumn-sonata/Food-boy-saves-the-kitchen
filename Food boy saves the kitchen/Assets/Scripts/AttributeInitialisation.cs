using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeInitialisation : MonoBehaviour
{
    public bool isCutAtStart;
    public bool isCookedAtStart;

    // Start is called before the first frame update
    public void Initialise()
    {
        if (isCutAtStart) GetComponent<Tags>().enableIsCut();
        if (isCookedAtStart) GetComponent<Tags>().enableCooked();
    }
}
