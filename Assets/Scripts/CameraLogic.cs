using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLogic : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference lookAction; // Asigna tu acción de Look (Mouse Delta, Stick, etc.)

    [Header("Settings")]
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private float verticalClamp = 80.0f;

    private float pitch = 0f; // Rotación vertical acumulada
    private float yaw = 0f;   // Rotación horizontal acumulada
    private Vector2 lookInput;

    private bool cursorLocked = true;

    private void OnEnable()
    {
        if (lookAction != null)
        {
            lookAction.action.performed += OnLook;
            lookAction.action.canceled += OnLookCanceled;
            lookAction.action.Enable();
        }
        LockCursor();
    }

    private void OnDisable()
    {
        if (lookAction != null)
        {
            lookAction.action.performed -= OnLook;
            lookAction.action.canceled -= OnLookCanceled;
            lookAction.action.Disable();
        }

        UnlockCursor();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext ctx)
    {
        lookInput = Vector2.zero;
    }

    private void Update()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        yaw += lookInput.x * sensitivity * Time.deltaTime;
        pitch -= lookInput.y * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined; // Cursor confinado dentro del GameView
        Cursor.visible = false;
        cursorLocked = true;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Cursor libre
        Cursor.visible = true;
        cursorLocked = false;
    }
}