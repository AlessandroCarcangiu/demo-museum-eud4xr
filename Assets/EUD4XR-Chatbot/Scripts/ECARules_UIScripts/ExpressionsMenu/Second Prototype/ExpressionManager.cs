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
        private float expressionDescriptorWidth;

        private void Awake()
        {
            if (uiGoBack == null) throw new Exception("uiGoBack is null");
            if (uiHierarchy == null) throw new Exception("uiHierarchy is null");
            if (uiExpressionContainerMask == null) throw new Exception("uiExpressionContainerMask is null");
            if (uiExpressionContainer == null) throw new Exception("uiExpressionContainer is null");
            
            expressionDescriptorWidth = uiExpressionDescriptorPrefab.GetComponent<RectTransform>().rect.width;

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
            // Instantiate expression descriptor
            var expressionDescriptor = Instantiate(uiExpressionDescriptorPrefab, uiExpressionContainer.transform).GetComponent<ExpressionDescriptor>();
            expressionDescriptor.OnPrefabCreated(expressionIndex);
            expressionDescriptors.Add(expressionDescriptor);
            CheckExpressionDescriptorsSize();
        }

        private void CheckExpressionDescriptorsSize()
        {
            // Second expression descriptor was just added
            if (expressionDescriptors.Count == 2)
            {
                var maskRect = uiExpressionContainerMask.GetComponent<RectTransform>();
                maskRect.sizeDelta = new Vector2(expressionDescriptorWidth * 2, maskRect.sizeDelta.y);

                var rect = gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x + expressionDescriptorWidth, rect.sizeDelta.y);
                
                UIExpressions.Instance.UpdatePlateSize(true);
            }
            else if (expressionDescriptors.Count > 2)
                StartCoroutine(UIExpressions.Instance.MoveContainer(uiExpressionContainer.GetComponent<RectTransform>(), -expressionDescriptorWidth, 0.2f));
        }

        public void ClearExpressionData()
        {
            // Set container and mask data to default
            if (expressionDescriptors.Count > 1)
            {
                var maskRect = uiExpressionContainerMask.GetComponent<RectTransform>();
                maskRect.sizeDelta = new Vector2(expressionDescriptorWidth, maskRect.sizeDelta.y);

                var rect = gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x - expressionDescriptorWidth, rect.sizeDelta.y);

                if (expressionDescriptors.Count > 2)
                    uiExpressionContainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            // Clear expression descriptors
            foreach (Transform child in uiExpressionContainer.transform)
                Destroy(child.gameObject);
            expressionDescriptors.Clear();
        }

        public int GetCurrentExpressionIndex() => currentExpressionIndex;
    }
}
