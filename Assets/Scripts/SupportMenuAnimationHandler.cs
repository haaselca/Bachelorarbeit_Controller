using UnityEngine;

public class SupportMenuAnimationHandler : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ProgressBarHandler.onProgressBarComplete += PlayCheckpointAnimation;
        LifeCountChanger.onResetCheckpoint += PlayResetAnimation;
    }

    private void OnDisable()
    {
        ProgressBarHandler.onProgressBarComplete -= PlayCheckpointAnimation;
        LifeCountChanger.onResetCheckpoint -= PlayResetAnimation;
    }

    private void PlayCheckpointAnimation()
    {
        if(animator != null)
        {
            animator.Play("CheckpointAnimation");
        }
    }

    private void PlayResetAnimation()
    {
        if(animator != null)
        {
            animator.Play("ResetAnimation");
        }
    }

    public void OnCancelEveryAnimation()
    {
        if(animator != null)
        {
            animator.Play("Empty");
        }
    }
}
