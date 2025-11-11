using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCardAutomation : MonoBehaviour
{
    public TMP_Text contentRef;
    public Button buttonRef;

    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
    }

    public void OnPrefabCreated(Expression expression, Dictionary<string, string> automations, int index, GameObject selectedStep)
    {
        if (expression == null) throw new Exception("expression is null");
        if (selectedStep == null) throw new Exception("selectedStep is null");

        if (expression is Sequence)
        {
            // Sets automation name in operator card
            SetAutomationName(expression.Contents[index].Name, contentRef);
            // disables listener
            buttonRef.enabled = false;
        }
        else
        {
            // Handles multiple automations in same operator card
            var numberOfAutomations = expression.Contents.Count;
            if (index < 2 || numberOfAutomations <= 3)
            {
                // Sets automation name in operator card
                SetAutomationName(expression.Contents[index].Name, contentRef);
            }
            else
            {
                // Shows number of remaining automations
                SetAutomationName($"+{numberOfAutomations - index}...", contentRef);
            }
            // Sets listener to select a specific step
            buttonRef.onClick.AddListener(() => selectedStep.GetComponent<SelectedStep>().ShowStep(expression, automations, index));
        }
    }

    private void SetAutomationName(string text, TMP_Text tmp_text) => tmp_text.text = text.Substring("automation.".Length);
}
