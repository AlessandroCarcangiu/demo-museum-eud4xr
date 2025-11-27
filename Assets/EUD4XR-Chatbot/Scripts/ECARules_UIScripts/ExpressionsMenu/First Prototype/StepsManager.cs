using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepsManager : MonoBehaviour
{
    public GameObject uiLeftArrow;
    public GameObject uiRightArrow;
    public GameObject uiUpArrow;
    public GameObject uiDownArrow;
    public TMP_Text uiOperatorName;
    public TMP_Text uiOperatorDescription;
    public TMP_Text uiItemInfo;
    public Image imageRef;

    private int horizontalIndex;
    private int verticalIndex;

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

    public void OnPrefabCreated()
    {
        // First step is automatically selected by default
        ShowStep(0, 0);
    }

    public void ShowStep(int horizontalIndex, int verticalIndex)
    {
        this.horizontalIndex = horizontalIndex;
        this.verticalIndex = verticalIndex;

        // Save UIExpressions reference
        var uiExprRef = UIExpressions.Instance;

        var card = uiExprRef.expressionManager.GetOperatorCard(horizontalIndex);

        Expression expression;
        int stepIndex;

        if (!card.IsSubExpression())
            expression = uiExprRef.GetCurrentExpression();
        else
            expression = uiExprRef.GetExpressionAtIndex(card.GetIndex());

        if (expression is Sequence)
            stepIndex = card.GetIndex();
        else
            stepIndex = verticalIndex;

        // Handle navigation arrows based on expression type
        HandleNavigation(expression, stepIndex, uiExprRef);

        // Set operator info in the sequence bar
        SetOperatorInfo(expression, stepIndex);
        // Set current item info in the sequence bar
        SetItemInfo(expression, stepIndex);
        // Set background color of the sequence bar
        SetColor(uiExprRef.ExpressionToColor(expression, stepIndex));
    }

    private void HandleNavigation(Expression expression, int stepIndex, UIExpressions uiExprRef)
    {
        // Remove all previous listeners from arrows to avoid multiple calls
        ResetListeners();

        var openCards = uiExprRef.expressionManager.GetOpenCards();

        // Handles sequence
        if (expression is Sequence)
        {
            // Handle horizontal navigation
            if (horizontalIndex > 0)
            {
                // Activate left arrow if current step has predecessors
                ShowArrow(uiLeftArrow, true);
                uiLeftArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex - 1, 0));
            }
            else
                ShowArrow(uiLeftArrow, false);
            if (horizontalIndex < openCards - 1)
            {
                // Activate right arrow if current step has successors
                ShowArrow(uiRightArrow, true);
                uiRightArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex + 1, 0));
            }
            else
                ShowArrow(uiRightArrow, false);

            // Hide vertical navigation if current step is an automation
            if (expression.Contents[stepIndex].Name.StartsWith("automation."))
            {
                ShowArrow(uiUpArrow, false);
                ShowArrow(uiDownArrow, false);
            }
            // Handle vertical navigation if current step is an expression
            else
            {
                var currentStepExpression = UIExpressions.Instance.GetExpressionByName(expression.Contents[stepIndex].Name);

                // Activate up arrow if current step has predecessors
                if (verticalIndex > 0)
                {
                    ShowArrow(uiUpArrow, true);
                    uiUpArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex, verticalIndex - 1));
                }
                else
                    ShowArrow(uiUpArrow, false);

                // Activate down arrow if current step has successors
                if (verticalIndex < currentStepExpression.Contents.Count - 1)
                {
                    ShowArrow(uiDownArrow, true);
                    uiDownArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex, verticalIndex + 1));
                }
                else
                    ShowArrow(uiDownArrow, false);
            }
        }
        // Handles other expressions
        else
        {
            var currentExpression = uiExprRef.GetCurrentExpression();

            //  Expression is not a subexpression
            if (expression == currentExpression)
            {
                // Hide horizontal navigation
                ShowArrow(uiLeftArrow, false);
                ShowArrow(uiRightArrow, false);
            }
            // Expression is a subexpression
            else
            {
                if (horizontalIndex > 0)
                {
                    // Activate left arrow if is a subexpression
                    ShowArrow(uiLeftArrow, true);
                    uiLeftArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex - 1, 0));
                }
                else
                    ShowArrow(uiLeftArrow, false);

                if (horizontalIndex < openCards - 1)
                {
                    // Activate right arrow if current step has successors
                    ShowArrow(uiRightArrow, true);
                    uiRightArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex + 1, 0));
                }
                else
                    ShowArrow(uiRightArrow, false);
            }

            // Handle vertical navigation
            if (stepIndex > 0)
            {
                // Activate up arrow if current step has predecessors
                ShowArrow(uiUpArrow, true);
                uiUpArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex, verticalIndex - 1));
            }
            else
                ShowArrow(uiUpArrow, false);
            if (stepIndex < expression.Contents.Count - 1)
            {
                // Activate down arrow if current step has successors
                ShowArrow(uiDownArrow, true);
                uiDownArrow.GetComponent<Button>().onClick.AddListener(() => ShowStep(horizontalIndex, verticalIndex + 1));
            }
            else
                ShowArrow(uiDownArrow, false);
        }
    }

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

    private void SetItemInfo([NotNull] Expression expression, int stepIndex)
    {
        var item = expression.Contents[stepIndex];

        if (item.Name.StartsWith("automation."))
        {
            // da formattare meglio
            var name = item.Name.Substring("automation.".Length);
            var info = UIExpressions.Instance.GetAutomationInfo(item);
            SetItemInfo($"<size=7>{name}</size=7>\n\n{info}");
        }
        else if (expression is Sequence)
        {
            var stepExpression = UIExpressions.Instance.GetExpressionByName(item.Name);
            SetItemInfo(stepExpression, verticalIndex);
        }
        else
            SetItemInfo($"<size=7>{item.Name.Substring(item.Name.IndexOf(".") + 1)}</size=7>\n\n" +
                $"Sottoespressione rilevata, fare click sull'etichetta dedicata per aprire una carta temporanea");
    }

    private void SetItemInfo(string info) => uiItemInfo.text = info;

    private void SetColor([NotNull] Color color) => imageRef.color = color;

    private void ShowArrow(GameObject arrow, bool flag)
    {
        arrow.GetComponent<Button>().interactable = flag;
        arrow.GetComponent<Image>().enabled = flag;
    }

    public int GetVerticalIndex() => verticalIndex;
}
