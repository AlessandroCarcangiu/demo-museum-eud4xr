using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
/*
public class HassData
{
    private List<Expression> expressions;
    private Dictionary<string, string> automations;

    public List<Expression> Expressions { get => expressions; set => expressions = value; }
    public Dictionary<string, string> Automations { get => automations; set => automations = value; }
}
*/
public class UIExpressions : Singleton<UIExpressions>
{
    public ExpressionsPanel expressionsPanel;
    public ExpressionManager expressionManager;
    public RectTransform menuContainer;

    private List<Expression> expressions;
    private Dictionary<string, string> automations;

    private float deltaX = 210f;
    private float duration = 0.2f;

    private void Awake()
    {
        // Check for ref not null
        if (expressionsPanel == null) throw new Exception("expressionsPanel is null");
        if (expressionManager == null) throw new Exception("expressionManager is null");
        if (menuContainer == null) throw new Exception("menuContainer is null");

        // Init data structures
        expressions = new List<Expression>();
        automations = new Dictionary<string, string>();
    }

    private async void Start()
    {
        // Reads data from home assistant
        await UpdateExpressions();

        // Updates expressions panel with retrieved data
        expressionsPanel.UpdateMenu();
        
        // Adds listener to the go back button in the expression manager, expressions panel listeners are dynamically added in B_Expression_Prefab
        expressionManager.uiGoBack.onClick.AddListener(() => SwitchMenu(false));

        // Expressions panel is shown at start
        expressionsPanel.gameObject.SetActive(true);
        expressionManager.gameObject.SetActive(false);
    }

    private async Task UpdateExpressions()
    {
        // Reads data from home assistant
        AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
        var (jArrayExpressions, dictAutomations) = await ((HomeAssistantClient)hassClient).GetExpressionsAndAutomations();
        
        // Converts JArray to parse it as a list and saves it in the menu data
        var JObjectExpressions = new JObject { ["expressions"] = new JObject { ["default"] = jArrayExpressions } };
        expressions = ExpressionUtils.ParseExpressions(JObjectExpressions);

        // Saves automations dictionary in the menu data
        automations = dictAutomations;
    }

    public void LoadExpression(int index)
    {
        SwitchMenu(true);
        expressionManager.ShowExpression(index);
    }

    private IEnumerator MoveMenu(float deltaX, float duration)
    {
        Vector2 start = menuContainer.anchoredPosition;
        Vector2 target = start + new Vector2(deltaX, 0f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            menuContainer.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }
        menuContainer.anchoredPosition = target;
    }

    public void SwitchMenu(bool goForward)
    {
        if (goForward)
        {
            // Container is moving to the left, show selected expression panel
            expressionManager.gameObject.SetActive(true);
            StartCoroutine(MoveMenu(-deltaX, duration));
            expressionsPanel.gameObject.SetActive(false);
        }
        else
        {
            // Container is moving to the right, show expressions panel
            expressionsPanel.gameObject.SetActive(true);
            StartCoroutine(MoveMenu(deltaX, duration));
            expressionManager.gameObject.SetActive(false);
        }
    }

    public Color ExpressionToColor(Expression expression, int stepIndex)
    {
        if (expression is Order)
            return Color.blue;

        if (expression is Choice)
            return Color.yellow;

        if (expression is Sequence)
            return AutomationToColor(expression.Contents[stepIndex]);

        return Color.white;
    }

    public Color AutomationToColor(Automation automation)
    {
        var name = automation.Name;

        if (name.StartsWith("automation."))
            return Color.gray;

        if (name.StartsWith("order."))
            return Color.blue;

        if (name.StartsWith("choice."))
            return Color.yellow;

        return Color.white;
    }

    public List<Expression> GetExpressions() => expressions;

    public Expression GetExpressionAtIndex(int index) => expressions[index];

    public int GetExpressionIndexByName(string name)
    {
        name = name.Substring(name.IndexOf(".") + 1);

        for (int i = 0; i < expressions.Count; i++)
        {
            if (expressions[i].Name == name)
                return i;
        }

        return -1;
    }

    public Dictionary<string, string> GetAutomations() => automations;

    public Dictionary<string, string> GetExpressionAutomations(int index)
    {
        var expression = expressions[index];

        var expressionAutomations = new Dictionary<string, string>();

        foreach (var content in expression.Contents)
        {
            if (content.Name.StartsWith("automation."))
            {
                string name = content.Name.Substring("automation.".Length);

                foreach (var (key, value) in automations)
                {
                    if (key == name)
                    {
                        expressionAutomations.Add(key, value);
                        break;
                    }
                }
            }
        }

        return expressionAutomations;
    }
}
