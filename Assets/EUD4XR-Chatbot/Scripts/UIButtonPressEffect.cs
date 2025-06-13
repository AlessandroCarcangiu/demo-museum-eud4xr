using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonPressEffect : MonoBehaviour
{
    public Vector3 pressOffset = new Vector3(0, 0, -10f); 
    public float pressDuration = 0.5f; 

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private bool isPressed = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void PressButton()
    {
        if (!isPressed)
        {
            StartCoroutine(PressRoutine());
        }
    }

    private IEnumerator PressRoutine()
    {
        isPressed = true;
        rectTransform.anchoredPosition = originalPosition + pressOffset;

        yield return new WaitForSeconds(pressDuration);

        rectTransform.anchoredPosition = originalPosition;
        isPressed = false;
    }
}
