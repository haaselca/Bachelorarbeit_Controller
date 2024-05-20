using UnityEngine;

public class UIMoodHandler : MonoBehaviour
{
    [SerializeField] GameObject[] eyeBrows;

    public void OnChangeMood()
    {
        foreach (GameObject eyeBrow in eyeBrows)
        {
            eyeBrow.SetActive(false);
        }

        eyeBrows[LifeCountChanger.CurrentLifes - 1].SetActive(true);
    }
}
