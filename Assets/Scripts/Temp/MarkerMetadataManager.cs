using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class MarkerMetadataManager : MonoBehaviour
{
    ///////////////////////// INSPECTOR ///////////////////////////////////////
    [Header("Path settings")] public bool doUseRemoteConfig = false;

    public string remoteServerUrl = "http://192.168.1.4:8090/get-json-config"; // Python server for debugging purposes

    [Tooltip("Path to the JSON configuration file inside StreamingAssets folder.")]
    // public string streamingAssetsJsonPath = "MarkerDetection/config-PI.json";


    [Header("Prefabs List")] public List<GameObject> markerPrefabList;

    public GameObject root;


    [Header("Other debugging settings")]
    [Tooltip("Determines whether the instantiated markerPrefab will be active at creation. Set to:\n" +
             " - false to instantiate the virtual objects as inactive.\n" +
             " - true (for debugging purposes) to instantiate the virtual objects as active.")]
    public bool isInstantiationActiveAtCreation = false;
    ////////////////////////////////////////////////////////////////////////


    // List to store the parsed items
    private Dictionary<int, MarkerData> markerMetadatas = new();

    public Dictionary<int, MarkerData> MarkerMetadatas
    {
        get => markerMetadatas;
    }

    private void Awake()
    {
        // Read the JSON file from StreamingAssets
        if (doUseRemoteConfig)
        {
            InvokeRepeating(nameof(LoadItemsFromJsonAsync), 0f, 10f);
        }
        else
        {
            LoadItemsFromJsonAsync();
        }
    }

    private void LoadItemsFromJsonAsync()
    {
        Debug.Log("[MarkerMetadataManager - LoadItemsFromJsonAsync] Loading JSON file from StreamingAssets...");

        void OnFileContentRetrieved(string jsonData)
        {
            // Parse the JSON content directly to our items list using Newtonsoft.Json
            List<MarkerJsonData> markerMetadatas = JsonConvert.DeserializeObject<List<MarkerJsonData>>(jsonData);

            // Log the results to verify
            Debug.Log($"[MarkerMetadataManager - OnFileContentRetrieved] Loaded {markerMetadatas.Count} items from JSON");
            this.markerMetadatas.Clear();
            //TODO Delete in future remove all previous children of root
            foreach (Transform child in root.transform)
            {
                Destroy(child.gameObject);
            }

            float counterX = 0;
            foreach (var item in markerMetadatas)
            {
                Debug.Log($"[MarkerMetadataManager - OnFileContentRetrieved] Item {item.ID}: {item.name} - {item.description}");
                // Store in the dictionary
                this.markerMetadatas[item.ID] = new MarkerData();
                this.markerMetadatas[item.ID].metadata = item;

                var prefabName = this.markerMetadatas[item.ID].metadata.prefabName;
                var markerPrefab = markerPrefabList.Find(x => x.name == prefabName);

                counterX += 5f;
                this.markerMetadatas[item.ID].vrObject = Instantiate(markerPrefab, new Vector3(counterX, 25f, 0), Quaternion.identity);
                Debug.Log("[MarkerMetadataManager - OnFileContentRetrieved] Instantiate name = [" + this.markerMetadatas[item.ID].vrObject.name +
                          "] | from prefabName = " + prefabName);
                //this.markerMetadatas[item.ID].vrObject.GetComponentInChildren<TMP_Text>().text = item.ID.ToString();
                this.markerMetadatas[item.ID].vrObject.name = item.name;
                this.markerMetadatas[item.ID].vrObject.transform.SetParent(root.transform);
                this.markerMetadatas[item.ID].vrObject.SetActive(isInstantiationActiveAtCreation);
            }

            //TODO TEMP
            // yield return new WaitForEndOfFrame();
            // Debug.Log("FFF Provo a caricare le regole personalizzate...");
            // FindObjectsByType<AddCustomRules>( FindObjectsSortMode.None)[0].AddRules();
            //


            Debug.Log("[MarkerMetadataManager - LoadItemsFromJsonAsync] ALL OK!");
        }

        if (doUseRemoteConfig)
        {
            StartCoroutine(
                UnityWebRequestUtils.GET_JSON(remoteServerUrl, OnFileContentRetrieved)
            );
        }
        else
        {
            IEnumerator ReadFileAfterSettingsReady()
            {
                yield return new WaitUntil(() => DualChatbotServerSettings.Instance.IsInitialized);
                string streamingAssetsJsonPath = DualChatbotServerSettings.Instance.GetSettings_Markers().urlJson;
                StartCoroutine(
                    StreamingAssetsUtils.GET_File(streamingAssetsJsonPath, OnFileContentRetrieved)
                );
            }
            StartCoroutine(ReadFileAfterSettingsReady());
        }
    }

    // private void Update()
    // {
        // Dictionary<int, MarkerData> arObjects = this.MarkerMetadatas;
        // Debug.Log("FFF metadataManager.MarkerMetadatas count=" + this.MarkerMetadatas.Count);
        // foreach (var key in arObjects.Keys)
        // { 
        //     Debug.Log($"HELLO 1A: arObjects key="+key);
        // }
        // Debug.Log("");
    // }
}

[System.Serializable]
public class MarkerJsonData
{
    public string name;
    public string description;
    public string prefabName;
    public int ID;
    public Vector3 trans_offset;
    public Vector3 scale;
}

[System.Serializable]
public class MarkerData
{
    public MarkerJsonData metadata;
    public GameObject vrObject;
}