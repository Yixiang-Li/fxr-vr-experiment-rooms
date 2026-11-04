using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Share one ray result per click. Start can reposition the camera, so another
// button must not reinterpret that same press from the new task view.
internal static class DeskPointer
{
    static int sampledFrame = -1;
    static Collider clickedCollider;
    static readonly List<RaycastResult> uiHits = new List<RaycastResult>();
    public static bool Released => Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;

    public static bool PressedOn(Component target, Camera camera = null)
    {
        if (Time.timeScale <= 0f) return false;
        var mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return false;
        if (sampledFrame != Time.frameCount)
        {
            sampledFrame = Time.frameCount;
            clickedCollider = null;
            uiHits.Clear();
            if (EventSystem.current != null)
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = mouse.position.ReadValue() }, uiHits);
            if (uiHits.Count == 0)
            {
                if (camera == null) camera = Camera.main;
                if (camera != null && Physics.Raycast(camera.ScreenPointToRay(mouse.position.ReadValue()), out var hit))
                    clickedCollider = hit.collider;
            }
        }
        return clickedCollider != null && clickedCollider.gameObject == target.gameObject;
    }
}
