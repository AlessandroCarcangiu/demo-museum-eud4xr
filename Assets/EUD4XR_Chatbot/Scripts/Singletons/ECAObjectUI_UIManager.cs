using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using TMPro;
using UnityEngine;

public class ECAObjectUI_UIManager : UIGenericMenu
{
    public void PlayerStartsTriggeringEcaObject(ECAObject ecaObject, Transform playerTransform)
    {
        Debug.Log("Player enters trigger with " + ecaObject.name);
        var currentCanvas = ShowDefaultCanvas();

        // var transformToMove = currentCanvas.transform;
        var transformToMove =  currentCanvas.transform.parent;
        var d = (playerTransform.position - ecaObject.transform.position).normalized;
        
        
        // SetCanvasCloseToPlayer(transformToMove, playerTransform, d);
        SetCanvasCloseToTargetLookingAtPlayer(transformToMove, playerTransform, ecaObject.gameObject.transform);
    }
    
    public void PlayerStopsTriggeringEcaObject(ECAObject ecaObject, Transform playerTransform)
    {
        Debug.Log("Player leaves trigger");
        HideAll();
    }
    
    
    // public string Get()
     // {
         // RuleEngine.GetInstance().GetRulesInvolvingGameObject(GameObject.Find("Cylinder"));
     // }
    
}

public class ECAObjectUI_UIManagerSingleton : Singleton<ECAObjectUI_UIManager, UIGenericMenu>
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("ECAObjectUI_UIManagerSingleton initialized.");
    }
}