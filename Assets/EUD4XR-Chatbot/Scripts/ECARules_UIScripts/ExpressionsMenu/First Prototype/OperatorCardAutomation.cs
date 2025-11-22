using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCardAutomation : MonoBehaviour
{
    public UIExpressions uiExpressions;
    public TMP_Text contentRef;
    public Button buttonRef;
    public GameObject imagesRef;

    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (imagesRef == null) throw new Exception("imagesRef is null");
    }

    public void OnPrefabCreated(int expressionIndex, int stepIndex, int positionIndex, UIExpressions uiExprRef)
    {
        uiExpressions = uiExprRef; 
        
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);
        
        // If expression is a sequence of automations the operator card contains only one item
        if (expression is Sequence && expression.Contents[stepIndex].Name.StartsWith("automation."))
        {
            // Sets automation name in operator card
            SetAutomationName(expression.Contents[stepIndex], contentRef);
            // disables listener
            buttonRef.enabled = false;
        }
        else
        {
            // Handles multiple items in same operator card
            var numberOfItems = expression.Contents.Count;
            if (stepIndex < 2 || numberOfItems <= 3)
            {
                var content = expression.Contents[stepIndex];
                // Sets automation name in operator card
                SetAutomationName(content, contentRef);
                if (content.Name.StartsWith("automation."))
                {
                    // Sets listener to select a specific step
                    buttonRef.onClick.AddListener(() =>
                    {
                        uiExpressions.expressionManager.ClearSubExpressions(positionIndex);
                        var stepsManager = uiExpressions.expressionManager.GetStepsManagerInstance();
                        stepsManager.ShowStep(expressionIndex, stepIndex);
                    });
                }
                else
                {
                    // Sets listener to load a temporary sub-expression card
                    buttonRef.onClick.AddListener(() => HandleSubExpressions(expressionIndex, stepIndex, positionIndex));
                }
            }
            else
            {
                // Shows number of remaining automations
                SetAutomationName($"+{numberOfItems - stepIndex}...", contentRef);
                // Sets listener to handle hidden steps
                buttonRef.onClick.AddListener(() => HandleHiddenSteps(expressionIndex, stepIndex, positionIndex));
            }
        }
    }

    private void HandleHiddenSteps(int expressionIndex, int firstHiddenStep, int positionIndex)
    {
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);
        var expressionManager = uiExpressions.expressionManager;
        var stepsManager = expressionManager.GetStepsManagerInstance();
        
        // Initializes next step index variable
        int nextStep;
        // Gets current step shown
        var currentStep = stepsManager.GetCurrentStep();
        // Stores index of last step to loop back to first hidden step in case of overflow
        var lastStep = expression.Contents.Count - 1;

        // Skips to first hidden step if current step comes before it, or loops back to it if current step is the last
        if (currentStep < firstHiddenStep || currentStep == lastStep)
            nextStep = firstHiddenStep;
        // Goes regularly to next step otherwise
        else
            nextStep = currentStep + 1;

        if (expression.Contents[nextStep].Name.StartsWith("automation."))
        {
            // Sets listener to select a specific step
            stepsManager.ShowStep(expressionIndex, nextStep);
        }
        else
        {
            HandleSubExpressions(expressionIndex, nextStep, positionIndex);
        }
    }

    private void HandleSubExpressions(int expressionIndex, int stepIndex, int positionIndex)
    {
        ActivateLinks(positionIndex);
        uiExpressions.expressionManager.ClearSubExpressions(positionIndex);
        uiExpressions.expressionManager.LoadSubExpression(expressionIndex, stepIndex, positionIndex);
    }

    private void ActivateLinks(int positionIndex)
    {
        if ((positionIndex + 1) % 4 == 0)
            imagesRef.transform.GetChild(1).gameObject.SetActive(true);
        else
            imagesRef.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DeactivateLinks()
    {
        imagesRef.transform.GetChild(0).gameObject.SetActive(false);
        imagesRef.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void SetAutomationName([NotNull] Automation automation, TMP_Text tmp_text) => tmp_text.text = automation.Name.Substring(automation.Name.IndexOf(".") + 1);

    private void SetAutomationName(string text, TMP_Text tmp_text) => tmp_text.text = text;
}
