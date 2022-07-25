using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabUI : MonoBehaviour
{
    private static int TabToOpen = 0;
    [SerializeField] List <GameObject> TabPanels;

    #region Unity specific functions

    void Start()
    {
        /* Set all active panels to false;
         */

        foreach (GameObject panel in TabPanels)
        {
            panel.SetActive(false);
        }
    }

    void Update()
    {
        /* If the TAB key is pressed, then open 
         * the tab menu, which displays all the rules
         * of the game.
         */

        if (Input.GetKey(KeyCode.Tab))
        {
            TabPanels[TabToOpen].SetActive(true);
        }
        else
        {
            TabPanels[TabToOpen].SetActive(false);
        }
    }

    #endregion

    #region Panel functions

    public void NextPanel()
    {
        /* Get the next panel in order
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        TabToOpen += 1;
        TabPanels[TabToOpen - 1].SetActive(false);
    }
    public void PreviousPanel()
    {
        /* Get the previous panel in order
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        TabToOpen -= 1;
        TabPanels[TabToOpen + 1].SetActive(false);
    }

    #endregion

    #region Miscellaneous

    public bool isTabOpen()
    {
        /* Check if the TAB is currently open.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * bool: True if the tab is open, False
         *   otherwise.
         */

        if (Input.GetKey(KeyCode.Tab))
        {
            return true;
        }
        return false;
    }

    #endregion
}
