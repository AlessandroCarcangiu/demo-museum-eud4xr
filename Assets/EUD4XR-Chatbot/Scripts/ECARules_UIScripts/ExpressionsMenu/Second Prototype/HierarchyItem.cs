using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class HierarchyItem : MonoBehaviour
    {
        public Image arrowRef;
        public GameObject iconRef;
        public Image iconBackgroundRef;
        public Image iconLogoRef;
        public TMP_Text textRef;
        public Button buttonRef;
        public Sprite choiceIcon;
        public Sprite orderIcon;

        private void Awake()
        {
            // Check for ref not null
            if (arrowRef == null) throw new Exception("arrowRef is null");
            if (iconRef == null) throw new Exception("iconRef is null");
            if (iconBackgroundRef == null) throw new Exception("iconBackgroundRef is null");
            if (iconLogoRef == null) throw new Exception("iconLogoRef is null");
            if (textRef == null) throw new Exception("textRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
            if (choiceIcon == null) throw new Exception("choiceIcon is null");
            if (orderIcon == null) throw new Exception("orderIcon is null");
        }

        public void OnPrefabCreated(int expressionIndex, int level)
        {
            var uiExprRef = UIExpressions.Instance;

            // Set icon and name
            var expression = uiExprRef.GetExpressionAtIndex(expressionIndex);
            SetExpressionIcon(expression);
            SetExpressionName(expression);

            // Show or hide arrow based on if it is a sub-expression or not
            var flag = level == 0 ? false : true;
            arrowRef.gameObject.SetActive(flag);

            // Add listener to button so it updates the expression manager
            buttonRef.onClick.AddListener(() =>
            {
                // Close any open automation info
                uiExprRef.expressionManager.CloseAutomationInfo();
                // Update hierarchy and expression viewer
                uiExprRef.expressionManager.UpdateHierarchy(level);
            });
        }

        private void SetExpressionIcon([NotNull] Expression expression)
        {
            // Hide icon if expression is a sequence
            if (expression is Sequence)
                iconRef.SetActive(false);
            // Set icon and background color based on expression type
            else
            {
                iconRef.SetActive(true);
                iconLogoRef.sprite = expression is Order ? orderIcon : choiceIcon;
                iconBackgroundRef.color = expression is Order ? Color.blue : Color.yellow;
            }
        }

        private void SetExpressionName([NotNull] Expression expression) => textRef.text = expression.Name;
    }
}