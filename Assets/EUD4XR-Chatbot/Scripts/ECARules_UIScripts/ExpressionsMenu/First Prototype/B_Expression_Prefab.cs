using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class B_Expression_Prefab : MonoBehaviour
{
    public UIExpressions uiExpressions;
    public TMP_Text contentRef;
    public Button buttonRef;

    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
    }

    public void OnPrefabCreated(UIExpressions uiExpressions, int index)
    {
        if (uiExpressions == null) throw new Exception("uiExpressions is null");

        // Set expression name as button text
        var expression = uiExpressions.GetExpressionAtIndex(index);
        SetBody(expression);

        // Add listener to button so it loads the expression when pressed
        buttonRef.onClick.AddListener(() => uiExpressions.LoadExpression(index));
    }

    private void SetBody([NotNull] Expression expression) => contentRef.text = expression.Name;
}
