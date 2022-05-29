using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinOrder : MonoBehaviour
{
    private GameObject[,] winConfig;
    private Vector2 topLeftCentered;
    // Start is called before the first frame update
    void Start()
    {
        //Get the winning configuration
        int x = (int) GetComponent<BoxCollider2D>().size.x;
        int y = (int) GetComponent<BoxCollider2D>().size.y;
        winConfig = new GameObject[y, x];
        topLeftCentered = new Vector2(transform.position.x - x / 2f, transform.position.y + y) + new Vector2(0.5f, -0.5f);

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                GameObject withinWinTile = Physics2D.OverlapPoint(topLeftCentered + new Vector2(j, i), LayerMask.GetMask("Push")).gameObject;
                withinWinTile.GetComponent<FoodTags>().enableWinTileTag();
                winConfig[i, j] = withinWinTile;
                //TODO: WHAT HAPPENS WHEN THE WIN CONDITION CHANGES
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the winning configuration is found anywhere in the map
        GameObject[] referenceFoods = GameObject.FindGameObjectsWithTag(winConfig[0, 0].tag);
        foreach (GameObject food in referenceFoods)
        {
            bool matchConfig = true;
            Vector2 relativeTopLeftCentered = food.transform.position;
            for (int i = 0; i < winConfig.GetLength(0); i++)
            {
                for (int j = 0; j < winConfig.GetLength(1); j++)
                {
                    Collider2D col = Physics2D.OverlapPoint(relativeTopLeftCentered + new Vector2(j, i), LayerMask.GetMask("Push"));
                    if (col == null || col.GetComponent<FoodTags>().isInWinTile() || col.tag != winConfig[i, j].tag)
                    {
                        matchConfig = false;
                    }
                }
            }
            if (matchConfig)
            {
                SceneManager.LoadScene(4);
            }
        }
    }
}
