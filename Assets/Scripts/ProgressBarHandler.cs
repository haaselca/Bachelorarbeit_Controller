using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProgressBarHandler : MonoBehaviour
{
    [SerializeField] Vector2 tileSize;
    [SerializeField] GameObject progressTile;
    [SerializeField] Sprite filledTile;
    [SerializeField] Sprite halfTile;

    [SerializeField] Material emptyColor;
    [SerializeField] Material currentStepColor;
    [SerializeField] Material normalClearColor;
    [SerializeField] Material supportClearColor;
    [SerializeField] Material errorClearColor;

    [SerializeField] private int nextProgressSteps = 10;
    private int currentProgressSteps;

    private Vector2 cornerPartSize;
    private float tileAngle;
    private float currentAngle;
    private List<GameObject> tiles;
    private Component[] spriteRenderers;
    private List<SpriteRenderer> tileColors;
    private int currentStep;

    const int corners = 1;
    const float angleSpectrum = 160f;
    const float minAngle = -80f;
    const float maxWidth = 100;

    public delegate void OnProgressBarComplete();
    public static event OnProgressBarComplete onProgressBarComplete;

    private delegate void VoidFunction();

    void Start()
    {
        cornerPartSize = new Vector2(tileSize.y * 2, tileSize.y * 2);
        tileColors = new List<SpriteRenderer>();
        tiles = new List<GameObject>();
        InitializeProgressBar();
    }

    private void OnEnable()
    {
        DummyInputTester.onProgressTraining += CompleteProgressStep;
        LifeCountChanger.onResetCheckpoint += LoadLastCheckpoint;
    }

    private void OnDisable()
    {
        DummyInputTester.onProgressTraining -= CompleteProgressStep;
        LifeCountChanger.onResetCheckpoint -= LoadLastCheckpoint;
    }


    private void InitializeProgressBar()
    {
        DeleteOldSteps();
        CalculateStepCount();
        GenerateProgressBar();
        EmptyTiles();
        CompleteProgressStep();
    }

    private void DeleteOldSteps()
    {
        if(tiles.Count > 0)
        {
            for(int i = 0; i < tiles.Count; i++)
            {
                Destroy(tiles[i]);
            }

            tiles.Clear();
            spriteRenderers = null;
            tileColors.Clear();
        }
    }

    private void CalculateStepCount()
    {
        currentProgressSteps = nextProgressSteps;  // Will be expanded when steps are defined in creator
    }

    private void GenerateProgressBar()
    {
        // Tiles are smaller the more progress steps there are
        tileSize.x = maxWidth / currentProgressSteps - 1;
        tileAngle = angleSpectrum / (currentProgressSteps + corners - 1);

        for (int i = 0; i < currentProgressSteps + corners; i++)
        {
            InstantiateAndRotateTile(i);

            bool isFirstOrLastStep = (i == 0 || i == currentProgressSteps + corners - 1);

            DefineTileSize(isFirstOrLastStep);
        }
    }

    private void InstantiateAndRotateTile(int i)
    {
        currentAngle = minAngle + tileAngle * i;
        var prefabRot = Quaternion.Euler(0f, 0f, currentAngle);

        var part = Instantiate(progressTile, this.transform);
        part.transform.localRotation = prefabRot;
        tiles.Add(part);

        spriteRenderers = part.GetComponentsInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Sets the length of the tile. If first or last, the with is also changed
    /// </summary>
    /// <param name="isFirstOrLastStep">First and last step are shaped differently</param>
    private void DefineTileSize(bool isFirstOrLastStep)
    {
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (isFirstOrLastStep)
            {
                renderer.size = cornerPartSize;
            }
            else
            {
                renderer.size = tileSize;
            }

            if (renderer.name == "ProgressTileFilled")
            {
                tileColors.Add(renderer);
            }
        }
    }

    private void EmptyTiles()
    {
        foreach (SpriteRenderer field in tileColors)
        {
            field.material = emptyColor;
        }
    }

    /// <summary>
    /// Fills the current tile, selects the next tile and plays animation when checkpoint is reached
    /// </summary>
    private void CompleteProgressStep()
    {
        if(currentStep < currentProgressSteps + corners)
        {
            tileColors[currentStep].material = normalClearColor;
            tileColors[currentStep].sprite = filledTile;
            currentStep++;

            if (currentStep < currentProgressSteps + corners)
            {
                tileColors[currentStep].material = currentStepColor;
                tileColors[currentStep].sprite = halfTile;

            }
            else
            {
                onProgressBarComplete();
                PlayCheckPointAnimation();
            }
        }
        else
        {
            currentStep = 0;
            InitializeProgressBar();
        }
    }

    /// <summary>
    /// Swaps the current and next progress bar in sync with animation
    /// </summary>
    private void PlayCheckPointAnimation()
    {
        StartCoroutine(ProgessAfterSeconds(2f, new VoidFunction[] { CompleteProgressStep }));
    }

    private IEnumerator ProgessAfterSeconds(float seconds, VoidFunction[] functionsAfterWait)
    {
        yield return new WaitForSeconds(seconds);
        
        foreach(VoidFunction f in functionsAfterWait)
        {
            f();
        }
    }

    private void LoadLastCheckpoint()
    {
        for(int i = 0; i < currentStep; i++)
        {
            DummyInputTester.RevertProgressStep();
        }

        currentStep = 0;
        WaitForResetAnimation();
    }

    /// <summary>
    /// Emptys the tiles in sync with the animation
    /// </summary>
    private void WaitForResetAnimation()
    {
        StartCoroutine(ProgessAfterSeconds(1.25f, new VoidFunction[] { EmptyTiles, CompleteProgressStep }));
    }
}
