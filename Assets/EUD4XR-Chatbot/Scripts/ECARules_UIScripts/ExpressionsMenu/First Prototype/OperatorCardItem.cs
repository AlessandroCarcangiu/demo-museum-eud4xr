using System;
using System.Diagnostics.CodeAnalysis;
using ECARules4All_DLL.SmartHomeHubClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstPrototype
{
    public class OperatorCardItem : MonoBehaviour
    {
        public TMP_Text contentRef;
        public Button buttonRef;
        public GameObject linkRef;

        private bool isLinkOpen;

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
            if (linkRef == null) throw new Exception("linkRef is null");
        }

        public void OnPrefabCreated(int itemIndex, OperatorCard parentCard)
        {
            // Save local reference to UIExpressions
            var uiExprRef = UIExpressions.Instance;

            var cardIndex = parentCard.GetIndex();

            var currentExpression = uiExprRef.GetCurrentExpression();

            isLinkOpen = false;

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
                            buttonRef.onClick.AddListener(() => HandleSubExpressions(subExpressionIndex, parentCard));
                        }
                    }
                    // Handle order independence and choice
                    else
                    {
                        itemContent = currentExpression.Contents[itemIndex];

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
                            buttonRef.onClick.AddListener(() => HandleSubExpressions(subExpressionIndex, parentCard));
                        }
                    }
                }
            }
            // Handle subexpressions
            else
            {
                var expression = uiExprRef.GetExpressionAtIndex(cardIndex); 
                var itemContent = expression.Contents[itemIndex];
            
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
                    buttonRef.onClick.AddListener(() => HandleSubExpressions(subExpressionIndex, parentCard));
                }
            }
        }

        private void HandleSubExpressions(int subExpressionIndex, OperatorCard parentCard)
        {
            var positionIndex = parentCard.GetPosition();

            // Close sub-expressions if link is open
            if (isLinkOpen)
                UIExpressions.Instance.expressionManager.CloseSubExpressionsAfterIndex(positionIndex);
            // Open sub-expression if link is closed
            else
            {
                // Check for other sub-expressions depending from this card and closes them
                foreach (var item in parentCard.GetItems())
                {
                    if (item.IsLinkOpen())
                    {
                        UIExpressions.Instance.expressionManager.CloseSubExpressionsAfterIndex(positionIndex);
                        item.HandleLink();
                        break;
                    }
                }
                // Create sub-expression card
                UIExpressions.Instance.expressionManager.CreateSubExpressionOperatorCard(subExpressionIndex, positionIndex);
            }

            // Update link icon
            HandleLink();
        }

        public void HandleLink()
        {
            isLinkOpen = !isLinkOpen;
            linkRef.transform.Rotate(0f, 180f, 0f);
        }

        public bool IsLinkOpen() => isLinkOpen;

        private void SetItemName([NotNull] Automation automation, TMP_Text tmp_text) => tmp_text.text = automation.Name.Substring(automation.Name.IndexOf(".") + 1);
    }
}
