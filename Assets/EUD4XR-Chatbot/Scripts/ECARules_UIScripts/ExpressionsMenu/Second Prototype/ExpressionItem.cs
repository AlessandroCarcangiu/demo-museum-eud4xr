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
        public GameObject automationInfo;
        public TMP_Text automationInfoText;

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
            if (automationInfo == null) throw new Exception("automationInfo is null");
            if (automationInfoText == null) throw new Exception("automationInfoText is null");
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
                buttonRef.onClick.AddListener(() => ToggleAutomationInfo(item));
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

        private void ToggleAutomationInfo(Automation automation)
        {
            automationInfo.SetActive(!automationInfo.activeInHierarchy);
            // da formattare meglio e fixare aggiornamento altezza transform
            //automationInfoText.text = UIExpressions.Instance.GetAutomationInfo(automation);
        }

        private void HandleExpressionOpening(int currentExpressionIndex, int newExpressionIndex)
        {
            var uiExprRef = UIExpressions.Instance;
            
            // Perform checks if multiple expressions are already open
            if (uiExprRef.expressionManager.GetExpressionIndexesCount() > 1)
            {
                // Check if new expression is already open
                if (uiExprRef.expressionManager.CheckDuplicates(newExpressionIndex))
                    return;

                // Check if current expression already opened sub-expressions and close them
                uiExprRef.expressionManager.CheckSubExpressions(currentExpressionIndex);
            }

            // Open new expression
            uiExprRef.expressionManager.OpenExpression(newExpressionIndex);
        }

        private void SetItemName([NotNull] Automation automation) => contentRef.text = automation.Name[(automation.Name.IndexOf(".") + 1)..];

        private void SetBackground(Expression expression) => buttonRef.image.color = expression is Order ? Color.blue : Color.yellow;
    }
}