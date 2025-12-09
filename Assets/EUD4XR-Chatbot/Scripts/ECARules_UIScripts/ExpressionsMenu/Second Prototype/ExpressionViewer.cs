using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

namespace SecondPrototype
{
    public class ExpressionViewer : MonoBehaviour
    {
        public TMP_Text uiExpressionName;
        public GameObject uiExpressionStepsContent;
        public GameObject uiExpressionStepHorizontalPrefab;
        public GameObject uiExpressionStepVerticalPrefab;

        private List<ExpressionStep> expressionSteps;
        
        private void Awake()
        {
            if (uiExpressionName == null) throw new Exception("uiExpressionName is null");
            if (uiExpressionStepsContent == null) throw new Exception("uiExpressionStepsContent is null");

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
                    CreateSequenceStep(expressionIndex, i);
            else
                // Pass -1 as step index to indicate that it's not part of a sequence
                CreateExpressionStep(expressionIndex, -1);
        }

        // Create expression step for sequences
        private void CreateSequenceStep(int expressionIndex, int stepIndex)
        {
            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);
            var contentName = expression.Contents[stepIndex].Name;

            if (contentName.StartsWith("automation."))
            {
                // Instantiate the prefab
                var expressionStep = Instantiate(uiExpressionStepHorizontalPrefab, uiExpressionStepsContent.transform).GetComponent<ExpressionStep>();
                expressionStep.OnPrefabCreated(expressionIndex, stepIndex);

                // Add expression step reference to the list
                expressionSteps.Add(expressionStep);
            }
            else
                CreateExpressionStep(UIExpressions.Instance.GetExpressionIndexByName(contentName), stepIndex);
        }

        // Create expression step for other expressions
        private void CreateExpressionStep(int expressionIndex, int stepIndex)
        {
            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);

            // Instantiate the prefab
            var expressionStep = expression is Choice ? 
                Instantiate(uiExpressionStepHorizontalPrefab, uiExpressionStepsContent.transform).GetComponent<ExpressionStep>():
                Instantiate(uiExpressionStepVerticalPrefab, uiExpressionStepsContent.transform).GetComponent<ExpressionStep>();
            expressionStep.OnPrefabCreated(expressionIndex, stepIndex);
            
            // Add expression step reference to the list
            expressionSteps.Add(expressionStep);
        }

        private void SetExpressionName([NotNull] Expression expression) => uiExpressionName.text = expression.Name;
    }
}