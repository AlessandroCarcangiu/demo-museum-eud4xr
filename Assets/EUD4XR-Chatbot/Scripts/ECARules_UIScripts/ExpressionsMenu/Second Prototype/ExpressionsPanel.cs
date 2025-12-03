using System;
using UnityEngine;

namespace SecondPrototype
{
    public class ExpressionsPanel : MonoBehaviour
    {
        public GameObject uiExpressionItemPrefab;
        public GameObject uiExpressionsList;
        public GameObject uiExpressionsListContent;
        public GameObject uiNoExpressions;

        private void Awake()
        {
            // Check for ref not null
            if (uiExpressionItemPrefab == null) throw new Exception("uiExpressionItemPrefab is null");
            if (uiExpressionsList == null) throw new Exception("uiExpressionsList is null");
            if (uiExpressionsListContent == null) throw new Exception("uiExpressionsListContent is null");
            if (uiNoExpressions == null) throw new Exception("uiNoExpressions is null");
        }

        public void UpdateMenu()
        {
            var expressions = UIExpressions.Instance.GetExpressions();

            // No expressions found
            if (expressions.Count == 0)
            {
                // Clear panel
                uiExpressionsList.SetActive(false);
                // Show 'no expressions' message
                uiNoExpressions.SetActive(true);
            }
            // Expressions found
            else
            {
                // Create menu listing all expressions
                for (int i = 0; i < expressions.Count; i++)
                {
                    // Instantiate the button prefab and add it to the list
                    var expressionItem = Instantiate(uiExpressionItemPrefab, uiExpressionsListContent.transform).GetComponent<B_Expression_Prefab>();
                    expressionItem.OnPrefabCreated(i);
                }

                // Clear panel
                uiNoExpressions.SetActive(false);
                // Show menu
                uiExpressionsList.SetActive(true);
            }
        }
    }
}
