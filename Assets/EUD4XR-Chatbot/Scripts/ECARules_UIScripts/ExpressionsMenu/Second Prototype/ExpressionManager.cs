using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class ExpressionManager : MonoBehaviour
    {
        public TMP_Text uiExpressionName;
        public Button uiGoBack;
        public GameObject uiHierarchy;
        public GameObject uiHierachyItemPrefab;
        public GameObject uiExpressionContainerMask;
        public GameObject uiExpressionContainer;
        public GameObject uiExpressionDescriptorPrefab;

        private List<ExpressionDescriptor> expressionDescriptors;
        private int currentExpressionIndex;

        private void Awake()
        {
            if (uiGoBack == null) throw new Exception("uiGoBack is null");
            if (uiHierarchy == null) throw new Exception("uiHierarchy is null");
            if (uiExpressionContainerMask == null) throw new Exception("uiExpressionContainerMask is null");
            if (uiExpressionContainer == null) throw new Exception("uiExpressionContainer is null");

            // Init list
            expressionDescriptors = new List<ExpressionDescriptor>();
        }

        public void ShowExpression(int index)
        {
            currentExpressionIndex = index;
            var currentExpression = UIExpressions.Instance.GetExpressionAtIndex(currentExpressionIndex);

            CreateExpressionDescriptor(index);

            // Set root in the hierarchy
            /*if (currentExpression is Sequence)
            {
                foreach (var item in currentExpression.Contents)
                {
                    if (item.Name.StartsWith("automation."))
                        continue;
                    

                }
            }*/

        }

        public void CreateExpressionDescriptor(int expressionIndex)
        {
            Debug.Log("creating expression descriptor for expression: " + expressionIndex);
            // Instantiate expression descriptor
            var expressionDescriptor = Instantiate(uiExpressionDescriptorPrefab, uiExpressionContainer.transform).GetComponent<ExpressionDescriptor>();
            expressionDescriptor.OnPrefabCreated(expressionIndex);
            expressionDescriptors.Add(expressionDescriptor);
        }

        public void ClearExpressionData()
        {
            // Clear expression descriptors
            foreach (Transform child in uiExpressionContainer.transform)
                Destroy(child.gameObject);
            expressionDescriptors.Clear();
        }

        public int GetCurrentExpressionIndex() => currentExpressionIndex;
    }
}
