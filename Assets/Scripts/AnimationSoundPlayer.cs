using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] animationAudios;

    public int soundToPlay = -1;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPlayAudio(int clip)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(animationAudios[clip]);
        }
    }
}
