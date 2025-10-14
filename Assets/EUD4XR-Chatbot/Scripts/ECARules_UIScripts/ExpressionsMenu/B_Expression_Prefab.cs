using System;
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

    public void OnPrefabCreated(Expression expressionToDisplay)
    {
        if (expressionToDisplay == null) throw new Exception("expressionToDisplay is null");
        
        SetBody(expressionToDisplay);

        buttonRef.onClick.AddListener(() => UpdateSelectedExpression(expressionToDisplay));
    }

    private void SetBody([NotNull] Expression expression) => contentRef.text = expression.Name;

    private void UpdateSelectedExpression(Expression expression)
    {
        var selectedExpressionScript = FindObjectOfType<SelectedExpression>();
        /* non può essere null
        if (selectedExpressionScript == null)
            selectedExpressionScript = Instantiate(uiSelectedExpressionPrefab, parent).GetComponent<SelectedExpression>();
        */
        selectedExpressionScript.LoadExpression(expression);
    }
}
