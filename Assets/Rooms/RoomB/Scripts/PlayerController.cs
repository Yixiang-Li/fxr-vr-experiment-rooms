
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;

    [Header("Observation Camera Limits")]
    public float maxObservationYaw = 30f;
    public float maxObservationPitch = 25f;

    private CharacterController controller;
    private Camera playerCamera;

    private float verticalRotation;
    private float verticalVelocity;

    private bool isObserving = false;

    private float horizontalRotation;
    private float observationStartYaw;
    private float observationStartPitch;

    private bool hasStarted = false;

    void Start()
    {
        ConfigureForRoom();
    }

    public void ConfigureForRoom()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        hasStarted = false; isObserving = false; verticalVelocity = 0;

        horizontalRotation = transform.eulerAngles.y;
        verticalRotation = Mathf.DeltaAngle(
            0f,
            playerCamera.transform.localEulerAngles.x
        );

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (!hasStarted)
            return;
        // Click inside the Game view to resume mouse look.
        if (Application.isFocused &&
            mouse != null &&
            Cursor.lockState != CursorLockMode.Locked &&
            mouse.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // Right drag restores captured mouse look after Escape or focus loss.
        if (Application.isFocused && mouse != null && mouse.rightButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // Mouse input
        bool looking = mouse != null &&
            (Cursor.lockState == CursorLockMode.Locked || mouse.rightButton.isPressed);
        // Match the uploaded legacy mouse-axis sensitivity (0.1).
        Vector2 mouseDelta = looking ? mouse.delta.ReadValue() : Vector2.zero;
        float mouseX = mouseDelta.x * mouseSensitivity * 0.1f;
        float mouseY = mouseDelta.y * mouseSensitivity * 0.1f;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;

        if (isObserving)
        {
            // Limit horizontal rotation around starting direction
            horizontalRotation = Mathf.Clamp(
                horizontalRotation,
                observationStartYaw - maxObservationYaw,
                observationStartYaw + maxObservationYaw
            );

            // Limit vertical rotation around starting direction
            verticalRotation = Mathf.Clamp(
                verticalRotation,
                observationStartPitch - maxObservationPitch,
                observationStartPitch + maxObservationPitch
            );
        }

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -80f,
            80f
        );

        transform.rotation = Quaternion.Euler(
            0f,
            horizontalRotation,
            0f
        );

        playerCamera.transform.localRotation =
            Quaternion.Euler(
                verticalRotation,
                0f,
                0f
            );

        // WASD movement is disabled during observation
        Vector3 movement = Vector3.zero;

        if (!isObserving)
        {
            float horizontal = keyboard == null ? 0f :
                ((keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f) -
                 (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f));
            float vertical = keyboard == null ? 0f :
                ((keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f) -
                 (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f));
            movement =
                transform.right * horizontal +
                transform.forward * vertical;

            movement = Vector3.ClampMagnitude(
                movement,
                1f
            );

            movement *= moveSpeed;
        }

        // Gravity remains active
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        movement.y = verticalVelocity;

        controller.Move(movement * Time.deltaTime);

        // Release mouse
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void StartExperiment()
    {
        hasStarted = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void BeginObservation()
    {
        isObserving = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Remember the direction at the start of observation
        observationStartYaw = horizontalRotation;
        observationStartPitch = verticalRotation;
    }

    public void EndObservation()
    {
        isObserving = false;
    }

    public void TeleportTo(Transform destination)
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (destination == null)
        {
            Debug.LogWarning("Teleport destination is missing!");
            return;
        }

        // Temporarily disable CharacterController for teleportation
        controller.enabled = false;

        transform.position = destination.position;

        // Update player facing direction
        horizontalRotation = destination.eulerAngles.y;

        transform.rotation = Quaternion.Euler(
            0f,
            horizontalRotation,
            0f
        );

        // Reset camera pitch
        verticalRotation = 0f;

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation =
                Quaternion.Euler(0f, 0f, 0f);
        }

        // Reset falling speed
        verticalVelocity = 0f;

        controller.enabled = true;
    }
}
