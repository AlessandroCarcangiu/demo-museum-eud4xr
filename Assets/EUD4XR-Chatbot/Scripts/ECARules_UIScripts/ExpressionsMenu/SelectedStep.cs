using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.Utils;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedStep : Singleton<SelectedStep>
{
    public GameObject uiLeftArrow;
    public GameObject uiRightArrow;
    public GameObject uiUpArrow;
    public GameObject uiDownArrow;
    public TMP_Text uiOperatorName;
    public TMP_Text uiOperatorDescription;
    public TMP_Text uiAutomationInfo;
    public Image imageRef;

    private bool UIVisible;

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

    public void OnPrefabCreated(Expression expression, Color color)
    {
        if (expression == null) throw new Exception("expression is null");
        if (color == null) throw new Exception("color is null");

        // Sets background color of the nav bar
        SetColor(color); // da spostare in showstep per gestire le sequenze complesse
        // First step is automatically selected by default
        ShowStep(expression, 0, expression.Contents.Count);
    }

    public void ShowStep(Expression expression, int index, int steps)
    {
        // Sets operator name in the nav bar
        SetOperatorName(expression);
        // Sets a text description of the operator in the nav bar
        SetOperatorDescription(expression);
        // Sets current automation info in the nav bar
        SetAutomationInfo(expression.Contents[index]);

        if (expression is Sequence)
        {
            // Hides vertical navigation (da riabilitare per gestire le sequenze complesse)
            ShowArrow(uiUpArrow, false);
            ShowArrow(uiDownArrow, false);

            // Activates left arrow if current step has predecessors
            if (index > 0)
            {
                ShowArrow(uiLeftArrow, true);
                uiLeftArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expression, index - 1, steps));
            }
            else
                ShowArrow(uiLeftArrow, false);
            // Activates right arrow if current step has successors
            if (index < steps - 1)
            {
                ShowArrow(uiRightArrow, true);
                uiRightArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expression, index + 1, steps));
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
            if (index > 0)
            {
                ShowArrow(uiUpArrow, true);
                uiUpArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expression, index - 1, steps));
            }
            else
                ShowArrow(uiUpArrow, false);
            // Activates down arrow if current step has successors
            if (index < steps - 1)
            {
                ShowArrow(uiDownArrow, true);
                uiDownArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(expression, index + 1, steps));
            }
            else
                ShowArrow(uiDownArrow, false);
        }
    }

    private void SetOperatorName([NotNull] Expression expression)
    {
        if (expression is Sequence)
            SetOperatorName("AUTOMAZIONE"); // da cambiare per gestire le sequenze complesse
        
        else if (expression is Order)
            SetOperatorName("ORDER INDIPENDENCE");

        else if (expression is Choice)
            SetOperatorName("SCELTA");
    }

    private void SetOperatorName(string name) => uiOperatorName.text = name;

    private void SetOperatorDescription([NotNull] Expression expression)
    {
        if (expression is Sequence)
            SetOperatorDescription("La singola automazione deve essere eseguita per andare al passo successivo");

        else if (expression is Order)
            SetOperatorDescription("Tutte le automazioni devono essere eseguite, in qualsiasi ordine, per andare al passo successivo");

        else if (expression is Choice)
            SetOperatorDescription("Le automazioni sono mutualmente esclusive, ne viene eseguita solo una per andare al passo successivo");
    }

    private void SetOperatorDescription(string description) => uiOperatorDescription.text = description;

    private void SetAutomationInfo([NotNull] Automation automation) => uiAutomationInfo.text = automation.Name; // non basta il nome

    private void ShowArrow(GameObject arrow, bool flag)
    {
        arrow.GetComponent<Button>().interactable = flag;
        arrow.GetComponent<Image>().enabled = flag;
    }

    private void SetColor([NotNull] Color color) => imageRef.color = color;
}
