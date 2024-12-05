using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using TMPro;
using UnityEngine;

public class CapabilityManager : Singleton<CapabilityManager>
{
    public GameObject rootToMove;
    public TextMeshProUGUI textToFill;

    protected override void OnAwake()
    {
        base.OnAwake();
        rootToMove.SetActive(false);
    }

    // private void OnGUI()
    // {
    //     if (GUI.Button(new Rect(10, 10, 150, 100), "Test Component Tracker"))
    //     {
    //         // test
    //         string gName = "Cylinderrr";
    //         var stateVariableNames = new List<string>();
    //         var actionMethodNames = new List<string>();
    //
    //         var allComponents = ComponentTracker.Instance.GetAllComponents()
    //             .Where(x => x.Key.StartsWith(gName));
    //         foreach (var component in allComponents)
    //         {
    //             // Look for the StateVariableAttribute (variable) and the ActionAttribute (function)
    //             var f = component.Value.GetType();
    //             // var variables = f.GetFields();
    //             // get both fields and properties in the same list
    //             var variables =
    //                 from it in f.GetMembers( BindingFlags.Public | BindingFlags.Instance)
    //                 where it is PropertyInfo || it is FieldInfo
    //                 select it;
    //             foreach (var variable in variables)
    //             {
    //                 var stateVariable = (StateVariableAttribute)variable
    //                     .GetCustomAttributes(typeof(StateVariableAttribute), true).FirstOrDefault();
    //                 if (stateVariable != null)
    //                 {
    //                     stateVariableNames.Add(variable.Name);
    //                 }
    //             }
    //
    //             var methods = f.GetMethods();
    //             foreach (var method in methods)
    //             {
    //                 var action =
    //                     (ActionAttribute)method.GetCustomAttributes(typeof(ActionAttribute), true).FirstOrDefault();
    //                 if (action != null)
    //                 {
    //                     actionMethodNames.Add(method.Name);
    //                 }
    //             }
    //         }
    //
    //         Debug.Log("START");
    //         Debug.Log("StateVariables: " + string.Join(", ", stateVariableNames));
    //         Debug.Log("Actions: " + string.Join(", ", actionMethodNames));
    //         Debug.Log("END");
    //     }
    //     else if (GUI.Button(new Rect(10, 120, 150, 100), "Test ECAObjectInfo"))
    //     {
    //         // test
    //         string gName = "Cylinderrr";
    //         var info = ECAObjectInfo.Instance.GetAllInfoAboutCurrentECAObjects();
    //
    //         // var stateVariableNames = new List<string>();
    //         // var actionMethodNames = info.allActionAttributes[gName].Select(kv => kv.Key).Distinct().ToList();
    //         // var GTVK = RuleUtils.GetStateVariableKeys();
    //         var x = FindStateVariables(GameObject.Find(gName));
    //         // foreach kv in x, create the string "<Key> <Value.Item1.ToString()>"
    //         var y = x.Select(kv => kv.Key + " " + kv.Value.Item1.ToString()).ToList();
    //         Debug.Log("START");
    //         Debug.Log("StateVariables: " + string.Join(", ", y));
    //         Debug.Log("Actions: " + string.Join(", ", info.allActionAttributes[gName].Select(kv => kv.Key).Distinct().ToList()));
    //         Debug.Log("END");
    //     }
    //     // StateVariableAttribute[] variables = (StateVariableAttribute[]) f.GetCustomAttributes(typeof(StateVariableAttribute),true);
    // }

    public void ShowCapability(ECAObject ecaObject, Transform playerTransform)
    {
        // make the canvas in front of the ecaObject.gameObject, half bouds in front of the object
        rootToMove.transform.position = ecaObject.gameObject.transform.position - ecaObject.gameObject.transform.forward * ecaObject.gameObject.transform.localScale.z / 2;
        // rootToMove.transform.position = ecaObject.transform.position + CalcTransform(ecaObject.transform, playerTransform);

        textToFill.text = GetTextCapability(ecaObject);

        rootToMove.SetActive(true);
    }

    private Vector3 CalcTransform(Transform ecaTransform, Transform playerTransform)
    {
        // Get the direction vector from the object to the player
        Vector3 directionToPlayer = (playerTransform.position - ecaTransform.position).normalized;

        // Determine the local direction relative to the object's orientation
        Vector3 localDirection = ecaTransform.InverseTransformDirection(directionToPlayer);

        // Position the canvas based on the approach direction
        Vector3 canvasOffset = Vector3.zero;

        // Front/Back/Left/Right decision
        if (localDirection.z > 0.5f) // Front
        {
            canvasOffset = ecaTransform.forward * ecaTransform.localScale.z / 2;
        }
        else if (localDirection.z < -0.5f) // Back
        {
            canvasOffset = -ecaTransform.forward * ecaTransform.localScale.z / 2;
        }
        else if (localDirection.x > 0.5f) // Right
        {
            canvasOffset = ecaTransform.right * ecaTransform.localScale.x / 2;
        }
        else if (localDirection.x < -0.5f) // Left
        {
            canvasOffset = -ecaTransform.right * ecaTransform.localScale.x / 2;
        }

        return canvasOffset;
    }

    public void HideCapability()
    {
        rootToMove.SetActive(false);
    }

    private string GetTextCapability(ECAObject ecaObject)
    {
        var output = "";

        var info = ECAObjectInfo.Instance.GetAllInfoAboutCurrentECAObjects();

        var name = ecaObject.gameObject.name;

        var ecaStateVariables = RuleUtils.FindStateVariables(GameObject.Find(name))
            .Select(kv => kv.Value.Item1 + " " + kv.Key).ToList();
        var ecaMethodNames = info.allActionAttributes[name].Select(kv => kv.Key).Distinct().ToList();
        
        Debug.Log("StateVariables: " + string.Join(", ", ecaStateVariables));
        Debug.Log("Actions: " + string.Join(", ", ecaMethodNames));
        output += "Name: " + name + "\n"
                    + "StateVariables: " + string.Join(", ", ecaStateVariables) + "\n"
                    + "Actions: " + string.Join(", ", ecaMethodNames);
        return output;
    }

    // public void ShowCapability(ECAObject ecaObject, Transform playerTransform)
    // {
    //     // Get the direction vector from the object to the player
    //     Vector3 directionToPlayer = (playerTransform.position - ecaObject.transform.position).normalized;
    //
    //     // Determine the local direction relative to the object's orientation
    //     Vector3 localDirection = ecaObject.transform.InverseTransformDirection(directionToPlayer);
    //
    //     // Position the canvas based on the approach direction
    //     Vector3 canvasOffset = Vector3.zero;
    //
    //     // Front/Back/Left/Right decision
    //     if (localDirection.z > 0.5f) // Front
    //     {
    //         canvasOffset = ecaObject.transform.forward * ecaObject.transform.localScale.z / 2;
    //     }
    //     else if (localDirection.z < -0.5f) // Back
    //     {
    //         canvasOffset = -ecaObject.transform.forward * ecaObject.transform.localScale.z / 2;
    //     }
    //     else if (localDirection.x > 0.5f) // Right
    //     {
    //         canvasOffset = ecaObject.transform.right * ecaObject.transform.localScale.x / 2;
    //     }
    //     else if (localDirection.x < -0.5f) // Left
    //     {
    //         canvasOffset = -ecaObject.transform.right * ecaObject.transform.localScale.x / 2;
    //     }
    //
    //     // Update the position and text
    //     rootToMove.transform.position = ecaObject.transform.position + canvasOffset;
    //     textToFill.text = GetTextCapability(ecaObject);
    //
    //     rootToMove.SetActive(true);
    // }
}