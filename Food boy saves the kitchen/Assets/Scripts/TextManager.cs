using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] List<GameObject> text;
    [SerializeField] GameObject CanvasPause;
    [SerializeField] GameObject CanvasTab;

    void Update()
    {
            if (CanvasTab.GetComponent<TabUI>().isTabOpen() || CanvasPause.GetComponent<PauseManager>().isPauseOpen())
        {
            foreach (GameObject texts in text)
            {
                texts.SetActive(false);
            }
        }
            else
        {
            foreach (GameObject texts in text)
            {
                texts.SetActive(true);
            }
        }
    }
}
