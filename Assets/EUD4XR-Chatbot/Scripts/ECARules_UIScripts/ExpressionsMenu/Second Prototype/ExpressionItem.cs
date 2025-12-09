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
        public Image iconRef;
        public Button buttonRef;
        public Sprite choiceIcon;
        public Sprite orderIcon;
        public GameObject automationInfo;
        public TMP_Text automationInfoText;

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
            if (iconRef == null) throw new Exception("iconRef is null");
            if (choiceIcon == null) throw new Exception("choiceIcon is null");
            if (orderIcon == null) throw new Exception("orderIcon is null");
            if (automationInfo == null) throw new Exception("automationInfo is null");
            if (automationInfoText == null) throw new Exception("automationInfoText is null");
        }

        public void OnPrefabCreated(int expressionIndex, int stepIndex)
        {
            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);
            var item = expression.Contents[stepIndex];
            
            if (item.Name.StartsWith("automation."))
            {
                // Show automation name
                SetItemName(item);
                iconRef.gameObject.SetActive(false);
                // Open pop-up with automation info
                buttonRef.onClick.AddListener(() => ToggleAutomationInfo(item));
            }
            else
            {
                // Show operator icon
                var uiExprRef = UIExpressions.Instance;
                SetLogo(UIExpressions.Instance.GetExpressionByName(item.Name));
                contentRef.gameObject.SetActive(false);
                iconRef.gameObject.SetActive(true);
                // Add listener to button so it opens the expression when pressed
                buttonRef.onClick.AddListener(() => HandleExpressionOpening(expressionIndex, uiExprRef.GetExpressionIndexByName(item.Name)));
            }
        }

        private void ToggleAutomationInfo(Automation automation)
        {
            automationInfo.SetActive(!automationInfo.activeInHierarchy);
            // da fixare aggiornamento altezza transform
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

        private void SetLogo(Expression expression)
        {
            if (expression is Order)
            {
                iconRef.sprite = orderIcon;
                buttonRef.image.color = Color.blue;
            }
            else
            {
                iconRef.sprite = choiceIcon;
                buttonRef.image.color = Color.yellow;
            }
        }
    }
}