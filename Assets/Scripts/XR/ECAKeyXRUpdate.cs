using ECARules4All_DLL;
using UnityEngine;
using ECARules4All_DLL.Taxonomies.Objects.Taverna;           
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;
using ECARules4All_DLL.Utils;                            

public class ECAKeyXRUpdate : MonoBehaviour
{
    private ECAKey _key;
    private ECAXRGrabbable _xrGrabbable;
    private bool _lastGrabbedState = false;

    private void Awake()
    {
        _key = GetComponent<ECAKey>();
        _xrGrabbable = GetComponent<ECAXRGrabbable>();

        if (_key == null || _xrGrabbable == null)
        {
            Debug.LogError($"[{gameObject.name}] Manca ECAKey o ECAXRGrabbable sulla chiave!");
        }
    }

    private void Update()
    {
        if (_xrGrabbable == null || _key == null) return;

        bool currentGrabbed = _xrGrabbable.grabbed.GetBoolType() == ECABoolean.BoolType.TRUE;

        if (currentGrabbed != _lastGrabbedState)
        {
            _lastGrabbedState = currentGrabbed;

            _key.isPickedUp = currentGrabbed? ECABoolean.TRUE: ECABoolean.FALSE;
            
            Debug.Log($" Sincronizzato isPickedUp (DLL) con grabbed (Unity XR): {currentGrabbed}");
        }
    }
}