using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepsManager : Singleton<StepsManager>
{
    public UIExpressions uiExpressions;
    public GameObject uiLeftArrow;
    public GameObject uiRightArrow;
    public GameObject uiUpArrow;
    public GameObject uiDownArrow;
    public TMP_Text uiOperatorName;
    public TMP_Text uiOperatorDescription;
    public TMP_Text uiAutomationInfo;
    public Image imageRef;

    private int currentStep;

    private void Awake()
    {
        // Check for ref not null
        if (uiLeftArrow == null) throw new Exception("uiLeftArrow is null");
        if (uiRightArrow == null) throw new Exception("uiRightArrow is null");
        if (uiUpArrow == null) throw new Exception("uiUpArrow is null");
        if (uiDownArrow == null) throw new Exception("uiDownArrow is null");
        if (uiOperatorName == null) throw new Exception("uiOperatorName is null");
        if (uiOperatorDescription == null) throw new Exception("uiOperatorDescription is null");
        if (uiAutomationInfo == null) throw new Exception("uiAutomationInfo is null");
        if (imageRef == null) throw new Exception("imageRef is null");
    }

    public void OnPrefabCreated(int expressionIndex, UIExpressions uiExprRef)
    {
        uiExpressions = uiExprRef;
        // First step is automatically selected by default
        ShowStep(expressionIndex, 0);
    }

    public void ShowStep(int expressionIndex, int stepIndex)
    {
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);
        var expressionAutomations = uiExpressions.GetExpressionAutomations(expressionIndex);

        // Sets operator info in the sequence bar
        SetOperatorInfo(expression, stepIndex);
        // Sets current automation info in the sequence bar
        SetAutomationInfo(expression.Contents[stepIndex], expressionAutomations); // da gestire meglio
        // Sets background color of the sequence bar
        SetColor(uiExpressions.ExpressionToColor(expression, stepIndex));

        if (expression is Sequence)
        {
            // Hides vertical navigation if current step is an automation
            if (expression.Contents[stepIndex].Name.StartsWith("automation."))
            {
                ShowArrow(uiUpArrow, false);
                ShowArrow(uiDownArrow, false);
            }
            // Handles vertical navigation if current step is an expression
            else
            {
                /* DA RISCRIVERE 
                // Activates up arrow if current step has predecessors
                if (index > 0)
                {
                    ShowArrow(uiUpArrow, true);
                    uiUpArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expression, index - 1, automations));
                }
                else
                    ShowArrow(uiUpArrow, false);

                // Activates down arrow if current step has successors
                if (index < expression.Contents.Count - 1)
                {
                    ShowArrow(uiDownArrow, true);
                    uiDownArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expression, index + 1, automations));
                }
                else
                    ShowArrow(uiDownArrow, false);
                */
            }

            // Activates left arrow if current step has predecessors
            if (stepIndex > 0)
            {
                ShowArrow(uiLeftArrow, true);
                uiLeftArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex - 1));
            }
            else
                ShowArrow(uiLeftArrow, false);

            // Activates right arrow if current step has successors
            if (stepIndex < expression.Contents.Count - 1)
            {
                ShowArrow(uiRightArrow, true);
                uiRightArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex + 1));
            }
            else
                ShowArrow(uiRightArrow, false);
        }
        else
        {
            // Hides horizontal navigation
            ShowArrow(uiLeftArrow, false);
            ShowArrow(uiRightArrow, false);

            // Activates up arrow if current step has predecessors
            if (stepIndex > 0)
            {
                ShowArrow(uiUpArrow, true);
                uiUpArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex - 1));
            }
            else
                ShowArrow(uiUpArrow, false);

            // Activates down arrow if current step has successors
            if (stepIndex < expression.Contents.Count - 1)
            {
                ShowArrow(uiDownArrow, true);
                uiDownArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex + 1));
            }
            else
                ShowArrow(uiDownArrow, false);
        }

        currentStep = stepIndex;
    }

    public int GetCurrentStep() => currentStep;

    private void SetOperatorInfo([NotNull] Expression expression, int index)
    {
        if (expression is Order)
        {
            SetOperatorName("ORDER INDEPENDENCE");
            SetOperatorDescription("Tutte le automazioni devono essere eseguite, in qualsiasi ordine, per andare al passo successivo");
        }
        else if (expression is Choice)
        {
            SetOperatorName("SCELTA");
            SetOperatorDescription("Le automazioni sono mutualmente esclusive, ne viene eseguita solo una per andare al passo successivo");
        }
        else
            SetOperatorInfo(expression.Contents[index]);
    }

    private void SetOperatorInfo([NotNull] Automation automation)
    {
        var name = automation.Name;

        if (name.StartsWith("automation."))
        {
            SetOperatorName("AUTOMAZIONE");
            SetOperatorDescription("La singola automazione deve essere eseguita per andare al passo successivo");
        }
        else if (name.StartsWith("order."))
        {
            SetOperatorName("ORDER INDEPENDENCE");
            SetOperatorDescription("Tutte le automazioni devono essere eseguite, in qualsiasi ordine, per andare al passo successivo");
        }
        else if (name.StartsWith("choice."))
        {
            SetOperatorName("SCELTA");
            SetOperatorDescription("Le automazioni sono mutualmente esclusive, ne viene eseguita solo una per andare al passo successivo");
        }
    }

    // Sets operator name in the sequence bar
    private void SetOperatorName(string name) => uiOperatorName.text = name;

    // Sets a text description of the operator in the sequence bar
    private void SetOperatorDescription(string description) => uiOperatorDescription.text = description;

    private void SetAutomationInfo([NotNull] Automation automation, Dictionary<string, string> dictAutomations)
    {
        foreach (var (key, value) in dictAutomations)
        {
            string name = automation.Name.Substring("automation.".Length);
            if (key == name)
            {
                uiAutomationInfo.text = $"<size=7>{name}</size=7>\n\n{value}";
                break;
            }
        }
    }

    private void SetColor([NotNull] Color color) => imageRef.color = color;

    private void ShowArrow(GameObject arrow, bool flag)
    {
        arrow.GetComponent<Button>().interactable = flag;
        arrow.GetComponent<Image>().enabled = flag;
    }

}
