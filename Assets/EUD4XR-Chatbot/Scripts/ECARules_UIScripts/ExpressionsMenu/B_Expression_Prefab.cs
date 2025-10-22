using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class B_Expression_Prefab : MonoBehaviour
{
    public GameObject uiSelectedExpressionPrefab;
    public TMP_Text contentRef;
    public Button buttonRef;

    private void Awake()
    {
        // Check for ref not null
        if (uiSelectedExpressionPrefab == null) throw new Exception("uiSelectedExpressionPrefab is null");
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
    }

    public void OnPrefabCreated(Expression expression, Dictionary<string, string> automations)
    {
        if (expression == null) throw new Exception("expression is null");
        // Sets expression name as button text
        SetBody(expression);
        // Adds listener to the button so it shows the expression in detail when pressed
        buttonRef.onClick.AddListener(() => UpdateSelectedExpression(expression, automations));
    }

    private void SetBody([NotNull] Expression expression) => contentRef.text = expression.Name;

    private void UpdateSelectedExpression(Expression expression, Dictionary<string, string> automations)
    {
        var selectedExpressionScript = FindObjectOfType<SelectedExpression>();
        selectedExpressionScript.LoadExpression(expression, automations);
    }
}
