using FXR.Memory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FXR.Shared
{
    // Desktop preview only; Quest uses the shared XRI rig and its controller bindings.
    [RequireComponent(typeof(CharacterController))]
    public sealed class DesktopExplorer : MonoBehaviour
    {
        public Camera view;
        public float moveSpeed = 2f;
        float pitch;
        CharacterController body;
        void Awake() { body = GetComponent<CharacterController>(); }

        void Update()
        {
            var mouse = Mouse.current;
            var keys = Keyboard.current;
            if (mouse == null || keys == null) return;
            if (mouse.rightButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue() * 0.12f;
                transform.Rotate(0f, delta.x, 0f);
                pitch = Mathf.Clamp(pitch - delta.y, -65f, 65f);
                view.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
            }
            Vector2 direction = new Vector2((keys.dKey.isPressed ? 1 : 0) - (keys.aKey.isPressed ? 1 : 0),
                (keys.wKey.isPressed ? 1 : 0) - (keys.sKey.isPressed ? 1 : 0));
            Vector3 move = transform.right * direction.x + transform.forward * direction.y;
            body.Move((move.normalized * moveSpeed + Vector3.down * 2f) * Time.deltaTime);
            if (mouse.leftButton.wasPressedThisFrame
                && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
            {
                Ray ray = view.ScreenPointToRay(mouse.position.ReadValue());
                if (Physics.Raycast(ray, out var hit, 20f))
                    hit.collider.GetComponentInParent<MemoryCard>()?.Select();
            }
        }
    }
}
