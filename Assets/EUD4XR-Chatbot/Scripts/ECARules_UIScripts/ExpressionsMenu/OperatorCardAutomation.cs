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
            SetAutomationName(expression.Contents[index], contentRef);
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
                SetAutomationName(expression.Contents[index], contentRef);
                // Sets listener to select a specific step
                buttonRef.onClick.AddListener(() => selectedStep.GetComponent<SelectedStep>().ShowStep(expression, automations, index));
            }
            else
            {
                // Shows number of remaining automations
                SetAutomationName($"+{numberOfAutomations - index}...", contentRef);
                // Sets listener to handle hidden steps
                buttonRef.onClick.AddListener(() => HandleHiddenSteps(selectedStep, expression, automations, index));
            }
        }
    }

    private void HandleHiddenSteps(GameObject selectedStep, Expression expression, Dictionary<string, string> automations, int firstHiddenStep)
    {
        // Initializes next step index variable
        int nextStep;
        // Gets current step shown
        var currentStep = selectedStep.GetComponent<SelectedStep>().GetCurrentStep();
        // Stores index of last step to loop back to first hidden step in case of overflow
        var lastStep = expression.Contents.Count - 1;

        // Skips to first hidden step if current step comes before it, or loops back to it if current step is the last
        if (currentStep < firstHiddenStep || currentStep == lastStep)
            nextStep = firstHiddenStep;
        // Goes regularly to next step otherwise
        else
            nextStep = currentStep + 1;

        // Selects a specific step
        selectedStep.GetComponent<SelectedStep>().ShowStep(expression, automations, nextStep);
    }

    private void SetAutomationName([NotNull] Automation automation, TMP_Text tmp_text) => tmp_text.text = automation.Name.Substring("automation.".Length);

    private void SetAutomationName(string text, TMP_Text tmp_text) => tmp_text.text = text;
}
