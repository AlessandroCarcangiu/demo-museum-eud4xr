using System;
using System.Collections.Generic;
using System.Linq;
using ECARules4All_DLL;
using ECARules4All_DLL.UI;
using ECARules4All_DLL.Utils;
using UnityEngine;
using Action = ECARules4All_DLL.Action;

public class ECAUI_UIManager : Singleton<ECAUI_UIManager>
{
    public ECAUI_ShowAllRulesManager showAllRules;
    public ECAUI_ShowSelectedRuleManager showSelectedRule;
    // private ECAUI_RuleOverview _selectedUIRuleRendering = null; // useless
    
    public Transform playerCamera;
    public float distanceFromCamera = 2.0f;

    private List<Rule> rules = new List<Rule>();
    private Rule selectedRule = null;
    public Rule SelectedRule => selectedRule;

    private void Awake()
    {
        //////// DEBUG CODE: ADDING RULES ////////
        var fishAnimal = GameObject.Find("FishAnimal");
        var landVeh = GameObject.Find("LandVehicle");
        var artInt = GameObject.Find("ArtInteractable");
        var ecaLuce = GameObject.Find("EcaLuce");

        // if Any of this gameobject are null, skil
        if (fishAnimal != null && landVeh != null && artInt != null && ecaLuce != null)
        {
            var r = Rule.TryCreateRule(
                // new Action(GameObject.Find("FishAnimal"), "interacts with", artInt),
                new Action(fishAnimal, "activates"),
                new List<Action>
                {
                    new Action(artInt, "deactivates"),
                    new Action(landVeh, "deactivates")
                }
            );
            RuleEngine.GetInstance().Add(r);

            RuleEngine.GetInstance().Add(Rule.TryCreateRule(
                    // new Action(fishAnimal, "interacts with", artInt),
                    new Action(landVeh, "activates"),
                    new List<Action> { new Action(fishAnimal, "deactivates") }
                )
            );

            RuleEngine.GetInstance().Add(Rule.TryCreateRule(
                    new Action(fishAnimal, "interacts with", artInt),
                    // new Action(landVeh, "activates"),
                    new List<Action>
                    {
                        new Action(fishAnimal, "changes", "visible", "to", "yes"),
                        new Action(landVeh, "activates"),
                    }
                )
            );

            RuleEngine.GetInstance().Add(Rule.TryCreateRule(
                    new Action(fishAnimal, "interacts with", artInt),
                    // new Action(landVeh, "activates"),
                    new List<Action> { new Action(ecaLuce, "sets", "intensity", "to", 2f) }
                )
            );

            RuleEngine.GetInstance().Add(Rule.TryCreateRule(
                    new Action(landVeh, "activates"),
                    new SimpleCondition(fishAnimal, "visible", "is", ECABoolean.YES),
                    new List<Action> { new Action(ecaLuce, "sets", "intensity", "to", 2f) }
                )
            );
            Debug.LogError(
                "Ti ricordo che stai inizializzando delle regole qui per provare robe. Non ti dimenticare di cancellarle prima dei test!");
        }
        /////////////////////////////////////////


        if (showAllRules == null)
        {
            throw new ArgumentNullException("showAllRules", "showAllRules must be set");
        }

        if (showSelectedRule == null)
        {
            throw new ArgumentNullException("showSelectedRule", "showSelectedRule must be set");
        }

        rules = RuleEngine.GetInstance().Rules().ToList();

        //EnableShowAllRules();
    }

    private void Update()
    {
        if (showSelectedRule.gameObject.activeSelf)
        {
            Vector3 forward = playerCamera.forward;
            forward.y = 0;
            showSelectedRule.transform.position = playerCamera.position + forward.normalized * distanceFromCamera;
            showSelectedRule.transform.rotation = Quaternion.LookRotation(forward);
        }
    }

    private void EnableShowAllRules()
    {
        showAllRules.gameObject.SetActive(true);
        showSelectedRule.gameObject.SetActive(false);
    }

    private void EnableShowSelectedRule(Rule r)
    {
        if (r == null)
        {
            throw new ArgumentNullException("r", "r must be set");
        }

        showAllRules.gameObject.SetActive(false);
        selectedRule = r;
        showSelectedRule.gameObject.SetActive(true);
    }

    public void Intention_DeleteRuleFromUI(Rule rule)
    {
        // Delete the Rule, in the future we could add a double check via GUI. Like enable a prefab dialog to confirm the deletion, if the user clicks yes, then we delete the rule and update the UI
        RuleEngine.GetInstance().Remove(rule);

        // Update the UI
        OnRuleDeleted();
    }


    public void Intention_EditRuleFromUI(Rule ruleToRender)
    {
        // _selectedUIRuleRendering = ruleRendering; // useless
        EnableShowSelectedRule(ruleToRender);
    }
    public void Intention_EditRuleFromUI(ECAUI_RuleOverview ruleRendering) => this.Intention_EditRuleFromUI(ruleRendering.GetRule());

    public void Intention_SaveRuleEditedFromUI(Rule originalRule, ECAUI_Utils.RulePlaceholder newPlaceholderRule)
    {
        Rule newRule = newPlaceholderRule.ToEcaRule();
        Debug.Log("ALL WENT WELL SO FAR. Created the new rule from the UI:" + newRule.ToString());
        if (newRule != null)
        {
            RuleEngine.GetInstance().Remove(originalRule);
            RuleEngine.GetInstance().Add(newRule);
            OnRuleAdded();
        }
    }

    public void Intention_ShowAllRules()
    {
        EnableShowAllRules();
    }

    private void OnRuleDeleted()
    {
        // if we were looking at the rule that was deleted, we should go back to the show all rules view
        if (showSelectedRule.gameObject.activeSelf)
        {
            Debug.LogError("[NOT NORMAL BEHAVIOUR] Rule was deleted, going back to show all rules view");
            EnableShowAllRules();
        }
        // else if we were looking at all the rules, we should refresh the view
        else if (showAllRules.gameObject.activeSelf)
        {
            showAllRules.RefreshView();
        }
    }

    private void OnRuleAdded()
    {
        // if we are looking at all the rules, we should refresh the view
        if (showAllRules.gameObject.activeSelf)
        {
            showAllRules.RefreshView();
        }
        // else if we are looking at the selected rule, we don't need to do anything. When we go back to the show all rules view, we will see the new rule due to the script re-enabling
        else if (showSelectedRule.gameObject.activeSelf)
        {
            // do nothing
        }
    }
}