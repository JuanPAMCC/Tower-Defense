using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchManager : MonoBehaviour
{
    public delegate void PlatformTouched(GameObject platform);
    public event PlatformTouched OnPlatformTouched;

    public InputActionAsset inputActions;

    private InputAction touchAction;
    private InputAction touchPositionAction;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        TouchSimulation.Enable();

        if (inputActions == null)
        {
            return;
        }

        inputActions.Enable();

        InputActionMap touchMap = inputActions.FindActionMap("Toques");

        if (touchMap == null)
        {
            return;
        }

        touchAction = touchMap.FindAction("Toque");
        touchPositionAction = touchMap.FindAction("PosicionToque");

        if (touchAction != null)
        {
            touchAction.performed += Touch;
        }
    }

    void OnDisable()
    {
        if (touchAction != null)
        {
            touchAction.performed -= Touch;
        }

        if (inputActions != null)
        {
            inputActions.Disable();
        }

        TouchSimulation.Disable();
    }

    private void Touch(InputAction.CallbackContext context)
    {
        if (mainCamera == null || touchPositionAction == null)
        {
            return;
        }

        Vector2 touchPosition = touchPositionAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.Log("Touched: " + hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Plataforma"))
            {
                Debug.Log("Platform touched");

                if (OnPlatformTouched != null)
                {
                    OnPlatformTouched(hit.collider.gameObject);
                }
            }
        }
        else
        {
            Debug.Log("No hit");
        }
    }
}