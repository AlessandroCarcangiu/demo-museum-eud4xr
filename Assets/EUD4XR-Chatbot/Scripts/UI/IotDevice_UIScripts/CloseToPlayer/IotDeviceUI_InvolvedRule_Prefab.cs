    using System;
    using System.Diagnostics.CodeAnalysis;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class IotDeviceUI_InvolvedRule_Prefab : MonoBehaviour
    {
        public TMP_Text contentRef;
        // public Button buttonRef;


        private void Awake()
        {
            // Check for ref not null
            if (contentRef == null) throw new Exception("contentRef is null");
            // if (buttonRef == null) throw new Exception("buttonRef is null");
        }

        public void OnPrefabCreated(HASSRule ruleToDisplay, IotDevice iotDevice)
        {
            // Rule ecaObject = ECAObjectUI_Capabilities.Instance.dataSourceRef;
            if (ruleToDisplay == null) throw new Exception("ruleToDisplay is null");
            if (iotDevice == null) throw new Exception("objectInvolved is null");

            SetBody(ruleToDisplay);
        
            // buttonRef.OnClicked.RemoveAllListeners();
            // buttonRef.onClick.AddListener(() =>
            // {
            //     ECAUI_UIManager.Instance.Intention_EditRuleFromUI(ruleToDisplay);
            //     ECAObjectUI_InvolvedRule.Instance.T_GoToRules.SetActive(true); // TODO UI. E' da rivedere
            // });
        }

        private void SetBody([NotNull] HASSRule rule) => contentRef.text = "TODO";  //RuleUtils.FormatRuleLabel(rule); // TODO UI. E' da rivedere
    }
