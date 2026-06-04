using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.Utils;
using TMPro;
using UnityEngine;

public class IotDeviceUI_Capabilities : Singleton<IotDeviceUI_Capabilities>
{
    public GameObject uiNoInfoPrefab;
    public GameObject uiInfoItemPrefab; // It contains 
    public GameObject listParent;

    public TMP_Text titleRef;


    private void Awake()
    {
        // Check for ref not null
        if (uiNoInfoPrefab == null) throw new Exception("uiNoInfoPrefab is null");
        if (uiInfoItemPrefab == null) throw new Exception("uiInfoItemPrefab is null");
        if (listParent == null) throw new Exception("listParent is null");

        if (titleRef == null) throw new Exception("titleRef is null");
    }

    private void OnEnable()
    {
        var iotDevice = IotDeviceUI_UIManagerSingleton.Instance.dataSourceRef;
        if (iotDevice == null) return;

        //TODO Ottenere una lista di stringhe o un dict {"variables": <>, "actions": <>}
        (string Variables, string Actions) t = iotDevice.GetCapabilitiesAsDoubleString();
        
        // Instantiate the prefab and add it to the list
        var ruleItem = Instantiate(uiInfoItemPrefab, listParent.transform);
        // Set the rule to the prefab
        var ruleItemScript = ruleItem.GetComponent<IotDeviceUI_Capabilities_Prefab>();
        // var s = $"test {i}";
        ruleItemScript.OnPrefabCreated(t.Variables, iotDevice);
        
        // Instantiate the prefab and add it to the list
        var ruleItem2 = Instantiate(uiInfoItemPrefab, listParent.transform);
        // Set the rule to the prefab
        var ruleItemScript2 = ruleItem2.GetComponent<IotDeviceUI_Capabilities_Prefab>();
        // var s = $"test {i}";
        ruleItemScript2.OnPrefabCreated(t.Actions, iotDevice);
        
        
        SetTitle(iotDevice);
    }

    private void OnDisable()
    {
        // Remove all children excluded the first from listParent
        for (int i = listParent.transform.childCount - 1; i > 0; i--)
        {
            Destroy(listParent.transform.GetChild(i).gameObject);
        }
    }

    private void SetTitle([NotNull] GameObject gO)
    {
        const string template = "<size=11>Info di </size><size=11><color=#548AF7>{0}</color></size>";
        // const string template = "<size=14>Info </size><size=11>di <color=#7f7f7f>{0}</color></size>";
        titleRef.text = string.Format(template, gO.name);
    }

    private void SetTitle([NotNull] IotDevice iotDevice) => SetTitle(iotDevice.gameObject);
}