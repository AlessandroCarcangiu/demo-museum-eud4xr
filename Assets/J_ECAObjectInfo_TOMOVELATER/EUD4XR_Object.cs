using System;
using ECARules4All_DLL;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(BoxCollider))]
public class EUD4XR_Object : MonoBehaviour
{
    private ECAObject _ecaObject;
    void Start()
    {
        _ecaObject = GetComponent<ECAObject>();
        if (_ecaObject == null)
        {
            throw new Exception("EUD4XR_Object attached to a NON-ECAObject! If you want to use this script, please attach it to an ECAObject, otherwise remove it.");
        }
        
        // make the box collider trigger and slightly bigger
        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = boxCollider.size * 2.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        Debug.Log("EUD4XR_Object OnTriggerEnter");
        CapabilityManager.Instance.ShowCapability(_ecaObject, other.transform);       
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        Debug.Log("EUD4XR_Object OnTriggerExit");
        CapabilityManager.Instance.HideCapability();
    }   
}
