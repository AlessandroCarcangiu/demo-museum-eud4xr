using System;
using System.Collections.Generic;
using ECARules4All_DLL;
using UnityEngine;
using Action = ECARules4All_DLL.Action;

public class TestRegoleJacopo : MonoBehaviour
{
    private void Awake()
    {
        Debug.LogError("MESSAGGIO DI WARNING: RICORDATI DI CANCELLARE QUESTO SCRIPT (TestRegoleJacopo)");
        
        var gO_cylinder = GameObject.Find("Cylinderrr");
        var gO_cylinder1 = GameObject.Find("Cylinderrr (1)");
        
        
        List<Rule> myRules = new List<Rule>();
        myRules.Add(
            Rule.TryCreateRule(
                // new Action(fishAnimal, "interacts with", artInt),
                new Action(gO_cylinder, "interacts with", gO_cylinder1),
                new List<Action> { new Action(gO_cylinder, "hides") }
            )
        );
        foreach (var r in myRules)
        {
            RuleEngine.GetInstance().Add(r);
        }
    }
}
