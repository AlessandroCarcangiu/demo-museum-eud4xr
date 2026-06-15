using System;
using System.Collections.Generic;
using UnityEngine;


public enum ChatbotState
{
    Idle,
    Listening,
    Analyzing,
    GeneratingAnswer,
    PreparingAnswerAudio,
    Answering
}

public class ChatbotEventArgs : EventArgs
{
    public ChatbotState State { get; private set; }

    public ChatbotEventArgs(ChatbotState state)
    {
        State = state;
    }
}

public class ChatbotAnimationController : MonoBehaviour
{
    private Animator animator;
    public static event EventHandler<ChatbotEventArgs> OnChatbotEventChange;
    
    private readonly Dictionary<ChatbotState, string> animations = new Dictionary<ChatbotState, string> 
    {
        {
            ChatbotState.Idle,
            "Idle"
        },
        {
            ChatbotState.Analyzing,
            "pose4"
        },
        {
            ChatbotState.Answering,
            "pose6"
        },
        {
            ChatbotState.Listening,
            "pose4"
        },
        {
            ChatbotState.GeneratingAnswer,
            "pose5"
        },
        {
            ChatbotState.PreparingAnswerAudio,
            "pose5"
        }
    };
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        OnChatbotEventChange += HandleAnimationChange;
    }
    
    private void HandleAnimationChange(object sender, ChatbotEventArgs e)
    {
        if (animator == null)
        {
            throw new Exception("Animator is null");
        };

        if (animations.TryGetValue(e.State, out string poseName))
        {
            animator.Play(poseName);
            Debug.Log($"Changing animation to: {e.State}");
        }
        else
        {
            Debug.LogWarning($"Animation trigger not found for: {e.State}");
        }
    }

    public static void RequestAnimationChange(ChatbotState state)
    {
        OnChatbotEventChange?.Invoke(null, new ChatbotEventArgs(state));
    }
}
