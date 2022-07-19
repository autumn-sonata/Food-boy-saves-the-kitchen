using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabUI : MonoBehaviour
{

    [SerializeField] GameObject TabCanvas;
    [SerializeField] GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        TabCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            TabCanvas.SetActive(true);
            if (text != null)
            {
                text.SetActive(false);
            }
        }
        else
        {
            TabCanvas.SetActive(false);
            if (text != null)
            {
                text.SetActive(true);
            }
        }
    }
}
