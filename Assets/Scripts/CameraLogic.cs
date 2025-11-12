using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLogic : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference lookAction; // Asigna tu acción de Look (Mouse Delta, Stick, etc.)

    [Header("Settings")]
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private float verticalClamp = 80.0f;
    [SerializeField] private int width = 16;
    [SerializeField] private int height = 9;

     private float targetAspectRatio = 0 ;
    float currentAspectRatio = 0;

    private float pitch = 0f; // Rotación vertical acumulada
    private float yaw = 0f;   // Rotación horizontal acumulada
    private Vector2 lookInput;

    private void Awake()
    {
        targetAspectRatio = width/height;

        currentAspectRatio = targetAspectRatio;

        Camera cam = GetComponent<Camera>();

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

        if (scaleHeight < 1.0f)
        {
            // Letterbox
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Pillarbox
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }

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

        targetAspectRatio = width / height;

        if (targetAspectRatio != currentAspectRatio)
        {
            currentAspectRatio = targetAspectRatio;

            Camera cam = GetComponent<Camera>();

            float windowAspect = (float)Screen.width / Screen.height;
            float scaleHeight = windowAspect / targetAspectRatio;

            if (scaleHeight < 1.0f)
            {
                // Letterbox
                Rect rect = cam.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                cam.rect = rect;
            }
            else
            {
                // Pillarbox
                float scaleWidth = 1.0f / scaleHeight;
                Rect rect = cam.rect;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
                cam.rect = rect;
            } 
        }
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
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Cursor libre
        Cursor.visible = true;
    }
}