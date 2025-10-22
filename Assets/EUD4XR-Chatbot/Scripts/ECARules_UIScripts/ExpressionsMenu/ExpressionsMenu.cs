using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECARules4All_DLL.Utils;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ExpressionsMenu : Singleton<ExpressionsMenu>
{
    public GameObject uiNoExpressionsPrefab;
    public GameObject uiExpressionItemPrefab;
    public GameObject uiSelectedExpressionPrefab;
    public GameObject uiPlate;
    public GameObject uiExpressionsPanel;
    public GameObject uiExpressionsList;
    public GameObject uiExpressionsListContent;

    private GameObject noExpressionsInstance;
    private GameObject selectedExpressionInstance;

    private void Awake()
    {
        // Check for ref not null
        if (uiNoExpressionsPrefab == null) throw new Exception("uiNoExpressionsPrefab is null");
        if (uiExpressionItemPrefab == null) throw new Exception("uiExpressionItemPrefab is null");
        if (uiSelectedExpressionPrefab == null) throw new Exception("uiSelectedExpressionPrefab is null");
        if (uiPlate == null) throw new Exception("uiPlate is null");
        if (uiExpressionsPanel == null) throw new Exception("uiExpressionsPanel is null");
        if (uiExpressionsList == null) throw new Exception("uiExpressionsList is null");
        if (uiExpressionsListContent == null) throw new Exception("uiExpressionsListContent is null");
    }

    private async void Start()
    {
        await UpdateExpressions();
    }

    private async Task UpdateExpressions()
    {
        // Read from Home Assistant
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        var (jArrayExpressions, automations) = await ((HomeAssistantClient)hassClient).GetExpressionsAndAutomations();
        
        // converte il JArray restituito da GetListExpressions() per renderlo parsabile da ParseExpressions()
        var JObjectExpressions = new JObject { ["expressions"] = new JObject { ["default"] = jArrayExpressions } };
        List<Expression> expressions = ExpressionUtils.ParseExpressions(JObjectExpressions);
        
        UpdateMenu(expressions, automations);
    }

    private void UpdateMenu(List<Expression> expressions, Dictionary<string, string> automations)
    {
        // No expressions found
        if (expressions.Count == 0)
        {
            // Clears panel
            uiExpressionsList.SetActive(false);            
            if (selectedExpressionInstance != null)
                Destroy(selectedExpressionInstance);
            
            // Shows 'no expressions' message
            if (noExpressionsInstance == null)
                noExpressionsInstance = Instantiate(uiNoExpressionsPrefab, uiExpressionsPanel.transform);
        }
        // At least one expression is found
        else
        {
            // Creates menu listing all expressions
            foreach (var expression in expressions)
            {
                // Gets info for automations involved in the expression
                var expressionAutomations = GetExpressionAutomations(expression, automations);
                // Instantiate the prefab and add it to the list
                var expressionItem = Instantiate(uiExpressionItemPrefab, uiExpressionsListContent.transform);
                // Set the expression to the prefab
                var expressionItemScript = expressionItem.GetComponent<B_Expression_Prefab>();
                expressionItemScript.OnPrefabCreated(expression, expressionAutomations);
            }

            // Clears panel
            if (noExpressionsInstance != null)
                Destroy(noExpressionsInstance);
            
            // Sets up panel
            uiExpressionsList.SetActive(true);
            if (selectedExpressionInstance == null)
                selectedExpressionInstance = Instantiate(uiSelectedExpressionPrefab, uiPlate.transform);
            else
                selectedExpressionInstance.GetComponent<SelectedExpression>().ShowUI(false);
        }
    }

    private Dictionary<string, string> GetExpressionAutomations(Expression expression, Dictionary<string, string> automations)
    {
        var expressionAutomations = new Dictionary<string, string>();
        
        foreach (var (key, value) in automations)
        {
            foreach (var automation in expression.Contents)
            {
                string name = automation.Name.Substring("automation.".Length);
                if (key == name)
                {
                    expressionAutomations.Add(key, value);
                    break;
                }
            }
        }

        if (expression.Contents.Count != expressionAutomations.Count)
            throw new Exception("Automations count does not match");

        return expressionAutomations;
    }
}
