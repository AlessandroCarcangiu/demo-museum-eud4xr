using System;
using System.Collections.Generic;
using ECARules4All_DLL;
using UnityEngine;

public class ArtworkMenu_AttachToECAObject : MonoBehaviour
{
    public List<string> videoList;
    public List<string> audioList;
    public List<string> textList;
    public List<string> lightList;

    public GameObject listPrefab;
    private ArtworkMenu_PrefabLogic _menuPrefabLogic;

    public Transform spawnPrefab;
    private void Start()
    {
        // If this gameobject doesn't have ECAObject, throw an exception
        if (GetComponent<ECAObject>() == null)
            throw new Exception("Missing ECAObject component");
        
        // If the sum of all list is 0, then return
        if (videoList.Count + audioList.Count + textList.Count + lightList.Count == 0)
        {
            Debug.LogError("[WARNING] No multimedia content to show");
            return;
        }
        
        // Otherwise, instantiate the prefab list
        var obj = Instantiate(listPrefab, spawnPrefab);
        _menuPrefabLogic = obj.GetComponent<ArtworkMenu_PrefabLogic>();
        if (_menuPrefabLogic == null)
            throw new Exception("The prefab list must have a ECAMultimediaMenu component");
        _menuPrefabLogic.OnObjectLoaded += OnObjectLoaded;
    }
    
    private void OnObjectLoaded()
    {
        _menuPrefabLogic.LoadArguments(this);
    }
}
