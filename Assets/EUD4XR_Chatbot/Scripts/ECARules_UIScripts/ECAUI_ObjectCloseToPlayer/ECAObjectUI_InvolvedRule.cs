using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using TMPro;
using UnityEngine;

public class ECAObjectUI_InvolvedRule : Singleton<ECAObjectUI_InvolvedRule>
{
    public GameObject uiNoRulePrefab;
    public GameObject uiRuleItemPrefab; // It contains 
    public GameObject listParent;

    public TMP_Text titleRef;
    public GameObject T_GoToRules;
    

    private void Awake()
    {
        // Check for ref not null
        if (uiNoRulePrefab == null) throw new Exception("uiNoRulePrefab is null");
        if (uiRuleItemPrefab == null) throw new Exception("uiRuleItemPrefab is null");
        if (listParent == null) throw new Exception("listParent is null");

        if (titleRef == null) throw new Exception("titleRef is null");
        if (T_GoToRules == null) throw new Exception("T_GoToRules is null");
    }

    private void OnEnable()
    {
        var ecaObject = ECAObjectUI_UIManagerSingleton.Instance.dataSourceRef;
        if (ecaObject == null) return;

        var rulesInvolvingEcaObject = RuleEngine.GetInstance().GetRulesInvolvingGameObject(ecaObject.gameObject);

        if (rulesInvolvingEcaObject.Count == 0)
            Instantiate(uiNoRulePrefab, listParent.transform);
        else
            foreach (var rule in rulesInvolvingEcaObject)
            {
                // Instantiate the prefab and add it to the list
                var ruleItem = Instantiate(uiRuleItemPrefab, listParent.transform);

                // Set the rule to the prefab
                var ruleItemScript = ruleItem.GetComponent<ECAObjectUI_InvolvedRule_Prefab>();
                ruleItemScript.OnPrefabCreated(rule, ecaObject);
            }

        SetTitle(ecaObject);
        
        T_GoToRules.SetActive(false);
    }

    private void OnDisable()
    {
        // Remove all children excluded the first from listParent
        for (int i = listParent.transform.childCount - 1; i > 0; i--)
        {
            Destroy(listParent.transform.GetChild(i).gameObject);
        }
    }

    private void SetTitle([NotNull] GameObject ecaObject)
    {
        const string template = "<size=14>Regole </size><size=11>con <color=#7f7f7f>{0}</color></size>";
        titleRef.text = string.Format(template, ecaObject.gameObject.name);
    }

    private void SetTitle([NotNull] ECAObject ecaObject) => SetTitle(ecaObject.gameObject);
}