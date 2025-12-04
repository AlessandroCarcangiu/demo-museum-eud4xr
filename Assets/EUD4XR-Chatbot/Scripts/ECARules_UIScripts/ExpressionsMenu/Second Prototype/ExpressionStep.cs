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
        public Image uiHintImageRef;
        public TMP_Text uiHintTextRef;
        public GameObject uiLayout;
        public GameObject uiExpressionItemPrefab;

        private void Awake()
        {
            if (uiHintImageRef == null) throw new Exception("uiHintImageRef is null");
            if (uiHintTextRef == null) throw new Exception("uiHintTextRef is null");
        }

        // OnPrefabCreated for sequences
        public void OnPrefabCreated(Expression expression, int stepIndex)
        {
            if (expression is not Sequence) throw new Exception("expression is not a sequence, use other OnPrefabCreated()");

            SetHint(expression);

            CreateExpressionItem(expression, stepIndex);
        }

        // OnPrefabCreated for other expressions
        public void OnPrefabCreated(Expression expression)
        {
            if (expression is Sequence) throw new Exception("expression is a sequence, use other OnPrefabCreated()");

            SetHint(expression);

            for (int i = 0; i < expression.Contents.Count; i++)
                CreateExpressionItem(expression, i);
        }

        private void CreateExpressionItem(Expression expression, int stepIndex)
        {
            var expressionItem = Instantiate(uiExpressionItemPrefab, uiLayout.transform).GetComponent<ExpressionItem>();
            expressionItem.OnPrefabCreated(expression, stepIndex);
        }

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