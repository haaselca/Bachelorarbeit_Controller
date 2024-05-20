using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SupportMenuController : MonoBehaviour
{
    [SerializeField] Material defaultColor;
    [SerializeField] Material hoveredColor;
    [SerializeField] Material selectedColor;
    [SerializeField] Material disabledColor;

    [SerializeField] Image[] uiElements;
    [SerializeField] GameObject[] crosses;
    [SerializeField] GameObject[] uiCollider;

    [SerializeField] InputActionAsset dummyAsset;
    [SerializeField] GameObject supportMenu;
    [SerializeField] float scaleSpeed = 0.25f;

    [SerializeField] AudioClip hoveredSound;
    [SerializeField] AudioClip selectedSound;
    [SerializeField] AudioClip backSound;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip stepCompleteSound;

    private Image lastHoveredElement;
    private List<Image> usedElements;

    private InputActionMap supportActionMap;
    private InputActionMap regularActionMap;

    private Image selectedSupport;

    private AudioSource audioSource;

    public delegate void OnSupportSelected(string support);
    public static event OnSupportSelected onSupportSelected;

    private void Start()
    {
        regularActionMap = dummyAsset.FindActionMap("RegularActionMap");  // Will be replaced with creator ActionMap
        supportActionMap = dummyAsset.FindActionMap("SupportMenu");

        audioSource = supportMenu.GetComponent<AudioSource>();
        usedElements = new List<Image>();
    }

    private void OnEnable()
    {
        ProgressBarHandler.onProgressBarComplete += ResetSupport;
        LifeCountChanger.onResetCheckpoint += ResetSupport;
        DummyInputTester.onProgressTraining += DeactivateSupport;
    }

    private void OnDisable()
    {
        ProgressBarHandler.onProgressBarComplete -= ResetSupport;
        LifeCountChanger.onResetCheckpoint -= ResetSupport;
        DummyInputTester.onProgressTraining -= DeactivateSupport;
    }

    public void OnControlStickMove(InputAction.CallbackContext context)
    {
        if (selectedSupport == null)
        {
            var stickDirection = context.ReadValue<Vector2>();

            MoveCursor(stickDirection);
        }
    }

    private void MoveCursor(Vector2 stickDirection)
    {
        if (stickDirection != Vector2.zero)
        {
            var angle = Vector2.Angle(stickDirection, Vector2.up);

            if (stickDirection.x < 0)
            {
                angle = 360 - angle;
            }

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle + 180);
        }
    }

    public void OnToggleSupportMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (supportActionMap.enabled)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    private void CloseMenu()
    {
        if (lastHoveredElement.transform.parent.name != "BackButton" && selectedSupport == null)
        {
            ActivateSupport();

            if(audioSource != null)
            {
                audioSource.PlayOneShot(selectedSound);
            }
        }
        else
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(backSound);
            }
        }

        StartCoroutine(ScaleOverTime(supportMenu.transform, new Vector3(0.003f, 0.003f, 0.003f), scaleSpeed));

        supportActionMap.Disable();
        regularActionMap.Enable();
    }

    private void OpenMenu()
    {
        StartCoroutine(ScaleOverTime(supportMenu.transform, new Vector3(1, 1, 1), scaleSpeed));

        if (audioSource != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        regularActionMap.Disable();
        supportActionMap.Enable();

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    /// <summary>
    /// Deactivates menuing and fires event with the selected support
    /// </summary>
    private void ActivateSupport()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            Image elem = uiElements[i];

            if (usedElements.Contains(elem))
            {
                crosses[i].SetActive(true);
            }
            
            elem.material = disabledColor;
        }

        uiElements[0].material = defaultColor;

        selectedSupport = lastHoveredElement;
        selectedSupport.material = selectedColor;
        usedElements.Add(selectedSupport);

        onSupportSelected(selectedSupport.name);
    }

    /// <summary>
    /// Enables menuing but disables used supports
    /// </summary>
    private void DeactivateSupport()
    {
        for (int i = 0; i < uiElements.Length; i++)
        {
            Image elem = uiElements[i];

            if (usedElements.Contains(elem))
            {
                elem.material = disabledColor;
                crosses[i].SetActive(true);
                uiCollider[i].SetActive(false);
            }
            else
            {
                elem.material = defaultColor;
            }

            selectedSupport = null;
        }

        PlayStepCompleteSound();
    }

    private void PlayStepCompleteSound()
    {
        if(audioSource != null)
        {
            audioSource.PlayOneShot(stepCompleteSound);
        }
    }

    private IEnumerator ScaleOverTime(Transform objectToScale, Vector3 toScale, float duration)
    {
        float counter = 0;
        Vector3 startScaleSize = objectToScale.localScale;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var uiElement = other.GetComponentInParent<Image>();

        HighlightHoveredElement(uiElement);
    }

    /// <summary>
    /// Colors each element according to its state
    /// </summary>
    /// <param name="uiElement">The hovered element</param>
    private void HighlightHoveredElement(Image uiElement)
    {
        if (uiElement != null && selectedSupport == null)
        {
            foreach (var elem in uiElements)
            {
                elem.material = defaultColor;
            }

            foreach(var elem in usedElements)
            {
                if(elem == selectedSupport)
                {
                    elem.material = selectedColor;
                }
                else
                {
                    elem.material = disabledColor;
                }
            }

            if(supportMenu.transform.localScale.x >= 1 && audioSource != null)
            {
                audioSource.PlayOneShot(hoveredSound);
            }

            uiElement.material = hoveredColor;
            lastHoveredElement = uiElement;
        }
        else
        {
            uiElements[0].material = hoveredColor;
        }
    }

    private void ResetSupport()
    {
        selectedSupport = null;
        usedElements.Clear();

        for (int i = 0; i < uiElements.Length; i++)
        {
            Image elem = uiElements[i];
            elem.material = defaultColor;

            crosses[i].SetActive(false);
            uiCollider[i].SetActive(true);
        }
    }
}
