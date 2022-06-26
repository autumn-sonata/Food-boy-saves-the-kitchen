using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpBlade : MonoBehaviour
{
    /* Script to be connected to the blade of the knife.
     */

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Tags>().enableIsSharp();
    }
}
