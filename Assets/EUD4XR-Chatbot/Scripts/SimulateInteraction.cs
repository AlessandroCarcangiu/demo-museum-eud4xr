using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimulateInteraction : MonoBehaviour
{
    public Button propertiesButton;
    public Button rulesButton;
    public Button backToPropertiesButton;
    public Button backToRulesButton;

    private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(SimulateButtonInteraction(propertiesButton.gameObject));
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(SimulateButtonInteraction(rulesButton.gameObject));

        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(SimulateButtonInteraction(backToPropertiesButton.gameObject));

        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(SimulateButtonInteraction(backToRulesButton.gameObject));
    }

    IEnumerator SimulateButtonInteraction(GameObject button)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);

        // simulates hovering
        ExecuteEvents.Execute<IPointerEnterHandler>(button, pointerData, ExecuteEvents.pointerEnterHandler);
        Debug.Log("Hovered");
        yield return new WaitForSeconds(0.3f);

        // simulates pressing
        ExecuteEvents.Execute<IPointerDownHandler>(button, pointerData, ExecuteEvents.pointerDownHandler);
        Debug.Log("Pressed");
        yield return new WaitForSeconds(0.3f);

        // simulates releasing
        ExecuteEvents.Execute<IPointerUpHandler>(button, pointerData, ExecuteEvents.pointerUpHandler);
        Debug.Log("Released");
        yield return new WaitForSeconds(0.3f);

        // simulates click
        ExecuteEvents.Execute<IPointerClickHandler>(button, pointerData, ExecuteEvents.pointerClickHandler);
        Debug.Log("Clicked");
    }
}