using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCardItem : MonoBehaviour
{
    public UIExpressions uiExpressions;
    public TMP_Text contentRef;
    public Button buttonRef;
    public Image imageRef;

    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (imageRef == null) throw new Exception("imageRef is null");
    }

    public void OnPrefabCreated(int expressionIndex, int stepIndex, UIExpressions uiExprRef)
    {
        // Save reference to UIExpressions
        uiExpressions = uiExprRef;
        // Gets expression at index
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);
        // Gets content of expression at step index
        var content = expression.Contents[stepIndex];

        Debug.Log($"OperatorCardItem for expression \"{expression.Name}\" - OnPrefabCreated - expressionIndex: {expressionIndex}, stepIndex: {stepIndex}, positionIndex: {GetComponentInParent<OperatorCard>().GetCardPosition()}");

        // Handle single item in operator card
        if (expression is Sequence && content.Name.StartsWith("automation."))
        {
            // Set item name in operator card
            SetItemName(expression.Contents[stepIndex], contentRef);

            // Disable item listener, card listener still active
            buttonRef.enabled = false;
        }
        // Handle multiple items in same operator card
        else
        {
            var numberOfItems = expression.Contents.Count;

            if (stepIndex < 2 || numberOfItems <= 3)
            {
                // Set item name in operator card
                SetItemName(content, contentRef);
                if (content.Name.StartsWith("automation."))
                {
                    // Set listener to select a specific step
                    buttonRef.onClick.AddListener(() =>
                    {
                        Debug.Log($"OperatorCard for expression \"{expression.Name}\" - Button clicked to show expressionIndex: {expressionIndex}, stepIndex: {stepIndex}, positionIndex: {GetComponentInParent<OperatorCard>().GetCardPosition()}");
                        var stepsManager = uiExpressions.expressionManager.GetStepsManagerInstance();
                        stepsManager.ShowStep(expressionIndex, stepIndex);
                    });
                }
                else
                {
                    // Set listener to handle opening and closing of sub-expression cards
                    buttonRef.onClick.AddListener(() =>
                    {
                        var positionIndex = GetComponentInParent<OperatorCard>().GetCardPosition();
                        HandleSubExpressions(expressionIndex, stepIndex, positionIndex);
                        Debug.Log($"OperatorCard for expression \"{expression.Name}\" - Button clicked to show expressionIndex: {expressionIndex}, stepIndex: {stepIndex} , positionIndex: {positionIndex}");
                    });
                }
            }
            else
            {
                // Show number of hidden steps
                SetItemName($"+{numberOfItems - stepIndex}...", contentRef);

                // Set listener to handle hidden steps
                buttonRef.onClick.AddListener(() =>
                {
                    HandleHiddenSteps(expressionIndex, stepIndex);
                    Debug.Log($"OperatorCard for expression \"{expression.Name}\" - Button clicked to show expressionIndex: {expressionIndex}, stepIndex: {stepIndex} , positionIndex: {GetComponentInParent<OperatorCard>().GetCardPosition()}");
                });
            }
        }

        // Link image is disabled by default
        imageRef.enabled = false;
    }

    private void HandleHiddenSteps(int expressionIndex, int firstHiddenStep)
    {
        var expression = uiExpressions.GetExpressionAtIndex(expressionIndex);
        var expressionManager = uiExpressions.expressionManager;
        var stepsManager = expressionManager.GetStepsManagerInstance();
        
        // Initialize next step index variable
        int nextStep;
        // Get current step shown
        var currentStep = stepsManager.GetCurrentStep();
        // Store index of last step to loop back to first hidden step in case of overflow
        var lastStep = expression.Contents.Count - 1;

        // Skip to first hidden step if current step comes before it, loops back to it if current step is the last
        if (currentStep < firstHiddenStep || currentStep == lastStep)
            nextStep = firstHiddenStep;
        // Go regularly to next step otherwise
        else
            nextStep = currentStep + 1;

        if (expression.Contents[nextStep].Name.StartsWith("automation."))
        {
            // Set listener to select a specific step
            stepsManager.ShowStep(expressionIndex, nextStep);
        }
        else
        {
            var positionIndex = GetComponentInParent<OperatorCard>().GetCardPosition();
            HandleSubExpressions(expressionIndex, nextStep, positionIndex);
        }
    }

    private void HandleSubExpressions(int expressionIndex, int stepIndex, int positionIndex)
    {
        if (imageRef.enabled)
            uiExpressions.expressionManager.CloseSubExpressionsAfterIndex(positionIndex);
        else
            uiExpressions.expressionManager.OpenSubExpression(expressionIndex, stepIndex, positionIndex);

        imageRef.enabled = !imageRef.enabled;
    }

    private void SetItemName([NotNull] Automation automation, TMP_Text tmp_text) => tmp_text.text = automation.Name.Substring(automation.Name.IndexOf(".") + 1);

    private void SetItemName(string text, TMP_Text tmp_text) => tmp_text.text = text;
}
