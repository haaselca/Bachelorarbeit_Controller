using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCountChanger : MonoBehaviour
{
    [SerializeField] GameObject heart;
    [SerializeField] int lifeCount = 3;
    [SerializeField] float lifeAngle = 15f;
    [SerializeField] Vector3 heartsScale = new Vector3 (50f,50f, 50f);
    private Vector3 originalHeartsScale;

    [SerializeField] float scaleTime = 0.25f;
    [SerializeField] float zoomTime = 0.5f;

    [SerializeField] GameObject supportMenu;
    [SerializeField] AudioClip errorSound;
    [SerializeField] AudioClip shrinkSound;
    [SerializeField] AudioClip growSound;

    private AudioSource audioSource;

    private float startRotation;
    private Vector3 startScale;
    private float currentAngle;
    private List<GameObject> lifes;

    private bool isShieldEnabled;

    private static int currentLifes;
    public static int CurrentLifes { get => currentLifes; }

    public delegate void OnResetCheckpoint();
    public static event OnResetCheckpoint onResetCheckpoint;


    private void OnEnable()
    {
        DummyInputTester.onError += SubtractLife;
        ProgressBarHandler.onProgressBarComplete += RefillLife;
        ShieldSupportActivator.onShieldToggle += ToggleErrors;
    }

    private void OnDisable()
    {
        DummyInputTester.onError -= SubtractLife;
        ProgressBarHandler.onProgressBarComplete -= RefillLife;
        ShieldSupportActivator.onShieldToggle -= ToggleErrors;
    }

    private void Start()
    {
        audioSource = supportMenu.GetComponent<AudioSource>();
        originalHeartsScale = transform.localScale;
        InitializeLifes();
    }

    private void InitializeLifes()
    {
        lifes = new List<GameObject>();

        startRotation = lifeAngle * (lifeCount - 1);

        RotateLifes();

        currentLifes = lifes.Count;
        startScale = lifes[0].transform.localScale;
    }

    private void RotateLifes()
    {
        for (int i = 0; i < lifeCount; i++)
        {
            currentAngle = startRotation - lifeAngle * i;
            var prefabRot = Quaternion.Euler(0f, 0f, currentAngle);

            var life = Instantiate(heart, this.transform);
            life.transform.localRotation = prefabRot;
            lifes.Add(life.transform.GetChild(0).gameObject);
        }
    }

    private void SubtractLife()
    {
        if(currentLifes > 0 && !isShieldEnabled)
        {
            currentLifes--;

            if(audioSource != null)
            {
                audioSource.PlayOneShot(errorSound);
            }
        }

        StartCoroutine(ScaleHearts(true, currentLifes));
    }

    private void RefillLife()
    {
        currentLifes = lifeCount;
        StartCoroutine(StartAfterSeconds(5f));
    }

    private IEnumerator StartAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        yield return ScaleHearts(false, currentLifes);
    }

    private IEnumerator ScaleHearts(bool isLosingLife, int remainingHearts)
    {
        yield return ScaleOverTime(this.transform, heartsScale, scaleTime);
        yield return new WaitForSeconds(zoomTime);

        if(isLosingLife)
        {
            for (int i = lifeCount - 1; i >= remainingHearts; i--)
            {
                if (lifes[i].transform.localScale.x >= startScale.x)
                {
                    if(audioSource != null)
                    {
                        audioSource.PlayOneShot(shrinkSound);
                    }

                    yield return ScaleOverTime(lifes[i].transform, Vector3.zero, scaleTime);
                }
            }       
        }
        else
        {
            for(int i = 0; i < remainingHearts; i++)
            {
                if (lifes[i].transform.localScale.x < startScale.x)
                {
                    if(audioSource != null)
                    {
                        audioSource.PlayOneShot(growSound);
                    }

                    yield return ScaleOverTime(lifes[i].transform, startScale, scaleTime);
                }            
            }
        }

        yield return new WaitForSeconds(zoomTime);
        yield return ScaleOverTime(this.transform, originalHeartsScale, scaleTime);

        if (currentLifes <= 0)
        {
            onResetCheckpoint();
            RefillLife();
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

    private void ToggleErrors(bool isShieldEnabled)
    {
        this.isShieldEnabled = isShieldEnabled;
    }
}
