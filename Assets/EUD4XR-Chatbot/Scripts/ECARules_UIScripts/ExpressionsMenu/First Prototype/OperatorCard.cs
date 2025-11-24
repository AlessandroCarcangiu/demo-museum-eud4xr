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
    public GameObject uiItemPrefab;
    public GameObject uiItems;
    public TMP_Text uiOperatorName;
    public Button buttonRef;
    public Image imageRef;

    private bool isSubExpression;
    private List<OperatorCardItem> operatorCardItems;

    private void Awake()
    {
        // Check for ref not null
        if (uiItemPrefab == null) throw new Exception("uiItemPrefab is null");
        if (uiItems == null) throw new Exception("uiItems is null");
        if (uiOperatorName == null) throw new Exception("uiOperatorName is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (imageRef == null) throw new Exception("imageRef is null");

        // Init list
        operatorCardItems = new List<OperatorCardItem>();
    }

    public void OnPrefabCreated(int expressionIndex, int stepIndex, bool state, UIExpressions uiExprRef)
    {
        isSubExpression = state;

        // Save reference to UIExpressions
        uiExpressions = uiExprRef;

        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);

        var stepsManager = uiExpressions.expressionManager.GetStepsManagerInstance();

        Debug.Log($"OperatorCard for expression \"{expression.Name}\" - OnPrefabCreated called with expressionIndex: {expressionIndex}, stepIndex: {stepIndex}, state: {state}, positionIndex: {transform.GetSiblingIndex()}");

        // Set operator type in operator card
        SetOperatorName(expression, stepIndex);

        // Set operator card color
        SetColor(uiExpressions.ExpressionToColor(expression, stepIndex));

        // If operator is an automation the operator card contains one item
        if (expression is Sequence && expression.Contents[stepIndex].Name.StartsWith("automation."))
            CreateOperatorCardItem(expressionIndex, stepIndex);
        // If operator is an expression the operator card contains all its items
        else
        {
            // Handle complex sequences
            if (expression is Sequence)
            {
                expressionIndex = uiExpressions.GetExpressionIndexByName(expression.Contents[stepIndex].Name);
                HandleMultipleItems(expressionIndex);
            }
            // Handle order independence and choice
            else
                HandleMultipleItems(expressionIndex);
        }

        // Set listener to select a specific step
        buttonRef.onClick.AddListener(() => {
            Debug.Log($"OperatorCard for expression \"{expression.Name}\" - Button clicked to show expressionIndex: {expressionIndex}, stepIndex: {stepIndex}, positionIndex: {transform.GetSiblingIndex()}");
            stepsManager.ShowStep(expressionIndex, stepIndex);
        });
    }

    private void HandleMultipleItems(int expressionIndex)
    {
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);

        for (int i = 0; i < expression.Contents.Count; i++)
        {
            // Show only first 3 items
            if (i == 3)
                break;

            CreateOperatorCardItem(expressionIndex, i);
        }
    }

    private void CreateOperatorCardItem(int expressionIndex, int stepIndex)
    {
        // Instantiate the prefab and add it to the operator card
        var operatorCardItem = Instantiate(uiItemPrefab, uiItems.transform).GetComponent<OperatorCardItem>();
        operatorCardItem.OnPrefabCreated(expressionIndex, stepIndex, uiExpressions);
        // Add item reference to the list
        operatorCardItems.Add(operatorCardItem);
    }

    public bool IsSubExpression()
    {
        return isSubExpression;
    }

    public int GetCardPosition()
    {
        return transform.GetSiblingIndex();
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
