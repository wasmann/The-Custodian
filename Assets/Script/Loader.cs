using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        Battle,
        MainMenu,
        Tutorial,
        SettingsMenu,
        WorldMap,
        LoadingScreen,
    }

    private static Action onLoaderCallback;

    public static void Load(Scene scene)
    {
        if (scene == Scene.Battle)
        {
            // Set the loader callback action to load the target scene
            onLoaderCallback = () => { SceneManager.LoadScene(scene.ToString()); };

            // Load the loading screen
            SceneManager.LoadScene(Scene.LoadingScreen.ToString());
        }
        else //if (scene != Scene.Battle)
        {
            SceneManager.LoadScene(scene.ToString());
        }
    }

    // Function that enables us to call the loading screen before the actual target scene is
    // loaded. Otherwise, since the target scene is loaded immediately after the loading screen is
    // loaded, we can't actually see it.
    public static void LoaderCallback()
    {
        // Triggered after the first Update which lets the screen refresh
        // Execute the loader callback action which will load the target scene
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
