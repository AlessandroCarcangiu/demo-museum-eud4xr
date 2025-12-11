using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class AutomationInfo : MonoBehaviour
    {
        public TMP_Text contentRef;

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
        }
        
        public void OnPrefabCreated(string info, Button expressionItemButton)
        {
            var uiExprRef = UIExpressions.Instance;

            // Set the info text
            SetAutomationInfo(info); // da formattare meglio
            // Open the automation info in the expression manager
            uiExprRef.expressionManager.OpenAutomationInfo(this);
            // Set the expression item button reference in the expression manager
            uiExprRef.expressionManager.SetOpenAutomation(expressionItemButton);
        }

        private void SetAutomationInfo(string info) => contentRef.text = info;
    }
}