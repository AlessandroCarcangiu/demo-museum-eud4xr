using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;

public class ECAObjectUI_InvolvedRule_Prefab : MonoBehaviour
{
    public TMP_Text contentRef;
    public PressableButton buttonRef;

    
    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
    }

    public void OnPrefabCreated(Rule ruleToDisplay, ECAObject ecaObjectInvolved)
    {
        
        // Rule ecaObject = ECAObjectUI_Capabilities.Instance.dataSourceRef;
        if (ruleToDisplay == null) throw new Exception("ruleToDisplay is null");
        if (ecaObjectInvolved == null) throw new Exception("objectInvolved is null");
        
        SetBody(ruleToDisplay);
    }
    
    private void SetBody([NotNull] Rule rule) => contentRef.text = rule.ToString();
}
