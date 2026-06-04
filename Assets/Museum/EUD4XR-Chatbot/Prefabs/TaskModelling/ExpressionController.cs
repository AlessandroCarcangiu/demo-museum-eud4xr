using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

[System.Serializable]
public class OrderData
{
    public string name;
    public List<string> order;
}

[System.Serializable]
public class ChoiceData
{
    public string name;
    public List<string> choice;
}

[System.Serializable]
public class IterationData
{
    public string name;
    public Iteration iteration;
}

[System.Serializable]
public class Iteration
{
    public int n_steps;
    public string expression;
}

[System.Serializable]
public class ConditionalData
{
    public string name;
    public Conditional conditional;
}

[System.Serializable]
public class Conditional
{
    public ConditionalBranch @if;
    public ConditionalBranch @else;
}

[System.Serializable]
public class ConditionalBranch
{
    public string trigger;
    public List<string> @do;
}

public class ExpressionController : MonoBehaviour
{
    public Button leftArrow;
    public Button rightArrow;
    public Transform container;
    public GameObject expressionPrefab;

    [Header("Deck Effect")] public int visibleCards = 3;
    [Range(0f, 0.04f)] public float stackOffsetYPercent = 0.01f;
    [Range(0f, 0.04f)] public float stackOffsetXPercent = 0.005f;
    public float animTime = 0.8f;

    [Header("Order Animation")] public float orderAnimTime = 0.6f;
    public float maxOrderSpacing = 1f;

    [Header("World Space Settings")] public bool autoSetupWorldSpace = true;
    public float worldSpaceScale = 0.01f;

    [Header("Order Text")] public TMP_Text orderText;

    [Header("Name Sequence Prefab")] public GameObject nameSequencePrefab;

    [Header("Step Expression Prefab")] public GameObject stepExpressionPrefab;

    [Header("Vertical Compact View UI")] public Button verticalUpArrow;
    public Button verticalDownArrow;
    public Button toggleViewButton;

    private List<object> sequence = new List<object>();
    private int currentIndex = 0;
    private bool isAnimating = false;

    private List<GameObject> expressions = new List<GameObject>();
    private List<GameObject> deckBackgroundCards = new List<GameObject>();
    private List<GameObject> orderCards = new List<GameObject>();
    private OrderData currentOrder = null;

    private Vector2 rightArrowInitialPos;
    private Vector2 leftArrowInitialPos;

    private TextMeshProUGUI nameExpressionText;
    private TextMeshProUGUI stepExpressionText;

    // Compact view state
    private bool compactView = true;
    private int compactIndex = 0;

    private int conditionalBranchIndex = 0; // 0 = if, 1 = else
    private int conditionalDoIndex = -1; // -1 = trigger, >=0 = automazione do

    // Stato per la vista estesa: quali rami sono espansi
    private bool isIfExpanded = false;
    private bool isElseExpanded = false;

    void Awake()
    {
        expressions.Clear();
        orderCards.Clear();
        deckBackgroundCards.Clear();
        sequence.Clear();
        if (expressionPrefab != null) expressionPrefab.SetActive(false);
    }

    void Start()
    {
        if (autoSetupWorldSpace)
            SetupWorldSpaceCanvas();
        //Inizializzazione frecce per cambiare elemento
        if (rightArrow != null)
            rightArrowInitialPos = rightArrow.GetComponent<RectTransform>().anchoredPosition;
        if (leftArrow != null)
            leftArrowInitialPos = leftArrow.GetComponent<RectTransform>().anchoredPosition;

        string path = Path.Combine(Application.streamingAssetsPath, "expressions.json");
        string json = File.ReadAllText(path);
        JObject root = JObject.Parse(json);
        string name = root["name"]?.ToString();

        if (!string.IsNullOrEmpty(name) && nameSequencePrefab != null)
        {
            nameExpressionText = nameSequencePrefab.transform.Find("Plate/Text").GetComponent<TextMeshProUGUI>();
            if (nameExpressionText != null)
                nameExpressionText.text = name;
        }

        if (stepExpressionPrefab != null)
        {
            stepExpressionText = stepExpressionPrefab.transform.Find("Plate/Text").GetComponent<TextMeshProUGUI>();
        }

        if (root["sequence"] != null && root["sequence"].Type == JTokenType.Array)
        {
            // Sequence
            JArray arr = (JArray)root["sequence"];
            if (stepExpressionText != null)
                stepExpressionText.text = "Passo " + (currentIndex + 1) + " di " + arr.Count;

            foreach (var token in arr)
            {
                if (token.Type == JTokenType.String)
                    sequence.Add(token.ToString());
                else if (token.Type == JTokenType.Object)
                {
                    JObject obj = (JObject)token;
                    if (obj["order"] != null)
                        sequence.Add(obj.ToObject<OrderData>());
                    else if (obj["choice"] != null)
                        sequence.Add(obj.ToObject<ChoiceData>());
                    else if (obj["iteration"] != null)
                        sequence.Add(obj.ToObject<IterationData>());
                    else if (obj["conditional"] != null)
                        sequence.Add(obj.ToObject<ConditionalData>());
                }
            }

            if (stepExpressionText != null)
                stepExpressionText.text = "Passo " + (currentIndex + 1) + " di " + sequence.Count;
        }
        else if (root["conditional"] != null && root["conditional"].Type == JTokenType.Object)
        {
            //Elemento conditional separato dalla sequence
            sequence.Add(root.ToObject<ConditionalData>());
            if (stepExpressionText != null)
                stepExpressionText.text = "Passo 1 di 1";
        }
        else
        {
            Debug.LogError("Formato JSON non riconosciuto!");
        }
        //disabilitato e mostrato solamente in caso di elementi complessi
        orderText.gameObject.SetActive(false);
        ShowCurrent();

        leftArrow.onClick.AddListener(OnLeftArrow);
        rightArrow.onClick.AddListener(OnRightArrow);

        if (toggleViewButton != null)
        {
            toggleViewButton.onClick.AddListener(() =>
            {
                compactView = !compactView;
                compactIndex = 0;
                // Reset stato conditional
                conditionalBranchIndex = 0;
                conditionalDoIndex = -1;
                ResetConditionalExpandedState();
                ShowCurrent();
            });
        }

        if (verticalUpArrow != null)
            verticalUpArrow.onClick.AddListener(ScrollUp);
        if (verticalDownArrow != null)
            verticalDownArrow.onClick.AddListener(ScrollDown);

        UpdateArrowInteractable();
    }

