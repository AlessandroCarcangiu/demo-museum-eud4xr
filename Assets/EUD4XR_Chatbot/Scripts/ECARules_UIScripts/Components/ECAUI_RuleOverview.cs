using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ECAUI_RuleOverview : MonoBehaviour
{
    private Rule rule;
    public TextMeshProUGUI ruleText;
    public Button deleteButton;
    public Button editButton;

    private ECAUI_UIManager uiManager;
    private void Awake()
    {
        uiManager = GameObject.FindObjectOfType<ECAUI_UIManager>();
        if (uiManager == null)
        {
            throw new ArgumentNullException("uiManager", "uiManager must be set");
        }
    }

    public void SetRule(Rule rule)
    {
        if (rule == null)
        {
            throw new System.ArgumentNullException("rule", "rule must be set");
        }
        this.rule = rule;
        ruleText.text = RuleUtils.FormatRuleLabel(rule);
        
        deleteButton.onClick.AddListener(() => { uiManager.Intention_DeleteRuleFromUI(rule); });
        editButton.onClick.AddListener(() => { uiManager.Intention_EditRuleFromUI(this); });
    }
    
    public Rule GetRule()
    {
        return rule;
    }
}
