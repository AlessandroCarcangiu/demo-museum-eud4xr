using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.SmartHomeHubClients.Clients;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SecondPrototype
{
    public class UIExpressions : Singleton<UIExpressions>
    {
        public ExpressionsPanel expressionsPanel;
        public ExpressionManager expressionManager;
        public RectTransform menuContainer;
        public RectTransform plate;

        private float deltaX;
        private float duration;

        private List<Expression> expressions;
        private Dictionary<string, string> automations;

        private void Awake()
        {
            // Check for ref not null
            if (expressionsPanel == null) throw new Exception("expressionsPanel is null");
            if (expressionManager == null) throw new Exception("expressionManager is null");
            if (menuContainer == null) throw new Exception("menuContainer is null");

            // Set up variables for menu switching
            deltaX = plate.rect.width;
            duration = 0.2f;

            // Init data structures
            expressions = new List<Expression>();
            automations = new Dictionary<string, string>();
        }

        private async void Start()
        {
            // Read data from home assistant
            await UpdateExpressions();

            // Update expressions panel with retrieved data
            expressionsPanel.UpdateMenu();

            // Add listener to the go back button in the expression manager, expressions panel listeners are dynamically added in B_Expression_Prefab
            expressionManager.uiGoBack.onClick.AddListener(() => BackToMenu());

            // Expressions panel is shown at start
            expressionsPanel.gameObject.SetActive(true);
            expressionManager.gameObject.SetActive(false);
        }

        private async Task UpdateExpressions()
        {
            // Read data from home assistant
            AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();
            var (jArrayExpressions, dictAutomations) = await ((HomeAssistantClient)hassClient).GetExpressionsAndAutomations();

            // Convert JArray to parse it as a list
            var JObjectExpressions = new JObject { ["expressions"] = new JObject { ["default"] = jArrayExpressions } };

            // Save expressions and automations
            expressions = ExpressionUtils.ParseExpressions(JObjectExpressions);
            automations = dictAutomations;
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
            UpdatePlateSize(false);
        }

        public IEnumerator MoveContainer(RectTransform container, float deltaX, float duration)
        {
            Vector2 start = container.anchoredPosition;
            Vector2 target = start + new Vector2(deltaX, 0f);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                t = t * t * (3f - 2f * t);
                container.anchoredPosition = Vector2.Lerp(start, target, t);
                yield return null;
            }
            container.anchoredPosition = target;
        }

        public void SwitchMenu(bool goForward)
        {
            if (goForward)
            {
                // Container is moving to the left, show expression manager
                expressionManager.gameObject.SetActive(true);
                StartCoroutine(MoveContainer(menuContainer, -deltaX, duration));
                expressionsPanel.gameObject.SetActive(false);
            }
            else
            {
                // Container is moving to the right, show expressions panel
                expressionsPanel.gameObject.SetActive(true);
                StartCoroutine(MoveContainer(menuContainer, deltaX, duration));
                expressionManager.gameObject.SetActive(false);
            }
        }

        public void UpdatePlateSize(bool enlarge)
        {
            if (plate.rect.width == deltaX && enlarge)
            {
                // Make plate as wide as the expression manager
                plate.sizeDelta = new Vector2(expressionManager.gameObject.GetComponent<RectTransform>().rect.width, plate.sizeDelta.y);
                return;
            }
            if (plate.rect.width > deltaX && !enlarge)
            {
                // Go back to default size
                plate.sizeDelta = new Vector2(deltaX, plate.sizeDelta.y);
                return;
            }
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

        public string GetAutomationInfo(Automation automation)
        {
            var name = automation.Name["automation.".Length..];

            foreach (var (key, value) in automations)
            {
                if (key == name)
                    return value;
            }

            return "";
        }
    }
}
