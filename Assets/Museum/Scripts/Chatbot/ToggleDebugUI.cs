using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleDebugUI : MonoBehaviour
{
    public GameObject debugUI;
    public InputActionProperty toggleAction;

    void Update()
    {
        if (toggleAction.action.WasPressedThisFrame())
        {
            // Inverte lo stato (se è attiva la spegne, se è spenta la attiva)
            debugUI.SetActive(!debugUI.activeSelf);
        }
    }
}