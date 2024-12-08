// using System.Linq;
// using ECARules4All_DLL;
// using ECARules4All_DLL.Utils;
// using TMPro;
// using UnityEngine;
//
// public class ECAObjectUI_CapabilityManager : Singleton<ECAObjectUI_CapabilityManager>
// {
//     public GameObject rootToMove;
//     public TextMeshProUGUI textToFill;
//
//     protected override void OnAwake()
//     {
//         base.OnAwake();
//         // rootToMove.SetActive(false);
//     }
//
//     // private void OnGUI()
//     // {
//     //     if (GUI.Button(new Rect(10, 10, 150, 100), "Test Component Tracker"))
//     //     {
//     //         // test
//     //         string gName = "Cylinderrr";
//     //         var stateVariableNames = new List<string>();
//     //         var actionMethodNames = new List<string>();
//     //
//     //         var allComponents = ComponentTracker.Instance.GetAllComponents()
//     //             .Where(x => x.Key.StartsWith(gName));
//     //         foreach (var component in allComponents)
//     //         {
//     //             // Look for the StateVariableAttribute (variable) and the ActionAttribute (function)
//     //             var f = component.Value.GetType();
//     //             // var variables = f.GetFields();
//     //             // get both fields and properties in the same list
//     //             var variables =
//     //                 from it in f.GetMembers( BindingFlags.Public | BindingFlags.Instance)
//     //                 where it is PropertyInfo || it is FieldInfo
//     //                 select it;
//     //             foreach (var variable in variables)
//     //             {
//     //                 var stateVariable = (StateVariableAttribute)variable
//     //                     .GetCustomAttributes(typeof(StateVariableAttribute), true).FirstOrDefault();
//     //                 if (stateVariable != null)
//     //                 {
//     //                     stateVariableNames.Add(variable.Name);
//     //                 }
//     //             }
//     //
//     //             var methods = f.GetMethods();
//     //             foreach (var method in methods)
//     //             {
//     //                 var action =
//     //                     (ActionAttribute)method.GetCustomAttributes(typeof(ActionAttribute), true).FirstOrDefault();
//     //                 if (action != null)
//     //                 {
//     //                     actionMethodNames.Add(method.Name);
//     //                 }
//     //             }
//     //         }
//     //
//     //         Debug.Log("START");
//     //         Debug.Log("StateVariables: " + string.Join(", ", stateVariableNames));
//     //         Debug.Log("Actions: " + string.Join(", ", actionMethodNames));
//     //         Debug.Log("END");
//     //     }
//     //     else if (GUI.Button(new Rect(10, 120, 150, 100), "Test ECAObjectInfo"))
//     //     {
//     //         // test
//     //         string gName = "Cylinderrr";
//     //         var info = ECAObjectInfo.Instance.GetAllInfoAboutCurrentECAObjects();
//     //
//     //         // var stateVariableNames = new List<string>();
//     //         // var actionMethodNames = info.allActionAttributes[gName].Select(kv => kv.Key).Distinct().ToList();
//     //         // var GTVK = RuleUtils.GetStateVariableKeys();
//     //         var x = FindStateVariables(GameObject.Find(gName));
//     //         // foreach kv in x, create the string "<Key> <Value.Item1.ToString()>"
//     //         var y = x.Select(kv => kv.Key + " " + kv.Value.Item1.ToString()).ToList();
//     //         Debug.Log("START");
//     //         Debug.Log("StateVariables: " + string.Join(", ", y));
//     //         Debug.Log("Actions: " + string.Join(", ", info.allActionAttributes[gName].Select(kv => kv.Key).Distinct().ToList()));
//     //         Debug.Log("END");
//     //     }
//     //     // StateVariableAttribute[] variables = (StateVariableAttribute[]) f.GetCustomAttributes(typeof(StateVariableAttribute),true);
//     // }
//
//     public void ShowCapability(ECAObject ecaObject, Transform playerTransform)
//     {
//         // Choose on of the following two methods to set the canvas position
//         SetCanvasCloseToPlayer(playerTransform, ecaObject.gameObject.transform);
//         // SetCanvasCloseToEcaObject(playerTransform, ecaObject.gameObject.transform);
//         
//         textToFill.text = ECAObjectInfo.Instance.GetCapabilitiesAsString(ecaObject);
//         rootToMove.SetActive(true);
//     }
//     
//     public void HideCapability()
//     {
//         rootToMove.SetActive(false);
//     }
//     
//     // private void SetCanvasCloseToPlayer(Transform playerTransform, Transform ecaObjectTransform)
//     // {
//     //     var direction__EcaObj_to_P = (playerTransform.position - ecaObjectTransform.position).normalized;
//     //     Vector3 newPos = playerTransform.position - direction__EcaObj_to_P * 1.6f;
//     //     newPos.y = 2f;
//     //     rootToMove.transform.position = newPos;
//     //     rootToMove.transform.LookAt(playerTransform.position+playerTransform.transform.rotation*Vector3.forward, playerTransform.rotation*Vector3.up);
//     // }
//     //
//     // private void SetCanvasCloseToEcaObject(Transform playerTransform, Transform ecaObjectTransform)
//     // {
//     //     var direction__EcaObj_to_P = (playerTransform.position - ecaObjectTransform.position).normalized;
//     //     Vector3 newPos = ecaObjectTransform.position + direction__EcaObj_to_P * ecaObjectTransform.localScale.z *0.75f;
//     //     newPos = playerTransform.position - direction__EcaObj_to_P * 1.6f;
//     //     // newPos = ecaObject.gameObject.transform.position - ecaObject.gameObject.transform.forward * ecaObject.gameObject.transform.localScale.z / 2;
//     //     newPos.y = 2f;
//     //     rootToMove.transform.position = newPos;
//     //     rootToMove.transform.LookAt(playerTransform.position+playerTransform.transform.rotation*Vector3.forward, playerTransform.rotation*Vector3.up);
//     //     // rootToMove.transform.position = ecaObject.transform.position + CalcTransform(ecaObject.transform, playerTransform);
//     // }
// }