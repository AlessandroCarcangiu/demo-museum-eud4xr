using System;
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
        uiManager = ECAUI_UIManager.Instance;
        if (uiManager == null)
        {
            throw new ArgumentNullException("uiManager", "uiManager must be set");
        }
    }

    public void SetRule(Rule rule)
    {
        this.rule = rule ?? throw new ArgumentNullException("rule", "rule must be set");
        ruleText.text = RuleUtils.FormatRuleLabel(rule);
        
        deleteButton.onClick.AddListener(() => { uiManager.Intention_DeleteRuleFromUI(rule); });
        editButton.onClick.AddListener(() => { uiManager.Intention_EditRuleFromUI(this); });
    }
    
    public Rule GetRule()
    {
        return rule;
    }
}
