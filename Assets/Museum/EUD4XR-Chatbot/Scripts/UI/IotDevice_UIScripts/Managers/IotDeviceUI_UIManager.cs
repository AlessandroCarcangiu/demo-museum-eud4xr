using ECARules4All_DLL.Utils;
using UnityEngine;

public class IotDeviceUI_UIManager  : UIGenericMenu, ISourceProvider<IotDevice>
{
    public void PlayerStartsTriggeringIotDevice(IotDevice iotDevice, Transform playerTransform)
    {
        dataSourceRef = iotDevice;
        
        Debug.Log("Player enters trigger with " + iotDevice.name);
        var currentCanvas = ShowDefaultCanvas();

        // var transformToMove = currentCanvas.transform;
        var transformToMove =  currentCanvas.transform.parent;
        
        // var d = (playerTransform.position - ecaObject.transform.position).normalized;
        // SetCanvasCloseToPlayer(transformToMove, playerTransform, d);
        SetCanvasCloseToTargetLookingAtPlayer(transformToMove, playerTransform, iotDevice.gameObject.transform);
    }
    
    public void PlayerStopsTriggeringIotDevice(IotDevice ecaObject, Transform playerTransform)
    {
        dataSourceRef = null;
        
        Debug.Log("Player leaves trigger");
        HideAll();
    }

    public IotDevice dataSourceRef { get; set; }

    public string GetDeepestTypePlusNameAsString(IotDevice iotDevice)
    {
        return $"The {iotDevice.gameObject.name}";
    }
    
}

// public class ECAObjectUI_UIManagerSingleton : Singleton<ECAObjectUI_UIManager, UIGenericMRTKMenu>
public class IotDeviceUI_UIManagerSingleton : Singleton<IotDeviceUI_UIManager, UIGenericMenu>
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Debug.Log($"{nameof(IotDeviceUI_UIManagerSingleton)} initialized.");
    }
}