using System.Collections.Generic;
using UnityEngine;

public class SupportObjectManager : MonoBehaviour
{
    [SerializeField] GameObject generalSupportReceiver;  // Every object that can be interacted with has one
    [SerializeField] GameObject specificSupportReceiver;  // Only the current activity object has one
    private List<GameObject> generalReceiverList;
    private GameObject specificReceiver;

    private void OnEnable()
    {
        DummyInputTester.onProgressTraining += AddCurrentActivityObject;
    }

    private void OnDisable()
    {
        DummyInputTester.onProgressTraining -= AddCurrentActivityObject;
    }

    private void Start()
    {
        generalReceiverList = new List<GameObject>();
        var interactableObjects = Object.FindObjectsByType<DummyAlwaysGrabable>(FindObjectsSortMode.None);  // Replace with corresponding script in project

        foreach ( var obj in interactableObjects)
        {
            var receiver = Instantiate(generalSupportReceiver, obj.transform);
            generalReceiverList.Add(receiver);
        }

        AddCurrentActivityObject();
    }

    private void AddCurrentActivityObject()
    {
        if(specificReceiver == null)
        {
            specificReceiver = Instantiate(specificSupportReceiver);
        }

        specificReceiver.transform.SetParent(DummyInputTester.CurrentActivity.transform, false);
    }
}
