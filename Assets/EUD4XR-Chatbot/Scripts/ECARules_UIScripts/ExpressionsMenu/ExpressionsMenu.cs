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
        List<Expression> expressions = new List<Expression>();
        /*
        // Read from local file
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "expressions.json");
        string json = System.IO.File.ReadAllText(path);
        JObject root = JObject.Parse(json);
        expressions = ExpressionUtils.ParseExpressions(root);
        */
        // Read from Home Assistant
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        var jArrayExpressions = await ((HomeAssistantClient)hassClient).GetListExpressions();
        //var (jArrayExpressions, jArrayAutomations) = await ((HomeAssistantClient)hassClient).GetExpressionsAndAutomations();
        expressions = JArrayToExpressions(jArrayExpressions);
        
        UpdateMenu(expressions);
    }

    private void UpdateMenu(List<Expression> expressions)
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
            // Clears panel
            if (noExpressionsInstance != null)
                Destroy(noExpressionsInstance);
            
            // Sets up panel
            uiExpressionsList.SetActive(true);
            if (selectedExpressionInstance == null)
                selectedExpressionInstance = Instantiate(uiSelectedExpressionPrefab, uiPlate.transform);
            else
                selectedExpressionInstance.GetComponent<SelectedExpression>().ShowUI(false);
            
            // Creates menu listing all expressions
            foreach (var expression in expressions)
            {
                // Instantiate the prefab and add it to the list
                var expressionItem = Instantiate(uiExpressionItemPrefab, uiExpressionsListContent.transform);
                // Set the expression to the prefab
                var expressionItemScript = expressionItem.GetComponent<B_Expression_Prefab>();
                expressionItemScript.OnPrefabCreated(expression);
            }
        }
    }

    private List<Expression> JArrayToExpressions(JArray jArray)
    {
        List<Expression> expressions = new List<Expression>();
        
        foreach (var obj in jArray.OfType<JObject>())
        {
            var name = (string)obj["name"] ?? "";

            if (obj["sequence"] is JArray seq)
                expressions.Add(new Sequence { Name = name, Contents = seq.Values<string>().Select(s => new Automation(s)).ToList() });

            else if (obj["order"] is JArray ord)
                expressions.Add(new Order { Name = name, Contents = ord.Values<string>().Select(s => new Automation(s)).ToList() });
            
            else if (obj["choice"] is JArray ch)
                expressions.Add(new Choice { Name = name, Contents = ch.Values<string>().Select(s => new Automation(s)).ToList() });
        }

        return expressions;
    }
}
