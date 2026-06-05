using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstPrototype
{
    public class ExpressionManager : MonoBehaviour
    {
        public GameObject uiOperatorCardPrefab;
        public GameObject uiStepsManagerPrefab;
        public TMP_Text uiExpressionName;
        public Button uiGoBack;
        public GameObject uiExpressionGrid;

        private int currentExpressionIndex;
        private List<OperatorCard> operatorCards;
        private StepsManager stepsManagerInstance;

        private void Awake()
        {
            // Check for ref not null
            if (uiOperatorCardPrefab == null) throw new Exception("uiOperatorCardPrefab is null");
            if (uiStepsManagerPrefab == null) throw new Exception("uiStepsManagerPrefab is null");
            if (uiExpressionName == null) throw new Exception("uiExpressionName is null");
            if (uiGoBack == null) throw new Exception("uiGoBack is null");
            if (uiExpressionGrid == null) throw new Exception("uiExpressionGrid is null");

            // Init list
            operatorCards = new List<OperatorCard>();
        }

        public void ShowExpression(int index)
        {
            currentExpressionIndex = index;
            var currentExpression = UIExpressions.Instance.GetExpressionAtIndex(currentExpressionIndex);

            // Set expression name in the nav bar
            SetExpressionName(currentExpression);

            // Instantiate steps manager prefab
            stepsManagerInstance = Instantiate(uiStepsManagerPrefab, transform).GetComponent<StepsManager>();

            // If expression is a sequence then an operator card for each step is instantiated
            if (currentExpression is Sequence)
                for (int i = 0; i < currentExpression.Contents.Count; i++)
                    CreateOperatorCard(i);
            // If expression is not a sequence then a single operator card is instantiated
            else
                CreateOperatorCard(0);

            stepsManagerInstance.OnPrefabCreated();
        }

        private void CreateOperatorCard(int stepIndex)
        {
            // Instantiate the prefab and add it to the grid
            var operatorCard = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
            operatorCard.OnPrefabCreated(stepIndex, false);
            // Add operator card reference to the list
            operatorCards.Add(operatorCard);
        }

        public void CreateSubExpressionOperatorCard(int subExpressionIndex, int positionIndex)
        {
            var operatorCard = Instantiate(uiOperatorCardPrefab, uiExpressionGrid.transform).GetComponent<OperatorCard>();
            operatorCard.transform.SetSiblingIndex(positionIndex + 1);
            operatorCard.OnPrefabCreated(subExpressionIndex, true);

            if (positionIndex == operatorCards.Count - 1)
                operatorCards.Add(operatorCard);
            else
                operatorCards.Insert(positionIndex + 1, operatorCard);
        }

        public void CloseSubExpressionsAfterIndex(int index)
        {
            // Count how many sub-expressions depend on the pressed card
            var openedSubExpressions = CountCardSubExpressions(index);

            // Get index of last sub-expression card to be deleted
            var lastSubExpressionIndex = index + openedSubExpressions;

            // Get current step in steps manager
            var currentHorizontalIndex = stepsManagerInstance.GetHorizontalIndex();

            // Remove all sub-expressions depending on the pressed card
            for (int i = lastSubExpressionIndex; i > index; i--)
            {
                // Destroy sub-expression card and remove it from list
                Destroy(operatorCards[i].gameObject);
                operatorCards.RemoveAt(i);
            }

            // Change step to pressed card if current step is deleted
            if (currentHorizontalIndex > index && currentHorizontalIndex <= lastSubExpressionIndex)
                stepsManagerInstance.ShowStep(index, 0);
        }

        private int CountCardSubExpressions(int cardIndex)
        {
            var count = 0;

            for (int i = cardIndex + 1; i < operatorCards.Count; i++)
            {
                if (operatorCards[i].IsSubExpression())
                    count++;
                else
                    break;
            }

            return count;
        }

        public void ClearExpressionData()
        {
            // Clear operator cards
            foreach (Transform child in uiExpressionGrid.transform)
                Destroy(child.gameObject);
            operatorCards.Clear();

            // Clear steps manager
            if (stepsManagerInstance != null)
                Destroy(stepsManagerInstance.gameObject);
        }

        public StepsManager GetStepsManagerInstance() => stepsManagerInstance;

        public int GetCurrentExpressionIndex() => currentExpressionIndex;

        public OperatorCard GetOperatorCard(int index) => operatorCards[index];

        public int GetOpenCards() => operatorCards.Count;

        private void SetExpressionName([NotNull] Expression expression) => uiExpressionName.text = expression.Name.Replace("_", " ");
    }
}
