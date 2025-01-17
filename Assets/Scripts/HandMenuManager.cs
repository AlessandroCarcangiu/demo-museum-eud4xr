using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandMenuManager : Singleton<HandMenuManager>
{
    [ContextMenu("Quit")]
    public void Quit()
    {
        Debug.Log("I'm quitting the app");
        Application.Quit();
    }
    
    [ContextMenu("Reset")]
    public void Reset()
    {
        var oldScene = GetCurrentlyActiveScene();
        var oldSceneName = oldScene.name;
        Debug.Log("Old scene name: " + oldSceneName);
        
        // Unload the current scene
        // SceneManager.UnloadSceneAsync(oldScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
     
        // Load the current scene
        SceneManager.LoadScene(0);
    }
    
    private static Scene GetCurrentlyActiveScene()
    {
        return SceneManager.GetActiveScene();
    }
}
