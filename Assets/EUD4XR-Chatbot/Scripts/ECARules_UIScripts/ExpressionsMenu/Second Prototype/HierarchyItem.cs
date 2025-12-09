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
        public Image iconRef;
        public TMP_Text textRef;
        public Button buttonRef;
        public Sprite choiceIcon;
        public Sprite orderIcon;

        private void Awake()
        {
            // Check for ref not null
            if (arrowRef == null) throw new Exception("arrowRef is null");
            if (iconRef == null) throw new Exception("iconRef is null");
            if (textRef == null) throw new Exception("textRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
        }

        public void OnPrefabCreated(int expressionIndex, int level)
        {
            if (level == 0)
                arrowRef.gameObject.SetActive(false);
            else
                arrowRef.gameObject.SetActive(true);

            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);
            SetExpressionIcon(expression);
            SetExpressionName(expression);

            // Add listener to button so it shows the corresponding expression viewer when pressed
            buttonRef.onClick.AddListener(() => UIExpressions.Instance.expressionManager.UpdateHierarchy(level));
        }

        private void SetExpressionIcon([NotNull] Expression expression)
        {
            if (expression is Sequence)
                iconRef.enabled = false;
            else
            { 
                iconRef.enabled = true;
                iconRef.sprite = expression is Choice ? choiceIcon : orderIcon;
            }
        }

        private void SetExpressionName([NotNull] Expression expression) => textRef.text = expression.Name;
    }
}