using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollTextAreaByUpDownButtons : MonoBehaviour
{
   [SerializeField] private int canvasHeight;
   [SerializeField] private int textMarginTop;
   [SerializeField] private  int textMarginBottom;

   public Canvas textCanvas;
   public TMP_Text text;
   public PressableButton scrollUp;
   public PressableButton scrollDown;

   [SerializeField] private RectTransform rt;

   private Coroutine currentRoutine = null;
   
    void Awake()
   {
      rt = text.GetComponent<RectTransform>();
      
      if (rt == null)
      {
         throw new ArgumentNullException("rt", "rt must be set");
      }
      
      if (textCanvas == null)
      {
         throw new ArgumentNullException("textCanvas", "textCanvas must be set");
      }
      
      if (text == null)
      {
         throw new ArgumentNullException("text", "text must be set");
      }
      
      if (scrollUp == null)
      {
         throw new ArgumentNullException("scrollUp", "scrollUp must be set");
      }
      
      if (scrollDown == null)
      {
         throw new ArgumentNullException("scrollDown", "scrollDown must be set");
      }
      
      canvasHeight = (int) textCanvas.GetComponent<RectTransform>().rect.height;
      Debug.Log("Canvas height" + canvasHeight);
      
      if (canvasHeight == 0)
      {
         throw new ArgumentNullException("canvasHeight", "canvasHeight must be set");
      }

      var margins = text.GetComponent<TMP_Text>().margin;
      textMarginTop = (int) margins.y;
      textMarginBottom = (int) margins.w;
   }


   void Start()
   {
      scrollUp.OnClicked.AddListener(ScrollUp);
      scrollDown.OnClicked.AddListener(ScrollDown);
   }
   
   public void ScrollUp()
   {
      // Check if there is a current routine running. If so, stop it
      if (currentRoutine != null)
      {
         StopCoroutine(currentRoutine);
      }
      
      // Decrease text z position by one page
      int offset = CalcOnePageHeight();
      var targetLocPos = new Vector3(rt.localPosition.x, rt.localPosition.y - offset, rt.localPosition.z);
      currentRoutine = StartCoroutine(SmoothScrollToPosition(targetLocPos, 3f));
   }
   
   public void ScrollDown()
   {
      // Check if there is a current routine running. If so, stop it
      if (currentRoutine != null)
      {
         StopCoroutine(currentRoutine);
      }
      
      // Increase text z position by one page
      int offset = CalcOnePageHeight();
      var targetLocPos = new Vector3(rt.localPosition.x, rt.localPosition.y + offset, rt.localPosition.z);
      currentRoutine = StartCoroutine(SmoothScrollToPosition(targetLocPos, 3f));
   }
   
   private int CalcOnePageHeight()
   {
      int r =  canvasHeight - textMarginTop - textMarginBottom;
      // Debug.Log("Page page offset" + r);
      return r;
   }
   
   private IEnumerator SmoothScrollToPosition(Vector3 targetPosition, float duration)
   {
      float time = 0;
      Vector3 startPosition = rt.localPosition;
      Debug.Log("Start position" + startPosition);
      Debug.Log("Target position" + targetPosition);
      while (time < duration)
      {
         rt.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
         time += Time.deltaTime;
         yield return null;
      }
      rt.localPosition = targetPosition;
   }
}
