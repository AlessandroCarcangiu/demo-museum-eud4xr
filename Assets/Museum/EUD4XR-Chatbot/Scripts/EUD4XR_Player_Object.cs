using ECARules4All_DLL;
using ECARules4All_DLL.Taxonomies.Utils;
using ECARules4All_DLL.Utils;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(BoxCollider))]
public class EUD4XR_Player_Object : Singleton<EUD4XR_Player_Object>
{
    // public GameObject playerGoRef;
    // public Transform playerTransform;
    // public ECACharacter playerEcaCharacterRef;
    // void Start()
    // {
    //     if (playerTransform == null)
    //     {
    //         throw new Exception("YOU MUST ASSIGN THE PLAYER TRANSFORM TO THE PLAYER OBJECT");
    //     }
    //     
    //     if (playerEcaCharacterRef == null)
    //     {
    //         throw new Exception("YOU MUST ASSIGN THE ECA CHARACTER REFERENCE TO THE PLAYER OBJECT");
    //     }
    // }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return;

        var ecaObject = other.transform.GetComponent<ECAObject>();
        if (ecaObject != null)
        {
            Debug.LogWarning("EUD4XR_Object OnCollisionEnter with [ECAObject] " + other.gameObject.name);
            return;
        }
        
        
        var iotDevice = other.transform.GetComponent<IotDevice>();
        if (iotDevice != null)
        {
            Debug.LogWarning("EUD4XR_Object OnCollisionEnter with [IoTDevice] " + other.gameObject.name);
            return;
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[EUD4XR_Object: OnTriggerEnter] {this.gameObject} is triggering with {other.gameObject.name}");

        if (other.gameObject.CompareTag("Player")) return;

        
        //var ecaObject = other.GetComponent<ECAObject>();
        var ecaObject = other.GetComponentInParent<ECAObject>();
        if (ecaObject != null)
        {
            Debug.Log("EUD4XR_Object OnTriggerEnter is an ECAObject " + other.gameObject.name);
            ECAObjectUI_UIManagerSingleton.Instance.PlayerStartsTriggeringEcaObject(ecaObject, ECAPlayer_Singleton.Instance.playerTransform); 
            return;            
        }
        
        var iotDevice = other.GetComponent<IotDevice>();
        if (iotDevice != null)
        {
            Debug.Log("EUD4XR_Object OnTriggerEnter is an IoTDevice " + other.gameObject.name);
            IotDeviceUI_UIManagerSingleton.Instance.PlayerStartsTriggeringIotDevice(iotDevice, ECAPlayer_Singleton.Instance.playerTransform); 
            return;            
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[EUD4XR_Object: OnTriggerExit] {this.gameObject} has stopped triggering with {other.gameObject.name}");

        
        if (other.gameObject.CompareTag("Player")) return;

        
        var ecaObject = other.GetComponent<ECAObject>();
        if (ecaObject != null)
        {
            Debug.Log("EUD4XR_Object OnTriggerExit is an ECAObject " + other.gameObject.name);
            ECAObjectUI_UIManagerSingleton.Instance.PlayerStopsTriggeringEcaObject(ecaObject, ECAPlayer_Singleton.Instance.playerTransform); 
            return;
        }

        var iotDevice = other.GetComponent<IotDevice>();
        if (iotDevice != null)
        {
            Debug.Log("EUD4XR_Object OnTriggerEnter is an IoTDevice " + other.gameObject.name);
            IotDeviceUI_UIManagerSingleton.Instance.PlayerStopsTriggeringIotDevice(iotDevice, ECAPlayer_Singleton.Instance.playerTransform); 
            return;            
        }
    }

    public void ForceTriggerExit(GameObject other)
    {
        Debug.Log($"[EUD4XR_Object: ForceTriggerExit] {this.gameObject} has stopped triggering with {other.gameObject.name}");
        
        if (other.gameObject.CompareTag("Player")) return;
        
        var ecaObject = other.GetComponent<ECAObject>();
        if (ecaObject != null)
        {
            Debug.Log("EUD4XR_Object OnTriggerExit is an ECAObject " + other.gameObject.name);
            ECAObjectUI_UIManagerSingleton.Instance.PlayerStopsTriggeringEcaObject(ecaObject, ECAPlayer_Singleton.Instance.playerTransform); 
            return;
        }

        var iotDevice = other.GetComponent<IotDevice>();
        if (iotDevice != null)
        {
            Debug.Log("EUD4XR_Object OnTriggerEnter is an IoTDevice " + other.gameObject.name);
            IotDeviceUI_UIManagerSingleton.Instance.PlayerStopsTriggeringIotDevice(iotDevice, ECAPlayer_Singleton.Instance.playerTransform); 
            return;            
        }
    }
}
