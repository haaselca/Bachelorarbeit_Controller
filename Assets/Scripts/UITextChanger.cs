using TMPro;
using UnityEngine;

public class UITextChanger : MonoBehaviour
{
    [SerializeField] TMP_Text displayedText;
    [SerializeField] string discription;

    private void OnTriggerEnter(Collider other)
    {
        displayedText.text = discription;
    }
}
