using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MyCustomButton : MonoBehaviour
{
    private Button button;
    private bool _isInteractable = true; // You may think it's redundant but it's not

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            throw new NullReferenceException($"Button component not found on the GameObject \"{gameObject.name}\".");
        }

        // if there are MyCustomButton components on the same GameObject, throw an exception
        var customButtons = GetComponents<MyCustomButton>();
        // if (customButtons.Length > 1)
        // {
        //     throw new Exception($"Multiple MyCustomButton components found on the GameObject \"{gameObject.name}\". Only one is allowed.");
        // }
    }

    /*void Update()
    {
        if (this.gameObject.name == "B_Properties")
        {
            button.onClick.Invoke();
        }
    }*/

    private void OnPrivateClicked()
    {
        if (!fakeEnabled) return;
        // Disable the eligibleForClick for one frame to prevent multiple clicks
        button.interactable = false;
        Debug.LogWarning("Button clicked, disabling for one frame");
        StartCoroutine(EnableButtonNextFrame()); //TODO Coroutine couldn't be started error sometimes
    }

    private IEnumerator EnableButtonNextFrame()
    {
        yield return null; // Wait for the next frame
        // yield return null; // Wait for the next frame
        if (_isInteractable)
            button.interactable = true;
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
        button.interactable = interactable;
    }

    public void AddOnClickListener(Action action)
    {
        button.onClick.AddListener(() => action());
    }
    bool fakeEnabled = true;
    public void RemoveAllListeners() => button.onClick.RemoveAllListeners();

    private void OnDisable()
    {
        fakeEnabled = false; 
        
        // Debug.LogWarning("DISABLED");
        button.onClick.RemoveListener(OnPrivateClicked);
        // turn of all coroutines
        // RemoveAllListeners();
    }

    public void SimulateClickForOnGUI()
    {
        button.onClick.Invoke();
    }
    
    
}