    void SetupWorldSpaceCanvas()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            RectTransform canvasRT = canvas.transform as RectTransform;
            if (canvasRT != null)
            {
                canvasRT.localScale = Vector3.one * worldSpaceScale;
                if (canvasRT.position == Vector3.zero)
                {
                    Camera mainCam = Camera.main;
                    if (mainCam != null)
                    {
                        Vector3 cameraPos = mainCam.transform.position;
                        Vector3 cameraForward = mainCam.transform.forward;
                        canvasRT.position = cameraPos + cameraForward * 5f;
                        canvasRT.LookAt(cameraPos);
                        canvasRT.Rotate(0, 180, 0);
                    }
                }
            }
        }
    }

    void UpdateStepExpressionText()
    {
        if (stepExpressionText == null)
            return;

        object current = sequence.Count > 0 ? sequence[currentIndex] : null;
        //Se l'elemento è conditional il campo stepExpressionText viene aggiornato in base al tipo di ramo, non contano i passi
        if (current is ConditionalData cond)
        {
            if (compactView)
            {
                if (conditionalBranchIndex == 0)
                {
                    if (conditionalDoIndex == -1)
                        stepExpressionText.text = "SE";
                    else
                        stepExpressionText.text = "ALLORA";
                }
                else
                {
                    if (conditionalDoIndex == -1)
                        stepExpressionText.text = "ALTRIMENTI";
                    else
                        stepExpressionText.text = "ALLORA";
                }
            }
            else
            {
                stepExpressionText.text = "SE / ALTRIMENTI";
            }
        }
        else
        {  
            // Per ogni altro elemento vengono mostrati i passi della sequenza
            stepExpressionText.text = "Passo " + (currentIndex + 1) + " di " + sequence.Count;
        }
    }

    void UpdateToggleViewButtonActive()
    {
        // Attivo solo se:
        // - Ci si trova in una sequenza e la card attuale è OrderData, ChoiceData, ConditionalData
        // - Oppure se c'è un solo ConditionalData in tutta la sequenza

        bool shouldBeActive = false;
        if (sequence.Count == 1 && sequence[0] is ConditionalData)
        {
            shouldBeActive = true;
        }
        else if (sequence.Count > 0)
        {
            object current = sequence[currentIndex];
            if (current is OrderData || current is ChoiceData || current is ConditionalData)
            {
                shouldBeActive = true;
            }
        }
        if (toggleViewButton != null)
            toggleViewButton.gameObject.SetActive(shouldBeActive);
    }
    

    void ShowCurrent()
{
    // Reset delle frecce
    if (rightArrow != null)
        rightArrow.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(rightArrowInitialPos.x, rightArrowInitialPos.y);
    if (leftArrow != null)
        leftArrow.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(leftArrowInitialPos.x, leftArrowInitialPos.y);

    // Distrugge le card precedenti
    foreach (var go in expressions) Destroy(go);
    expressions.Clear();
    foreach (var go in orderCards) Destroy(go);
    orderCards.Clear();
    currentOrder = null;
    foreach (var go in deckBackgroundCards) Destroy(go);
    deckBackgroundCards.Clear();

    if (sequence.Count == 0) return;

    HashSet<string> used = new HashSet<string>();
    List<string> currentAutomations = new List<string>();
    object current = sequence[currentIndex];

    // Controllo sull'elemento corrente della sequenza
    if (current is string s)
        currentAutomations.Add(s);
    else if (current is OrderData ord)
        currentAutomations.AddRange(ord.order);
    else if (current is ChoiceData choice)
        currentAutomations.AddRange(choice.choice);
    else if (current is IterationData iter)
        currentAutomations.Add(iter.iteration.expression);
    else if (current is ConditionalData cond)
    {
        if (cond.conditional?.@if != null)
        {
            currentAutomations.Add(cond.conditional.@if.trigger);
            currentAutomations.AddRange(cond.conditional.@if.@do);
        }
        if (cond.conditional?.@else != null)
        {
            currentAutomations.Add(cond.conditional.@else.trigger);
            currentAutomations.AddRange(cond.conditional.@else.@do);
        }
    }

    foreach (var a in currentAutomations)
        used.Add(a);

    // Preparazione sfondo card
    int bgRequired = Mathf.Max(visibleCards - 1, 2);
    int bgCount = 0;
    int idx = currentIndex - 1;

    while (bgCount < bgRequired)
    {
        if (idx < 0)
        {
            for (int i = 0; i < sequence.Count && bgCount < bgRequired; i++)
            {
                if (i == currentIndex) continue;
                foreach (string bgAuto in AutomationsForBackground(sequence[i], used))
                {
                    if (bgCount < bgRequired)
                    {
                        var card = CreateExpression(bgAuto, true);
                        AddBackground(card, bgCount + 1);
                        used.Add(bgAuto);
                        bgCount++;
                    }
                }
            }
            break;
        }
        else
        {
            foreach (string bgAuto in AutomationsForBackground(sequence[idx], used))
            {
                if (bgCount < bgRequired)
                {
                    var card = CreateExpression(bgAuto, true);
                    AddBackground(card, bgCount + 1);
                    used.Add(bgAuto);
                    bgCount++;
                }
            }
            idx--;
        }
    }

    UpdateStepExpressionText();
    UpdateToggleViewButtonActive();

    // AUTOMATION 
    if (current is string fgAuto)
    {
        var expr = CreateExpression(fgAuto, false);
        expressions.Add(expr);
        RectTransform frontRT = expr.GetComponent<RectTransform>();
        frontRT.anchoredPosition = GetDeckPosition(0, frontRT.rect.width, frontRT.rect.height);
        frontRT.localRotation = Quaternion.identity;
        frontRT.localScale = Vector3.one;
        expr.transform.SetAsLastSibling();
        orderText.gameObject.SetActive(false);
        SetCompactControlsActive(false, false, false);
    }
    // ORDER
    else if (current is OrderData ord2)
    {
        currentOrder = ord2;
        if (compactView)
        {
            compactIndex = Mathf.Clamp(compactIndex, 0, ord2.order.Count - 1);
            var expr = CreateExpression(ord2.order[compactIndex], false);
            expressions.Add(expr);
            RectTransform rt = expr.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            expr.transform.SetAsLastSibling();
            SetCompactControlsActive(true, compactIndex > 0, compactIndex < ord2.order.Count - 1);
        }
        else
        {
            compactIndex = 0;
            SetCompactControlsActive(true, false, false);
            for (int i = 0; i < ord2.order.Count; i++)
            {
                GameObject expr = CreateExpression(ord2.order[i], false);
                expr.SetActive(false);
                RectTransform rt = expr.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                orderCards.Add(expr);
            }

            RectTransform cardRT = orderCards.Count > 0 ? orderCards[0].GetComponent<RectTransform>() : null;
            float w = cardRT != null ? cardRT.rect.width : 200f;
            float h = cardRT != null ? cardRT.rect.height : 200f;
            StartCoroutine(AnimateOrderCardsFromCenter(orderCards, w, h, orderAnimTime));
        }

        orderText.gameObject.SetActive(true);
        orderText.text = "Scegli l'automazione che vuoi\n(non conta l'ordine)";
    }
    // CHOICE 
    else if (current is ChoiceData choiceData)
    {
        if (compactView)
        {
            compactIndex = Mathf.Clamp(compactIndex, 0, choiceData.choice.Count - 1);
            var expr = CreateExpression(choiceData.choice[compactIndex], false);
            expressions.Add(expr);
            RectTransform rt = expr.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            expr.transform.SetAsLastSibling();
            SetCompactControlsActive(true, compactIndex > 0, compactIndex < choiceData.choice.Count - 1);
        }
        else
        {
            compactIndex = 0;
            SetCompactControlsActive(true, false, false);
            StartCoroutine(AnimateChoiceCardsFromCenter(choiceData));
        }

        orderText.gameObject.SetActive(true);
        orderText.text = "Scegli una sola automazione";
    }
    // ITERATION
    else if (current is IterationData iterationData)
    {
        var expr = CreateExpression(iterationData.iteration.expression, false);
        expressions.Add(expr);
        RectTransform rt = expr.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;
        var numTimes = expr.transform.Find("IterationInfo");
        numTimes.gameObject.SetActive(true);
        numTimes.transform.Find("Times").GetComponent<TMP_Text>().text = $"x{iterationData.iteration.n_steps}";
        expr.transform.SetAsLastSibling();

        orderText.gameObject.SetActive(true);
        orderText.text = $"L'iterazione verrà eseguita {iterationData.iteration.n_steps} volte";
        SetCompactControlsActive(false, false, false);
    }
    // CONDITIONAL 
    else if (current is ConditionalData conditionalData)
    {
        if (compactView)
        {
            if (conditionalBranchIndex != 0 && conditionalBranchIndex != 1)
                conditionalBranchIndex = 0;
            var branch = (conditionalBranchIndex == 0)
                ? conditionalData.conditional.@if
                : conditionalData.conditional.@else;
            int doCount = branch.@do != null ? branch.@do.Count : 0;
            if (conditionalDoIndex < -1) conditionalDoIndex = -1;
            if (conditionalDoIndex > doCount - 1) conditionalDoIndex = doCount - 1;

            string display;
            if (conditionalDoIndex == -1)
                display = branch.trigger;
            else
                display = branch.@do[conditionalDoIndex];

            var expr = CreateExpression(display, false, null);
            expressions.Add(expr);
            RectTransform rt = expr.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            expr.transform.SetAsLastSibling();

            if (leftArrow != null) leftArrow.gameObject.SetActive(true);
            if (rightArrow != null) rightArrow.gameObject.SetActive(true);

            bool showUp = conditionalDoIndex > -1;
            bool showDown = (conditionalDoIndex < doCount - 1 && conditionalDoIndex >= 0) ||
                            (conditionalDoIndex == -1 && doCount > 0);
            if (verticalUpArrow != null) verticalUpArrow.gameObject.SetActive(showUp);
            if (verticalDownArrow != null) verticalDownArrow.gameObject.SetActive(showDown);

           
        }
        else
        {
            SetCompactControlsActive(true, false, false);

            // Disabilita le frecce nella vista estesa
            if (leftArrow != null) leftArrow.gameObject.SetActive(false);
            if (rightArrow != null) rightArrow.gameObject.SetActive(false);

            float cardWidth = (expressionPrefab != null)
                ? expressionPrefab.GetComponent<RectTransform>().rect.width
                : 200f;
            float distX = cardWidth + maxOrderSpacing * 0.5f;

            // ramo IF
            var exprIf = CreateExpression(conditionalData.conditional.@if.trigger, false, null);
            expressions.Add(exprIf);
            var rtIf = exprIf.GetComponent<RectTransform>();
            rtIf.anchoredPosition = Vector2.zero;
            rtIf.localRotation = Quaternion.identity;
            rtIf.localScale = Vector3.one;
            exprIf.transform.SetAsLastSibling();

            // ArrowDown per il ramo if
            var arrowIf = exprIf.transform.Find("ArrowDown");
            if (arrowIf != null)
            {
                var btn = arrowIf.GetComponent<Button>();
                arrowIf.gameObject.SetActive(true);
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    isIfExpanded = !isIfExpanded;
                    ShowCurrent();
                });
                arrowIf.localRotation = Quaternion.Euler(0, 0, isIfExpanded ? 180 : 0);
            }

            float cardHeightIf = rtIf != null ? rtIf.rect.height : 200f;

            // Aggiunta delle card del ramo if sotto l'automazione trigger
            float yOffsetIf = 0f;
            if (isIfExpanded && conditionalData.conditional.@if.@do != null)
            {
                for (int i = 0; i < conditionalData.conditional.@if.@do.Count; i++)
                {
                    var doExpr = CreateExpression(conditionalData.conditional.@if.@do[i], false, null);
                    expressions.Add(doExpr);
                    var doRT = doExpr.GetComponent<RectTransform>();
                    doRT.anchoredPosition = new Vector2(0, -cardHeightIf * (i + 1));
                    doRT.localRotation = Quaternion.identity;
                    doRT.localScale = Vector3.one;
                    doExpr.transform.SetAsLastSibling();
                    yOffsetIf = -cardHeightIf * (i + 1);
                }
            }

            // Ramo ELSE
            var exprElse = CreateExpression(conditionalData.conditional.@else.trigger, false, null);
            expressions.Add(exprElse);
            var rtElse = exprElse.GetComponent<RectTransform>();
            rtElse.anchoredPosition = new Vector2(distX, 0f); // Allineato verticalmente con il ramo IF
            rtElse.localRotation = Quaternion.identity;
            rtElse.localScale = Vector3.one;
            exprElse.transform.SetAsLastSibling();

            // ArrowDown per ramo ELSE
            var arrowElse = exprElse.transform.Find("ArrowDown");
            if (arrowElse != null)
            {
                var btn = arrowElse.GetComponent<Button>();
                arrowElse.gameObject.SetActive(true);
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    isElseExpanded = !isElseExpanded;
                    ShowCurrent();
                });
                arrowElse.localRotation = Quaternion.Euler(0, 0, isElseExpanded ? 180 : 0);
            }

            float cardHeightElse = rtElse != null ? rtElse.rect.height : 200f;

            // Aggiunta delle card del ramo else sotto l'automazione trigger
            if (isElseExpanded && conditionalData.conditional.@else.@do != null)
            {
                for (int i = 0; i < conditionalData.conditional.@else.@do.Count; i++)
                {
                    var doExpr = CreateExpression(conditionalData.conditional.@else.@do[i], false, null);
                    expressions.Add(doExpr);
                    var doRT = doExpr.GetComponent<RectTransform>();
                    doRT.anchoredPosition = new Vector2(distX, -cardHeightElse * (i + 1));
                    doRT.localRotation = Quaternion.identity;
                    doRT.localScale = Vector3.one;
                    doExpr.transform.SetAsLastSibling();
                }
            }
        }

        orderText.gameObject.SetActive(true);
        orderText.text = "Condizione IF / ELSE";
    }
}
    
    
    
    void SetCompactControlsActive(bool showToggle, bool showUp, bool showDown)
    {
        UpdateToggleViewButtonActive();
        if (verticalUpArrow != null)
            verticalUpArrow.gameObject.SetActive(showUp);
        if (verticalDownArrow != null)
            verticalDownArrow.gameObject.SetActive(showDown);
    }

    void ScrollUp()
    {
        // Animazione per scorrimento verso l'alto
        if (isAnimating) return;
        object current = sequence[currentIndex];
        int max = 0;
        if (current is OrderData od && compactIndex > 0)
        {
            max = od.order.Count;
            string oldAutomation = od.order[compactIndex];
            string newAutomation = od.order[compactIndex - 1];
            compactIndex--;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, false));
        }
        else if (current is ChoiceData cd && compactIndex > 0)
        {
            max = cd.choice.Count;
            string oldAutomation = cd.choice[compactIndex];
            string newAutomation = cd.choice[compactIndex - 1];
            compactIndex--;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, false));
        }
        else if (current is ConditionalData cond)
        {
            var branch = (conditionalBranchIndex == 0) ? cond.conditional.@if : cond.conditional.@else;
            if (conditionalDoIndex > -1)
            {
                string oldAutomation = branch.@do[conditionalDoIndex];
                string newAutomation;
                conditionalDoIndex--;
                if (conditionalDoIndex == -1)
                    newAutomation = branch.trigger;
                else
                    newAutomation = branch.@do[conditionalDoIndex];
                StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, false));
            }

            UpdateConditionalArrows(branch);
        }
    }

    void ScrollDown()
    {
        // Animazione per scorrimento verso il basso
        if (isAnimating) return;
        object current = sequence[currentIndex];
        int max = 0;
        if (current is OrderData od && compactIndex < od.order.Count - 1)
        {
            // Caso per scorrimento verso il basso in un order indipendence
            max = od.order.Count;
            string oldAutomation = od.order[compactIndex];
            string newAutomation = od.order[compactIndex + 1];
            compactIndex++;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, true));
        }
        else if (current is ChoiceData cd && compactIndex < cd.choice.Count - 1)
        {
            // Caso per scorrimento verso il basso in una choice 
            max = cd.choice.Count;
            string oldAutomation = cd.choice[compactIndex];
            string newAutomation = cd.choice[compactIndex + 1];
            compactIndex++;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, true));
        }
        else if (current is ConditionalData cond)
        {
            // Caso per scorrimento verso il basso in un conditional 
            var branch = (conditionalBranchIndex == 0) ? cond.conditional.@if : cond.conditional.@else;
            int doCount = branch.@do != null ? branch.@do.Count : 0;
            if (conditionalDoIndex == -1 && doCount > 0)
            {
                string oldAutomation = branch.trigger;
                string newAutomation = branch.@do[0];
                conditionalDoIndex = 0;
                StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, true));
            }
            else if (conditionalDoIndex >= 0 && conditionalDoIndex < doCount - 1)
            {
                string oldAutomation = branch.@do[conditionalDoIndex];
                string newAutomation = branch.@do[conditionalDoIndex + 1];
                conditionalDoIndex++;
                StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, true));
            }

            UpdateConditionalArrows(branch);
        }
    }
    IEnumerator AnimateCompactCardChange(string oldAutomation, string newAutomation, bool toDown)
    {
        // Animazione per il cambio di card in modalità compatta
        isAnimating = true;

        GameObject oldCard = CreateExpression(oldAutomation, false);
        RectTransform oldCardRT = oldCard.GetComponent<RectTransform>();
        oldCardRT.anchoredPosition = Vector2.zero;
        oldCardRT.localRotation = Quaternion.identity;
        oldCardRT.localScale = Vector3.one;
        oldCard.transform.SetAsLastSibling();

        GameObject newCard = CreateExpression(newAutomation, false, null, oldCardRT);
        float height = newCard.GetComponent<RectTransform>().rect.height;

        float verticalOffset = height * 0.9f;
        Vector2 startPos = toDown ? new Vector2(0, verticalOffset) : new Vector2(0, -verticalOffset);
        Vector2 endPos = Vector2.zero;
        newCard.GetComponent<RectTransform>().anchoredPosition = startPos;
        newCard.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        newCard.GetComponent<RectTransform>().localScale = Vector3.one;
        newCard.transform.SetAsLastSibling();

        float duration = 0.24f;
        float elapsed = 0f;
        var oldRT = oldCard.GetComponent<RectTransform>();
        var newRT = newCard.GetComponent<RectTransform>();
        var oldCG = oldCard.GetComponent<CanvasGroup>();
        var newCG = newCard.GetComponent<CanvasGroup>();
        if (oldCG == null) oldCG = oldCard.AddComponent<CanvasGroup>();
        if (newCG == null) newCG = newCard.AddComponent<CanvasGroup>();
        oldCG.alpha = 1f;
        newCG.alpha = 0f;
        // Inizio animazione
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector2 oldEnd = toDown ? new Vector2(0, -verticalOffset) : new Vector2(0, verticalOffset);
            oldRT.anchoredPosition = Vector2.Lerp(Vector2.zero, oldEnd, t);
            newRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            oldCG.alpha = Mathf.Lerp(1f, 0f, t);
            newCG.alpha = Mathf.Lerp(0f, 1f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        oldRT.anchoredPosition = toDown ? new Vector2(0, -verticalOffset) : new Vector2(0, verticalOffset);
        newRT.anchoredPosition = endPos;
        oldCG.alpha = 0f;
        newCG.alpha = 1f;

        Destroy(oldCard);

        foreach (var go in expressions) Destroy(go);
        expressions.Clear();
        expressions.Add(newCard);

        SetCompactControlsActive(true,
            compactIndex > 0,
            (sequence[currentIndex] is OrderData od && compactIndex < od.order.Count - 1)
            || (sequence[currentIndex] is ChoiceData cd && compactIndex < cd.choice.Count - 1)
        );

        object curr = sequence[currentIndex];
        if (curr is ConditionalData cdata && compactView)
        {
            var branch = (conditionalBranchIndex == 0) ? cdata.conditional.@if : cdata.conditional.@else;
            UpdateConditionalArrows(branch);
        }

        UpdateStepExpressionText();

        isAnimating = false;
    }

    IEnumerator AnimateConditionalBranchHorizontal(string oldAutomation, string newAutomation, bool toElse)
    {
        // Animazione per il cambio di card in un ramo di conditional
        isAnimating = true;

        GameObject oldCard = CreateExpression(oldAutomation, false);
        RectTransform oldCardRT = oldCard.GetComponent<RectTransform>();
        oldCardRT.anchoredPosition = Vector2.zero;
        oldCardRT.localRotation = Quaternion.identity;
        oldCardRT.localScale = Vector3.one;
        oldCard.transform.SetAsLastSibling();
        // Creazione della nuova card
        GameObject newCard = CreateExpression(newAutomation, false, null, oldCardRT);
        float width = newCard.GetComponent<RectTransform>().rect.width;

        float horizontalOffset = width * 0.9f;
        Vector2 startPos = toElse ? new Vector2(horizontalOffset, 0) : new Vector2(-horizontalOffset, 0);
        Vector2 endPos = Vector2.zero;
        newCard.GetComponent<RectTransform>().anchoredPosition = startPos;
        newCard.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        newCard.GetComponent<RectTransform>().localScale = Vector3.one;
        newCard.transform.SetAsLastSibling();
        
        float duration = 0.24f;
        float elapsed = 0f;
        var oldRT = oldCard.GetComponent<RectTransform>();
        var newRT = newCard.GetComponent<RectTransform>();
        var oldCG = oldCard.GetComponent<CanvasGroup>();
        var newCG = newCard.GetComponent<CanvasGroup>();
        if (oldCG == null) oldCG = oldCard.AddComponent<CanvasGroup>();
        if (newCG == null) newCG = newCard.AddComponent<CanvasGroup>();
        oldCG.alpha = 1f;
        newCG.alpha = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector2 oldEnd = toElse ? new Vector2(-horizontalOffset, 0) : new Vector2(horizontalOffset, 0);
            oldRT.anchoredPosition = Vector2.Lerp(Vector2.zero, oldEnd, t);
            newRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            oldCG.alpha = Mathf.Lerp(1f, 0f, t);
            newCG.alpha = Mathf.Lerp(0f, 1f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        oldRT.anchoredPosition = toElse ? new Vector2(-horizontalOffset, 0) : new Vector2(horizontalOffset, 0);
        newRT.anchoredPosition = endPos;
        oldCG.alpha = 0f;
        newCG.alpha = 1f;
        // Distruzione della vecchia card
        Destroy(oldCard);

        foreach (var go in expressions) Destroy(go);
        expressions.Clear();
        expressions.Add(newCard);

        SetCompactControlsActive(true, false, false);

        object current = sequence[0];
        if (current is ConditionalData cond)
        {
            ConditionalBranch branch = conditionalBranchIndex == 0 ? cond.conditional.@if : cond.conditional.@else;
            UpdateConditionalArrows(branch);
        }

        UpdateStepExpressionText();

        isAnimating = false;
    }

    IEnumerator AnimateCompactStepTransition(bool forward)
    {
        // Animazione per il cambio di step in modalità compatta
        isAnimating = true;
        
        object current = sequence[currentIndex];
        string currentAutomation = "";
        string currentPrefix = null;
        if (current is string s)
            currentAutomation = s;
        else if (current is OrderData od && od.order.Count > 0)
            currentAutomation = od.order[0];
        else if (current is ChoiceData cd && cd.choice.Count > 0)
            currentAutomation = cd.choice[0];
        else if (current is IterationData it)
            currentAutomation = it.iteration.expression;
        else if (current is ConditionalData cond)
            currentAutomation = cond.conditional?.@if?.trigger ?? "";
        
        GameObject currentCard = null;
        if (!string.IsNullOrEmpty(currentAutomation))
        {
            
            currentCard = CreateExpression(currentAutomation, false, currentPrefix);
            RectTransform currentRT = currentCard.GetComponent<RectTransform>();
            currentRT.anchoredPosition = Vector2.zero;
            currentRT.localRotation = Quaternion.identity;
            currentRT.localScale = Vector3.one;
            currentCard.transform.SetAsLastSibling();
        }

        currentIndex += forward ? 1 : -1;
        compactIndex = 0;
        // Reset stato conditional
        conditionalBranchIndex = 0;
        conditionalDoIndex = -1;

        object newStep = sequence[currentIndex];
        string newAutomation = "";
        string newPrefix = null;
        if (newStep is string ns)
            newAutomation = ns;
        else if (newStep is OrderData nod && nod.order.Count > 0)
            newAutomation = nod.order[0];
        else if (newStep is ChoiceData ncd && ncd.choice.Count > 0)
            newAutomation = ncd.choice[0];
        else if (newStep is IterationData nit)
            newAutomation = nit.iteration.expression;
        else if (newStep is ConditionalData ncond)
            newAutomation = ncond.conditional?.@if?.trigger ?? "";

        GameObject newCard = null;
        if (!string.IsNullOrEmpty(newAutomation))
        {
                        
            newCard = CreateExpression(newAutomation, false, newPrefix);
            RectTransform newRT = newCard.GetComponent<RectTransform>();
            float cardW = newRT.rect.width;
            float cardH = newRT.rect.height;
            int stackPos = visibleCards - 1;
            Vector3 deckPos = GetDeckPosition(stackPos, cardW, cardH);
            Vector2 startPos = (Vector2)deckPos;
            startPos.x += forward ? 10f : -10f;
            newRT.anchoredPosition = startPos;
            newRT.localRotation = Quaternion.identity;
            newRT.localScale = Vector3.one * 0.95f;
            newCard.transform.SetAsLastSibling();
            var cg = newCard.GetComponent<CanvasGroup>();
            if (cg == null) cg = newCard.AddComponent<CanvasGroup>();
            cg.alpha = 0.35f;
        }
        
        float duration = 0.28f;
        float elapsed = 0f;

        Vector2 center = Vector2.zero;
        Vector3 scaleFrom = Vector3.one * 0.95f;
        Vector3 scaleTo = Vector3.one;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = Mathf.SmoothStep(0, 1, t);
            // Interpolazione della posizione e scala delle card
            if (currentCard != null)
            {
                var currentRT = currentCard.GetComponent<RectTransform>();
                var currentCG = currentCard.GetComponent<CanvasGroup>();
                if (currentCG == null) currentCG = currentCard.AddComponent<CanvasGroup>();
                float cardW = currentRT.rect.width;
                float cardH = currentRT.rect.height;
                Vector3 outPos = GetDeckPosition(visibleCards - 1, cardW, cardH);
                outPos.x += forward ? -10f : 10f;
                currentRT.anchoredPosition = Vector2.Lerp(center, (Vector2)outPos, eased);
                currentRT.localScale = Vector3.Lerp(Vector3.one, scaleFrom, eased);
                currentCG.alpha = Mathf.Lerp(1f, 0.35f, eased);
            }
            // Interpolazione della nuova card
            if (newCard != null)
            {
                var newRT = newCard.GetComponent<RectTransform>();
                var newCG = newCard.GetComponent<CanvasGroup>();
                if (newCG == null) newCG = newCard.AddComponent<CanvasGroup>();
                float cardW = newRT.rect.width;
                float cardH = newRT.rect.height;
                Vector3 fromPos = GetDeckPosition(visibleCards - 1, cardW, cardH);
                fromPos.x += forward ? 10f : -10f;
                newRT.anchoredPosition = Vector2.Lerp((Vector2)fromPos, center, eased);
                newRT.localScale = Vector3.Lerp(scaleFrom, Vector3.one, eased);
                newCG.alpha = Mathf.Lerp(0.35f, 1f, eased);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (currentCard != null) Destroy(currentCard);
        // Distruzione della vecchia card
        foreach (var go in expressions) Destroy(go);
        expressions.Clear();
        if (newCard != null) expressions.Add(newCard);
        
        ShowCurrent();
        UpdateArrowInteractable();
        isAnimating = false;
    }

    IEnumerable<string> AutomationsForBackground(object entry, HashSet<string> used)
    {
        // Genera le automazioni da usare come sfondo per le card
        if (entry is string s)
        {
            if (!used.Contains(s))
                yield return s;
        }
        else if (entry is OrderData ord && ord.order.Count > 0)
        {
            // Se è un OrderData, restituisce tutte le automazioni nell'ordine
            foreach (string oauto in ord.order)
                if (!used.Contains(oauto))
                    yield return oauto;
        }
        else if (entry is ChoiceData choice && choice.choice.Count > 0)
        {
            // Se è un ChoiceData, restituisce tutte le automazioni nella scelta
            foreach (string cauto in choice.choice)
                if (!used.Contains(cauto))
                    yield return cauto;
        }
        else if (entry is IterationData iter && !used.Contains(iter.iteration.expression))
        {
            // Se è un IterationData, restituisce l'espressione dell'iterazione
            yield return iter.iteration.expression;
        }
        else if (entry is ConditionalData cond)
        {
            // Se è un ConditionalData, restituisce i trigger e le do di IF/ELSE
            if (!used.Contains(cond.conditional.@if.trigger))
                yield return cond.conditional.@if.trigger;
            foreach (var d in cond.conditional.@if.@do)
                if (!used.Contains(d))
                    yield return d;
            if (!used.Contains(cond.conditional.@else.trigger))
                yield return cond.conditional.@else.trigger;
            foreach (var d in cond.conditional.@else.@do)
                if (!used.Contains(d))
                    yield return d;
        }
    }

    void AddBackground(GameObject card, int stackIndex)
    {
        // Aggiunge una card di sfondo al mazzo delle card
        RectTransform backRT = card.GetComponent<RectTransform>();
        float cardW = backRT.rect.width;
        float cardH = backRT.rect.height;
        backRT.anchoredPosition = GetDeckPosition(stackIndex, cardW, cardH);
        backRT.localRotation = Quaternion.identity;
        backRT.localScale = Vector3.one;
        card.transform.SetAsFirstSibling();
        deckBackgroundCards.Add(card);
    }

    GameObject CreateExpression(string automation, bool isBackground = false, string prefix = null,
        RectTransform reference = null)
    {
        // Crea un GameObject per l'espressione dell'automazione
        GameObject expr = Instantiate(expressionPrefab, container);
        expr.SetActive(true);
        RectTransform rect = expr.GetComponent<RectTransform>();
        if (reference != null)
        {
            rect.anchoredPosition = reference.anchoredPosition;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }
        else
        {
            
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        SetExpressionName(expr, automation, isBackground, prefix);
        var cg = expr.GetComponent<CanvasGroup>();
        if (cg == null) cg = expr.AddComponent<CanvasGroup>();
        cg.alpha = isBackground ? 0.35f : 1f;
        return expr;
    }

    void SetExpressionName(GameObject expr, string automation, bool isBackground = false, string prefix = null)
    {
        TMP_Text nameText = expr.transform.Find("NameAutomation")?.GetComponent<TMP_Text>();
        if (nameText != null)
        {
            string displayName = automation.Contains(".")
                ? automation.Substring(automation.IndexOf('.') + 1)
                : automation;
            string pfx = prefix != null ? prefix + ": " : (isBackground ? " " : "Automazione: ");
            nameText.text = pfx + displayName;
        }
    }

    private void SwitchConditionalBranch(bool toElse)
    {
        // Funzione per cambiare il ramo di un conditional
        object current = sequence[0];
        if (current is ConditionalData cond)
        {
            int newBranch = toElse ? 1 : 0;
            if (conditionalBranchIndex != newBranch)
            {
                string oldAutomation = GetConditionalDisplay(cond, conditionalBranchIndex, conditionalDoIndex);
                string newAutomation = GetConditionalDisplay(cond, newBranch, -1);
                conditionalBranchIndex = newBranch;
                conditionalDoIndex = -1; // reset su trigger quando cambio ramo
                StartCoroutine(AnimateConditionalBranchHorizontal(oldAutomation, newAutomation, toElse));
                UpdateConditionalArrows(newBranch == 0 ? cond.conditional.@if : cond.conditional.@else);
            }
        }
    }

    void OnRightArrow()
    {
        //Funzione per gestire il click sulla freccia destra
        object current = sequence[currentIndex];

        if (sequence.Count == 1 && current is ConditionalData)
        {
            if (conditionalBranchIndex == 0)
            {
                SwitchConditionalBranch(true);
            }
            return;
        }

        if (current is ConditionalData && compactView)
        {
            var cond = (ConditionalData)current;
            List<(int branch, int doIdx)> logicalSeq = new List<(int, int)>();
            int ifDoCnt = cond.conditional.@if.@do?.Count ?? 0;
            int elseDoCnt = cond.conditional.@else.@do?.Count ?? 0;
            logicalSeq.Add((0, -1)); 
            for (int i = 0; i < ifDoCnt; ++i) logicalSeq.Add((0, i));
            logicalSeq.Add((1, -1)); 
            for (int i = 0; i < elseDoCnt; ++i) logicalSeq.Add((1, i));
            int pos = logicalSeq.FindIndex(e => e.branch == conditionalBranchIndex && e.doIdx == conditionalDoIndex);
            if (pos < logicalSeq.Count - 1)
            {
                var next = logicalSeq[pos + 1];
                var oldAutomation = GetConditionalDisplay(cond, conditionalBranchIndex, conditionalDoIndex);
                var newAutomation = GetConditionalDisplay(cond, next.branch, next.doIdx);
                Debug.Log($"[OnRightArrow][Conditional][Compact] Avanzo: {oldAutomation} => {newAutomation}, nextBranch={next.branch} nextDoIdx={next.doIdx}");
                conditionalBranchIndex = next.branch;
                conditionalDoIndex = next.doIdx;
                StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, true));
                UpdateConditionalArrows(next.branch == 0 ? cond.conditional.@if : cond.conditional.@else);
            }
            else
            {
                Debug.Log("[OnRightArrow][Conditional][Compact] Nessun avanzamento possibile.");
            }
            return;
        }

        if (compactView)
        {
            Debug.Log($"[OnRightArrow][Compact] currentIndex={currentIndex} sequence.Count={sequence.Count}");
            if (currentIndex < sequence.Count - 1)
                StartCoroutine(AnimateCompactStepTransition(true));
            return;
        }
        object nextObj = sequence[currentIndex + 1];
        object currentStep = sequence[currentIndex];

        if (currentStep is OrderData od && compactIndex < od.order.Count - 1)
        {
            Debug.Log($"[OnRightArrow][OrderData][Compact] compactIndex={compactIndex}, orderCount={od.order.Count}");
            string oldAutomation = od.order[compactIndex];
            string newAutomation = od.order[compactIndex + 1];
            compactIndex++;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, true));
            return;
        }
        else if (currentStep is ChoiceData cd && compactIndex < cd.choice.Count - 1)
        {
            Debug.Log($"[OnRightArrow][ChoiceData][Compact] compactIndex={compactIndex}, choiceCount={cd.choice.Count}");
            string oldAutomation = cd.choice[compactIndex];
            string newAutomation = cd.choice[compactIndex + 1];
            compactIndex++;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, true));
            return;
        }

        Debug.Log("[OnRightArrow] compactIndex reset a 0");
        compactIndex = 0;

        if (currentStep is string && nextObj is string)
        {
            Debug.Log("[OnRightArrow] Animating deck rotation (forward)");
            StartCoroutine(AnimateDeckRotation(true));
            orderText.gameObject.SetActive(false);
        }
        else if (currentStep is string && (nextObj is OrderData || nextObj is ChoiceData || nextObj is IterationData || nextObj is ConditionalData))
        {
            Debug.Log("[OnRightArrow] CardToOrderOrChoiceTransition +1");
            StartCoroutine(CardToOrderOrChoiceTransition(+1, nextObj));
        }
        else if ((currentStep is OrderData || currentStep is ChoiceData || currentStep is IterationData || currentStep is ConditionalData) && nextObj is string)
        {
            Debug.Log("[OnRightArrow] OrderOrChoiceToCardTransition +1");
            StartCoroutine(OrderOrChoiceToCardTransition(+1));
            orderText.gameObject.SetActive(false);
        }
        else if ((currentStep is OrderData || currentStep is ChoiceData || currentStep is IterationData || currentStep is ConditionalData) &&
                 (nextObj is OrderData || nextObj is ChoiceData || nextObj is IterationData || nextObj is ConditionalData))
        {
            Debug.Log("[OnRightArrow] OrderOrChoiceToOrderOrChoiceTransition +1");
            StartCoroutine(OrderOrChoiceToOrderOrChoiceTransition(+1, nextObj));
        }
        else
        {
            Debug.Log("[OnRightArrow] currentIndex++ e ShowCurrent");
            currentIndex++;
            ShowCurrent();
            UpdateArrowInteractable();
        }
    }

    void OnLeftArrow()
    {
        //Funzione per gestire il click sulla freccia sinistra
        if (isAnimating) { Debug.Log("[OnLeftArrow] isAnimating=true, exit"); return; }
        Debug.LogWarning(sequence.Count);

        if (sequence.Count == 0) { Debug.Log("[OnLeftArrow] sequence.Count==0, exit"); return; }
        object current = sequence[currentIndex];
        Debug.Log($"[OnLeftArrow] currentIndex={currentIndex} compactView={compactView}");

        if (sequence.Count == 1 && current is ConditionalData)
        {
            if (conditionalBranchIndex == 1)
            {
                SwitchConditionalBranch(false);
            }
            return;
        }

        if (current is ConditionalData && compactView)
        {
            // Gestione del caso in cui siamo in modalità compatta e stiamo navigando tra i rami di un conditional
            var cond = (ConditionalData)current;
            List<(int branch, int doIdx)> logicalSeq = new List<(int, int)>();
            int ifDoCnt = cond.conditional.@if.@do?.Count ?? 0;
            int elseDoCnt = cond.conditional.@else.@do?.Count ?? 0;
            logicalSeq.Add((0, -1)); 
            for (int i = 0; i < ifDoCnt; ++i) logicalSeq.Add((0, i));
            logicalSeq.Add((1, -1)); 
            for (int i = 0; i < elseDoCnt; ++i) logicalSeq.Add((1, i));
            int pos = logicalSeq.FindIndex(e => e.branch == conditionalBranchIndex && e.doIdx == conditionalDoIndex);
            Debug.Log($"[OnLeftArrow][Conditional][Compact] branch={conditionalBranchIndex} doIdx={conditionalDoIndex} pos={pos} logicalSeq.Count={logicalSeq.Count}");
            if (pos > 0)
            {
                var prev = logicalSeq[pos - 1];
                var oldAutomation = GetConditionalDisplay(cond, conditionalBranchIndex, conditionalDoIndex);
                var newAutomation = GetConditionalDisplay(cond, prev.branch, prev.doIdx);
                Debug.Log($"[OnLeftArrow][Conditional][Compact] Indietro: {oldAutomation} => {newAutomation}, prevBranch={prev.branch} prevDoIdx={prev.doIdx}");
                conditionalBranchIndex = prev.branch;
                conditionalDoIndex = prev.doIdx;
                StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, false));
                UpdateConditionalArrows(prev.branch == 0 ? cond.conditional.@if : cond.conditional.@else);
            }
            else
            {
                Debug.Log("[OnLeftArrow][Conditional][Compact] Nessun indietreggiamento possibile.");
            }
            return;
        }

        if (compactView)
        {
            Debug.Log($"[OnLeftArrow][Compact] currentIndex={currentIndex} sequence.Count={sequence.Count}");
            if (currentIndex > 0)
                StartCoroutine(AnimateCompactStepTransition(false));
            return;
        }
        object prevObj = sequence[currentIndex - 1];
        object currentStep = sequence[currentIndex];

        if (currentStep is OrderData od && compactIndex > 0)
        {
            Debug.Log($"[OnLeftArrow][OrderData][Compact] compactIndex={compactIndex}, orderCount={od.order.Count}");
            string oldAutomation = od.order[compactIndex];
            string newAutomation = od.order[compactIndex - 1];
            compactIndex--;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, false));
            return;
        }
        else if (currentStep is ChoiceData cd && compactIndex > 0)
        {
            Debug.Log($"[OnLeftArrow][ChoiceData][Compact] compactIndex={compactIndex}, choiceCount={cd.choice.Count}");
            string oldAutomation = cd.choice[compactIndex];
            string newAutomation = cd.choice[compactIndex - 1];
            compactIndex--;
            StartCoroutine(AnimateCompactCardChange(oldAutomation, newAutomation, false));
            return;
        }

        Debug.Log("[OnLeftArrow] compactIndex reset a 0");
        compactIndex = 0;

        if (currentStep is string && prevObj is string)
        {
            Debug.Log("[OnLeftArrow] Animating deck rotation (backward)");
            StartCoroutine(AnimateDeckRotation(false));
            orderText.gameObject.SetActive(false);
        }
        else if (currentStep is string && (prevObj is OrderData || prevObj is ChoiceData || prevObj is IterationData || prevObj is ConditionalData))
        {
            Debug.Log("[OnLeftArrow] AnimateDeckInOrderOrChoiceThenExpand -1");
            StartCoroutine(AnimateDeckInOrderOrChoiceThenExpand(prevObj));
        }
        else if ((currentStep is OrderData || currentStep is ChoiceData || currentStep is IterationData || currentStep is ConditionalData) && prevObj is string)
        {
            Debug.Log("[OnLeftArrow] OrderOrChoiceToCardTransition -1");
            StartCoroutine(OrderOrChoiceToCardTransition(-1));
            orderText.gameObject.SetActive(false);
        }
        else if ((currentStep is OrderData || currentStep is ChoiceData || currentStep is IterationData || currentStep is ConditionalData) &&
                (prevObj is OrderData || prevObj is ChoiceData || prevObj is IterationData || prevObj is ConditionalData))
        {
            Debug.Log("[OnLeftArrow] OrderOrChoiceToOrderOrChoiceTransition -1");
            StartCoroutine(OrderOrChoiceToOrderOrChoiceTransition(-1, prevObj));
        }
        else
        {
            Debug.Log("[OnLeftArrow] currentIndex-- e ShowCurrent");
            currentIndex--;
            ShowCurrent();
            UpdateArrowInteractable();
        }
    }

    string GetConditionalDisplay(ConditionalData cond, int branch, int doIdx)
    {
        if (branch == 0)
        {
            if (doIdx == -1)
                return cond.conditional.@if.trigger;
            else
                return cond.conditional.@if.@do[doIdx];
        }
        else
        {
            if (doIdx == -1)
                return cond.conditional.@else.trigger;
            else
                return cond.conditional.@else.@do[doIdx];
        }
    }

    IEnumerator CardToOrderOrChoiceTransition(int direction, object nextData)
    {
        isAnimating = true;
        if (expressions.Count > 0)
        {
            yield return StartCoroutine(DeckCardOutAndSendToBack(expressions[0], direction > 0, animTime));
        }

        currentIndex += direction;
        ShowCurrent();
        isAnimating = false;
    }

    IEnumerator OrderOrChoiceToCardTransition(int direction)
    {
        isAnimating = true;
        if (orderCards.Count > 0)
        {
            RectTransform cardRT = orderCards[0].GetComponent<RectTransform>();
            float w = cardRT.rect.width;
            float h = cardRT.rect.height;
            yield return StartCoroutine(CloseOrderCardsToCenter(orderCards, w, h, orderAnimTime));
            string animAuto = orderCards[0].transform.Find("NameAutomation").GetComponent<TMP_Text>().text
                .Replace("Automazione: ", "").Trim();
            if (string.IsNullOrWhiteSpace(animAuto))
                animAuto = currentOrder != null && currentOrder.order.Count > 0 ? currentOrder.order[0] : "";
            yield return StartCoroutine(AnimateOrderCardOut(animAuto, direction > 0, animTime));
        }

        if (expressions.Count > 0)
        {
            yield return StartCoroutine(CloseChoiceCardsToCenter(expressions, orderAnimTime));
        }

        currentIndex += direction;
        ShowCurrent();
        isAnimating = false;
    }

    IEnumerator OrderOrChoiceToOrderOrChoiceTransition(int direction, object nextData)
    {
        isAnimating = true;
        if (orderCards.Count > 0)
        {
            RectTransform cardRT = orderCards[0].GetComponent<RectTransform>();
            float w = cardRT.rect.width;
            float h = cardRT.rect.height;
            yield return StartCoroutine(CloseOrderCardsToCenter(orderCards, w, h, orderAnimTime));
            string animAuto = orderCards[0].transform.Find("NameAutomation").GetComponent<TMP_Text>().text
                .Replace("Automazione: ", "").Trim();
            if (string.IsNullOrWhiteSpace(animAuto))
                animAuto = currentOrder != null && currentOrder.order.Count > 0 ? currentOrder.order[0] : "";
            yield return StartCoroutine(AnimateOrderCardOut(animAuto, direction > 0, animTime));
        }

        if (expressions.Count > 0)
        {
            yield return StartCoroutine(CloseChoiceCardsToCenter(expressions, orderAnimTime));
        }

        currentIndex += direction;
        ShowCurrent();
        isAnimating = false;
    }

    IEnumerator AnimateDeckInOrderOrChoiceThenExpand(object prevData)
    {
        isAnimating = true;
        if (deckBackgroundCards.Count == 0)
        {
            isAnimating = false;
            yield break;
        }

        float cardW = deckBackgroundCards[deckBackgroundCards.Count - 1].GetComponent<RectTransform>().rect.width;
        float cardH = deckBackgroundCards[deckBackgroundCards.Count - 1].GetComponent<RectTransform>().rect.height;
        float outRadius = cardW * 0.4f;
        yield return StartCoroutine(DeckInFromBackground(cardW, cardH, outRadius, animTime));
        currentIndex--;
        foreach (var go in orderCards) Destroy(go);
        orderCards.Clear();
        foreach (var go in expressions) Destroy(go);
        expressions.Clear();
        ShowCurrent();
        UpdateArrowInteractable();
        isAnimating = false;
    }

    IEnumerator AnimateOrderCardsFromCenter(List<GameObject> cards, float cardWidth, float cardHeight, float duration)
    {
        // Animazione per disporre le card di un OrderData in una griglia
        isAnimating = true;
        int n = cards.Count;
        Vector2 center = Vector2.zero;
        float distY = cardHeight + maxOrderSpacing * 0.5f;
        float distX = cardWidth + maxOrderSpacing * 0.5f;

        List<Vector2> gridPositions = new List<Vector2>
        {
            center,
            center + Vector2.right * distX,
            center + Vector2.down * distY,
            center + new Vector2(distX, -distY),
            center + Vector2.left * distX,
            center + new Vector2(-distX, -distY),
            center + Vector2.up * distY,
            center + new Vector2(distX, distY),
            center + new Vector2(-distX, distY),
        };

        Vector2[] positions = new Vector2[n];
        for (int i = 0; i < n; i++)
            positions[i] = i < gridPositions.Count ? gridPositions[i] : center;

        for (int i = 0; i < n; i++)
        {
            var rt = cards[i].GetComponent<RectTransform>();
            cards[i].SetActive(true);
            rt.anchoredPosition = center;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            var cg = cards[i].GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = Mathf.SmoothStep(0, 1, t);
            for (int i = 0; i < n; i++)
            {
                var rt = cards[i].GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.Lerp(center, positions[i], eased);
                var cg = cards[i].GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = eased;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            var rt = cards[i].GetComponent<RectTransform>();
            rt.anchoredPosition = positions[i];
            var cg = cards[i].GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }

        if (rightArrow != null)
        {
            var rightRT = rightArrow.GetComponent<RectTransform>();
            if (n >= 2)
            {
                var pos = rightArrowInitialPos;
                pos.x += cardWidth + maxOrderSpacing;
                rightRT.anchoredPosition = new Vector2(pos.x, rightArrowInitialPos.y);
            }
            else
            {
                rightRT.anchoredPosition = new Vector2(rightArrowInitialPos.x, rightArrowInitialPos.y);
            }
        }

        if (leftArrow != null)
        {
            var leftRT = leftArrow.GetComponent<RectTransform>();
            if (n > 4)
            {
                var pos = leftArrowInitialPos;
                pos.x -= cardWidth + maxOrderSpacing;
                leftRT.anchoredPosition = new Vector2(pos.x, leftArrowInitialPos.y);
            }
            else
            {
                leftRT.anchoredPosition = new Vector2(leftArrowInitialPos.x, leftArrowInitialPos.y);
            }
        }

        isAnimating = false;
        UpdateArrowInteractable();
    }

    IEnumerator AnimateChoiceCardsFromCenter(ChoiceData choiceData)
    {
        // Animazione per disporre le card di un ChoiceData in una griglia
        isAnimating = true;
        foreach (var go in expressions) Destroy(go);
        expressions.Clear();

        int n = choiceData.choice.Count;
        float cardWidth = (expressionPrefab != null)
            ? expressionPrefab.GetComponent<RectTransform>().rect.width
            : 200f;
        float cardHeight = (expressionPrefab != null)
            ? expressionPrefab.GetComponent<RectTransform>().rect.height
            : 200f;
        float distX = cardWidth + maxOrderSpacing * 0.5f;
        float centerY = 0f;

        List<Vector2> positions = new List<Vector2>();
        if (n > 0)
        {
            positions.Add(new Vector2(0, centerY));
            for (int i = 1; i < n; i++)
            {
                int sign = (i % 2 == 1) ? 1 : -1;
                int offset = (i + 1) / 2;
                positions.Add(new Vector2(sign * offset * distX, centerY));
            }
        }

        List<RectTransform> rts = new List<RectTransform>();
        for (int i = 0; i < n; i++)
        {
            GameObject expr = CreateExpression(choiceData.choice[i], false);
            expressions.Add(expr);
            RectTransform rt = expr.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            var cg = expr.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
            rts.Add(rt);
        }

        float elapsed = 0f;
        while (elapsed < orderAnimTime)
        {
            float t = Mathf.SmoothStep(0, 1, elapsed / orderAnimTime);
            for (int i = 0; i < n; i++)
            {
                rts[i].anchoredPosition = Vector2.Lerp(Vector2.zero, positions[i], t);
                var cg = expressions[i].GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = t;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            rts[i].anchoredPosition = positions[i];
            var cg = expressions[i].GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }

        if (rightArrow != null)
        {
            var rightRT = rightArrow.GetComponent<RectTransform>();
            int rightCards = n / 2;
            var pos = rightArrowInitialPos;
            pos.x += rightCards * (cardWidth + maxOrderSpacing);
            rightRT.anchoredPosition = new Vector2(pos.x, rightArrowInitialPos.y);
        }

        if (leftArrow != null)
        {
            var leftRT = leftArrow.GetComponent<RectTransform>();
            int leftCards = (n - 1) / 2;
            var pos = leftArrowInitialPos;
            pos.x -= leftCards * (cardWidth + maxOrderSpacing);
            leftRT.anchoredPosition = new Vector2(pos.x, leftArrowInitialPos.y);
        }

        isAnimating = false;
        UpdateArrowInteractable();
    }

    IEnumerator CloseOrderCardsToCenter(List<GameObject> cards, float cardWidth, float cardHeight, float duration)
    {
        // Animazione per chiudere le card di un OrderData verso il centro
        int n = cards.Count;
        Vector2 center = Vector2.zero;
        float distY = cardHeight + maxOrderSpacing * 0.5f;
        float distX = cardWidth + maxOrderSpacing * 0.5f;

        List<Vector2> gridPositions = new List<Vector2>
        {
            center,
            center + Vector2.right * distX,
            center + Vector2.down * distY,
            center + new Vector2(distX, -distY),
            center + Vector2.left * distX,
            center + new Vector2(-distX, -distY),
            center + Vector2.up * distY,
            center + new Vector2(distX, distY),
            center + new Vector2(-distX, distY),
        };

        Vector2[] startPositions = new Vector2[n];
        for (int i = 0; i < n; i++)
            startPositions[i] = i < gridPositions.Count ? gridPositions[i] : center;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = Mathf.SmoothStep(0, 1, t);
            for (int i = 0; i < n; i++)
            {
                var rt = cards[i].GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.Lerp(startPositions[i], center, eased);
                var cg = cards[i].GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 1 - eased;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            var rt = cards[i].GetComponent<RectTransform>();
            rt.anchoredPosition = center;
            var cg = cards[i].GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
            cards[i].SetActive(false);
        }
    }

    IEnumerator CloseChoiceCardsToCenter(List<GameObject> cards, float duration)
    {
        // Animazione per chiudere le card di un ChoiceData verso il centro
        int n = cards.Count;
        Vector2 center = Vector2.zero;
        List<Vector2> startPositions = new List<Vector2>();
        for (int i = 0; i < n; i++)
        {
            var rt = cards[i].GetComponent<RectTransform>();
            startPositions.Add(rt.anchoredPosition);
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = Mathf.SmoothStep(0, 1, t);
            for (int i = 0; i < n; i++)
            {
                var rt = cards[i].GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.Lerp(startPositions[i], center, eased);
                var cg = cards[i].GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 1 - eased;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < n; i++)
        {
            var rt = cards[i].GetComponent<RectTransform>();
            rt.anchoredPosition = center;
            var cg = cards[i].GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
            cards[i].SetActive(false);
        }
    }

    IEnumerator AnimateOrderCardOut(string automation, bool toRight, float deckAnimTime)
    {
        // Animazione per rimuovere una card di un OrderData e mandarla in fondo al mazzo
        GameObject temp = CreateExpression(automation, false);
        temp.transform.SetAsLastSibling();
        RectTransform rt = temp.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;
        var cg = temp.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 1f;
        yield return StartCoroutine(DeckCardOutAndSendToBack(temp, toRight, deckAnimTime));
        Destroy(temp);
    }

    IEnumerator DeckCardOutAndSendToBack(GameObject card, bool toRight, float deckAnimTime)
    {
        // Animazione per rimuovere una card dal mazzo e mandarla in fondo
        var rt = card.GetComponent<RectTransform>();
        var cg = card.GetComponent<CanvasGroup>();
        Vector2 start = rt.anchoredPosition;
        Vector2 end = start + (toRight ? Vector2.right : Vector2.left) * rt.rect.width * 1.2f;
        float startAlpha = cg != null ? cg.alpha : 1f;
        float elapsed = 0f;
        float dur = deckAnimTime;
        while (elapsed < dur)
        {
            float t = Mathf.SmoothStep(0, 1, elapsed / dur);
            rt.anchoredPosition = Vector2.Lerp(start, end, t);
            if (cg != null) cg.alpha = Mathf.Lerp(startAlpha, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (cg != null) cg.alpha = 0f;
        card.transform.SetAsFirstSibling();
    }

    IEnumerator AnimateDeckRotation(bool forward)
    {
        // Animazione per ruotare il mazzo di espressioni
        isAnimating = true;

        if (expressions.Count == 0)
        {
            isAnimating = false;
            yield break;
        }

        RectTransform mainCard = expressions[0].GetComponent<RectTransform>();
        float cardH = mainCard.rect.height;
        float cardW = mainCard.rect.width;
        float outRadius = cardW * 0.4f;
        Vector2 center = (Vector2)GetDeckPosition(0, cardW, cardH);

        if (forward)
        {
            // Rotazione avanti
            Vector2 outRight = center + Vector2.right * outRadius;

            Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();
            Dictionary<GameObject, float> startAlphas = new Dictionary<GameObject, float>();

            foreach (var bg in deckBackgroundCards)
            {
                RectTransform bgRT = bg.GetComponent<RectTransform>();
                CanvasGroup bgCG = bg.GetComponent<CanvasGroup>();
                startPositions[bg] = bgRT.anchoredPosition;
                startAlphas[bg] = bgCG != null ? bgCG.alpha : 0.35f;
                bg.SetActive(true);
            }

            foreach (var expr in expressions)
            {
                RectTransform rt = expr.GetComponent<RectTransform>();
                CanvasGroup cg = expr.GetComponent<CanvasGroup>();
                startPositions[expr] = rt.anchoredPosition;
                startAlphas[expr] = cg != null ? cg.alpha : 1f;
                expr.SetActive(true);
            }

            AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            float elapsed = 0f;
            while (elapsed < animTime)
            {
                float t = elapsed / animTime;
                float s = curve.Evaluate(t);

                foreach (var go in startPositions.Keys)
                {
                    RectTransform rt = go.GetComponent<RectTransform>();
                    CanvasGroup cg = go.GetComponent<CanvasGroup>();

                    Vector3 targetPos = startPositions[go];
                    float targetAlpha = startAlphas[go];

                    if (expressions.Count > 0 && go == expressions[0])
                    {
                        targetPos = outRight;
                        targetAlpha = Mathf.Lerp(1f, 0f, s);
                    }
                    else if (deckBackgroundCards.Count > 0 && deckBackgroundCards.Contains(go))
                    {
                        int stackIndex = deckBackgroundCards.IndexOf(go) + 1;
                        targetPos = GetDeckPosition(stackIndex, cardW, cardH);
                        targetAlpha = 0.35f;
                    }

                    Vector3 currentPos = Vector3.Lerp(startPositions[go], targetPos, s);
                    rt.anchoredPosition = (Vector2)currentPos;
                    if (cg != null)
                        cg.alpha = Mathf.Lerp(startAlphas[go], targetAlpha, s);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            currentIndex++;
            ShowCurrent();
            UpdateArrowInteractable();
            isAnimating = false;
        }
        else
        {
            // Rotazione indietro
            float outRadiusBack = cardW * 0.4f;
            yield return StartCoroutine(DeckInFromBackground(cardW, cardH, outRadiusBack, animTime));
            currentIndex--;
            ShowCurrent();
            UpdateArrowInteractable();
            isAnimating = false;
        }
    }

    IEnumerator DeckInFromBackground(float cardW, float cardH, float outRadius, float duration)
    {
        // Animazione per portare il mazzo di espressioni in primo piano
        if (deckBackgroundCards.Count == 0)
            yield break;
        Vector2 center = (Vector2)GetDeckPosition(0, cardW, cardH);
        Vector2 outLeft = center + Vector2.left * outRadius;
        GameObject backCard = deckBackgroundCards[deckBackgroundCards.Count - 1];
        RectTransform backRT = backCard.GetComponent<RectTransform>();
        CanvasGroup backCG = backCard.GetComponent<CanvasGroup>();

        float elapsed = 0f;
        backRT.anchoredPosition = outLeft;
        backRT.localRotation = Quaternion.identity;
        backRT.localScale = Vector3.one * 0.9f;
        if (backCG != null) backCG.alpha = 0.3f;
        backCard.transform.SetAsFirstSibling();

        backCard.transform.SetAsLastSibling();
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            backRT.anchoredPosition = Vector2.Lerp(outLeft, center, t);
            backRT.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, t);
            backRT.localRotation = Quaternion.identity;
            if (backCG != null) backCG.alpha = Mathf.Lerp(0.3f, 1f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        backRT.anchoredPosition = center;
        backRT.localScale = Vector3.one;
        backRT.localRotation = Quaternion.identity;
        if (backCG != null) backCG.alpha = 1f;
    }


    void UpdateArrowInteractable()
    {
        // Aggiorna lo stato delle frecce in base all'indice corrente
        if (sequence.Count == 1 && sequence[0] is ConditionalData)
        {
            if (leftArrow != null) leftArrow.interactable = true;
            if (rightArrow != null) rightArrow.interactable = true;
            return;
        }
        if (leftArrow != null)
            leftArrow.interactable = currentIndex > 0;
        if (rightArrow != null)
            rightArrow.interactable = currentIndex < sequence.Count - 1;
    }

    Vector3 GetDeckPosition(int stackPosition, float cardW, float cardH)
    {
        // Calcola la posizione della carta nello stack in base alla sua posizione
        if (stackPosition == 0)
            return Vector3.zero;
        else if (stackPosition < visibleCards)
            return new Vector3(
                stackOffsetXPercent * cardW * stackPosition,
                -stackOffsetYPercent * cardH * stackPosition,
                0f
            );
        else
            return new Vector3(
                stackOffsetXPercent * cardW * (visibleCards - 1),
                -stackOffsetYPercent * cardH * (visibleCards - 1),
                0f
            );
    }

    void UpdateConditionalArrows(ConditionalBranch branch)
    {
        // Aggiorna lo stato delle frecce condizionali in base all'indice corrente
        int doCount = branch.@do != null ? branch.@do.Count : 0;
        bool showUp = conditionalDoIndex > -1;
        bool showDown = (conditionalDoIndex < doCount - 1 && conditionalDoIndex >= 0) ||
                        (conditionalDoIndex == -1 && doCount > 0);
        if (verticalUpArrow != null) verticalUpArrow.gameObject.SetActive(showUp);
        if (verticalDownArrow != null) verticalDownArrow.gameObject.SetActive(showDown);
        if (leftArrow != null) leftArrow.gameObject.SetActive(true);
        if (rightArrow != null) rightArrow.gameObject.SetActive(true);
        if (toggleViewButton != null) toggleViewButton.gameObject.SetActive(true);
    }
    private void ResetConditionalExpandedState()
    {
        // Resetta lo stato di espansione delle frecce condizionali
        isIfExpanded = false;
        isElseExpanded = false;
        if (expressions.Count > 2)
        {
            for (int i = expressions.Count - 1; i >= 2; i--)
            {
                Destroy(expressions[i]);
                expressions.RemoveAt(i);
            }
        }
    }
}