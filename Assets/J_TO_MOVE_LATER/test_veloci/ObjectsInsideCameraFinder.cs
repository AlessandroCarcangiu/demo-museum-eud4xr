using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsInsideCameraFinder : MonoBehaviour
{
    public Camera xrCamera; // Assign the XR camera here

    private void Awake()
    {
        if (xrCamera == null)
        {
            throw new ArgumentNullException("xrCamera", "xrCamera must be set");
        }
    }

    // If you want to test it.
    // private void OnGUI()
    // {
    //     void PrintObjectsInsideCamera()
    //     {
    //         // var l = GetObjectsInsideCamera();
    //         var l = GetECAObjectsInsideCamera();
    //         Debug.Log("Objects inside camera: " + l.Count);
    //         Debug.Log(string.Join("\n", l));
    //     }
    //     
    //     
    //     if (GUI.Button(new Rect(10, 10, 150, 100), "Print Objects Inside Camera"))
    //     {
    //         PrintObjectsInsideCamera();
    //     }
    // }

    List<GameObject> GetObjectsInsideCamera()
    {
        List<GameObject> objectsInsideCamera = new List<GameObject>();

        // Calculate the frustum planes of the XR camera
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(xrCamera);

        // Get all active GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Skip inactive objects
            if (!obj.activeInHierarchy) continue;

            // Get the object's Renderer component for bounds
            Renderer renderer = obj.GetComponent<Renderer>();

            if (renderer != null)
            {
                // Check if the object's bounds intersect with the frustum planes
                if (GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds))
                {
                    objectsInsideCamera.Add(obj); // Add to the list if inside frustum
                }
            }
        }

        // Now 'objectsInFrustum' contains all GameObjects within the camera's frustum

        return objectsInsideCamera;
    }


    public List<GameObject> GetEcaObjectsInsideCamera(bool onlyActive = false)
    {
        List<GameObject> objectsInsideCamera = new List<GameObject>();

        // Calculate the frustum planes of the XR camera
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(xrCamera);

        // Get all active GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if the object has the ECAObject component of ECARules4All_Dll namespace. If not, skip it.
            if (obj.TryGetComponent(out ECARules4All_DLL.ECAObject ecaScriptRef)) continue;

            // Skip inactive objects
            if (onlyActive && !obj.activeInHierarchy) continue;

            // Get the object's Renderer component for bounds
            Renderer renderer = obj.GetComponent<Renderer>();

            if (renderer != null)
            {
                // Check if the object's bounds intersect with the frustum planes
                if (GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds))
                {
                    objectsInsideCamera.Add(obj); // Add to the list if inside frustum
                    // ecaScriptRef.insideCamera = true; // TODO Carca de-commentali con il nome corretto della variabile
                }
                else
                {
                    // If the object is not inside the camera, we disable the insideCamera flag in the ECAObject script
                    // ecaScriptRef.insideCamera = false; // TODO Carca de-commentali con il nome corretto della variabile
                }
            }
            else
            {
                // If the ECAScript doesn't have a Renderer, we can't check if it's inside the camera, thus we disable the flag. Correct?
                // ecaScriptRef.insideCamera = false; TODO Carca de-commentali con il nome corretto della variabile
            }
        }

        //TODO Do we want to return the ECAObjects list
        return objectsInsideCamera;
    }
    
}