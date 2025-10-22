using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.Utils;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedExpression : Singleton<SelectedExpression>
{
    public GameObject uiOperatorCardPrefab;
    public GameObject uiSelectedStepPrefab;
    public TMP_Text uiExpressionName;
    public GameObject uiExpressionGrid;
    public Image imageRef;

    private bool UIVisible;
    private GameObject selectedStepInstance;

    private void Awake()
    {
        // Check for ref not null
        if (uiOperatorCardPrefab == null) throw new Exception("uiOperatorCardPrefab is null");
        if (uiSelectedStepPrefab == null) throw new Exception("uiSelectedStepPrefab is null");
        if (uiExpressionName == null) throw new Exception("uiExpressionName is null");
        if (uiExpressionGrid == null) throw new Exception("uiExpressionGrid is null");
        if (imageRef == null) throw new Exception("imageRef is null");
    }

    private void Start()
    {
        ShowUI(false); // UI is not shown until an expression is selected
    }

    public void LoadExpression(Expression expression, Dictionary<string, string> automations)
    {
        // Clears panel
        if (UIVisible)
            ClearPreviousExpressionData();
        else
            ShowUI(true);

        // Sets operator name in the nav bar
        SetExpressionName(expression);
        // Gets a color associated to the expression type
        var operatorColor = ExpressionToColor(expression);

        // Instantiate selected step prefab
        selectedStepInstance = Instantiate(uiSelectedStepPrefab, transform);
        selectedStepInstance.GetComponent<SelectedStep>().OnPrefabCreated(expression, automations, operatorColor);

        // If the expression is a sequence an operator card for each step is instantiated
        if (expression is Sequence) 
        {
            for (int i = 0; i < expression.Contents.Count; i++)
            {
                // Instantiate the prefab and add it to the grid
                var operatorCardScript = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
                operatorCardScript.OnPrefabCreated(expression, automations, i, selectedStepInstance, operatorColor);
            }
        }
        // If the expression is not a sequence a single operator card is instantiated
        else
        {
            // Instantiate the prefab and add it to the grid
            var operatorCardScript = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
            operatorCardScript.OnPrefabCreated(expression, automations, 0, selectedStepInstance, operatorColor);
        }
    }

    public void ShowUI(bool flag)
    {
        // Sets visibility of all the UI elements
        foreach (Transform child in transform)
            child.gameObject.SetActive(flag);
        
        // Sets visibility of background
        HandleBackgroundTransparency(Convert.ToSingle(flag));
        
        // Sets visibility flag
        UIVisible = flag;
    }

    private void HandleBackgroundTransparency(float alphaValue)
    {
        Color color = imageRef.color;
        color.a = alphaValue;
        imageRef.color = color;
    }

    private void ClearPreviousExpressionData()
    {
        // Clear grid
        foreach (Transform child in uiExpressionGrid.transform)
            Destroy(child.gameObject);

        // Clear selected step
        Destroy(selectedStepInstance);
    }

    private Color ExpressionToColor(Expression expression)
    {
        if (expression is Sequence)
            return Color.gray;

        if (expression is Order)
            return Color.blue;

        if (expression is Choice)
            return Color.yellow;

        return Color.white;
    }

    private void SetExpressionName([NotNull] Expression expression) => uiExpressionName.text = expression.Name;
}
