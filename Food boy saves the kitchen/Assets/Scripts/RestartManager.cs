using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartManager : MonoBehaviour
{
    [SerializeField] GameObject RestartUI;
    void Start()
    {
        RestartUI.SetActive(false);
    }


        //upon box collider
        //RestartUI.SetActive(true);


    public void DontRestart()
    {
        RestartUI.SetActive(false);
    }

    public void DoRestart()
    {
        //restart
    }
}
