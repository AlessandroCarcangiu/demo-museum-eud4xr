using System;
using System.Collections.Generic;
using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using UnityEngine;
using Action = ECARules4All_DLL.Action;


public class TESTECAObjectInfo : Singleton<TESTECAObjectInfo>
{
    private void Start()
    {
        var x = GetRulesInvolvedToSpecificGameObject(GameObject.Find("FishAnimal"));
        Debug.Log("Rules involved to FishAnimal: " + x);
    }

    private bool DoesActionInvolvesThisGameObject(Action a, GameObject gO)
    {
        return (a.GetSubject() == gO)
               || (a.GetObject() != null && a.GetObject() is GameObject objGo && objGo == gO)
               || (a.GetObject() != null && a.GetObject() is MonoBehaviour mB && mB.gameObject == gO)
               || (a.GetModifierValue() != null && a.GetModifierValue() is MonoBehaviour mB2 && mB2.gameObject == gO)
               || (a.GetModifierValue() != null && a.GetModifierValue() is GameObject objGo2 && objGo2 == gO);
    }

    private bool DoesConditionInvolvesThisGameObject(Condition c, GameObject gO)
    {
        if (c is SimpleCondition sc)
        {
            return (sc.GetSubject() == gO)
                   || (sc.GetValueToCompare() != null && sc.GetValueToCompare() is GameObject objGo && objGo == gO)
                   || (sc.GetValueToCompare() != null && sc.GetValueToCompare() is MonoBehaviour mB && mB.gameObject == gO);
        }

        if (c is CompositeCondition cc)
        {
            foreach (var cc_child in cc.Children())
            {
                if (DoesConditionInvolvesThisGameObject(cc_child, gO)) return true;
            }
            return false;
        }
        
        throw new Exception("Condition type not recognized");
    }
    
    //TODO GameObject or ECAObject as parameter? :thinking:
    public List<Rule> GetRulesInvolvedToSpecificGameObject(GameObject gO)
    {
        void ThrowExceptionsIfEventNotValid(Action e)
        {
            if (e == null)
            {
                throw new Exception("There is a rule with a null event recorded in the rule engine. How?!");
            }

            if (e.GetSubject() == null)
            {
                throw new Exception("There is a rule with a null subject (event) recorded in the rule engine. How?!");
            }

            if (!e.IsValid())
            {
                throw new Exception("There is a rule with a non-valid event recorded in the rule engine. How?!");
            }
        }

        void ThrowExceptionsIfActionNotValid(Action a, int pos)
        {
            if (a == null)
            {
                throw new Exception($"There is a rule with a null action (#{pos}) recorded in the rule engine. How?!");
            }

            if (a.GetSubject() == null)
            {
                throw new Exception(
                    $"There is a rule with a null subject (action #{pos}) recorded in the rule engine. How?!");
            }

            if (!a.IsValid())
            {
                throw new Exception(
                    $"There is a rule with a non-valid action (#{pos}) recorded in the rule engine. How?!");
            }
        }

        List<Rule> involvedRules = new();

        foreach (var r in RuleEngine.GetInstance().Rules())
        {
            bool isRuleInvolved = false;

            if (r == null)
            {
                throw new Exception("There is a null rule recorded in the rule engine. How?!");
            }

            // Check the event
            ThrowExceptionsIfEventNotValid(r.GetEvent());
            if (DoesActionInvolvesThisGameObject(r.GetEvent(), gO))
            {
                isRuleInvolved = true;
                involvedRules.Add(r);
                continue;
            }

            //TODO Check the condition
            var areThereConds = r.GetCondition() != null;
            if (areThereConds)
            {
                if (DoesConditionInvolvesThisGameObject(r.GetCondition(), gO))
                {
                    isRuleInvolved = true;
                    involvedRules.Add(r);
                    continue;
                }
            }

            // Check each action
            for (int i = 0; i < r.GetActions().Count; i++)
            {
                ThrowExceptionsIfActionNotValid(r.GetActions()[i], i);
                if (DoesActionInvolvesThisGameObject(r.GetActions()[i], gO))
                {
                    isRuleInvolved = true;
                    involvedRules.Add(r);
                    continue;
                }
            }
        }

        return involvedRules;
    }
}