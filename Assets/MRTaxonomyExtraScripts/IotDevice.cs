using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class IotDevice : MonoBehaviour
{
    public bool isInsideCamera { get; set; } = false;

    private bool last_isInsideCamera = false;
    private Vector3 last_position = Vector3.zero;

    private void Update()
    {
        if (Time.frameCount % Second2Frame(5) == 0) // 300 frames at 60 FPS is approximately 5 seconds
        {
            StartCoroutine(UpdateStateOnHASS());
        }
    }

    IEnumerator UpdateStateOnHASS()
    {
        // if (!gameObject.name.EndsWith("_real"))
        // {
        //     throw new Exception($"[UpdateStateOnHASS] IotDevice name ({gameObject.name}) must end with _real");
        // }
        Debug.Log("Device with name  " + gameObject.name + " and isInsideCamera " + isInsideCamera +
                  " is being updated on Home Assistant.");

        if (!HomeAssistantUnityIntegration.Instance.IsStarted)
        {
            Debug.Log($"[IotDevice] HomeAssistantUnityIntegration not ready. Skipping update for {gameObject.name}.");
            yield break; // No changes, skip the update            
        }
        if (last_isInsideCamera == isInsideCamera && last_position == gameObject.transform.position)
        {
            Debug.Log($"[IotDevice] No changes detected for {gameObject.name}. Skipping update.");
            yield break; // No changes, skip the update
        }

        // Create the JSON payload
        var jsonPayload = (
            sensor_name: gameObject.name, //TODO Rinominare in device_name??
            position: (
                x: gameObject.transform.position.x,
                y: gameObject.transform.position.y,
                z: gameObject.transform.position.z
            ),
            isInsideCamera = isInsideCamera
        );
        
        yield return
            HomeAssistantUnityIntegration.Instance.EndpointUtils_Update_IotDevice_Visibility(jsonPayload, null, () =>
            {
                last_isInsideCamera = isInsideCamera;
                last_position = gameObject.transform.position;
                Debug.Log(
                    $"[IotDevice] Update Successful of {gameObject.name}, with position={gameObject.transform.position} and isInsideCamera={isInsideCamera}");
            })
        ;
    }

    // Caching
    private (string Variables, string Actions) _cachedCapabilities;
    public (string Variables, string Actions) GetCapabilitiesAsDoubleString() => _cachedCapabilities;


    //region Utils
    private int Second2Frame(int seconds)
    {
        return seconds * 60; // Assuming 60 FPS
    }

    private void OnGUI()
    {
        /*if (GUI.Button(new Rect(10, 10, 200, 30), "Update IotDevice on HASS"))
        {
            StartCoroutine(UpdateStateOnHASS());
        }*/
    }
    //endregion
}