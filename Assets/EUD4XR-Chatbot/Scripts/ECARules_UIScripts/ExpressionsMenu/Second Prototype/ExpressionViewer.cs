using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class ExpressionViewer : MonoBehaviour
    {
        public TMP_Text uiExpressionName;
        public GameObject uiExpressionStepsContent;
        public GameObject uiExpressionStepHorizontalPrefab;
        public GameObject uiExpressionStepVerticalPrefab;

        private List<ExpressionStep> expressionSteps;
        private int expressionIndex;
        
        private void Awake()
        {
            if (uiExpressionName == null) throw new Exception("uiExpressionName is null");
            if (uiExpressionStepsContent == null) throw new Exception("uiExpressionStepsContent is null");

            // Init list
            expressionSteps = new List<ExpressionStep>();
        }

        public void OnPrefabCreated(int expressionIndex)
        {
            this.expressionIndex = expressionIndex;
            
            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);

            // Set expression name in the nav bar
            SetExpressionName(expression);

            if (expression is Sequence)
                for (int i = 0; i < expression.Contents.Count; i++)
                    CreateExpressionStep(expression, i);
            else
                CreateExpressionStep(expression);
        }

        // CreateExpressionStep for sequences
        private void CreateExpressionStep(Expression expression, int index)
        {
            var contentName = expression.Contents[index].Name;

            if (contentName.StartsWith("automation."))
            {
                // Instantiate the prefab
                var expressionStep = Instantiate(uiExpressionStepHorizontalPrefab, uiExpressionStepsContent.transform).GetComponent<ExpressionStep>();
                expressionStep.OnPrefabCreated(expression, index);

                // Add expression step reference to the list
                expressionSteps.Add(expressionStep);
            }
            else
                CreateExpressionStep(UIExpressions.Instance.GetExpressionByName(contentName));
        }

        // CreateExpressionStep for other expressions
        private void CreateExpressionStep(Expression expression)
        {
            // Instantiate the prefab
            GameObject expressionStepRef;
            if (expression is Choice)
                expressionStepRef = Instantiate(uiExpressionStepHorizontalPrefab, uiExpressionStepsContent.transform);
            else
                expressionStepRef = Instantiate(uiExpressionStepVerticalPrefab, uiExpressionStepsContent.transform);

            expressionStepRef.GetComponent<ExpressionStep>().OnPrefabCreated(expression);
            
            // Add expression step reference to the list
            expressionSteps.Add(expressionStepRef.GetComponent<ExpressionStep>());
        }

        private void SetExpressionName([NotNull] Expression expression) => uiExpressionName.text = expression.Name;

        public int GetExpressionIndex() => expressionIndex;
    }
}