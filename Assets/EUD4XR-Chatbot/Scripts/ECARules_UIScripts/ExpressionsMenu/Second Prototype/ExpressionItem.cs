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

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
            if (iconRef == null) throw new Exception("iconRef is null");
            if (choiceIcon == null) throw new Exception("choiceIcon is null");
            if (orderIcon == null) throw new Exception("orderIcon is null");
        }

        public void OnPrefabCreated(Expression expression, int index)
        {
            var item = expression.Contents[index];
            
            if (item.Name.StartsWith("automation."))
            {
                // Show automation name
                SetItemName(item);
                iconRef.gameObject.SetActive(false);
                // TODO: add popup with automation info
                //var uiExprRef = UIExpressions.Instance;
                //buttonRef.onClick.AddListener(() => 
            }
            else
            {
                var uiExprRef = UIExpressions.Instance;
                // Show operator icon
                SetLogo(uiExprRef.GetExpressionByName(item.Name));
                contentRef.gameObject.SetActive(false);
                // Add listener to button so it opens the expression when pressed
                buttonRef.onClick.AddListener(() => uiExprRef.expressionManager.OpenExpression(uiExprRef.GetExpressionIndexByName(item.Name)));
            }
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