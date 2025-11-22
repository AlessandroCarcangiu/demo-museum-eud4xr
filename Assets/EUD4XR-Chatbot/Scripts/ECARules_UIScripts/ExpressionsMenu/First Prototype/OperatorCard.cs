using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCard : MonoBehaviour
{
    public UIExpressions uiExpressions;
    public GameObject uiAutomationNamePrefab;
    public GameObject uiAutomations;
    public TMP_Text uiOperatorName;
    public Button buttonRef;
    public Image imageRef;

    private bool isTemporary;
    private List<OperatorCardAutomation> operatorCardAutomations;

    private void Awake()
    {
        // Check for ref not null
        if (uiAutomationNamePrefab == null) throw new Exception("uiAutomationNamePrefab is null");
        if (uiAutomations == null) throw new Exception("uiAutomations is null");
        if (uiOperatorName == null) throw new Exception("uiOperatorName is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (imageRef == null) throw new Exception("imageRef is null");

        // Init list
        operatorCardAutomations = new List<OperatorCardAutomation>();
    }

    public void OnPrefabCreated(int expressionIndex, int stepIndex, bool temporary, UIExpressions uiExprRef)
    {
        isTemporary = temporary;

        uiExpressions = uiExprRef;

        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);

        var stepsManager = uiExpressions.expressionManager.GetStepsManagerInstance();

        var positionIndex = transform.GetSiblingIndex();

        // If expression is a sequence each operator card contains one automation
        if (expression is Sequence && expression.Contents[stepIndex].Name.StartsWith("automation."))
        {
            // Instantiate the prefab
            var operatorCardAutomation = Instantiate(uiAutomationNamePrefab, uiAutomations.transform).GetComponent<OperatorCardAutomation>();
            operatorCardAutomation.OnPrefabCreated(expressionIndex, stepIndex, positionIndex, uiExpressions);
            operatorCardAutomations.Add(operatorCardAutomation);
        }
        // If expression is not a sequence the operator card contains all automations
        else
        {
            for (int i = 0; i < expression.Contents.Count; i++)
            {
                // Show only first 3 items
                if (i == 3)
                    break;

                // Instantiate the prefab
                var operatorCardAutomation = Instantiate(uiAutomationNamePrefab, uiAutomations.transform).GetComponent<OperatorCardAutomation>();
                operatorCardAutomation.OnPrefabCreated(expressionIndex, i, positionIndex, uiExpressions);
                operatorCardAutomations.Add(operatorCardAutomation);
            }
        }

        // Sets operator type in operator card
        SetOperatorName(expression, stepIndex);

        // Sets automation card color
        SetColor(uiExpressions.ExpressionToColor(expression, stepIndex));

        // Sets listener to select a specific step
        buttonRef.onClick.AddListener(() => {
            uiExpressions.expressionManager.ClearSubExpressions(positionIndex);
            stepsManager.GetComponent<StepsManager>().ShowStep(expressionIndex, stepIndex);
        });
    }

    public void RemoveLink(int index)
    {
        operatorCardAutomations[index].DeactivateLinks();
    }

    public bool IsTemporary()
    {
        return isTemporary;
    }

    private void SetOperatorName([NotNull] Expression expression, int index)
    {
        if (expression is Order)
            SetOperatorName("ORDER INDEPENDENCE");

        else if (expression is Choice)
            SetOperatorName("SCELTA");

        else
            SetOperatorName(expression.Contents[index]);
    }

    private void SetOperatorName([NotNull] Automation automation)
    {
        var name = automation.Name;

        if (name.StartsWith("automation."))
            SetOperatorName("AUTOMAZIONE");

        else if (name.StartsWith("order."))
            SetOperatorName("ORDER INDEPENDENCE");

        else if (name.StartsWith("choice."))
            SetOperatorName("SCELTA");
    }

    private void SetOperatorName(string operatorName) => uiOperatorName.text = operatorName;

    private void SetColor([NotNull] Color color) => imageRef.color = color;
}
