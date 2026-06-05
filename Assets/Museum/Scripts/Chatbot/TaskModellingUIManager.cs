using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class TaskModellingUIManager : MonoBehaviour
{
    public GameObject UIVersion1;
    public GameObject UIVersion2;
    public InputActionProperty toggleUIAction;

    private GameObject activeUI;

    public class UIVersion
    {
        public string taskModellingUIVersion;
    }
    
    private UIVersion currentUIVersion;
    

    void Awake()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "settings.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentUIVersion = JsonUtility.FromJson<UIVersion>(json);
            Debug.Log("TaskModellingUIManager loaded");
        }
    }

    void Start()
    {
        if (UIVersion1) UIVersion1.SetActive(false);
        if (UIVersion2) UIVersion2.SetActive(false);
    }

    void OnEnable()
    {
        toggleUIAction.action.Enable();
        toggleUIAction.action.started += Toggle;
    }

    void OnDisable()
    {
        toggleUIAction.action.started -= Toggle;
        toggleUIAction.action.Disable();
    }

    private void Toggle(InputAction.CallbackContext context)
    {
        ServerStarter server = FindFirstObjectByType<ServerStarter>();
    
        if (activeUI == null && currentUIVersion != null)
        {
            activeUI = (currentUIVersion.taskModellingUIVersion == "V2") ? UIVersion2 : UIVersion1;
        }

        if (activeUI) activeUI.SetActive(!activeUI.activeSelf);
    }
}