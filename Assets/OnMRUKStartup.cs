using System;
using System.Collections;
using UnityEngine;
using Meta.XR.MRUtilityKit;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class OnMRUKStartup : MonoBehaviour
{
    public bool doLog = false; // Abilita o disabilita i log

    public MRUKAnchor.SceneLabels labelFilter;
    private GameObject pointedPlane = null;
    public GameObject tablePrefab; // Prefab da associare ai piani TABLE

    [Obsolete]
    void Start()
    {
        if (doLog)
        {
            Debug.Log("RARARAR MRUK.Instance: " + MRUK.Instance);
            Debug.Log("RARARAR MRUK.Instance.GetCurrentRoom:" + MRUK.Instance.GetCurrentRoom());
        }

        MRUKRoom room = MRUK.Instance.GetCurrentRoom();

        if (room == null)
        {
            if (doLog)
                throw new Exception("[DisplayLabel] START - Nessuna MRUKRoom trovata!");
        }

        foreach (var anchor in room.Anchors)
        {
            if (anchor != null)
            {
                GameObject anchorObj = anchor.gameObject;
                BoxCollider boxCollider = anchorObj.AddComponent<BoxCollider>();
                boxCollider.isTrigger = false;
            }
        }

        if (doLog)
            Debug.Log("BoxCollider aggiunti a tutti i piani!");


        StartCoroutine(SpawnObjectsOnPlanes());
    }

    [Obsolete]
    private IEnumerator SpawnObjectsOnPlanes()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        while (room == null)
        {
            if (doLog)
                Debug.LogError("[DisplayLabel] SpawnObjectsOnPlanes - Nessuna MRUKRoom trovata!");
            yield return null;
            room = MRUK.Instance.GetCurrentRoom();
        }

        foreach (var anchor in room.Anchors)
        {
            if (doLog)
                Debug.Log("AAASDADSASDA START anchor.gameObjectName" + anchor.gameObject.name);
            if (anchor != null && anchor.AnchorLabels.Count > 0)
            {
                if (doLog)
                    Debug.Log("AAASDADSASDA START anchor.gameObjectName (dentro il loop): " + anchor.gameObject.name);
                string label = anchor.AnchorLabels[0];
                if (doLog) Debug.Log("AAASDADSASDA START anchor Label (dentro il loop): " + label);
                Vector3 position = anchor.transform.position;
                Quaternion rotation = anchor.transform.rotation;

                GameObject prefabToSpawn = null;

                switch (label)
                {
                    case "TABLE":
                        prefabToSpawn = tablePrefab; // Instanzia il prefab associato via inspector
                        break;
                    case "FLOOR":
                        //prefabToSpawn = dirtyFloorPrefab; // Cubo giallo per FLOOR
                        break;
                    default:
                        // prefabToSpawn = cubePrefabBIANCO; // Cubo bianco per altri casi
                        break;
                }

                if (prefabToSpawn != null)
                {
                    // if (doLog) Debug.Log("AAASDADSASDA START PREFAB TO SPAWN NOT NULL: " + prefabToSpawn.name);
                    Vector3 offsetPosition = anchor.transform.position - 0.1f * Vector3.up;
                    Renderer newRenderer = anchor.GetComponent<Renderer>()
                                           ?? pointedPlane.GetComponentInChildren<Renderer>();
                    GameObject spawnedObject = Instantiate(prefabToSpawn, position, rotation);
                    spawnedObject.transform.SetParent(anchor.transform);
                    // if (doLog) Debug.Log($"AAASDADSASDA PRIMA DI FITINSIDE");
                    //MeshManager.FitInside(spawnedObject.transform, newRenderer);
                    // if (doLog) Debug.Log($"AAASDADSASDA DOPO DI FITINSIDE");
                    spawnedObject.transform.position = offsetPosition;
                    // if (doLog) Debug.Log($"AAASDADSASDA Oggetto {prefabToSpawn.name} spawnato su {label} a {position}");
                }
            }
        }
    }

}