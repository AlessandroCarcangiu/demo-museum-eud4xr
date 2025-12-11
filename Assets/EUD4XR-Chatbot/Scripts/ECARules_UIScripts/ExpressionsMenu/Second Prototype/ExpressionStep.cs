using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class ExpressionStep : MonoBehaviour
    {
        public TMP_Text uiNumberRef;
        public Image uiHintImageRef;
        public TMP_Text uiHintTextRef;
        public GameObject uiLayout;
        public GameObject uiExpressionItemPrefab;

        private void Awake()
        {
            // Check for ref not null
            if (uiNumberRef == null) throw new Exception("uiNumberRef is null");
            if (uiHintImageRef == null) throw new Exception("uiHintImageRef is null");
            if (uiHintTextRef == null) throw new Exception("uiHintTextRef is null");
            if (uiLayout == null) throw new Exception("uiLayout is null");
        }
        
        public void OnPrefabCreated(int expressionIndex, int stepIndex)
        {
            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);

            // Set sequence number if step index is valid
            if (stepIndex >= 0)
                SetSequenceNumber(stepIndex);
            // If step index is negative expression is not apart of a sequence
            else
                uiNumberRef.gameObject.SetActive(false);

            // Create single expression item for a sequence step
            if (expression is Sequence)
                CreateExpressionItem(expressionIndex, stepIndex);
            // Create expression items for each step if expression is not a sequence
            else
                for (int i = 0; i < expression.Contents.Count; i++)
                    CreateExpressionItem(expressionIndex, i);

            // Set hint based on expression type
            SetHint(expression);
        }

        private void CreateExpressionItem(int expressionIndex, int stepIndex)
        {
            var expressionItem = Instantiate(uiExpressionItemPrefab, uiLayout.transform).GetComponent<ExpressionItem>();
            expressionItem.OnPrefabCreated(expressionIndex, stepIndex);
        }

        private void SetSequenceNumber(int stepIndex) => uiNumberRef.text = (stepIndex + 1).ToString();

        private void SetHint([NotNull] Expression expression)
        {
            if (expression is Order)
            {
                uiHintImageRef.color = Color.blue;
                uiHintTextRef.text = "Esegui tutte:";
            }
            else if (expression is Choice)
            {
                uiHintImageRef.color = Color.yellow;
                uiHintTextRef.text = "Esegui una:";
            }
            else if (expression is Sequence)
            {
                uiHintImageRef.color = Color.gray;
                uiHintTextRef.text = "Esegui:";
            }
        }
    }
}