using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIGenericMenu : MonoBehaviour
{
    [Serializable]
    public struct MenuItem
    {
        public Canvas canvas;
        public Button b_open;
        public Button b_close;
    }

    public Canvas defaultCanvas;

    public bool ShowDefaultCanvasOnAwake = true;

    // Make a public list of Tuples<Canvas, Button that opens that Canvas>
    public List<MenuItem> canvasList;

    protected void Awake()
    {
        foreach (var menuItem in canvasList)
        {
            // Reset position and rotation of the canvas
            menuItem.canvas.transform.localPosition = Vector3.zero;
            
            // Add a listener to the button that opens the canvas
            menuItem.b_open.onClick.AddListener(() => SwitchCanvas(menuItem.canvas));

            // Add a listener to the button that closes the canvas
            if (menuItem.b_close != null)
            {
                menuItem.b_close.onClick.AddListener(() => ShowDefaultCanvas());
            }
            else
            {
                Debug.LogError("[UIGenericMenu] b_close is null.");
            }
        }

        // Show the default canvas
        defaultCanvas.transform.localPosition = Vector3.zero;
        
        if (ShowDefaultCanvasOnAwake)
        {
            ShowDefaultCanvas();
        }
        else
        {
            HideAll();
        }

        this.OnAwake();
    }

    protected virtual void OnAwake()
    {
    }

    protected void HideAll()
    {
        defaultCanvas.gameObject.SetActive(false);

        foreach (var menuItem in canvasList)
        {
            menuItem.canvas.gameObject.SetActive(false);
        }
    }

    protected Canvas ShowDefaultCanvas()
    {
        return SwitchCanvas(defaultCanvas);
    }

    private Canvas SwitchCanvas(Canvas canvas)
    {
        HideAll();
        canvas.gameObject.SetActive(true);
        return canvas;
    }

    protected void SetCanvasCloseToPlayer(Transform canvasTransform, Transform playerTransform, Vector3 direction = default)
    {
        
        if (direction == default)
        {
            direction = playerTransform.forward;
        }
        // var direction__EcaObj_to_P = (playerTransform.position - ecaObjectTransform.position).normalized;
        Vector3 newPos = playerTransform.position - direction * 1.6f;
        newPos.y = 2f;
        canvasTransform.position = newPos;
        canvasTransform.LookAt(playerTransform.position + playerTransform.transform.rotation * Vector3.forward,
            playerTransform.rotation * Vector3.up);
    }

    protected void SetCanvasCloseToTargetLookingAtPlayer(Transform canvasTransform, Transform playerTransform,
        Transform targetTransform)
    {
        var direction = (playerTransform.position - targetTransform.position).normalized;
        Vector3 newPos = targetTransform.position + direction * targetTransform.localScale.z * 0.75f;
        newPos = playerTransform.position - direction * 1.6f;
        // newPos = ecaObject.gameObject.transform.position - ecaObject.gameObject.transform.forward * ecaObject.gameObject.transform.localScale.z / 2;
        newPos.y = 2f;
        canvasTransform.position = newPos;
        canvasTransform.LookAt(playerTransform.position + playerTransform.transform.rotation * Vector3.forward,
            playerTransform.rotation * Vector3.up);
        // rootToMove.transform.position = ecaObject.transform.position + CalcTransform(ecaObject.transform, playerTransform);
    }
}

public abstract class UIGenericMRTKMenu : MonoBehaviour
{
    [Serializable]
    public struct MenuItem
    {
        public Canvas canvas;
        public Button b_open;
        public Button b_close;
    }

    public Canvas defaultCanvas;

    public bool ShowDefaultCanvasOnAwake = true;

    // Make a public list of Tuples<Canvas, Button that opens that Canvas>
    public List<MenuItem> canvasList;

    protected void Awake()
    {
        foreach (var menuItem in canvasList)
        {
            // Reset position and rotation of the canvas
            menuItem.canvas.transform.localPosition = Vector3.zero;
            
            // Add a listener to the button that opens the canvas
            menuItem.b_open.onClick.AddListener(() => SwitchCanvas(menuItem.canvas));

            // Add a listener to the button that closes the canvas
            menuItem.b_close.onClick.AddListener(() => ShowDefaultCanvas());
        }

        // Show the default canvas
        defaultCanvas.transform.localPosition = Vector3.zero;
        
        if (ShowDefaultCanvasOnAwake)
        {
            ShowDefaultCanvas();
        }
        else
        {
            HideAll();
        }

        this.OnAwake();
    }

    protected virtual void OnAwake()
    {
    }

    protected void HideAll()
    {
        defaultCanvas.gameObject.SetActive(false);

        foreach (var menuItem in canvasList)
        {
            menuItem.canvas.gameObject.SetActive(false);
        }
    }

    protected Canvas ShowDefaultCanvas()
    {
        return SwitchCanvas(defaultCanvas);
    }

    private Canvas SwitchCanvas(Canvas canvas)
    {
        HideAll();
        canvas.gameObject.SetActive(true);
        return canvas;
    }

    protected void SetCanvasCloseToPlayer(Transform canvasTransform, Transform playerTransform, Vector3 direction = default)
    {
        
        if (direction == default)
        {
            direction = playerTransform.forward;
        }
        // var direction__EcaObj_to_P = (playerTransform.position - ecaObjectTransform.position).normalized;
        Vector3 newPos = playerTransform.position - direction * 1.6f;
        newPos.y = 2f;
        canvasTransform.position = newPos;
        canvasTransform.LookAt(playerTransform.position + playerTransform.transform.rotation * Vector3.forward,
            playerTransform.rotation * Vector3.up);
    }

    protected void SetCanvasCloseToTargetLookingAtPlayer(Transform canvasTransform, Transform playerTransform,
        Transform targetTransform)
    {
        var direction = (playerTransform.position - targetTransform.position).normalized;
        Vector3 newPos = targetTransform.position + direction * targetTransform.localScale.z * 0.75f;
        newPos = playerTransform.position - direction * 1.6f;
        // newPos = ecaObject.gameObject.transform.position - ecaObject.gameObject.transform.forward * ecaObject.gameObject.transform.localScale.z / 2;
        newPos.y = 2f;
        canvasTransform.position = newPos;
        canvasTransform.LookAt(playerTransform.position + playerTransform.transform.rotation * Vector3.forward,
            playerTransform.rotation * Vector3.up);
        // rootToMove.transform.position = ecaObject.transform.position + CalcTransform(ecaObject.transform, playerTransform);
    }
}