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
    public GameObject uiAutomationsName;
    public TMP_Text uiExpressionType;
    public Button buttonRef;
    public Image imageRef;

    private void Awake()
    {
        // Check for ref not null
        if (uiAutomationNamePrefab == null) throw new Exception("uiAutomationNamePrefab is null");
        if (uiAutomationsName == null) throw new Exception("uiAutomationsName is null");
        if (uiExpressionType == null) throw new Exception("uiExpressionType is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (imageRef == null) throw new Exception("imageRef is null");
    }

    public void OnPrefabCreated(Expression expression, int index, GameObject selectedStep, Color color)
    {
        if (expression == null) throw new Exception("expression is null");
        if (selectedStep == null) throw new Exception("selectedStep is null");
        if (color == null) throw new Exception("color is null");

        // If expression is a sequence each operator card contains one automation
        if (expression is Sequence)
        {
            // Instantiate the prefab
            var operatorCardAutomationName = Instantiate(uiAutomationNamePrefab, uiAutomationsName.transform).GetComponentInChildren<TMP_Text>();
            // Sets automation name in operator card
            SetAutomationName(expression.Contents[index], operatorCardAutomationName);
        }
        // If expression is not a sequence the operator card contains all automations
        else
            SetAutomationNames(expression.Contents);
        // Sets expression type in operator card
        SetExpressionType(expression);
        // Sets automation card color
        SetColor(color);
        // Sets listener to select a specific step
        buttonRef.onClick.AddListener(() => selectedStep.GetComponent<SelectedStep>().ShowStep(expression, index, expression.Contents.Count));
    }

    private void SetAutomationName([NotNull] Automation automation, TMP_Text tmp_text) => tmp_text.text = automation.Name;
    
    private void SetAutomationName(string text, TMP_Text tmp_text) => tmp_text.text = text;

    private void SetAutomationNames(List<Automation> automations)
    {
        var numberOfAutomations = automations.Count;

        // Limits the number of automations shown in the operator card to a maximum of 3
        if (numberOfAutomations > 3)
        {
            for (int i = 0; i < numberOfAutomations; i++)
            {
                // Instantiate the prefab
                var operatorCardAutomationName = Instantiate(uiAutomationNamePrefab, uiAutomationsName.transform).GetComponentInChildren<TMP_Text>();
                if (i < 2)
                {
                    // Sets automation name in operator card
                    SetAutomationName(automations[i], operatorCardAutomationName);
                }
                else
                {
                    // Shows number of remaining automations
                    SetAutomationName($"+{numberOfAutomations - i}...", operatorCardAutomationName);
                    break;
                }
            }
        }
        else
        {
            foreach (var automation in automations)
            {
                // Instantiate the prefab
                var operatorCardAutomationName = Instantiate(uiAutomationNamePrefab, uiAutomationsName.transform).GetComponentInChildren<TMP_Text>();
                // Sets automation name in operator card
                SetAutomationName(automation, operatorCardAutomationName);
            }
        }
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
