using System;
using ECARules4All_DLL;
using JetBrains.Annotations;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ECAObjectUI_Capabilities_Prefab : MonoBehaviour
{
    public TMP_Text contentRef;
    public Button buttonRef;

    
    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
    }

    public void OnPrefabCreated([NotNull] string info, ECAObject ecaObjectInvolved, PokeInteractable pokeInteractable)
    {
        GetComponent<PokeInteractableVisual>().InjectPokeInteractable(pokeInteractable);

        // Rule ecaObject = ECAObjectUI_Capabilities.Instance.dataSourceRef;
        // if (ruleToDisplay == null) throw new Exception("ruleToDisplay is null");
        if (ecaObjectInvolved == null) throw new Exception("objectInvolved is null");
        
        SetBody(info);
    }
    
    private void SetBody([NotNull] string s) => contentRef.text = s;
}
