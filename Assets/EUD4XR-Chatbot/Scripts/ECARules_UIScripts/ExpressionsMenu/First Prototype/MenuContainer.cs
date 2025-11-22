using ECARules4All_DLL.Utils;
using System;
using System.Collections;
using UnityEngine;

public class MenuContainer : Singleton<MenuContainer>
{
    public GameObject uiExpressionsPanel;
    public GameObject uiExpressionManager;

    private RectTransform rectTransform;
    private float deltaX = 210f;
    private float duration = 0.2f;

    private void Awake()
    {
        // Check for ref not null
        if (uiExpressionsPanel == null) throw new Exception("uiExpressionsPanel is null");
        if (uiExpressionManager == null) throw new Exception("uiExpressionManager is null");
        if ((rectTransform = GetComponent<RectTransform>()) == null) throw new Exception("rectTransform component not found");
    }

    private void Start()
    {
        // Adds listener to the go back button in the selected expression panel, expressions panel listeners will be dynamically added in B_Expression_Prefab
        uiExpressionManager.GetComponent<ExpressionManager>().uiGoBack.onClick.AddListener(() => SwitchMenu(false));
    }

    private IEnumerator MoveMenu(float deltaX, float duration)
    {
        Vector2 start = rectTransform.anchoredPosition;
        Vector2 target = start + new Vector2(deltaX, 0f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }
        rectTransform.anchoredPosition = target;
    }

    public void SwitchMenu(bool goForward)
    {
        if (goForward)
        {
            // Container is moving to the left, show selected expression panel
            uiExpressionManager.SetActive(true);
            StartCoroutine(MoveMenu(-deltaX, duration));
            uiExpressionsPanel.SetActive(false);
        }
        else
        {
            // Container is moving to the right, show expressions panel
            uiExpressionsPanel.SetActive(true);
            StartCoroutine(MoveMenu(deltaX, duration));
            uiExpressionManager.SetActive(false);
        }
    }
}
