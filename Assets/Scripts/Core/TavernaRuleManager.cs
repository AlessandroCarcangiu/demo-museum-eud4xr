using System.Collections.Generic;
using UnityEngine;
using ECARules4All_DLL;
using Action = ECARules4All_DLL.Action;


namespace Core
{
    public class TavernaRuleManager : MonoBehaviour
    {
        private void Start()
        {
            InitializeStep1Rule();
            InitializeStep2Rule();
        }

        private void InitializeStep1Rule()
        {
            //regole step1
            GameObject fireplace = GameObject.Find("Fireplace");
            GameObject window = GameObject.Find("Window");
            
            if (fireplace != null && window != null)
            {
                Action fireplaceTrigger = new Action(fireplace, "ignite");
                Action fireplaceTriggerE = new Action(fireplace, "extinguish");


                Action openWindow = new Action(window, "opens");
                Action closeWindow = new Action(window, "closes");

                List<Action> actions = new List<Action> { openWindow };
                List<Action> actionsE = new List<Action> { closeWindow };


                Rule ruleStep1_1 = Rule.TryCreateRule(fireplaceTrigger, actions);
                Rule ruleStep1_2 = Rule.TryCreateRule(fireplaceTriggerE, actionsE);


                if (ruleStep1_1 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep1_1);
                    Debug.Log("[Rule Engine] Regola Step 1 'ignite' aggiunta con successo!");
                }

                if (ruleStep1_2 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep1_2);
                    Debug.Log("[Rule Engine] Regola Step 1 'extinguish' aggiunta con successo!");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore creazione regola.");
                }
            }
            else
            {
                Debug.LogWarning("[Rule Engine] ECAFireplace o ECAWindow non trovati nella scena");
            }
            
            GameObject player = GameObject.Find("Player");
            
            if (fireplace != null && player != null)
            {
                Action playerTrigger = new Action(player, "interacts with", fireplace);
                Action ignite = new Action(fireplace, "ignite");
                
                Action playerNotTrigger = new Action(player, "stops-interacting with", fireplace);
                Action extinguish = new Action(fireplace, "extinguish");
                
                List<Action> actionsI = new List<Action> {ignite };
                List<Action> actionsO = new List<Action> {extinguish };
                
                Rule ruleStep1_3 = Rule.TryCreateRule(playerTrigger, actionsI);
                Rule ruleStep1_4 = Rule.TryCreateRule(playerNotTrigger, actionsO);
                
                if (ruleStep1_3 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep1_3);
                    Debug.Log(
                        "[Rule Engine] Regola Step 1_3 configurata: quando il Player interagisce con il Camino, parte ignite.");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore critico nella creazione della regola step 1_3");
                }
                if (ruleStep1_4 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep1_4);
                    Debug.Log(
                        "[Rule Engine] Regola Step 1_4 configurata: quando il Player smette di interagisce con il Camino, parte extinguish.");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore critico nella creazione della regola step 1_4");
                }
            }
            else
            {
                Debug.LogError(
                    "Errore: Impossibile trovare nella scena gli oggetti 'Player' o 'Fireplace'. Verifica i nomi esatti nella Hierarchy.");
            }
        }

        private void InitializeStep2Rule()
        {
            //rule 2
            GameObject player = GameObject.Find("Player");
            GameObject beerBarrel = GameObject.Find("BeerBarrel");

            if (player != null && beerBarrel != null)
            {
                Action playerTrigger = new Action(player, "interacts with", beerBarrel);
                Action tapBeer = new Action(beerBarrel, "pour beer");

                List<Action> actions = new List<Action> { tapBeer };
                Rule ruleStep2 = Rule.TryCreateRule(playerTrigger, actions);

                if (ruleStep2 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep2);
                    Debug.Log(
                        "[Rule Engine] Regola Step 2 configurata: quando il Player interagisce con il Barile, parte pourBeer.");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore critico nella creazione della regola step 2");
                }
            }
            else
            {
                Debug.LogError(
                    "Errore: Impossibile trovare nella scena gli oggetti 'Player' o 'BeerBarrel'. Verifica i nomi esatti nella Hierarchy.");
            }
        }
        
    }
}