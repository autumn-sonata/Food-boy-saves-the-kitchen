using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabUI : MonoBehaviour
{
    private static int TabToOpen = 0;
    [SerializeField] List <GameObject> TabPanels;
    [SerializeField] List <GameObject> text;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject panel in TabPanels)
        {
            panel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            foreach (GameObject texts in text)
            {
                texts.SetActive(false);
            }

            TabPanels[TabToOpen].SetActive(true);

        }
        else
        {
            foreach (GameObject texts in text)
            {
                texts.SetActive(true);
            }
            TabPanels[TabToOpen].SetActive(false);
        }
    }

    public void NextPanel()
    {
        TabToOpen += 1;
        TabPanels[TabToOpen - 1].SetActive(false);
    }
    public void PreviousPanel()
    {
        TabToOpen -= 1;
        TabPanels[TabToOpen + 1].SetActive(false);
    }
}
