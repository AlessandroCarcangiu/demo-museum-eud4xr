using System;
using System.Collections.Generic;
using ECARules4All_DLL;
using UnityEngine;
using Action = ECARules4All_DLL.Action;

public class ECAUI_ShowAllRulesManager : MonoBehaviour
{
    public GameObject ruleOverviewPrefab;
    public GameObject objectContainingRules;
    private void Awake()
    {
       
        if (ruleOverviewPrefab == null)
        {
            throw new ArgumentNullException("ruleOverviewPrefab", "rulePrefab must be set");
        }
    }

    // void OnGUI()
    // {
    //     Debug.Log("HELLO");
    //     if (GUI.Button(new Rect(10, 10, 150, 100), "Trigger a Rule"))
    //     {
    //         // var a = new Action(GameObject.Find("Cube"), "interacts with", GameObject.Find("Sphere"));
    //         var a = new Action(GameObject.Find("Cube"), "activates");
    //         RuleEngine.GetInstance().ExecuteAction(a);
    //     }
    // }

    private void OnEnable()
    {
        RefreshView();
    }

    public void RefreshView()
    {
        RenderAllRules();
    }
    private void OnDisable()
    {
        // should we disable something?
    }
   
    private void RenderAllRules()
    {
        UnrenderAllRules();
        
        // For each rule, create a new GameObject with a Text component
        foreach (var rule in RuleEngine.GetInstance().Rules())
        {
            RenderRule(rule);
        }
    }
    
    private void UnrenderAllRules()
    {
        // Delete all the children of objectContainingRules
        foreach (Transform child in objectContainingRules.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void RenderRule(Rule rule)
    {
        // Create a new GameObject with a Text component
        var ruleRenderingObject = Instantiate(ruleOverviewPrefab, transform);
        // Set the text to the rule's name
        ruleRenderingObject.GetComponent<ECAUI_RuleOverview>().SetRule(rule);
        // Set the parent to this transform
        ruleRenderingObject.transform.SetParent(objectContainingRules.transform);
    }
    
    // private void UnrenderRule(Rule rule)
    // {
    //     // Find the GameObject with the rule's name
    //     // Destroy the GameObject
    // }
}
