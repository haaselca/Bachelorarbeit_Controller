using UnityEngine;

public class VibrationSupportActivator : GenericSupportActivator
{
    [SerializeField] Transform menuTransform;
    [SerializeField] float duration = 0.1f;

    private bool isRunning;
    private float distance;
    private float initialDistance;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Update()
    {
        if (isRunning)
        {
            CalculateDistanceToObject();
            RumbleController();
        }
    }

    private void CalculateDistanceToObject()
    {
        distance = Vector3.Distance(menuTransform.position, transform.position);
    }

    private void RumbleController()
    {
        var strenght = 1 - distance / initialDistance;

        if(strenght < 0)
        {
            Debug.Log("Haptics.SendHapticImpulse(" + strenght + ", " + duration);
        }
    }

    protected override void ActivateSupport(string support)
    {
        RumbleControllerAccordingToDistance(support);
    }

    private void RumbleControllerAccordingToDistance(string support)
    {
        if (support == "VibrationSupport")
        {
            isRunning = true;
            CalculateDistanceToObject();
            initialDistance = distance;
        }
    }

    protected override void DeactivateSupport()
    {
        isRunning = false;
    }
}
