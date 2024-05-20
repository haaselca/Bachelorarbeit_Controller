using UnityEngine;

public abstract class GenericSupportActivator : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        SupportMenuController.onSupportSelected += ActivateSupport;
        ProgressBarHandler.onProgressBarComplete += DeactivateSupport;
        LifeCountChanger.onResetCheckpoint += DeactivateSupport;
        DummyInputTester.onProgressTraining += DeactivateSupport;
    }

    protected virtual void OnDisable()
    {
        SupportMenuController.onSupportSelected -= ActivateSupport;
        ProgressBarHandler.onProgressBarComplete -= DeactivateSupport;
        LifeCountChanger.onResetCheckpoint -= DeactivateSupport;
        DummyInputTester.onProgressTraining -= DeactivateSupport;
    }

    protected abstract void ActivateSupport(string support);
    protected abstract void DeactivateSupport();
}
