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
    public TMP_Text uiItemInfo;
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
        if (uiItemInfo == null) throw new Exception("uiItemInfo is null");
        if (imageRef == null) throw new Exception("imageRef is null");
    }

    public void OnPrefabCreated(int expressionIndex, UIExpressions uiExprRef)
    {
        // Save reference to UIExpressions
        uiExpressions = uiExprRef;
        // First step is automatically selected by default
        ShowStep(expressionIndex, 0);
    }

    public void ShowStep(int expressionIndex, int stepIndex)
    {
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);

        var color = uiExpressions.ExpressionToColor(expression, stepIndex);

        // Remove all previous listeners from arrows to avoid multiple calls
        ResetListeners();
        // Set operator info in the sequence bar
        SetOperatorInfo(expression, stepIndex);
        // Set current automation info in the sequence bar
        SetItemInfo(expression.Contents[stepIndex], expressionIndex);
        // Set background color of the sequence bar
        SetColor(imageRef, color);

        if (expression is Sequence)
        {
            // Hide vertical navigation if current step is an automation
            if (expression.Contents[stepIndex].Name.StartsWith("automation."))
            {
                ShowArrow(uiUpArrow, false);
                ShowArrow(uiDownArrow, false);
            }
            // Handle vertical navigation if current step is an expression
            else
            {
                Debug.Log("Handling vertical navigation for expression at step index " + stepIndex);

                // Activates up arrow if current step has predecessors
                if (stepIndex > 0)
                {
                    Debug.Log("Adding listener to up arrow for expressionIndex: " + expressionIndex + ", stepIndex: " + (stepIndex - 1));
                    ShowArrow(uiUpArrow, true);
                    uiUpArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex - 1));
                }
                else
                {
                    Debug.Log("Deactivating up arrow");
                    ShowArrow(uiUpArrow, false);
                }

                // Activates down arrow if current step has successors
                if (stepIndex < expression.Contents.Count - 1)
                {
                    Debug.Log("Adding listener to up arrow for expressionIndex: " + expressionIndex + ", stepIndex: " + (stepIndex + 1));
                    ShowArrow(uiDownArrow, true);
                    uiDownArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex + 1));
                }
                else
                {
                    Debug.Log("Deactivating down arrow");
                    ShowArrow(uiDownArrow, false);
                }
                
            }

            // Activate left arrow if current step has predecessors
            if (stepIndex > 0)
            {
                ShowArrow(uiLeftArrow, true);
                uiLeftArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex - 1));
            }
            else
                ShowArrow(uiLeftArrow, false);

            // Activate right arrow if current step has successors
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
            // Hide vertical navigation if current step is an automation
            if (expression.Contents[stepIndex].Name.StartsWith("automation."))
            {
                // Hide horizontal navigation (da cambiare per gestire le sottoespressioni)
                ShowArrow(uiLeftArrow, false);
                ShowArrow(uiRightArrow, false);
            }

            // Activate up arrow if current step has predecessors
            if (stepIndex > 0)
            {
                ShowArrow(uiUpArrow, true);
                uiUpArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expressionIndex, stepIndex - 1));
            }
            else
                ShowArrow(uiUpArrow, false);

            // Activate down arrow if current step has successors
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

    private void ResetListeners()
    {
        uiLeftArrow.GetComponent<Button>().onClick.RemoveAllListeners();
        uiRightArrow.GetComponent<Button>().onClick.RemoveAllListeners();
        uiUpArrow.GetComponent<Button>().onClick.RemoveAllListeners();
        uiDownArrow.GetComponent<Button>().onClick.RemoveAllListeners();
    }

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

    // Set operator name in the sequence bar
    private void SetOperatorName(string name) => uiOperatorName.text = name;

    // Set a text description of the operator in the sequence bar
    private void SetOperatorDescription(string description) => uiOperatorDescription.text = description;

    private void SetItemInfo([NotNull] Automation automation, int expressionIndex)
    {
        if (automation.Name.StartsWith("automation."))
            SetItemInfo(automation, uiExpressions.GetExpressionAutomations(expressionIndex));
        else
            SetItemInfo($"<size=7>{automation.Name.Substring(automation.Name.IndexOf(".") + 1 )}</size=7>\n\n" +
                $"Sottoespressione rilevata, fare click sull'etichetta dedicata per aprire una carta temporanea");
    }

    private void SetItemInfo([NotNull] Automation automation, Dictionary<string, string> dictAutomations)
    {
        // da formattare meglio
        foreach (var (key, value) in dictAutomations)
        {
            string name = automation.Name.Substring("automation.".Length);
            if (key == name)
            {
                uiItemInfo.text = $"<size=7>{name}</size=7>\n\n{value}";
                break;
            }
        }
    }

    private void SetItemInfo(string info) => uiItemInfo.text = info;

    private void SetColor([NotNull] Image image, [NotNull] Color color) => image.color = color;

    private void ShowArrow(GameObject arrow, bool flag)
    {
        arrow.GetComponent<Button>().interactable = flag;
        arrow.GetComponent<Image>().enabled = flag;
    }

}
