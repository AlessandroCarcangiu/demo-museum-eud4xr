using System;
using System.Collections;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.UI;
using ECARules4All_DLL.Utils;
using UnityEngine;

public class ECAUI_Rule : MonoBehaviour
{
    private Rule _rule;
    private ECAUI_Utils.RulePlaceholder _rulePlaceholder;
    public ECAUI_Utils.RulePlaceholder GetRulePlaceholder() {
        var rule = new ECAUI_Utils.RulePlaceholder
        {
            When = this.whenActionUI.GetActionPlaceholder(),
            Then = new List<ECAUI_Utils.ActionPlaceholder>()
        };
        foreach (Transform child in thenActionParent.transform)
        {
            rule.Then.Add(child.GetComponent<ECAUI_Action>().GetActionPlaceholder());
        }
        return rule;
    }
    
    public ECAUI_Action whenActionUI;
    // [SerializeField] private List<ECAUI_Action> thenActionsUI = new List<ECAUI_Action>();
    public GameObject thenActionPrefab;
    public GameObject thenActionParent;
    
    private void Awake()
    {
        if (whenActionUI == null)
        {
            throw new ArgumentNullException("whenAction", "whenAction must be set");
        }
        
        if (thenActionPrefab == null)
        {
            throw new ArgumentNullException("thenActionPrefab", "thenActionPrefab must be set");
        }
        
        if (thenActionParent == null)
        {
            throw new ArgumentNullException("thenActionParent", "thenActionParent must be set");
        }
        
        // if (thenActionsUI.Count != 0)
        // {
        //     throw new ArgumentNullException("thenActions", "thenActions must have 0 prefab by default");
        // }
    }

    private void SetRule(Rule r)
    {
        if (r == null)
        {
            //todo this mean that the user want to create a new rule from scratch. Handle this in the future?
            throw new ArgumentNullException("rule", "rule must be set");
        }
        
        this._rule = r;
    }
    
    private void UnsetRule()
    {
        _rule = null;
    }
    public Rule GetRule() => _rule;

    public void DrawView(Rule r, ECAObjectInfo.ECAObjectsCapabilties infoCapabilities)
    {
        whenActionUI.gameObject.SetActive(true);
        this.SetRule(r);
        // this.infoCapabilities = infoCapabilities;
        whenActionUI.SetUIParameters(ECAUI_Action.ActionPreLabel.When, r.GetEvent(), infoCapabilities, this, false);
        for (var i = 0; i < r.GetActions().Count; i++)
        {
            var realAction = r.GetActions()[i];
            var thenAction = Instantiate(thenActionPrefab, thenActionParent.transform).GetComponent<ECAUI_Action>();
            
            var doEnableDelete = i > 0;
            var preLabel = i == 0 ? ECAUI_Action.ActionPreLabel.Then : ECAUI_Action.ActionPreLabel.None;
            thenAction.SetUIParameters(preLabel, realAction, infoCapabilities,this, i > 0);
        }
    }

    public void ClearView()
    {
        whenActionUI.gameObject.SetActive(false);
        // whenActionUI.enabled = false;
        // remove all children in thenActionParent except the first one
        for (var i = 0; i < thenActionParent.transform.childCount; i++)
        {
            Destroy(thenActionParent.transform.GetChild(i).gameObject);
        }
        // thenActionsUI.Clear();
        this.UnsetRule();
    }
    
    public void RemoveThenAction(ECAUI_Action actionUI)
    {
        if (actionUI == null)
        {
            throw new ArgumentNullException("actionUI", "action must be set");
        }
        
        // if (thenActionsUI.Count == 0)
        // {
        //     throw new ArgumentNullException("thenActions", "thenActions must have at least 1 prefab");
        // }
        //
        // if (!thenActionsUI.Contains(actionUI))
        // {
        //     throw new ArgumentNullException("actionUI", "action must be in the list");
        // }
        
        // thenActionsUI.Remove(actionUI);
        Destroy(actionUI.gameObject);
    }
}
