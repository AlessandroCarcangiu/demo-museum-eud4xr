using ECARules4All_DLL.Utils;
using System;
using UnityEngine;

public class ExpressionsPanel : Singleton<ExpressionsPanel>
{
    public UIExpressions uiExpressions;
    public GameObject uiExpressionItemPrefab;
    public GameObject uiExpressionsList;
    public GameObject uiExpressionsListContent;
    public GameObject uiNoExpressions;
    
    private void Awake()
    {
        // Check for ref not null
        if (uiExpressions == null) throw new Exception("uiExpressions is null");
        if (uiExpressionItemPrefab == null) throw new Exception("uiExpressionItemPrefab is null");
        if (uiExpressionsList == null) throw new Exception("uiExpressionsList is null");
        if (uiExpressionsListContent == null) throw new Exception("uiExpressionsListContent is null");
        if (uiNoExpressions == null) throw new Exception("uiNoExpressions is null");
    }

    public void UpdateMenu()
    {
        var expressions = uiExpressions.GetExpressions();

        // No expressions found
        if (expressions.Count == 0)
        {
            // Clears panel
            uiExpressionsList.SetActive(false);
            // Shows 'no expressions' message
            uiNoExpressions.SetActive(true);
        }
        // At least one expression found
        else
        {
            // Creates menu listing all expressions
            for (int i = 0; i < expressions.Count; i++)
            {
                // Instantiate the prefab and add it to the list
                var expressionItem = Instantiate(uiExpressionItemPrefab, uiExpressionsListContent.transform).GetComponent<B_Expression_Prefab>();
                expressionItem.OnPrefabCreated(uiExpressions, i);
            }

            // Clears panel
            uiNoExpressions.SetActive(false);
            // Sets up panel
            uiExpressionsList.SetActive(true);
        }
    }
}
