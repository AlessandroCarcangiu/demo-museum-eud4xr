using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IotDeviceUI_Capabilities_Prefab : MonoBehaviour
{
    public TMP_Text contentRef;
    public Button buttonRef;


    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
    }

    public void OnPrefabCreated([NotNull] string info, IotDevice iotDeviceInvolved)
    {
        if (iotDeviceInvolved == null) throw new Exception("objectInvolved is null");

        SetBody(info);
    }

    private void SetBody([NotNull] string s) => contentRef.text = s;
}