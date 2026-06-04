using System;
using System.Collections.Generic;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using UnityEngine;

public class IotDeviceManager : Singleton<IotDeviceManager>
{
    public string GetTypePlusNameAsString(IotDevice iotDevice)
    {
        return $"The [TODO GET TYPE] {iotDevice}";
    }
    
    
    private Dictionary<string, DeviceCapabilities> _deviceCache = new();
    private void Start()
    {
        // Caching endpoints
        StartCoroutine(HomeAssistantUnityIntegration.Instance.EndpointUtils_Get_IotDevices_Capabilities(
            (jsonString) =>
            {
                // Parse the JSON into dictionary
                _deviceCache = JsonConvert.DeserializeObject<Dictionary<string, DeviceCapabilities>>(jsonString);

                Debug.Log("HHHH Device capabilities cached.");
            }));
    }

    private void Update()
    {
        //Debug.Log("HHHH Device cache contains " + _deviceCache.Count + " devices.");
    }

    // Getter to retrieve a device’s capabilities by name
    public DeviceCapabilities GetDeviceCapabilities(string deviceName)
    {
        if (_deviceCache.TryGetValue(deviceName, out var capabilities))
        {
            return capabilities;
        }

        Debug.LogWarning($"HHHH Device '{deviceName}' not found in cache.");
        return null;
    }

    private void OnGUI()
    {
        // if button top right corner is pressed
        /*if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 30), "Test GetDeviceCapabilities"))
        {
           var x = this.GetDeviceCapabilities("NetatmoEUD4XR");
           Debug.Log("HHHH Capabilities for NetatmoEUD4XR: " + JsonConvert.SerializeObject(x) );
        }*/
    }
}

[Serializable]
public class DeviceCapabilities
{
    public string Fullname;
    public List<string> Variables;
    public List<string> Actions;
}