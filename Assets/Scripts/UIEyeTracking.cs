using UnityEngine;
using UnityEngine.InputSystem;

public class UIEyeTracking : MonoBehaviour
{
    [SerializeField] float movementStrenght = 1f;
    private Vector3 eyePosition;

    public void OnMoveEyes(InputAction.CallbackContext context)
    {
        var stickDirection = context.ReadValue<Vector2>();

        if (stickDirection != Vector2.zero)
        {
            eyePosition = new Vector3(stickDirection.x, stickDirection.y, transform.localPosition.z);
            transform.localPosition = eyePosition * movementStrenght;
        }
    }
}
