using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorCard : MonoBehaviour
{
    public GameObject uiItemPrefab;
    public GameObject uiItems;
    public TMP_Text uiOperatorName;
    public Button buttonRef;
    public Image backgroundRef;
    public Image uiIconLogoRef;
    public TMP_Text uiIconTextRef;
    public Sprite orderIcon;
    public Sprite choiceIcon;

    private int index;
    private bool isSubExpression;
    private OperatorCardItem[] operatorCardItems;

    private void Awake()
    {
        // Check for ref not null
        if (uiItemPrefab == null) throw new Exception("uiItemPrefab is null");
        if (uiItems == null) throw new Exception("uiItems is null");
        if (uiOperatorName == null) throw new Exception("uiOperatorName is null");
        if (buttonRef == null) throw new Exception("buttonRef is null");
        if (backgroundRef == null) throw new Exception("backgroundRef is null");
        if (uiIconLogoRef == null) throw new Exception("uiIconLogoRef is null");
        if (uiIconTextRef == null) throw new Exception("uiIconTextRef is null");

        // Init list
        operatorCardItems = new OperatorCardItem[1];
    }

    public void OnPrefabCreated(int index, bool isSubExpression)
    {
        this.index = index;
        this.isSubExpression = isSubExpression;

        // Save local reference to UIExpressions
        var uiExprRef = UIExpressions.Instance;

        if (!isSubExpression)
        {
            var expression = uiExprRef.GetCurrentExpression();

            // Set text in operator icon above card
            SetOperatorIconText(expression);

            // Set operator type in operator card
            SetOperatorName(expression, index);

            // Set operator card color
            SetColor(uiExprRef.ExpressionToColor(expression, index));

            var content = expression.Contents[index];

            // If operator is an automation the operator card contains one item
            if (expression is Sequence && content.Name.StartsWith("automation."))
            {
                // Set logo in operator icon above card
                SetOperatorIconLogo(expression);
                // Create automation item
                CreateOperatorCardItem(0);
            }
            // If operator is an expression the operator card contains all its items
            else
            {
                // Handle complex sequences
                if (expression is Sequence)
                {
                    // Set logo in operator icon above card
                    SetOperatorIconLogo(uiExprRef.GetExpressionByName(content.Name));
                    // Create expression items
                    HandleMultipleItems(uiExprRef.GetExpressionIndexByName(content.Name));
                }
                // Handle order independence and choice
                else
                {
                    // Set logo in operator icon above card
                    SetOperatorIconLogo(expression);
                    // Create expression items
                    HandleMultipleItems(uiExprRef.expressionManager.GetCurrentExpressionIndex());
                }
            }
        }
        else
        {
            var subExpression = uiExprRef.GetExpressionAtIndex(index);

            // Set text in operator icon above card
            SetOperatorIconText(subExpression);
            // Set logo in operator icon above card
            SetOperatorIconLogo(subExpression);

            // Set operator type in operator card
            SetOperatorName(subExpression, 0);

            // Set operator card color
            SetColor(uiExprRef.ExpressionToColor(subExpression, 0));

            HandleMultipleItems(index);
        }

        // Set listener to select a specific step
        buttonRef.onClick.AddListener(() => uiExprRef.expressionManager.GetStepsManagerInstance().ShowStep(transform.GetSiblingIndex(), 0));
    }

    private void HandleMultipleItems(int expressionIndex)
    {
        var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);
        var expressionItems = expression.Contents.Count;
        operatorCardItems = new OperatorCardItem[expressionItems];
        
        // Maximum 3 items are shown at once
        if (expressionItems > 3)
        {
            // Enable scroll rect and disable vertical layout height control
            GetComponentInChildren<ScrollRect>().enabled = true;
            uiItems.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
            uiItems.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // Create expression items
        for (int i = 0; i < expressionItems; i++)
            CreateOperatorCardItem(i);
    }

    private void CreateOperatorCardItem(int itemIndex)
    {
        // Instantiate the prefab and add it to the operator card
        var operatorCardItem = Instantiate(uiItemPrefab, uiItems.transform).GetComponent<OperatorCardItem>();
        operatorCardItem.OnPrefabCreated(itemIndex, this);
        // Add item reference to the list
        operatorCardItems[itemIndex] = operatorCardItem;
    }

    public bool IsSubExpression()
    {
        return isSubExpression;
    }

    public int GetIndex() => index;

    public int GetPosition() => transform.GetSiblingIndex();
    
    private void SetOperatorIconText([NotNull] Expression expression)
    {
        if (expression is Sequence)
        {
            uiIconTextRef.gameObject.SetActive(true);
            uiIconTextRef.text = index.ToString();
        }
        else
            uiIconTextRef.gameObject.SetActive(false);
    }

    private void SetOperatorIconLogo([NotNull] Expression expression)
    {
        if (expression is Sequence)
            uiIconLogoRef.gameObject.SetActive(false);
        else
        { 
            uiIconLogoRef.gameObject.SetActive(true);
            uiIconLogoRef.sprite = expression is Order ? orderIcon : choiceIcon;
        }
    }

    private void SetOperatorName([NotNull] Expression expression, int index)
    {
        if (expression is Order)
            SetOperatorName("ORDER INDEPENDENCE");

        else if (expression is Choice)
            SetOperatorName("SCELTA");

        else
            SetOperatorName(expression.Contents[index]);
    }

    private void SetOperatorName([NotNull] Automation automation)
    {
        var name = automation.Name;

        if (name.StartsWith("automation."))
            SetOperatorName("AUTOMAZIONE");

        else if (name.StartsWith("order."))
            SetOperatorName("ORDER INDEPENDENCE");

        else if (name.StartsWith("choice."))
            SetOperatorName("SCELTA");
    }

    private void SetOperatorName(string operatorName) => uiOperatorName.text = operatorName;

    private void SetColor([NotNull] Color color) => backgroundRef.color = color;
}
