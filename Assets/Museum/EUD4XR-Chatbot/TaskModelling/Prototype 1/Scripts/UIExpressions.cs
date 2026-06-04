using ECARules4All_DLL;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPrototype
{
    public class UIExpressions : Singleton<UIExpressions>
    {
        public ExpressionsPanel expressionsPanel;
        public ExpressionManager expressionManager;
        public RectTransform menuContainer;

        private RectTransform rectTransform;
        private float deltaX;
        private float duration;

        public Color automationColor;
        public Color orderColor;
        public Color choiceColor;

        private List<Expression> expressions;
        private List<RuleDTO> automations;

        private void Awake()
        {
            // Check for ref not null
            if (expressionsPanel == null) throw new Exception("expressionsPanel is null");
            if (expressionManager == null) throw new Exception("expressionManager is null");
            if (menuContainer == null) throw new Exception("menuContainer is null");
            if ((rectTransform = GetComponent<RectTransform>()) == null) throw new Exception("missing RectTransform component");
            if (automationColor == null) throw new Exception("automationColor is null");
            if (orderColor == null) throw new Exception("orderColor is null");
            if (choiceColor == null) throw new Exception("choiceColor is null");

            // Set up variables for menu switching
            deltaX = rectTransform.rect.width;
            duration = 0.2f;

            // Init data structures
            expressions = new List<Expression>();
            automations = new List<RuleDTO>();
        }

        private void Start()
        {
            // Add listener to the go back button in the expression manager, expressions panel listeners are dynamically added in B_Expression_Prefab
            expressionManager.uiGoBack.onClick.AddListener(() => BackToMenu());

            // Expressions panel is shown at start
            expressionsPanel.gameObject.SetActive(true);
            expressionManager.gameObject.SetActive(false);
        }

        public void HandleExpressions(JArray expressions, List<RuleDTO> automations)
        {
            // Convert JArray to parse it as a list
            var JObjectExpressions = new JObject { ["expressions"] = new JObject { ["default"] = expressions } };

            // Save expressions and automations
            this.expressions = ExpressionUtils.ParseExpressions(JObjectExpressions);
            this.automations = automations;

            // Go back to expressions panel if currently in expression manager
            if (expressionManager.gameObject.activeInHierarchy)
                BackToMenu();

            // Update expressions panel with retrieved data
            expressionsPanel.UpdateMenu();
        }

        public void LoadExpression(int index)
        {
            SwitchMenu(true);
            expressionManager.ShowExpression(index);
        }

        public void BackToMenu()
        {
            SwitchMenu(false);
            expressionManager.ClearExpressionData();
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
                // Container is moving to the left, show expression manager
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

        public bool IsSubExpression(int expressionIndex)
        {
            var expression = GetExpressionAtIndex(expressionIndex);

            // Sequences cannot be sub-expressions
            if (expression is Sequence)
                return false;
            else
            {
                for (int i = 0; i < expressions.Count; i++)
                {
                    // Avoid checking the expression itself
                    if (i != expressionIndex)
                    {
                        // Check if expression is contained in the contents of other expressions, if so it's a sub-expression
                        foreach (var content in expressions[i].Contents)
                        {
                            if (content.Name[(content.Name.IndexOf(".") + 1)..] == expression.Name)
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        public Color ExpressionToColor(Expression expression, int stepIndex)
        {
            if (expression is Order)
                return orderColor;

            if (expression is Choice)
                return choiceColor;

            if (expression is Sequence)
                return AutomationToColor(expression.Contents[stepIndex]);

            return Color.white;
        }

        public Color AutomationToColor(Automation automation)
        {
            var name = automation.Name;

            if (name.StartsWith("automation."))
                return automationColor;

            if (name.StartsWith("order."))
                return orderColor;

            if (name.StartsWith("choice."))
                return choiceColor;

            return Color.white;
        }

        public List<Expression> GetExpressions() => expressions;

        public Expression GetExpressionAtIndex(int index) => expressions[index];

        public int GetExpressionIndexByName(string name)
        {
            name = name[(name.IndexOf(".") + 1)..];

            for (int i = 0; i < expressions.Count; i++)
            {
                if (expressions[i].Name == name)
                    return i;
            }

            return -1;
        }

        public Expression GetExpressionByName(string name)
        {
            var index = GetExpressionIndexByName(name);
            return GetExpressionAtIndex(index);
        }

        public Expression GetCurrentExpression() => GetExpressionAtIndex(expressionManager.GetCurrentExpressionIndex());

        public object GetAutomationInfo(Automation automation)
        {
            var name = automation.Name["automation.".Length..];

            foreach (var rule in automations)
            {
                if (rule.alias == name)
                    return RuleSanitizer.SanitizeRuleString(rule.rule);
            }

            return "";
        }
    }
}
