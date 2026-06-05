using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using Oculus.Interaction;
using TMPro;
// using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class ECAObjectUI_InvolvedRule_Prefab : MonoBehaviour
{
    public TMP_Text contentRef;
    // public Button buttonRef;


    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        // if (buttonRef == null) throw new Exception("buttonRef is null");
    }

    public void OnPrefabCreated(Rule ruleToDisplay, ECAObject ecaObjectInvolved, PokeInteractable pokeInteractable)
    {
        GetComponent<PokeInteractableVisual>().InjectPokeInteractable(pokeInteractable);
        GetComponent<PokeInteractableVisual>().enabled = true;
        
        // Rule ecaObject = ECAObjectUI_Capabilities.Instance.dataSourceRef;
        if (ruleToDisplay == null) throw new Exception("ruleToDisplay is null");
        if (ecaObjectInvolved == null) throw new Exception("objectInvolved is null");

        SetBody(ruleToDisplay);
        
        // buttonRef.OnClicked.RemoveAllListeners();
        // buttonRef.onClick.AddListener(() =>
        // {
        //     ECAUI_UIManager.Instance.Intention_EditRuleFromUI(ruleToDisplay);
        //     // ECAUI_UIManager.Instance.GetComponent<Waypoint_Indicator>().enabled = true; //TODOLATER importare Waypoint_Indicator in the future
        //     ECAObjectUI_InvolvedRule.Instance.T_GoToRules.SetActive(true); // Do we want to make it more clear with a callback from ECAObjectUI_InvolvedRule?
        // });
    }

    private void SetBody([NotNull] Rule rule) => contentRef.text = RuleUtils.FormatRuleLabel(rule);

    // private void OnGUI()
    // {
        // WPI_Manager.
    // }
}