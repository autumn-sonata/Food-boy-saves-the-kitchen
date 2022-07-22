using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUpdateTile : MonoBehaviour
{
    public int nextScene;

    private void OnTriggerEnter2D(Collider2D col)
    {
        SceneManager.LoadScene(nextScene + 7);
    }
}
