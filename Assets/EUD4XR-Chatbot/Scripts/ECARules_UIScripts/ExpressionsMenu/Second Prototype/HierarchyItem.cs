using ECARules4All_DLL.SmartHomeHubClients;
using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SecondPrototype
{
    public class HierarchyItem : MonoBehaviour
    {
        public TMP_Text contentRef;
        public Image imageRef;
        public Button buttonRef;

        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            if (buttonRef == null) throw new Exception("buttonRef is null");
            if (imageRef == null) throw new Exception("imageRef is null");
        }

        public void OnPrefabCreated(int expressionIndex, int level)
        {
            if (level == 0)
                imageRef.enabled = false;
            else
                imageRef.enabled = true;

            var expression = UIExpressions.Instance.GetExpressionAtIndex(expressionIndex);
            SetExpressionName(expression);

            // Add listener to button so it shows the corresponding expression viewer when pressed
            buttonRef.onClick.AddListener(() => UIExpressions.Instance.expressionManager.UpdateHierarchy(level));
        }

        private void SetExpressionName([NotNull] Expression expression) => contentRef.text = expression.Name;
    }
}