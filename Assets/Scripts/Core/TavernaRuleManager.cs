using System.Collections.Generic;
using UnityEngine;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using NUnit.Framework;
using Action = ECARules4All_DLL.Action;


namespace Core
{
    public class TavernaRuleManager : MonoBehaviour
    {
        private void Start()
        {
            InitializeStep1Rule();
            InitializeStep2Rule();
            InitializeStep3Rule();
            InitializeStep4Rule();
            InitializeStep5Rule();
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
            GameObject candle = GameObject.Find("Candle");
            
            if (fireplace != null && player != null)
            {
                Action playerTrigger = new Action(player, "interacts with", fireplace);
                Action ignite = new Action(fireplace, "ignite");
                Action lightsUp = new Action(candle, "lights up");
                
                Action playerNotTrigger = new Action(player, "stops-interacting with", fireplace);
                Action extinguish = new Action(fireplace, "extinguish");
                
                List<Action> actionsI = new List<Action> {ignite , lightsUp};
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

        private void InitializeStep3Rule()
        {
            /*GameObject player = GameObject.Find("Player");
            GameObject buttonLights = GameObject.Find("ButtonLights");
            GameObject chandelier = GameObject.Find("Chandelier");

            if (player != null && chandelier != null && buttonLights != null)
            {
                Action playerTriggerI = new Action(player, "interacts with", buttonLights);
                Action playerTriggerO = new Action(player, "stops-interacting with", buttonLights);
                
                Action setBrightnessI = new Action(chandelier, "set brightness 100");
                Action setBrightnessO = new Action(chandelier, "set brightness 0");
                
                List<Action> actionsI = new List<Action> { setBrightnessI };
                List<Action> actionsO = new List<Action> { setBrightnessO };
                
                Rule ruleStep3_1 = Rule.TryCreateRule(playerTriggerI, actionsI);
                Rule ruleStep3_2 = Rule.TryCreateRule(playerTriggerO, actionsO);
                
                if (ruleStep3_1 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep3_1);
                    Debug.Log("[Rule Engine] Regola Step 3_1 configurata");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore critico nella creazione della regola step 3_1");
                }
                if (ruleStep3_2 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep3_2);
                    Debug.Log("[Rule Engine] Regola Step 3_2 configurata");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore critico nella creazione della regola step 3_2");
                }
            }
            else
            {
                Debug.LogError("[Rule Engine] Non è stato possibile trovare gli oggetti per la regola 3 nella scena");
            }
        }*/
            
            GameObject player = GameObject.Find("Player");
            GameObject buttonLights = GameObject.Find("ButtonLights");
            GameObject chandelier = GameObject.Find("Chandelier");

            if (player != null && chandelier != null && buttonLights != null)
            {
                Action playerTriggerI = new Action(player, "interacts with", buttonLights);
                
                Action setBrightnessI = new Action(chandelier, "invert state");
                
                List<Action> actionsI = new List<Action> { setBrightnessI };
                
                Rule ruleStep3 = Rule.TryCreateRule(playerTriggerI, actionsI);
                
                if (ruleStep3 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep3);
                    Debug.Log("[Rule Engine] Regola Step 3 configurata");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore critico nella creazione della regola step 3");
                }
            }
            else
            {
                Debug.LogError("[Rule Engine] Non è stato possibile trovare gli oggetti per la regola 3 nella scena");
            }
        }

        private void InitializeStep4Rule()
        {
            GameObject player = GameObject.Find("Player");
            GameObject key = GameObject.Find("Key");
            GameObject door = GameObject.Find("Door");

            if (player != null && door != null && key != null)
            {
                Action playerTrigger = new Action(player, "interacts with", door); 
                //Action keyTrigger = new Action(key, "interacts with", door);
                Action openDoor = new Action(door, "opens");

                //SimpleCondition keyIsHeld = new SimpleCondition(key, "isPickedUp", "=", ECABoolean.TRUE);
                
                List<Action> actions = new List<Action> { openDoor };
                
                Rule ruleStep4 = Rule.TryCreateRule(playerTrigger, actions);
                //Rule ruleStep4 = Rule.TryCreateRule(keyTrigger, actions);

                if (ruleStep4 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep4);
                    Debug.Log("[Rule Engine] Regola Step 4 configurata");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore configurazione regola 4");
                }
            }
            else
            {
                Debug.LogError("Impossibile trovare gli oggetti per la regola 4 nella scena");
            }
        }

        private void InitializeStep5Rule()
        {
            GameObject player = GameObject.Find("Player");
            GameObject carillon = GameObject.Find("Carillon");

            if (carillon != null && player != null)
            {
                Action triggerPlayer = new Action(player, "interacts with", carillon);
                Action play = new Action(carillon, "play");
                
                List<Action> actions = new List<Action> { play };

                Rule ruleStep5 = Rule.TryCreateRule(triggerPlayer, actions);

                if (ruleStep5 != null)
                {
                    RuleEngine.GetInstance().Add(ruleStep5);
                    Debug.Log("[Rule Engine] Regola Step 5 configurata");
                }
                else
                {
                    Debug.LogError("[Rule Engine] Errore configurazione regola 5");
                }
            }
            else
            {
                Debug.Log("[Rule Engine] impossibile trovare gli oggetti per la regola 5");
            }
        }
    }
}