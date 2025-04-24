using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackGenerator : MonoBehaviour
{
    public ParticleSystem feedbackParticles;
    public AudioSource feedbackAudio;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (feedbackParticles == null)
        {
            throw new Exception("FeedbackParticles not set in FeedbackGenerator");
        }
        
        if (feedbackAudio == null)
        {
            throw new Exception("FeedbackAudio not set in FeedbackGenerator");
        }
    }

    private void Start()
    {
        feedbackParticles.Stop();
        feedbackAudio.Stop();
    }

    public void PlayFeedback()
    {
        feedbackParticles.Play();
        feedbackAudio.Play();
        StartCoroutine(DeactivateFeedbackParticlesWithDelay());
    }
    
    IEnumerator DeactivateFeedbackParticlesWithDelay()
    {
        yield return new WaitForSeconds(2);
        feedbackParticles.Stop();
    }
}