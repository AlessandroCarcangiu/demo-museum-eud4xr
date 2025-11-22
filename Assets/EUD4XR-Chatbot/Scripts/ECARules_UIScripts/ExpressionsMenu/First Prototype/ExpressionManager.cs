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
    private List<int> subExpressionsPositions;
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

        // Init lists
        operatorCards = new List<OperatorCard>();
        subExpressionsPositions = new List<int>();
    }

    public void ShowExpression(int index)
    {
        // Clears panel
        ClearExpressionData();
        
        var expression = uiExpressions.GetExpressionAtIndex(index);

        // Sets expression name in the nav bar
        SetExpressionName(expression);

        // Instantiate selected step prefab
        stepsManagerInstance = Instantiate(uiStepsManagerPrefab, transform).GetComponent<StepsManager>();
        stepsManagerInstance.OnPrefabCreated(index, uiExpressions);

        // If the expression is a sequence an operator card for each step is instantiated
        if (expression is Sequence)
        {
            for (int i = 0; i < expression.Contents.Count; i++)
            {
                //if (expression.Contents[i].Name.StartsWith("automation."))
                // Instantiate the prefab and add it to the grid
                var operatorCard = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
                operatorCard.OnPrefabCreated(index, i, false, uiExpressions);
                operatorCards.Add(operatorCard);
            }
        }
        // If the expression is not a sequence a single operator card is instantiated
        else
        {
            // Instantiate the prefab and add it to the grid
            var operatorCard = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
            operatorCard.OnPrefabCreated(index, 0, false, uiExpressions);
            operatorCards.Add(operatorCard);
        }
    }

    private void ClearExpressionData()
    {
        foreach (Transform child in uiExpressionGrid.transform)
            Destroy(child.gameObject);

        operatorCards.Clear();
        subExpressionsPositions.Clear();
        /*
        Debug.Log(operatorCards.Count + " operator cards to clear.");
        // Clear cards
        for (int i = 0; i < operatorCards.Count; i++)
        {
            Debug.Log("steps manager destroying operator card " + i);
            Destroy(operatorCards[i].gameObject);
            operatorCards.RemoveAt(i);
            i--;
        }
        Debug.Log("Operator cards cleared. Remaining: " + operatorCards.Count);
        */
        // Clear steps manager
        if (stepsManagerInstance != null)
            Destroy(stepsManagerInstance.gameObject);
    }

    public void LoadSubExpression(int expressionIndex, int stepIndex, int positionIndex)
    {
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);
        var subExpressionIndex = uiExpressions.GetExpressionIndexByName(expression.Contents[stepIndex].Name);

        var subExpressionPrefab = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform);
        subExpressionPrefab.transform.SetSiblingIndex(positionIndex + 1);
        var subExpression = subExpressionPrefab.GetComponent<OperatorCard>();
        subExpression.OnPrefabCreated(subExpressionIndex, 0, true, uiExpressions);

        if (positionIndex == operatorCards.Count - 1)
            operatorCards.Add(subExpression);
        else
            operatorCards.Insert(positionIndex + 1, subExpression);

        subExpressionsPositions.Add(positionIndex + 1);
    }
    /*
    public void CloseAllSubExpressions()
    {
        for (int i = 0; i < subExpressionsPositions.Count; i++)
        {
            var subExpressionIndex = subExpressionsPositions[i];
            Destroy(uiExpressionGrid.transform.GetChild(subExpressionIndex).gameObject);
            subExpressionsPositions.RemoveAt(i);

            // Link image of the first deleted card needs to be explicitly destroyed
            if (i == 0)
            {
                var firstCard = uiExpressionGrid.transform.GetChild(subExpressionIndex - 1).GetComponent<OperatorCard>();
            }

            i--;
        }
    }
    */
    public void ClearSubExpressions(int positionIndex)
    {
        // Takes the index of the pressed card to check if subexpressions need to be cleared

        // USANDO OPERATOR CARDS POSSO CONTROLLARE CARTA PER CARTA SE LA CARTA È UNA SOTTOESPRESSIONE
        Debug.Log("operator card at index " + positionIndex + " pressed, checking for subexpressions to clear...");
        for (int i = 0; i < subExpressionsPositions.Count; i++)
        {
            var subExpressionIndex = subExpressionsPositions[i];
            Debug.Log("found subexpression at index " + subExpressionIndex);
            if (positionIndex != subExpressionIndex)
            {
                Debug.Log($"clearing subexpression at index {subExpressionIndex} because it is not {positionIndex}");
                Destroy(uiExpressionGrid.transform.GetChild(subExpressionIndex).gameObject);
                subExpressionsPositions.RemoveAt(i);

                // Link image of the first deleted card needs to be explicitly destroyed
                if (i == 0)
                {
                    var firstCard = uiExpressionGrid.transform.GetChild(subExpressionIndex - 1).GetComponent<OperatorCard>();
                }

                i--;
            }
        }
    }

    public StepsManager GetStepsManagerInstance() => stepsManagerInstance;

    public List<int> GetSubExpressionsPositions() => subExpressionsPositions;

    private void SetExpressionName([NotNull] Expression expression) => uiExpressionName.text = expression.Name;
}
