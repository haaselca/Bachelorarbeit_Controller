using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyInputTester : MonoBehaviour
{
    public delegate void OnProgressTraining();
    public static event OnProgressTraining onProgressTraining;

    public delegate void OnError();
    public static event OnError onError;

    [SerializeField] List<GameObject> dummyActvities = new List<GameObject>();
    private int activityNr = 0;

    public static GameObject CurrentActivity { get; private set; }

    private void Start()
    {
        CurrentActivity = dummyActvities[activityNr];
    }

    public void OnDummyAction(InputAction.CallbackContext context)
    {
        var isPressed = context.ReadValueAsButton();
        Debug.Log("Dummy: " + isPressed);
    }

    public void OnInstructionStepForward(InputAction.CallbackContext context)
    {
        var isPressed = context.started;

        if(isPressed)
        {
            NextActivity();
            onProgressTraining();
        }
    }

    private void NextActivity()
    {
        activityNr++;

        if (activityNr >= dummyActvities.Count)
        {
            activityNr = 0;
        }

        CurrentActivity = dummyActvities[activityNr];
    }

    public void OnMistakeMade(InputAction.CallbackContext context)
    {
        var isPressed = context.started;

        if (isPressed)
        {
            onError();
        }
    }

    public static void RevertProgressStep()
    {
        // Dummy revert
    }
}
