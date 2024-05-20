using TMPro;
using UnityEngine;

public class DescriptionSupportActivator : GenericSupportActivator
{
    [SerializeField] TMP_Text descText;
    [SerializeField] LineRenderer lineRenderer;

    private Transform parentTransform;
    private bool isRunning;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    void Update()
    {
        if (isRunning)
        {
            LookAtActiveCamera();
            DrawLineToObject();
        }
    }

    private void DisplayObjectName()
    {
        descText.text = parentTransform.name;
    }

    private void DrawLineToObject()
    {
        lineRenderer.SetPosition(0, descText.transform.position);
        lineRenderer.SetPosition(1, parentTransform.position);
    }

    private void LookAtActiveCamera()
    {
        var activeCam = Camera.main;
        transform.LookAt(activeCam.transform.position);
    }

    protected override void ActivateSupport(string support)
    {
        DisplayNameTagOverObject(support);
    }

    private void DisplayNameTagOverObject(string support)
    {
        if (support == "DescriptionSupport")
        {
            transform.GetChild(0).gameObject.SetActive(true);
            parentTransform = transform.parent.parent;
            DisplayObjectName();
            isRunning = true;
        }
    }

    protected override void DeactivateSupport()
    {
        isRunning = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
