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
    public Image imageRef;

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
        if (imageRef == null) throw new Exception("imageRef is null");

        // Init list
        operatorCardItems = new OperatorCardItem[1];
    }

    public void OnPrefabCreated(int index, bool isSubExpression)
    {
        this.index = index;
        this.isSubExpression = isSubExpression;

        // Save reference to UIExpressions
        var uiExprRef = UIExpressions.Instance;

        if (!isSubExpression)
        {
            var expression = uiExprRef.GetCurrentExpression();

            // Set operator type in operator card
            SetOperatorName(expression, index);

            // Set operator card color
            SetColor(uiExprRef.ExpressionToColor(expression, index));

            // If operator is an automation the operator card contains one item
            if (expression is Sequence && expression.Contents[index].Name.StartsWith("automation."))
                CreateOperatorCardItem(0);
            // If operator is an expression the operator card contains all its items
            else
            {
                // Handle complex sequences
                if (expression is Sequence)
                    HandleMultipleItems(uiExprRef.GetExpressionIndexByName(expression.Contents[index].Name));
                // Handle order independence and choice
                else
                    HandleMultipleItems(uiExprRef.expressionManager.GetCurrentExpressionIndex());
            }
        }
        else
        {
            var subexpression = UIExpressions.Instance.GetExpressionAtIndex(index);

            // Set operator type in operator card
            SetOperatorName(subexpression, 0);

            // Set operator card color
            SetColor(uiExprRef.ExpressionToColor(subexpression, 0));

            HandleMultipleItems(index);
        }

        // Set listener to select a specific step
        buttonRef.onClick.AddListener(() => uiExprRef.expressionManager.GetStepsManagerInstance().ShowStep(transform.GetSiblingIndex(), 0));
    }

    private void HandleMultipleItems(int expressionIndex)
    {
        var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);

        var expressionsCount = expression.Contents.Count;

        if (expressionsCount > 3)
            operatorCardItems = new OperatorCardItem[3];
        else
            operatorCardItems = new OperatorCardItem[expressionsCount];
        
        for (int i = 0; i < operatorCardItems.Length; i++)
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

    private void SetColor([NotNull] Color color) => imageRef.color = color;
}
