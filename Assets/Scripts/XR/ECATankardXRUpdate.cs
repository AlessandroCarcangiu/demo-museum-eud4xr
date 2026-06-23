using ECARules4All_DLL;
using UnityEngine;
using ECARules4All_DLL.Taxonomies.Objects.Taverna;             // Dalla DLL
using ECARules4All_DLL.Taxonomies.Behaviours.Subcategories;   // Dalla DLL (per ECAXRGrabbable)
using ECARules4All_DLL.Utils;                                 // Dalla DLL (per ECABoolean e ECAScript)

public class ECATankardXRUpdate : MonoBehaviour
{
    private ECATankard _tankard;
    private ECAXRGrabbable _xrGrabbable;
    private bool _lastGrabbedState = false;

    private void Awake()
    {
        _tankard = GetComponent<ECATankard>();
        _xrGrabbable = GetComponent<ECAXRGrabbable>();

        if (_tankard == null || _xrGrabbable == null)
        {
            Debug.LogError($"[{gameObject.name}] Manca ECATankard o ECAXRGrabbable sul boccale!");
        }
    }

    private void Update()
    {
        if (_xrGrabbable == null || _tankard == null) return;

        bool currentGrabbed = _xrGrabbable.grabbed.GetBoolType() == ECABoolean.BoolType.YES;

        if (currentGrabbed != _lastGrabbedState)
        {
            _lastGrabbedState = currentGrabbed;

            _tankard.isHeld = new ECABoolean(currentGrabbed ? ECABoolean.BoolType.TRUE : ECABoolean.BoolType.FALSE);
            
            ECAScript.NotifyUpdate(_tankard, nameof(_tankard.isHeld), _tankard.isHeld.ToString());
            
            Debug.Log($" Sincronizzato isHeld (DLL) con grabbed (Unity XR): {currentGrabbed}");
        }
    }
}