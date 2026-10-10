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
        MemoryCard hoveredCard;
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
            bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            MemoryCard target = null;
            if (!overUI)
            {
                Ray ray = view.ScreenPointToRay(mouse.position.ReadValue());
                if (Physics.Raycast(ray, out var hit, 20f))
                    target = hit.collider.GetComponentInParent<MemoryCard>();
            }
            if (hoveredCard != target) hoveredCard?.SetHover(false);
            hoveredCard = target;
            hoveredCard?.SetHover(true);
            if (mouse.leftButton.wasPressedThisFrame) target?.Select();
        }
    }
}
