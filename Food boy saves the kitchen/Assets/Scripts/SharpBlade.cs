using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpBlade : MonoBehaviour
{
    /* Determines that this object is sharp and is able to cut adjacent foods.
     * Behaves exactly like a food object otherwise.
     */

    void Awake()
    {
        GetComponent<Tags>().enableIsSharp();
    }
}
