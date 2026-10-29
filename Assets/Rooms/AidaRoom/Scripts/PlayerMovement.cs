
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = 0f;
        float forward = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            forward += 1f;

        if (Input.GetKey(KeyCode.DownArrow))
            forward -= 1f;

        if (Input.GetKey(KeyCode.RightArrow))
            horizontal += 1f;

        if (Input.GetKey(KeyCode.LeftArrow))
            horizontal -= 1f;

        Vector3 direction =
            transform.right * horizontal +
            transform.forward * forward;

        // Prevent diagonal movement from being faster.
        direction = Vector3.ClampMagnitude(direction, 1f);

        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity =
            direction * moveSpeed +
            Vector3.up * verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }
}
