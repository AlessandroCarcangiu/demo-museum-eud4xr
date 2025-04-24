using System;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.UI;

public class ECAUI_ShowSelectedRuleManager : MonoBehaviour
{
    // public GameObject ruleDetailsPrefab;
    // public GameObject objectContainingRules;
    public ECAUI_Rule ecaui_ruleScript;
    public PressableButton backRuleButton;
    public PressableButton saveRuleButton;
    private ECAUI_UIManager uiManager;
    
    // private ECAUI_Utils.RulePlaceholder rulePlaceholder;
    private ECAObjectInfo.ECAObjectsCapabilties infoCapabilities;
    private void Awake()
    {
       
        uiManager = GameObject.FindObjectOfType<ECAUI_UIManager>();
        if (uiManager == null)
        {
            throw new ArgumentNullException("uiManager", "uiManager must be set");
        }
        
        if (ecaui_ruleScript == null)
        {
            throw new ArgumentNullException("ecaui_RuleDetails", "ecaui_RuleDetails must be set");
        }
        
        if (backRuleButton == null)
        {
            throw new ArgumentNullException("backRuleButton", "backRuleButton must be set");
        }
        backRuleButton.OnClicked.AddListener(() => { uiManager.Intention_ShowAllRules(); });
        if (saveRuleButton == null)
        {
            throw new ArgumentNullException("saveRuleButton", "saveRuleButton must be set");
        }
        saveRuleButton.OnClicked.AddListener(() => { uiManager.Intention_SaveRuleEditedFromUI(ecaui_ruleScript.GetRule(), ecaui_ruleScript.GetRulePlaceholder()); });
    }
    
    private void OnEnable()
    {
        this.ClearView();

        if (uiManager.SelectedRule == null)
        {
            // The user want to create a new rule from scratch
            throw new NotImplementedException();
        }

        infoCapabilities = ECAObjectInfo.Instance.GetAllInfoAboutCurrentECAObjects_Cached();
        DrawView(uiManager.SelectedRule);
    }

    private void OnDisable()
    {
        this.ClearView();
    }
   
    
    private void DrawView(Rule rule)
    {
        // rulePlaceholder = ECAUI_Utils.RulePlaceholder.FromRule(uiManager.SelectedRule, "NaN");

        if (rule == null)
        {
            //todo Probably create an empty RulePlaceholder
            throw new System.ArgumentNullException("rule", "rule must be set");
        }
        
        ecaui_ruleScript.DrawView(rule, infoCapabilities);
    }

    private void ClearView()
    {
        ecaui_ruleScript.ClearView();
    }
    
    
    ///////// NEW CODE /////////
    // private void AddRulePlaceholder(string subject, string verb)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // private void DeleteRulePlaceholder([CanBeNull] string args)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // private void UpdateRulePlaceholder([CanBeNull] string args)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // private void SerializeRulePlaceholderInRuleEngine()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // private void SerializeAllRulePlaceholders() { throw new NotImplementedException(); }
    //
    // private void HandleSetInfoInUnity([CanBeNull] string args)
    // {
    //     // Recall what this method does.
    //     // P.S. I think this method takes the stringified JSON from Unity and set it to React. Prolly to remove
    //     throw new NotImplementedException();
    // }
    
}
