using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// A <see cref="IEffect" /> that switches a Unity image between two sprites.
/// </summary>
[Serializable]
public class SpriteTextSwapEffect : IEffect
{
    // [SerializeField] [HideInInspector]
// #pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
    // private string name = "Sprite Swap";
// #pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

    [SerializeField]
    [Tooltip(
        "Threshold value to activate this effect. When the state value is above this number, the effect will activate.")]
    private float activationThreshold = 0.001f;

    [SerializeField] [Tooltip("The TextMeshPro text to switch text for.")]
    private FontIconSelector target;

    [SerializeField] [Tooltip("The texture to set when the state is active.")]
    private string activeIcon;

    [SerializeField] [Tooltip("The texture to set when the state is inactive.")]
    private string inactiveIcon;
    

    /// <inheritdoc />
    public void Setup(PlayableGraph graph, GameObject owner)
    {
    }

    /// <inheritdoc />
    public bool Evaluate(float parameter)
    {
        if (target == null)
        {
            return false;
        }

        var correctSprite = parameter > activationThreshold ? activeIcon : inactiveIcon;

        if (target.CurrentIconName != correctSprite)
        {
            target.CurrentIconName = correctSprite;
        }

        return true; // We are always immediately done.
    }
}