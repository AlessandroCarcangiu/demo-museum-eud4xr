using System;
using System.Linq;
using ECARules4All_DLL;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(BoxCollider))]
public class EUD4XR_Player_Object : MonoBehaviour
{
    public Transform playerTransform;
    void Start()
    {
        // _ecaObject = GetComponent<ECAObject>();
        if (playerTransform == null)
        {
            throw new Exception("YOU MUST ASSIGN THE PLAYER TRANSFORM TO THE PLAYER OBJECT");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        var ecaObject = other.transform.GetComponent<ECAObject>();
        if (ecaObject == null || other.gameObject.CompareTag("Player")) return;
        Debug.LogWarning("EUD4XR_Object OnCollisionEnter with " + other.gameObject.name);
        throw new NotImplementedException("OnCollisionEnter Not implemented yet");
    }
    

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("EUD4XR_Object OnTriggerEnter with " + other.gameObject.name);
        var ecaObject = other.GetComponent<ECAObject>();
        if (ecaObject == null || other.gameObject.CompareTag("Player")) return;
        
        Debug.Log("EUD4XR_Object OnTriggerEnter");

        ECAObjectUI_UIManagerSingleton.Instance.PlayerStartsTriggeringEcaObject(ecaObject, playerTransform); 
    }
    
    private void OnTriggerExit(Collider other)
    {
        var ecaObject = other.GetComponent<ECAObject>();
        if (ecaObject == null || other.gameObject.CompareTag("Player")) return;

        Debug.Log("EUD4XR_Object OnTriggerExit");
        ECAObjectUI_UIManagerSingleton.Instance.PlayerStopsTriggeringEcaObject(ecaObject, playerTransform);
    }   
}
