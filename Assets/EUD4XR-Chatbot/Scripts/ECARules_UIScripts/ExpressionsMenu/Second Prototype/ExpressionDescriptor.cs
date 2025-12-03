using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

namespace SecondPrototype
{
    public class ExpressionDescriptor : MonoBehaviour
    {
        public TMP_Text uiExpressionName;
        public GameObject uiExpressionSteps;
        public GameObject uiExpressionStepPrefab;

        private List<ExpressionStep> expressionSteps;
        
        private void Awake()
        {
            if (uiExpressionName == null) throw new Exception("uiExpressionName is null");
            if (uiExpressionSteps == null) throw new Exception("uiExpressionSteps is null");

            // Init list
            expressionSteps = new List<ExpressionStep>();
        }

        public void OnPrefabCreated(int expressionIndex)
        {
            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);

            // Set expression name in the nav bar
            SetExpressionName(expression);

            if (expression is Sequence)
                for (int i = 0; i < expression.Contents.Count; i++)
                    CreateExpressionStep(expression, i);
            else
                CreateExpressionStep(expression);
        }

        private void CreateExpressionStep(Expression expression, int index)
        {
            var content = expression.Contents[index];

            // Instantiate the prefab
            var expressionStep = Instantiate(uiExpressionStepPrefab, uiExpressionSteps.transform).GetComponent<ExpressionStep>();

            if (content.Name.StartsWith("automation."))
                expressionStep.OnPrefabCreated(expression, index);
            else
                expressionStep.OnPrefabCreated(UIExpressions.Instance.GetExpressionByName(content.Name));

            // Add expression step reference to the list
            expressionSteps.Add(expressionStep);
        }

        private void CreateExpressionStep(Expression expression)
        {
            // Instantiate the prefab
            var expressionStep = Instantiate(uiExpressionStepPrefab, uiExpressionSteps.transform).GetComponent<ExpressionStep>();
            expressionStep.OnPrefabCreated(expression);
            // Add expression step reference to the list
            expressionSteps.Add(expressionStep);
        }

        private void SetExpressionName([NotNull] Expression expression) => uiExpressionName.text = expression.Name;
    }
}