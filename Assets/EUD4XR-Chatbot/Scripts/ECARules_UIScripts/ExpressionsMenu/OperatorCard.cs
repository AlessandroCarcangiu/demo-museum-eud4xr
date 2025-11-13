using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCard : MonoBehaviour
{
    public GameObject uiAutomationNamePrefab;
    public GameObject uiAutomations;
    public TMP_Text uiExpressionType;
    public Button buttonRef;
    public Image imageRef;

    private void Awake()
    {
        // Check for ref not null
        if (uiAutomationNamePrefab == null) throw new Exception("uiAutomationNamePrefab is null");
        if (uiAutomations == null) throw new Exception("uiAutomations is null");
        if (uiExpressionType == null) throw new Exception("uiExpressionType is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (imageRef == null) throw new Exception("imageRef is null");
    }

    public void OnPrefabCreated(Expression expression, Dictionary<string, string> automations, int index, GameObject selectedStep, Color color)
    {
        if (expression == null) throw new Exception("expression is null");
        if (selectedStep == null) throw new Exception("selectedStep is null");
        if (color == null) throw new Exception("color is null");

        // If expression is a sequence each operator card contains one automation
        if (expression is Sequence)
        {
            // Instantiate the prefab
            var operatorCardAutomationScript = Instantiate(uiAutomationNamePrefab, uiAutomations.transform).GetComponent<OperatorCardAutomation>();
            operatorCardAutomationScript.OnPrefabCreated(expression, automations, index, selectedStep);
        }
        // If expression is not a sequence the operator card contains all automations
        else
        {
            for (int i = 0; i < expression.Contents.Count; i++)
            {
                // Show only first 3 automations
                if (i == 3)
                    break;
                // Instantiate the prefab
                var operatorCardAutomationScript = Instantiate(uiAutomationNamePrefab, uiAutomations.transform).GetComponent<OperatorCardAutomation>();
                operatorCardAutomationScript.OnPrefabCreated(expression, automations, i, selectedStep);
            }
        }
        // Sets expression type in operator card
        SetExpressionType(expression);
        // Sets automation card color
        SetColor(color);
        // Sets listener to select a specific step
        buttonRef.onClick.AddListener(() => selectedStep.GetComponent<SelectedStep>().ShowStep(expression, automations, index));
    }

    private void SetExpressionType([NotNull] Expression expression)
    {
        if (expression is Sequence)
            SetExpressionType("AUTOMAZIONE"); // da cambiare per gestire le sequenze complesse

        else if (expression is Order)
            SetExpressionType("ORDER INDIPENDENCE");

        else if (expression is Choice)
            SetExpressionType("SCELTA");
    }

    private void SetExpressionType(string expressionType) => uiExpressionType.text = expressionType;

    private void SetColor([NotNull] Color color) => imageRef.color = color;
}
