using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Use the same pointer coordinate system as the shared desktop UI. Legacy
// Input.mousePosition can differ in a standalone macOS window.
internal static class DeskPointer
{
    public static bool Released => Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;

    public static bool PressedOn(Component target, Camera camera = null)
    {
        var mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return false;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return false;
        if (camera == null) camera = Camera.main;
        if (camera == null) return false;
        var ray = camera.ScreenPointToRay(mouse.position.ReadValue());
        return Physics.Raycast(ray, out var hit) && hit.collider.gameObject == target.gameObject;
    }
}
