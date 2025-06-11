using ECARules4All_DLL;
using ECARules4All_DLL.Utils;
using UnityEngine;

// public class ECAObjectUI_UIManager : UIGenericMRTKMenu, ISourceProvider<ECAObject>
public class ECAObjectUI_UIManager : UIGenericMenu, ISourceProvider<ECAObject>
{
    public void PlayerStartsTriggeringEcaObject(ECAObject ecaObject, Transform playerTransform)
    {
        dataSourceRef = ecaObject;
        
        Debug.Log("Player enters trigger with " + ecaObject.name);
        var currentCanvas = ShowDefaultCanvas();

        // var transformToMove = currentCanvas.transform;
        var transformToMove =  currentCanvas.transform.parent;
        
        // var d = (playerTransform.position - ecaObject.transform.position).normalized;
        // SetCanvasCloseToPlayer(transformToMove, playerTransform, d);
        SetCanvasCloseToTargetLookingAtPlayer(transformToMove, playerTransform, ecaObject.gameObject.transform);
    }
    
    public void PlayerStopsTriggeringEcaObject(ECAObject ecaObject, Transform playerTransform)
    {
        dataSourceRef = null;
        
        Debug.Log("Player leaves trigger");
        HideAll();
    }

     public ECAObject dataSourceRef { get; set; }
}

// public class ECAObjectUI_UIManagerSingleton : Singleton<ECAObjectUI_UIManager, UIGenericMRTKMenu>
public class ECAObjectUI_UIManagerSingleton : Singleton<ECAObjectUI_UIManager, UIGenericMenu>
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("ECAObjectUI_UIManagerSingleton initialized.");
    }
}