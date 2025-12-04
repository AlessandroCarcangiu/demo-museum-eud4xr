using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Collections.Generic;
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
        public GameObject uiExpressionViewerPrefab;

        private List<ExpressionViewer> expressionViewers;
        private int currentExpressionIndex;
        private float expressionViewerWidth;

        private void Awake()
        {
            if (uiGoBack == null) throw new Exception("uiGoBack is null");
            if (uiHierarchy == null) throw new Exception("uiHierarchy is null");
            if (uiExpressionContainerMask == null) throw new Exception("uiExpressionContainerMask is null");
            if (uiExpressionContainer == null) throw new Exception("uiExpressionContainer is null");
            
            expressionViewerWidth = uiExpressionViewerPrefab.GetComponent<RectTransform>().rect.width;

            // Init list
            expressionViewers = new List<ExpressionViewer>();
        }

        public void ShowExpression(int index)
        {
            currentExpressionIndex = index;
            var currentExpression = UIExpressions.Instance.GetExpressionAtIndex(currentExpressionIndex);

            // Set root in the hierarchy
            /*if (currentExpression is Sequence)
            {
                foreach (var item in currentExpression.Contents)
                {
                    if (item.Name.StartsWith("automation."))
                        continue;
                    

                }
            }*/

            CreateExpressionViewer(index);
        }

        public void CreateExpressionViewer(int expressionIndex)
        {
            // Check if expression viewer already exists
            if (CheckDuplicates(expressionIndex))
                return;

            // Instantiate expression viewer
            var expressionViewer = Instantiate(uiExpressionViewerPrefab, uiExpressionContainer.transform).GetComponent<ExpressionViewer>();
            expressionViewer.OnPrefabCreated(expressionIndex);
            expressionViewers.Add(expressionViewer);
            CheckExpressionViewersSize();
        }

        private bool CheckDuplicates(int expressionIndex)
        {
            foreach (var viewer in expressionViewers)
                if (viewer.GetExpressionIndex() == expressionIndex)
                    return true;
            
            return false;
        }

        private void CheckExpressionViewersSize()
        {
            // Second expression viewer was just added
            if (expressionViewers.Count == 2)
            {
                var maskRect = uiExpressionContainerMask.GetComponent<RectTransform>();
                maskRect.sizeDelta = new Vector2(expressionViewerWidth * 2, maskRect.sizeDelta.y);

                var rect = gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x + expressionViewerWidth, rect.sizeDelta.y);
                
                UIExpressions.Instance.UpdatePlateSize(true);
            }
            else if (expressionViewers.Count > 2)
                StartCoroutine(UIExpressions.Instance.MoveContainer(uiExpressionContainer.GetComponent<RectTransform>(), -expressionViewerWidth, 0.2f));
        }

        public void ClearExpressionData()
        {
            // Set container and mask data to default
            if (expressionViewers.Count > 1)
            {
                var maskRect = uiExpressionContainerMask.GetComponent<RectTransform>();
                maskRect.sizeDelta = new Vector2(expressionViewerWidth, maskRect.sizeDelta.y);

                var rect = gameObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x - expressionViewerWidth, rect.sizeDelta.y);

                if (expressionViewers.Count > 2)
                    uiExpressionContainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            // Clear expression viewers
            foreach (Transform child in uiExpressionContainer.transform)
                Destroy(child.gameObject);
            expressionViewers.Clear();
        }

        public int GetCurrentExpressionIndex() => currentExpressionIndex;
    }
}
