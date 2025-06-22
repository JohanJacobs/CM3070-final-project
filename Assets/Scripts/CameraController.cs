using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject _camera;
    [SerializeField] GameObject focusPoint;

    [SerializeField] float distance;
    [SerializeField] float cameraMoveDampingFactor;
    
    float pitch;// in degrees
    float yaw; // in degrees
    float zoom;
    bool moveCamera;

    [SerializeField]
    float startPitch;
    [SerializeField]
    float startYaw;

    [SerializeField]
    float minPitch;
    [SerializeField]
    float maxPitch;

    Vector2 mouseLookDeltaValue = Vector2.zero;

    VehicleControllerInputActions inputActions;

    private void Awake()
    {
        pitch = startPitch;
        yaw = startYaw;
        zoom = distance;

        moveCamera = false;

        inputActions = new VehicleControllerInputActions();
        inputActions.Enable();


    }

    private void OnEnable()
    {
        inputActions.VehicleControllerInputs.CameraInteract.performed += InputAction_Interact;
        inputActions.VehicleControllerInputs.CameraInteract.canceled += InputAction_Interact;

        inputActions.VehicleControllerInputs.CameraMove.performed += InputAction_MouseLookDelta;
        inputActions.VehicleControllerInputs.CameraMove.canceled += InputAction_MouseLookDelta;

        inputActions.VehicleControllerInputs.CameraZoom.performed += InputAction_MouseZoom;
        inputActions.VehicleControllerInputs.CameraZoom.canceled += InputAction_MouseZoom;

    }

    private void OnDisable()
    {
        inputActions.VehicleControllerInputs.CameraInteract.performed -= InputAction_Interact;
        inputActions.VehicleControllerInputs.CameraInteract.canceled -= InputAction_Interact;

        inputActions.VehicleControllerInputs.CameraMove.performed -= InputAction_MouseLookDelta;
        inputActions.VehicleControllerInputs.CameraMove.canceled -= InputAction_MouseLookDelta;

        inputActions.VehicleControllerInputs.CameraZoom.performed -= InputAction_MouseZoom;
        inputActions.VehicleControllerInputs.CameraZoom.canceled -= InputAction_MouseZoom;
    }

    private void Start()
    {
        UpdateCameraLocalPositionAndRotation();
    }
    private void LateUpdate()
    {
        transform.position = focusPoint.transform.position;
    }


    /*
        Mouse look delta is called by the input actions to rotate the camera around an object.
        look delta is a vector2 so that :
        - x is negative for moving the mouse to the left
        - x is positive for moving the mouse to the right
        - y is negative for moving the mouse down
        - y is positive for moving the mouse up
     */
    public void InputAction_MouseLookDelta(InputAction.CallbackContext ctx)
    {
        // only update camera if the player is interacting 
        if (!moveCamera) return;

        // only update when the action is performed
        if (!ctx.performed) return;


        mouseLookDeltaValue = ctx.ReadValue<Vector2>();
        // yaw is adjusted with the X axis 
        yaw += mouseLookDeltaValue.x * (1f / cameraMoveDampingFactor);
                
        // pitch is adjusted with the Y axis
        pitch += (-mouseLookDeltaValue.y) * (1f / cameraMoveDampingFactor);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        UpdateCameraLocalPositionAndRotation();
    }

    public void InputAction_MouseZoom(InputAction.CallbackContext ctx)
    {
        // only update camera if the player is interacting 
        if (!moveCamera) return;

        // only update when the action is performed
        if (!ctx.performed) return;
        Debug.Log(ctx.ReadValue<float>());
        zoom = Mathf.Clamp(zoom + Mathf.Sign(ctx.ReadValue<float>()), 1f, 10f);

        UpdateCameraLocalPositionAndRotation();
    }

    public void InputAction_Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            moveCamera = true;            
        }

        if (ctx.canceled)
        {
            moveCamera = false;            
        }
        
        
    }

    public void Update()
    {
        // update position 
        transform.position = focusPoint.transform.position;
    }

    public void UpdateCameraLocalPositionAndRotation()
    {

        // calculate desired local position 
        var t = Quaternion.Euler(new Vector3(-pitch, yaw, 0f));
        Vector3 newPosition = (t * Vector3.forward) * zoom;
        _camera.transform.localPosition = newPosition;
        _camera.transform.LookAt(transform.position);
    }
}
