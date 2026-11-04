
using UnityEngine;

public class InteractiveDeskLamp : MonoBehaviour
{
    [Header("Lamp Components")]
    [SerializeField] private Transform lever;
    [SerializeField] private Light bulbLight1;
    [SerializeField] private Light bulbLight2;

    [Header("Lever Animation")]
    [SerializeField] private float pressAngle = 12f;
    [SerializeField] private float animationSpeed = 12f;

    private Quaternion initialRotation;
    private bool isPressed;
    private bool isOn;

    private void Start()
    {
        if (lever == null || bulbLight1 == null || bulbLight2 == null)
        {
            Debug.LogError("Desk Lamp: Missing references!");
            enabled = false;
            return;
        }

        initialRotation = lever.localRotation;

        // Lamp starts OFF
        isOn = false;
        bulbLight1.enabled = false;
        bulbLight2.enabled = false;
    }

    private void Update()
    {
        if (DeskPointer.PressedOn(this)) Press();
        if (DeskPointer.Released) isPressed = false;
        Quaternion targetRotation = initialRotation;

        if (isPressed)
        {
            targetRotation = initialRotation *
                Quaternion.Euler(pressAngle, 0f, 0f);
        }

        lever.localRotation = Quaternion.Slerp(
            lever.localRotation,
            targetRotation,
            Time.deltaTime * animationSpeed
        );
    }

    public void Press()
    {
        if(UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        isPressed = true;

        isOn = !isOn;
        bulbLight1.enabled = isOn;
        bulbLight2.enabled = isOn;
    }

    private void OnDisable()
    {
        isPressed = false;
    }
}
