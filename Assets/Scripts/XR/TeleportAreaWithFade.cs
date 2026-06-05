using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportAreaWithFade : TeleportationArea
{
    private FadeCanvas fadeCanvas = null;

    protected override void Awake()
    {
        base.Awake();
        fadeCanvas = FadeCanvas.Instance;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // base.OnSelectExited(args);
        
        if (teleportTrigger == TeleportTrigger.OnSelectExited)
            StartCoroutine(FadeSequence(base.OnSelectExited, args));
    }

    private IEnumerator FadeSequence<T>(UnityAction<T> action, T args)
        where T : BaseInteractionEventArgs
    {
        // fadeCanvas.QuickFadeIn(); Original
        fadeCanvas.StartFadeIn();

        yield return fadeCanvas.CurrentRoutine;
        action.Invoke(args);

        // fadeCanvas.QuickFadeOut(); Original
        fadeCanvas.StartFadeOut();
    }
}