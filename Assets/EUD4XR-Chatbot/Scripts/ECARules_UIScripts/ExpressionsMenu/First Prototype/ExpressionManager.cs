using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpressionManager : Singleton<ExpressionManager>
{
    public UIExpressions uiExpressions;
    public GameObject uiOperatorCardPrefab;
    public GameObject uiStepsManagerPrefab;
    public TMP_Text uiExpressionName;
    public Button uiGoBack;
    public GameObject uiExpressionGrid;
    
    private List<OperatorCard> operatorCards;
    private StepsManager stepsManagerInstance;

    private void Awake()
    {
        // Check for ref not null
        if (uiExpressions == null) throw new Exception("uiExpressions is null");
        if (uiOperatorCardPrefab == null) throw new Exception("uiOperatorCardPrefab is null");
        if (uiStepsManagerPrefab == null) throw new Exception("uiStepsManagerPrefab is null");
        if (uiExpressionName == null) throw new Exception("uiExpressionName is null");
        if (uiGoBack == null) throw new Exception("uiGoBack is null");
        if (uiExpressionGrid == null) throw new Exception("uiExpressionGrid is null");

        // Init list
        operatorCards = new List<OperatorCard>();
    }

    public void ShowExpression(int index)
    {
        var expression = uiExpressions.GetExpressionAtIndex(index);

        // Set expression name in the nav bar
        SetExpressionName(expression);

        // Instantiate selected step prefab
        stepsManagerInstance = Instantiate(uiStepsManagerPrefab, transform).GetComponent<StepsManager>();
        stepsManagerInstance.OnPrefabCreated(index, uiExpressions);

        // If expression is a sequence then an operator card for each step is instantiated
        if (expression is Sequence)
        {
            for (int i = 0; i < expression.Contents.Count; i++)
                CreateOperatorCard(index, i);
        }
        // If expression is not a sequence then a single operator card is instantiated
        else
            CreateOperatorCard(index, 0);
    }

    private void CreateOperatorCard(int expressionIndex, int stepIndex)
    {
        // Instantiate the prefab and add it to the grid
        var operatorCard = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
        operatorCard.OnPrefabCreated(expressionIndex, stepIndex, false, uiExpressions);
        // Add operator card reference to the list
        operatorCards.Add(operatorCard);
    }

    public void OpenSubExpression(int expressionIndex, int stepIndex, int positionIndex)
    {
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);
        var subExpressionIndex = uiExpressions.GetExpressionIndexByName(expression.Contents[stepIndex].Name);

        CreateSubExpressionOperatorCard(subExpressionIndex, positionIndex);
    }

    private void CreateSubExpressionOperatorCard(int expressionIndex, int positionIndex)
    {
        Debug.Log("Creating sub-expression operator card for expression \"" + uiExpressions.GetExpressionAtIndex(expressionIndex) + "\" at index " + expressionIndex + " at position index " + (positionIndex + 1));
        
        var operatorCard = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
        operatorCard.transform.SetSiblingIndex(positionIndex + 1);
        operatorCard.OnPrefabCreated(expressionIndex, 0, true, uiExpressions);

        if (positionIndex == operatorCards.Count - 1)
            operatorCards.Add(operatorCard);
        else
            operatorCards.Insert(positionIndex + 1, operatorCard);
    }

    public void CloseSubExpressionsAfterIndex(int index)
    {
        // Count how many sub-expressions depend on the pressed card
        var openedSubExpressions = CountCardSubExpressions(index);

        // Remove all sub-expressions depending on the pressed card
        for (int i = index + openedSubExpressions; i > index; i--)
        {
            // Destroy sub-expression card and remove it from list
            Destroy(operatorCards[i].gameObject);
            operatorCards.RemoveAt(i);
        }
    }

    private int CountCardSubExpressions(int cardIndex)
    {
        var count = 0;
        
        for (int i = cardIndex + 1; i < operatorCards.Count; i++)
        {
            if (operatorCards[i].IsSubExpression())
                count++;
            else
                break;
        }

        return count;
    }

    public void ClearExpressionData()
    {
        // Clear operator cards
        foreach (Transform child in uiExpressionGrid.transform)
            Destroy(child.gameObject);
        operatorCards.Clear();
        
        // Clear steps manager
        if (stepsManagerInstance != null)
            Destroy(stepsManagerInstance.gameObject);
    }

    public StepsManager GetStepsManagerInstance() => stepsManagerInstance;

    private void SetExpressionName([NotNull] Expression expression) => uiExpressionName.text = expression.Name;
}
