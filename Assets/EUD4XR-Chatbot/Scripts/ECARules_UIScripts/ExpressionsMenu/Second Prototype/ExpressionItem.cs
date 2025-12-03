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

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
        }

        public void OnPrefabCreated(Expression expression, int index)
        {
            var item = expression.Contents[index];

            SetItemName(item);
            
            if (item.Name.StartsWith("automation."))
            {
                // TODO: add popup with automation info
                //var uiExprRef = UIExpressions.Instance;
                //buttonRef.onClick.AddListener(() => 
            }
            else
            {
                // Add listener to button so it loads the expression when pressed
                var uiExprRef = UIExpressions.Instance; 
                buttonRef.onClick.AddListener(() => uiExprRef.expressionManager.CreateExpressionDescriptor(uiExprRef.GetExpressionIndexByName(item.Name)));
            }
        }

        private void SetItemName([NotNull] Automation automation) => contentRef.text = automation.Name[(automation.Name.IndexOf(".") + 1)..];
    }
}