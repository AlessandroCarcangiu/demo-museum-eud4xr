using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ECASocket : MonoBehaviour
{
    private XRSocketInteractor _socketInteractor;
    public ECAOutline previewOutline;
    private void Awake()
    {
        _socketInteractor = GetComponent<XRSocketInteractor>();
        if (_socketInteractor == null)
        {
            Debug.LogError("No XRSocketInteractor found on " + gameObject.name);
        }
        
        if (previewOutline == null)
        {
            Debug.LogWarning("[WARNING] No ECAOutline found on " + gameObject.name);
        }
        _socketInteractor.selectEntered.AddListener(OnObjectSet);
        _socketInteractor.selectExited.AddListener(OnObjectUnset);
    }

    [StateVariable("content", ECARules4AllType.Text)]
    [ECARelevance(true)]
    public string objectName
    {
        get => _objectName;
        set
        {
            _objectName = value;
            ECAScript.NotifyUpdate(this, nameof(objectName), objectName);
        }
    }
    // [SerializeField]
    private string _objectName;
    private void OnObjectSet(SelectEnterEventArgs args)
    {
        var gO = args.interactableObject.transform.gameObject;
        ECAObject gO_ECAObject = gO.GetComponent<ECAObject>();
        if (gO_ECAObject == null) {return;}
        
        Debug.Log("Hand entered socket with " + gO.name);
        objectName = gO.name;
        if (previewOutline) previewOutline.enabled = false;
    }  
    
    private void OnObjectUnset(SelectExitEventArgs args)
    {
        var gO = args.interactableObject.transform.gameObject;
        ECAObject gO_ECAObject = gO.GetComponent<ECAObject>();
        if (gO_ECAObject == null) {return;}
        
        Debug.Log("Hand exited socket with " + gO.name);
        objectName = string.Empty;
        if (previewOutline) previewOutline.enabled = true;
    }
}
