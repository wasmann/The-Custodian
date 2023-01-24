using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject SettingsMenuUI;

    public void Back()
    {
        Debug.Log("Closing menu...");
        Loader.Load(Loader.Scene.Battle);
    }

    //TO DO: Add functionality for other options!
}
