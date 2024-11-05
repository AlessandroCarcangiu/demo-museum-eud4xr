// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using ECARules4All_DLL;
// using ECARules4All_DLL.Utils;
// using Newtonsoft.Json;
// using UnityEngine;
// using UnityEngine.Networking;
// using Action = ECARules4All_DLL.Action;
//
// public class CreateRuleFromDictUtils
// {
//     private bool PropertyIsValid(string property)
//     {
//         return !string.IsNullOrEmpty(property) && !property.Equals("null");
//     }
//
//     // private Action CreateActionFromJSON(object jObj)
//     // {
//     //     var eventJObject = (Newtonsoft.Json.Linq.JObject)jObj;
//     //
//     //     /*
//     //  * 				"subject":"Player",
//     //         "BoolType": "YESNO",
//     //         "ModifierString": "TODOMODIFIER",
//     //         "ObjectType": "TODOOBJECT",
//     //         "SubjectType": "TODOSUBJECT",
//     //         "TypeId": "ECARules4All.RuleEngine.ActionAttribute",
//     //         "ValueType": null,
//     //         "Verb": "interacts with",
//     //         "variableName": ""
//     //  */
//     //
//     //     Action output = null;
//     //
//     //     // Dropdown user choises
//     //     var subject = eventJObject["subject"].ToString();
//     //     var oobject = eventJObject["object"].ToString();
//     //     var verb = eventJObject["verb"].ToString();
//     //     var prep = eventJObject["prep"].ToString();
//     //     var value = eventJObject["value"].ToString();
//     //
//     //     var subjectGameObject = GameObject.Find(subject);
//     //
//     //     if (subjectGameObject == null)
//     //     {
//     //         AgileUtils.AgileErrorStatus(AgileUtils.OBJECT_NOT_FOUND, "", subject);
//     //         AgileUtils.AgileErrorStatus(AgileUtils.ECA_RULE_NOT_VALID, "Soggetto inesistente", subject);
//     //         return null;
//     //     }
//     //     
//     //     // The actionAttribute are in the "actionAttributeInfo" property. I save them all in a variable but I don't know if I'll need them all
//     //     if (!eventJObject["actionAttributeInfo"].Children().Any())
//     //     {
//     //         if (!PropertyIsValid(verb) && !PropertyIsValid(prep) && !PropertyIsValid(value))
//     //         {
//     //             AgileUtils.AgileErrorStatus(AgileUtils.GENERIC_WARNING, "Questo caso non è ancora supportato!", "");
//     //             throw new NotSupportedException("Case not supported yet");
//     //         }
//     //
//     //         output = new Action(subjectGameObject, verb);
//     //         return output;
//     //     }
//     //
//     //     var actionAttributeInfo_subject = eventJObject["actionAttributeInfo"]["Subject"].ToString();
//     //     var actionAttributeInfo_boolType = eventJObject["actionAttributeInfo"]["BoolType"].ToString();
//     //     var actionAttributeInfo_modifierString = eventJObject["actionAttributeInfo"]["ModifierString"].ToString();
//     //     var actionAttributeInfo_objectType = eventJObject["actionAttributeInfo"]["ObjectType"].ToString();
//     //     var actionAttributeInfo_subjectType = eventJObject["actionAttributeInfo"]["SubjectType"].ToString();
//     //     var actionAttributeInfo_typeId = eventJObject["actionAttributeInfo"]["TypeId"].ToString();
//     //     var actionAttributeInfo_valueType = eventJObject["actionAttributeInfo"]["ValueType"].ToString();
//     //     var actionAttributeInfo_verb = eventJObject["actionAttributeInfo"]["Verb"].ToString();
//     //     var actionAttributeInfo_variableName = eventJObject["actionAttributeInfo"]["variableName"].ToString();
//     //
//     //
//     //     // If ObjectType exists:
//     //     if (PropertyIsValid(actionAttributeInfo_objectType))
//     //     {
//     //         switch (actionAttributeInfo_objectType)
//     //         {
//     //             case "ECARules4All.Path":
//     //             case "ECARules4All.Rotation":
//     //             case "ECARules4All.Position":
//     //             {
//     //                 //TODO agile Should be implemented?
//     //                 throw new NotSupportedException($"{actionAttributeInfo_objectType} not supported yet");
//     //             }
//     //             case "ECAScripts.Utils.ECABoolean":
//     //             {
//     //                 // ECATestLight turns off
//     //                 var objectAsBoolean = bool.Parse(oobject);
//     //                 output = new Action(subjectGameObject, verb, new ECABoolean(objectAsBoolean));
//     //                 break;
//     //             }
//     //             // Player interacts with TV-box
//     //             case "UnityEngine.GameObject":
//     //             case "ECARules4All.RuleEngine.ECAObject":
//     //             case "ECAInteractable":
//     //             case "ECAVolume":
//     //                 oobject = oobject.Split(' ').Last(); // "Object BigCloset" non va bene --> splittiamo e prendiamo solo "BigCloset"
//     //                 var objectGameObject = GameObject.Find(oobject);
//     //
//     //                 if (objectGameObject == null)
//     //                 {
//     //                     AgileUtils.AgileErrorStatus(AgileUtils.OBJECT_NOT_FOUND, "", oobject);
//     //                     AgileUtils.AgileErrorStatus(AgileUtils.ECA_RULE_NOT_VALID, "Oggetto inesistente.", oobject);
//     //                     return null;
//     //                 }
//     //                 
//     //                 output = new Action(subjectGameObject, verb, objectGameObject);
//     //                 break;
//     //             default:
//     //                 AgileUtils.AgileErrorStatus(AgileUtils.ECA_RULE_NOT_VALID, "",
//     //                     subject + ". Tipo dell'oggetto: " + actionAttributeInfo_objectType);
//     //                 throw new NotSupportedException("Ciao Carlo. Sono Object type not supported: " + actionAttributeInfo_objectType);
//     //         }
//     //     }
//     //
//     //     // Otherwise, if the modifier exists
//     //     else if (PropertyIsValid(actionAttributeInfo_variableName) &&
//     //              PropertyIsValid(actionAttributeInfo_valueType) && PropertyIsValid(prep) && PropertyIsValid(value))
//     //     {
//     //         switch (actionAttributeInfo_valueType)
//     //         {
//     //             // playerone changes width to 10 (even though this method should have been removed)
//     //             case "System.Int32":
//     //                 var valueAsInt = Convert.ToInt32(value);
//     //                 output = new Action(GameObject.Find(subject), verb, oobject, prep, valueAsInt);
//     //                 break;
//     //             // TV-box changes volume to 10.03
//     //             case "System.Single":
//     //                 var valueAsSingle = Convert.ToSingle(value);
//     //                 output = new Action(GameObject.Find(subject), verb, oobject, prep, valueAsSingle);
//     //                 break;
//     //             //////////////////////////////////////////////////////////////////
//     //             case "ECAScripts.Utils.YesNo":
//     //                 var booleanYesNo = value == "yes" ? ECABoolean.YES : ECABoolean.NO;
//     //                 output = new Action(GameObject.Find(subject), verb, oobject, prep, new ECABoolean(booleanYesNo));
//     //                 break;
//     //             case "ECAScripts.Utils.TrueFalse":
//     //                 var booleanTrueFalse = value == "true" ? ECABoolean.TRUE : ECABoolean.FALSE;
//     //                 output = new Action(GameObject.Find(subject), verb, oobject, prep,
//     //                     new ECABoolean(booleanTrueFalse));
//     //                 break;
//     //             case "ECAScripts.Utils.OnOff":
//     //                 var booleanOnOff = value == "on" ? ECABoolean.ON : ECABoolean.OFF;
//     //                 output = new Action(GameObject.Find(subject), verb, oobject, prep, new ECABoolean(booleanOnOff));
//     //                 break;
//     //             //////////////////////////////////////////////////////////////////
//     //             // TV-box changes visible to yes
//     //             // case "ECAScripts.Utils.YesNo":
//     //             //     var valueAsBoolean = bool.Parse(value);
//     //             //     output = new Action(GameObject.Find(subject), verb, oobject, prep, new ECABoolean(valueAsBoolean));
//     //             //     break;
//     //             // TV-box changes source to nature_4k.mp4
//     //             case "System.String":
//     //                 output = new Action(GameObject.Find(subject), verb, oobject, prep, value);
//     //                 break;
//     //             case "ECAScripts.Utils.ECAColor":
//     //                 output = new Action(GameObject.Find(subject), verb, oobject, prep, new ECAColor(value));
//     //                 break;
//     //             default:
//     //                 throw new NotSupportedException("Value type not supported: " + actionAttributeInfo_valueType);
//     //         }
//     //     }
//     //
//     //     // it should be only [subject][verb]
//     //     else
//     //     {
//     //         if (PropertyIsValid(subject) && PropertyIsValid(verb) && !PropertyIsValid(oobject) &&
//     //             !PropertyIsValid(prep) && !PropertyIsValid(value))
//     //         {
//     //             throw new NotSupportedException("Case not supported yet");
//     //         }
//     //
//     //         output = new Action(GameObject.Find(subject), verb);
//     //     }
//     //
//     //     if (output == null)
//     //     {
//     //         AgileUtils.AgileErrorStatus(AgileUtils.ECA_RULE_NOT_VALID,
//     //             "L'azione generata non è valida. Assicurati che il JSON passato sia corretto.", "");
//     //         throw new Exception(
//     //             "The action parsed is null. Therefore it is not possible to create the rule. Is the JSON correct?");
//     //     }
//     //
//     //     return output;
//     //
//     //     //var condition = new SimpleCondition(GameObject.Find("Sphere"), )
//     //
//     //     RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
//     //             "interacts with", GameObject.Find("Radio")),
//     //         new SimpleCondition(GameObject.Find("Audio"), "playing", "is", ECABoolean.NO),
//     //         new List<Action>
//     //         {
//     //             new Action(GameObject.Find("Audio"), "plays")
//     //         }));
//     //     RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
//     //             "interacts with", GameObject.Find("Radio")),
//     //         new SimpleCondition(GameObject.Find("Audio"), "playing", "is", ECABoolean.YES),
//     //         new List<Action>
//     //         {
//     //             new Action(GameObject.Find("Audio"), "stops")
//     //         }));
//     //
//     //     return new Action(GameObject.Find(subject), verb, GameObject.Find("Inactive"));
//     // }
//
//     public void AddRuleFromReact(string stringRule)
//     {
//         // use this to create stringRule given a JS Object: https://tools.knowledgewalls.com/json-to-string
//         // var stringRule = 
//         //     "{\"id\":0.01388827147678473,\"fieldSetIcon\":\"pi pi-search\",\"fieldsetVerb\":\"interacts with\",\"data\":{\"event\":{\"subject\":\"Player\",\"verb\":\"is close to\",\"object\":\"Volume TV-trigger\",\"prep\":\"\",\"value\":null,\"actionAttributeInfo\":{\"Subject\":\"Player\",\"BoolType\":\"YESNO\",\"ModifierString\":\"\",\"ObjectType\":\"ECAVolume\",\"SubjectType\":\"ECACharacter\",\"TypeId\":\"ECARules4All.RuleEngine.ActionAttribute\",\"ValueType\":\"null\",\"Verb\":\"enters-volume\",\"variableName\":\"\"}},\"conditions\":[],\"actions\":[{\"subject\":\"TV-box\",\"verb\":\"changes\",\"object\":\"source\",\"prep\":\"to\",\"value\":\"nature_4k.mp4\",\"actionAttributeInfo\":{\"Subject\":\"TV-box\",\"BoolType\":\"YESNO\",\"ModifierString\":\"to\",\"ObjectType\":\"null\",\"SubjectType\":\"ECAVideo\",\"TypeId\":\"ECARules4All.RuleEngine.ActionAttribute\",\"ValueType\":\"System.String\",\"Verb\":\"changes\",\"variableName\":\"source\"}},{\"subject\":\"TV-box\",\"verb\":\"plays\",\"object\":\"\",\"prep\":\"\",\"value\":null,\"actionAttributeInfo\":{}}]}}"
//         //     ;
//         
//         Debug.Log("String rule: " + stringRule);
//         var jsonRule = JsonConvert.DeserializeObject<Dictionary<string, object>>(stringRule);
//
//         // var ruleId = jsonRule["id"];
//         // var ruleFieldSetIcon = jsonRule["fieldSetIcon"];
//         // var ruleFieldsetVerb = jsonRule["fieldsetVerb"];
//         var jsonDeRule = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonRule["data"].ToString());
//
//         var jsonEvent = jsonDeRule["event"];
//         // var jsonConditions = jsonDeRule["conditions"];
//         var jsonActions = jsonDeRule["actions"];
//
//         Action ecaEvent = CreateActionFromJSON(jsonEvent);
//
//         List<object> jsonActionsDeserialized = JsonConvert.DeserializeObject<List<object>>(jsonActions.ToString());
//         List<Action> ecaActions = new List<Action>();
//         foreach (var jsonIthAction in jsonActionsDeserialized)
//         {
//             var ithAction = CreateActionFromJSON(jsonIthAction);
//             ecaActions.Add(ithAction);
//         }
//
//         AgileUtils.AgileErrorStatus(AgileUtils.LOG, "Trying to create the rule", "");
//         // Debug.Log("prova");
//
//         //todo manage conditions
//         // var rule = new Rule(ecaEvent, ecaActions);
//         var rule = Rule.TryCreateRule(ecaEvent, null, ecaActions);
//         RuleEngine.GetInstance().Add(rule);
//     }
//
//     public void AddAllRulesFromReact(string allStringifiedRules)
//     {
//         Debug.Log("I received all the rules from React: " + allStringifiedRules);
//         RuleEngine.GetInstance().ClearRules();
//         Debug.Log("Cleared the old rules");
//
//         //string allStringifiedRules =
//         // "[{\"id\":0.17563050294458638,\"data\":{\"event\":{\"subject\":\"ECATestLight\",\"verb\":\"activates\",\"object\":\"\",\"prep\":\"\",\"value\":null,\"actionAttributeInfo\":{}},\"conditions\":[],\"actions\":[{\"subject\":\"TV-box\",\"verb\":\"deactivates\",\"object\":\"\",\"prep\":\"\",\"value\":null,\"actionAttributeInfo\":{}},{\"subject\":\"Radio\",\"verb\":\"stops\",\"object\":\"\",\"prep\":\"\",\"value\":null,\"actionAttributeInfo\":{}}]}},{\"id\":0.42859195609691914,\"data\":{\"event\":{\"subject\":\"Player\",\"verb\":\"changes\",\"object\":\"visible\",\"prep\":\"to\",\"value\":\"yes\",\"actionAttributeInfo\":{\"Subject\":\"Player\",\"BoolType\":\"YESNO\",\"ModifierString\":\"to\",\"ObjectType\":\"null\",\"SubjectType\":\"ECARules4All.RuleEngine.ECAObject\",\"TypeId\":\"ECARules4All.RuleEngine.ActionAttribute\",\"ValueType\":\"ECAScripts.Utils.YesNo\",\"Verb\":\"changes\",\"variableName\":\"visible\"}},\"conditions\":[],\"actions\":[{\"subject\":\"Radio\",\"verb\":\"looks at\",\"object\":\"titanic\",\"prep\":\"\",\"value\":null,\"actionAttributeInfo\":{\"Subject\":\"Radio\",\"BoolType\":\"YESNO\",\"ModifierString\":\"\",\"ObjectType\":\"UnityEngine.GameObject\",\"SubjectType\":\"ECARules4All.RuleEngine.ECAObject\",\"TypeId\":\"ECARules4All.RuleEngine.ActionAttribute\",\"ValueType\":\"null\",\"Verb\":\"looks at\",\"variableName\":\"\"}}]}}]"
//         //    ;
//         var jsonRules = JsonConvert.DeserializeObject<List<object>>(allStringifiedRules.Trim('"'));
//         foreach (var json in jsonRules)
//         {
//             AddRuleFromReact(json.ToString());
//         }
//
//         Debug.Log("Added the rules");
//
//
//         var r = RuleEngine.GetInstance().Rules();
//         Debug.Log($"{r.Count()} rules has been inserted into the rule engine");
//
//         // Save the file in save/rules.json
//         const string fileName = "VMXR_stringified_rules.txt";
//         #if !UNITY_EDITOR && UNITY_WEBGL
//             StartCoroutine(SaveVMXRRules(fileName, allStringifiedRules));
//         #else
//             var filePath = Application.streamingAssetsPath + "/saves/" + fileName;
//             System.IO.File.WriteAllText(filePath, allStringifiedRules);
//         #endif
//         Debug.Log("Serialized the rules");
//     }
//
//     private IEnumerator SaveVMXRRules(string fileName, string textContent)
//     {
//         
//         Dictionary<string, string> d = new() { { "body", textContent } };
//         var jsonString = JsonConvert.SerializeObject(d, Formatting.Indented, new JsonSerializerSettings() { 
//             ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
//         });
//         
//         using (UnityWebRequest uwr = UnityWebRequest.Put(AgileUtils.getFileSaveByName(fileName), textContent))
//         {
//             uwr.SetRequestHeader("Content-Type", "text/plain");
//             Debug.Log(uwr.result);
//             yield return uwr.SendWebRequest();
//             if (uwr.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.Log("C'è stato un problema nella richiesta HTTP per il salvataggio del file.");
//             }
//             else
//             {
//                 Debug.Log("File " + fileName + " salvato correttamente with content: " + textContent);
//             }
//         }
//
//         yield return null;
//     }
//
//     private IEnumerator GetVMXRRules(string fileName, Action<string> callback)
//     {
//         using (UnityWebRequest uwr = UnityWebRequest.Get(AgileUtils.getFileSaveByName(fileName)))
//         {
//             yield return uwr.SendWebRequest();
//             if (uwr.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.Log(
//                     "C'è stato un problema nella richiesta HTTP per il caricamento del salvataggio. Verifica che il file esista e che si trovi nell'apposita cartella e riprova.");
//             }
//             else
//             {
//                 Debug.Log("File " + fileName + " caricato correttamente.");
//                 var textContent = uwr.downloadHandler.text;
//                 callback(textContent);
//             }
//         }
//
//         yield return null;
//     }
//
//     public void SetRulesBasedOnUserType()
//     {
//         var VMXRScript = VMXR_EnvironmentManager.Instance;
//         switch (VMXRScript.GetUserType())
//         {
//             case VMXR_EnvironmentManager.UserType.Developer:
//             case VMXR_EnvironmentManager.UserType.EnvironmentConfigurator:
//                 ; // do nothing
//                 break;
//             case VMXR_EnvironmentManager.UserType.ExperienceConfigurator:
//             case VMXR_EnvironmentManager.UserType.User:
//                 // get the content of saves/VMXR_stringified_rules.txt and pass it to the function AddRulesFromReact
//                 const string fileName = "VMXR_stringified_rules.txt";
//                 #if !UNITY_EDITOR && UNITY_WEBGL
//                     StartCoroutine(GetVMXRRules(fileName, AddAllRulesFromReact));
//                 #else
//                     var filePath = Application.streamingAssetsPath + "/saves/" + fileName;
//                     var stringifiedRules = System.IO.File.ReadAllText(filePath);
//                     AddAllRulesFromReact(stringifiedRules);
//                 #endif
//                 break;
//         }
//     }
//
//     private void Start()
//     {
//         // SetRulesBasedOnUserType();
//     }
// }