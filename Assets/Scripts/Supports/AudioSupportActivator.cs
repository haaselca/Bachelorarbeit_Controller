using System.Collections;
using UnityEngine;

public class AudioSupportActivator : GenericSupportActivator
{
    [SerializeField] float pauseBetweenSounds = 0.5f;

    private AudioSource audioSource;
    private bool isRunning;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected override void ActivateSupport(string support)
    {
        PlaySoundFromObjectPosition(support);
    }

    private void PlaySoundFromObjectPosition(string support)
    {
        if (support == "HearSupport")
        {
            isRunning = true;
            StartCoroutine(LoopWithPause(pauseBetweenSounds));
        }
    }

    private IEnumerator LoopWithPause(float pauseSeconds)
    {
        while (isRunning)
        {
            if(audioSource != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
                yield return new WaitForSeconds(pauseSeconds);
            }
        }
    }

    protected override void DeactivateSupport()
    {
        isRunning = false;
    }
}
