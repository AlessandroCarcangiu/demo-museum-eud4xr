using System;
using ECARules4All_DLL;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Action = ECARules4All_DLL.Action;

public class WindowController : MonoBehaviour
{
    public Button TestSaveButton;
    public TMP_Text ActiveStateText;
    public TMP_InputField TextField; 
    
    public StateListView ListView;

    private void OnEnable()
    {
        DataManager.OnActiveStateChanged += ActiveStateChanged;
    }
    private void OnDisable()
    {
        DataManager.OnActiveStateChanged -= ActiveStateChanged;
    }

    private void Start()
    {
        if (TestSaveButton != null)
            TestSaveButton.onClick.AddListener(TestSave);
        if (TextField != null)
            TextField.onValueChanged.AddListener(OnFilterChanged);
    }

    private void ActiveStateChanged(int index)
    {
        ActiveStateText.text = index.ToString();
    }
    
    private void TestSave()
    {
        GameObject light = GameObject.Find("Light");
        var action = new Action(light, "increases", "intensity", "by", 10.0f);
        /*
        var ruleTrigger = new Action(light, "increases", "intensity", "by", 50.0f);
        var ruleActionList = new List<Action>();
        ruleActionList.Add(new Action(light,"changes", "color", "to", new ECAColor(Color.cyan)));
        Rule rule = Rule.TryCreateRule(ruleTrigger, ruleActionList);
        RuleEngine.GetInstance().Add(rule);
        */
        
        RuleEngine.GetInstance().ExecuteAction(action);
    }
    
    private void OnFilterChanged(string text)
    {
        ListView.SetSearchQuery(text);
    }
}