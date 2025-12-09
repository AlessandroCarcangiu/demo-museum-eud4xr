using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class ExpressionManager : MonoBehaviour
    {
        public Button uiGoBack;
        public GameObject uiHierarchyContent;
        public GameObject uiHierarchyItemPrefab;
        public GameObject uiExpressionContainerMask;
        public GameObject uiExpressionContainer;
        public GameObject uiExpressionViewerPrefab;

        private RectTransform rect;
        private RectTransform maskRect;

        private float defaultWidth;
        private float expressionViewerWidth;

        private List<int> expressionIndexes;
        private int currentLevel;

        private void Awake()
        {
            if (uiGoBack == null) throw new Exception("uiGoBack is null");
            if (uiHierarchyContent == null) throw new Exception("uiHierarchyContent is null");
            if (uiExpressionContainerMask == null) throw new Exception("uiExpressionContainerMask is null");
            if (uiExpressionContainer == null) throw new Exception("uiExpressionContainer is null");
            if ((rect = GetComponent<RectTransform>()) == null) throw new Exception("No RectTransform found in this gameobject");
            if ((maskRect = uiExpressionContainerMask.GetComponent<RectTransform>()) == null) throw new Exception("No RectTransform found in container mask");

            defaultWidth = rect.rect.width;
            expressionViewerWidth = maskRect.rect.width;
            currentLevel = 0;

            // Init list
            expressionIndexes = new List<int>();
        }
        
        public void OpenExpression(int expressionIndex)
        {
            CreateHierarchyItem(expressionIndex, expressionIndexes.Count);

            CreateExpressionViewer(expressionIndex);

            expressionIndexes.Add(expressionIndex);
        }
        
        public void CreateHierarchyItem(int expressionIndex, int hierarchyLevel)
        {
            // Instantiate hierarchy item
            var hierarchyItem = Instantiate(uiHierarchyItemPrefab, uiHierarchyContent.transform).GetComponent<HierarchyItem>();
            hierarchyItem.OnPrefabCreated(expressionIndex, hierarchyLevel);
        }
        
        public void CreateExpressionViewer(int expressionIndex)
        {
            // Instantiate expression viewer
            var expressionViewer = Instantiate(uiExpressionViewerPrefab, uiExpressionContainer.transform).GetComponent<ExpressionViewer>();
            expressionViewer.OnPrefabCreated(expressionIndex);

            UpdateExpressionViewers();
        }

        private void UpdateExpressionViewers()
        {
            // Second expression viewer was just added
            if (expressionIndexes.Count == 1)
                ActivateTwoViewersVisualization();
            // Hide oldest viewer to only show the last two
            else if (expressionIndexes.Count > 1)
                SwitchViewer(true, 1);
        }

        private void ActivateTwoViewersVisualization()
        {
            // Enlarge canvas to fit two viewers
            UpdateCanvasSize(true);
            currentLevel = 1;
        }

        public void SwitchViewer(bool goForward, int steps)
        {
            float deltaX;
            if (goForward)
            {
                currentLevel += steps;
                deltaX = -expressionViewerWidth;
            }
            else
            {
                currentLevel -= steps;
                deltaX = expressionViewerWidth;
            }

            if (currentLevel == 0)
            {
                // Set canvas size to default
                UpdateCanvasSize(false);

                steps -= 1;
            }

            StartCoroutine(UIExpressions.Instance.MoveContainer(uiExpressionContainer.GetComponent<RectTransform>(), deltaX * steps, 0.2f));
        }

        public void UpdateHierarchy(int level)
        {
            // Do nothing if expression is already shown
            if (level == currentLevel)
                return;

            if (level > currentLevel)
            {
                if (currentLevel == 0)
                {
                    ActivateTwoViewersVisualization();
                    if (level == 1)
                        return;
                }
                SwitchViewer(true, level - currentLevel);
            }
            else
                SwitchViewer(false, -(level - currentLevel));
        }

        public bool CheckDuplicates(int expressionIndex)
        {
            foreach (var index in expressionIndexes)
                if (index == expressionIndex)
                    return true;

            return false;
        }

        public void CheckSubExpressions(int expressionIndex)
        {
            var openExpressions = expressionIndexes.Count - 1;
            var startIndex = currentLevel == 0 ? currentLevel : currentLevel - 1;

            for (int i = startIndex; i <= currentLevel; i++)
                if (expressionIndexes[i] == expressionIndex)
                    if (i < openExpressions)
                        CloseSubExpressionsAfterIndex(i);
        }

        private void CloseSubExpressionsAfterIndex(int index)
        {
            for (int i = expressionIndexes.Count - 1; i > index; i--)
            {
                Destroy(uiHierarchyContent.transform.GetChild(i).gameObject);
                Destroy(uiExpressionContainer.transform.GetChild(i).gameObject);
                expressionIndexes.RemoveAt(i);
            }

            currentLevel = index;
        }

        public void ClearExpressionData()
        {
            if (expressionIndexes.Count > 1)
            {
                // Set canvas size to default
                UpdateCanvasSize(false);

                // Reset container position
                if (expressionIndexes.Count > 2)
                    uiExpressionContainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            // Clear hierarchy items
            foreach (Transform child in uiHierarchyContent.transform)
                Destroy(child.gameObject);

            // Clear expression viewers
            foreach (Transform child in uiExpressionContainer.transform)
                Destroy(child.gameObject);

            // Clear list
            expressionIndexes.Clear();
        }

        private void UpdateCanvasSize(bool enlarge)
        {
            if (enlarge)
            {
                // Set container and mask size to handle two viewers
                maskRect.sizeDelta = new Vector2(expressionViewerWidth * 2, maskRect.sizeDelta.y);
                rect.sizeDelta = new Vector2(defaultWidth + expressionViewerWidth, rect.sizeDelta.y);
            }
            else
            {
                // Set container and mask size to default
                maskRect.sizeDelta = new Vector2(expressionViewerWidth, maskRect.sizeDelta.y);
                rect.sizeDelta = new Vector2(defaultWidth, rect.sizeDelta.y);
            }

            // Notify UIExpressions to update plate size
            UIExpressions.Instance.UpdatePlateSize(enlarge);
        }

        public int GetExpressionIndexesCount() => expressionIndexes.Count;
    }
}
