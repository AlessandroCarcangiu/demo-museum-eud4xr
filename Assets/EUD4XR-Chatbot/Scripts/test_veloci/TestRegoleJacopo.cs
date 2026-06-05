using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using UnityEngine;
using Action = ECARules4All_DLL.Action;

public class TestRegoleJacopo : MonoBehaviour
{
    private void Awake()
    {
        Debug.LogError("MESSAGGIO DI WARNING: RICORDATI DI CANCELLARE QUESTO SCRIPT (TestRegoleJacopo)");
        
        var player = GameObject.Find("Player");
        var canvas_rondaDiNotte = GameObject.Find("Quadro_RondaDiNotte");
        var spotlight_rondaDiNotte = GameObject.Find("SpotLight_RondaDiNotte");
        
        
        List<Rule> myRules = new List<Rule>();
        myRules.Add(
            Rule.TryCreateRule(
                // new Action(fishAnimal, "interacts with", artInt),
                new Action(player, "interacts with", canvas_rondaDiNotte),
                new List<Action>
                {
                    new Action(player, "shows") ,
                    new Action(spotlight_rondaDiNotte, "turns", ECABoolean.ON) 
                    
                }
            )
        );
        
        foreach (var r in myRules)
        {
            RuleEngine.GetInstance().Add(r);
        }
    }

    // public void Togg(Single s)
    // {
    //     Debug.Log("Sono F e ho ricevuto: " + s);
    // }    public void UnTogg(Single s)
    // {
    //     Debug.Log("Sono UNTOGG AAAA e ho ricevuto: " + s);
    // }
}
