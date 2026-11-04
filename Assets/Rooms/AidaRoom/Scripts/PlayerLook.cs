
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float verticalLimit = 80f;

    private float verticalRotation;

    private void Start()
    {
        verticalRotation = Mathf.DeltaAngle(
            0f,
            transform.localEulerAngles.x
        );

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        playerBody.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -verticalLimit,
            verticalLimit
        );

        transform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
