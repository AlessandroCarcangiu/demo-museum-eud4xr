using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCardItem : MonoBehaviour
{
    public TMP_Text contentRef;
    public Button buttonRef;
    public GameObject linkRef;

    private OperatorCard parentCard;
    private bool linkOpen;

    private void Awake()
    {
        // Check for ref not null
        if (contentRef == null) throw new Exception("contentRef is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (linkRef == null) throw new Exception("linkRef is null");
    }

    public void OnPrefabCreated(int itemIndex, OperatorCard parentCard)
    {
        // Save reference to UIExpressions
        var uiExprRef = UIExpressions.Instance;

        this.parentCard = parentCard; // PROVARE A TOGLIERE ATTRIBUTO E VEDERE SE FUNGE

        var cardIndex = parentCard.GetIndex();

        var currentExpression = uiExprRef.GetCurrentExpression();

        linkOpen = false;

        // Handle 'regular' expressions
        if (!parentCard.IsSubExpression())
        {
            var itemContent = currentExpression.Contents[cardIndex];

            // Handle single item in operator card
            if (currentExpression is Sequence && itemContent.Name.StartsWith("automation."))
            {
                // Set item name in operator card
                SetItemName(itemContent, contentRef);

                // Disable link
                linkRef.SetActive(false);

                // Disable item listener, card listener still active
                buttonRef.enabled = false;
            }
            // Handle multiple items in same operator card
            else
            {
                // Handle complex sequence
                if (currentExpression is Sequence)
                {
                    var expressionIndex = uiExprRef.GetExpressionIndexByName(itemContent.Name);
                    var expression = uiExprRef.GetExpressionAtIndex(expressionIndex);

                    var numberOfItems = expression.Contents.Count;

                    // Handle visible steps
                    if (itemIndex < 2 || numberOfItems <= 3)
                    {
                        var expressionContent = expression.Contents[itemIndex];

                        // Set item name in operator card
                        SetItemName(expressionContent, contentRef);

                        // Handle automation item
                        if (expressionContent.Name.StartsWith("automation."))
                        {
                            // Disable link
                            linkRef.SetActive(false);

                            // Set listener to select a specific step
                            buttonRef.onClick.AddListener(() => uiExprRef.expressionManager.GetStepsManagerInstance().ShowStep(parentCard.GetPosition(), itemIndex));
                        }
                        // Handle item containing a subexpression
                        else
                        {
                            var subExpressionIndex = uiExprRef.GetExpressionIndexByName(expressionContent.Name);

                            // Enable link
                            linkRef.SetActive(true);

                            // Set listener to handle opening and closing of sub-expression cards
                            buttonRef.onClick.AddListener(() => HandleSubExpressions(subExpressionIndex, parentCard.GetPosition()));
                        }
                    }
                    // Handle hidden steps
                    else
                    {
                        // Disable link
                        linkRef.SetActive(false);

                        // Show number of hidden steps
                        SetItemName($"+{numberOfItems - itemIndex}...", contentRef);

                        // Set listener to handle hidden steps
                        buttonRef.onClick.AddListener(() => HandleHiddenSteps(expression, itemIndex));
                    }
                }
                // Handle order independence and choice
                else
                {
                    itemContent = currentExpression.Contents[itemIndex];

                    var numberOfItems = currentExpression.Contents.Count;
                    
                    // Handle visible steps
                    if (itemIndex < 2 || numberOfItems <= 3)
                    {
                        // Set item name in operator card
                        SetItemName(itemContent, contentRef);

                        // Handle automation item
                        if (itemContent.Name.StartsWith("automation."))
                        {
                            // Disable link
                            linkRef.SetActive(false);

                            // Set listener to select a specific step
                            buttonRef.onClick.AddListener(() => uiExprRef.expressionManager.GetStepsManagerInstance().ShowStep(parentCard.GetPosition(), itemIndex));
                        }
                        // Handle item containing a subexpression
                        else
                        {
                            var subExpressionIndex = uiExprRef.GetExpressionIndexByName(itemContent.Name);

                            // Enable link
                            linkRef.SetActive(true);

                            // Set listener to handle opening and closing of sub-expression cards
                            buttonRef.onClick.AddListener(() => HandleSubExpressions(subExpressionIndex, parentCard.GetPosition()));
                        }
                    }
                    // Handle hidden steps
                    else
                    {
                        // Disable link
                        linkRef.SetActive(false);

                        // Show number of hidden steps
                        SetItemName($"+{numberOfItems - itemIndex}...", contentRef);

                        // Set listener to handle hidden steps
                        buttonRef.onClick.AddListener(() => HandleHiddenSteps(currentExpression, itemIndex));
                    }
                }
            }
        }
        // Handle subexpressions
        else
        {
            var expression = uiExprRef.GetExpressionAtIndex(cardIndex); 
            var itemContent = expression.Contents[itemIndex];

            var numberOfItems = expression.Contents.Count;

            // Handle visible steps
            if (itemIndex < 2 || numberOfItems <= 3)
            {
                // Set item name in operator card
                SetItemName(itemContent, contentRef);

                // Handle automation item
                if (itemContent.Name.StartsWith("automation."))
                {
                    // Disable link
                    linkRef.SetActive(false);

                    // Set listener to select a specific step
                    buttonRef.onClick.AddListener(() => uiExprRef.expressionManager.GetStepsManagerInstance().ShowStep(parentCard.GetPosition(), itemIndex));
                }
                // Handle item containing a subexpression
                else
                {
                    var subExpressionIndex = uiExprRef.GetExpressionIndexByName(itemContent.Name);

                    // Enable link
                    linkRef.SetActive(true);

                    // Set listener to handle opening and closing of sub-expression cards
                    buttonRef.onClick.AddListener(() => HandleSubExpressions(subExpressionIndex, parentCard.GetPosition()));
                }
            }
            // Handle hidden steps
            else
            {
                // Disable link
                linkRef.SetActive(false);

                // Show number of hidden steps
                SetItemName($"+{numberOfItems - itemIndex}...", contentRef);

                // Set listener to handle hidden steps
                buttonRef.onClick.AddListener(() => HandleHiddenSteps(expression, itemIndex));
            }
        }
    }

    private void HandleHiddenSteps(Expression expression, int firstHiddenStep)
    {
        var stepsManager = UIExpressions.Instance.expressionManager.GetStepsManagerInstance();
        
        // Initialize next step index variable
        int nextStep;
        // Get current vertical index
        var currentStep = stepsManager.GetVerticalIndex();
        // Store index of last step to loop back to first hidden step in case of overflow
        var lastStep = expression.Contents.Count - 1;

        // Skip to first hidden step if current step comes before it, loops back to it if current step is the last
        if (currentStep < firstHiddenStep || currentStep == lastStep)
            nextStep = firstHiddenStep;
        // Go regularly to next step otherwise
        else
            nextStep = currentStep + 1;

        var content = expression.Contents[nextStep];
        var positionIndex = parentCard.GetPosition();

        // Set listener to select a specific step
        stepsManager.ShowStep(positionIndex, nextStep);
    }

    private void HandleSubExpressions(int subExpressionIndex, int positionIndex)
    {
        if (linkOpen)
            UIExpressions.Instance.expressionManager.CloseSubExpressionsAfterIndex(positionIndex);
        else
            UIExpressions.Instance.expressionManager.CreateSubExpressionOperatorCard(subExpressionIndex, positionIndex);

        linkOpen = !linkOpen;
        linkRef.transform.Rotate(0f, 180f, 0f);
    }

    private void SetItemName([NotNull] Automation automation, TMP_Text tmp_text) => tmp_text.text = automation.Name.Substring(automation.Name.IndexOf(".") + 1);

    private void SetItemName(string text, TMP_Text tmp_text) => tmp_text.text = text;
}
