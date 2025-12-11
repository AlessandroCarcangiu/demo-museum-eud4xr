using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class ExpressionItem : MonoBehaviour
    {
        public TMP_Text contentRef;
        public Button buttonRef;
        public GameObject automationInfoPrefab;

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
            if (automationInfoPrefab == null) throw new Exception("automationInfoPrefab is null");
        }

        public void OnPrefabCreated(int expressionIndex, int stepIndex)
        {
            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);
            var item = expression.Contents[stepIndex];

            // Show item name
            SetItemName(item);

            if (item.Name.StartsWith("automation."))
            {
                // Open pop-up with automation info
                buttonRef.onClick.AddListener(() => HandleAutomationInfo(item));
            }
            else
            {
                // Show operator icon
                var uiExprRef = UIExpressions.Instance;
                SetBackground(UIExpressions.Instance.GetExpressionByName(item.Name));
                // Add listener to button so it opens the expression when pressed
                buttonRef.onClick.AddListener(() => HandleExpressionOpening(expressionIndex, uiExprRef.GetExpressionIndexByName(item.Name)));
            }
        }

        private void HandleAutomationInfo(Automation automation)
        {
            var uiExprRef = UIExpressions.Instance;
            var openAutomation = uiExprRef.expressionManager.GetOpenAutomation();

            // Open pop-up if it's not already open
            if (buttonRef != openAutomation)
            {
                // Instantiate the prefab
                var popUp = Instantiate(automationInfoPrefab, transform).GetComponent<AutomationInfo>();
                popUp.OnPrefabCreated(uiExprRef.GetAutomationInfo(automation), buttonRef);
            }
            // Close pop-up if it's already open
            else
                uiExprRef.expressionManager.CloseAutomationInfo();
        }

        private void HandleExpressionOpening(int currentExpressionIndex, int newExpressionIndex)
        {
            var uiExprRef = UIExpressions.Instance;

            uiExprRef.expressionManager.CloseAutomationInfo();

            // Perform checks if multiple expressions are already open
            if (uiExprRef.expressionManager.GetExpressionIndexesCount() > 1)
            {
                // Get expression level to check if new expression is already open
                var expressionLevel = uiExprRef.expressionManager.GetExpressionLevel(newExpressionIndex);

                if (expressionLevel >= 0)
                {
                    // If new expression is already open, update hierarchy to that level
                    uiExprRef.expressionManager.UpdateHierarchy(expressionLevel);
                    return;
                }

                // Check if current expression already opened sub-expressions and close them
                uiExprRef.expressionManager.CheckSubExpressions(currentExpressionIndex);
            }

            // Open new expression
            uiExprRef.expressionManager.OpenExpression(newExpressionIndex);
        }

        private void SetItemName([NotNull] Automation automation) => contentRef.text = automation.Name[(automation.Name.IndexOf(".") + 1)..];

        private void SetBackground([NotNull] Expression expression) => buttonRef.image.color = expression is Order ? Color.blue : Color.yellow;
    }
}