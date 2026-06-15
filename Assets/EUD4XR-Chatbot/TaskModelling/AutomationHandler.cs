using ECARules4All_DLL.SmartHomeHubClients;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using System.Linq;

namespace EUD4XR_Chatbot.TaskModelling
{
    public class AutomationHandler
    {
	    private const string pathTriggerCard = "Container/TriggerSection/Trigger";
	    private const string pathConditionsSectionCard = "Container/ConditionsSection";
	    private const string pathConditionsCard = pathConditionsSectionCard + "/Conditions";
	    private const string pathActionsCard = "Container/ActionsSection/Actions";
	    
        public static void ShowAutomation(GameObject card, string automation)
        {
	        string trigger;
	        string conditions = "";
	        string actions;
	        var automationParts = automation.Split("then ");
	        
	        // actions
	        actions = automationParts[1];
	        var triggerConditionParts = automationParts[0].Split("if ");
	        if(triggerConditionParts.Length == 2)
	        {
		        trigger = automationParts[0];
		        conditions = automationParts[1];
	        }
	        else
	        {
		        trigger = automationParts[0];
	        }
	        
			// trigger
			GameObject triggerContainer = card.transform.Find(pathTriggerCard)?.gameObject;
			ShowAction(triggerContainer, "WHEN " + trigger.Replace("when ", ""));
			// conditions
			if (!string.IsNullOrEmpty(conditions))
			{
				GameObject conditionsContainer = card.transform.Find(pathConditionsCard)?.gameObject;
				ShowAction(conditionsContainer, "IF " + conditions);
			}
			else
			{
				GameObject conditionsSection = card.transform.Find(pathConditionsSectionCard)?.gameObject;
				if (conditionsSection)
				{
					conditionsSection.SetActive(false);
				}
			}
			// actions
			GameObject actionsContainer = card.transform.Find(pathActionsCard)?.gameObject;
			ShowAction(actionsContainer, "THEN " + actions);
		}

		private static void ShowAction(GameObject card, string act)
		{
			card.GetComponent<TextMeshProUGUI>().text = act;
		}
    }
}