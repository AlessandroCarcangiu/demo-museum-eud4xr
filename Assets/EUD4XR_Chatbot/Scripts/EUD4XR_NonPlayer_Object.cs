using System;
using System.Linq;
using ECARules4All_DLL;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(BoxCollider))]
public class EUD4XR_NonPlayer_Object : MonoBehaviour
{
    [SerializeField] private ECAObject _ecaObject;
    void Start()
    {
        _ecaObject = GetComponent<ECAObject>();
        if (_ecaObject == null)
        {
            throw new Exception("EUD4XR_Object attached to a NON-ECAObject! If you want to use this script, please attach it to an ECAObject, otherwise remove it.");
        }

        // Check if there is at least a collider
        if (this.GetComponent<Collider>() == null)
        {
            var boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            // boxCollider.size = boxCollider.size * 2.5f;
        }
    }

}
