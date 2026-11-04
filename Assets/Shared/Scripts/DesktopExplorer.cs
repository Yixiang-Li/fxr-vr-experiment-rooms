using FXR.Memory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FXR.Shared
{
    // Keyboard/mouse delivery mode. The optional XR rig remains a separate mode.
    [RequireComponent(typeof(CharacterController))]
    public sealed class DesktopExplorer : MonoBehaviour
    {
        public Camera view;
        public float moveSpeed = 2f;
        float pitch, viewYaw;
        CharacterController body;
        MemoryCard hoveredCard;
        MemoryPracticeObject dragged;
        MemoryRoomController room;
        SessionPhase previousPhase;
        public bool IsFlying { get; private set; }
        public bool TaskLocked => room != null && room.gameObject.activeInHierarchy && room.Session != null
            && (room.ResetPending || (room.Session.Phase != SessionPhase.Idle && room.Session.Phase != SessionPhase.Completed));
        public DesktopControlsUI Controls { get; private set; }
        readonly List<RaycastResult> uiHits = new List<RaycastResult>();
        void Awake()
        {
            body = GetComponent<CharacterController>();
            // Small frame steps still move at high frame rates, especially when
            // flying vertically. The stock threshold can discard these moves.
            body.minMoveDistance = 0;
        }

        void Start()
        {
            BindMemoryRoom(FindFirstObjectByType<MemoryRoomController>());
        }

        public void BindMemoryRoom(MemoryRoomController controller)
        {
            if(room != null) room.ResetPerformed -= ResetView;
            if(Controls != null) { Destroy(Controls.Canvas.gameObject); Destroy(Controls); Controls = null; }
            room = controller;
            if (room == null) return;
            room.SetDesktopInput();
            room.ResetPerformed += ResetView;
            Controls = gameObject.AddComponent<DesktopControlsUI>();
            Controls.Initialize(this, room, view);
            previousPhase = room.Session.Phase;
            if (NearMemoryRoom()) FocusTaskView();
        }

        public void ConfigureView(Vector3 position, Quaternion rotation, Vector3 cameraPosition, Quaternion cameraRotation, float fieldOfView)
        {
            dragged?.EndDrag(); dragged = null;
            body.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            view.transform.SetLocalPositionAndRotation(cameraPosition, cameraRotation);
            view.fieldOfView = fieldOfView;
            pitch = Mathf.DeltaAngle(0, cameraRotation.eulerAngles.x); IsFlying = false;
            viewYaw = Mathf.DeltaAngle(0, cameraRotation.eulerAngles.y);
            body.enabled = true;
        }

        bool NearMemoryRoom()
        {
            if (room == null || !room.gameObject.activeInHierarchy) return false;
            var p = room.transform.InverseTransformPoint(transform.position);
            return Mathf.Abs(p.x) < 4.7f && p.z > -4.5f && p.z < 7f;
        }

        public void FocusTaskView()
        {
            if (room == null) return;
            dragged?.EndDrag(); dragged = null;
            bool wasEnabled = body.enabled;
            body.enabled = false;
            transform.position = room.transform.TransformPoint(new Vector3(0, 0, .08f));
            transform.rotation = room.transform.rotation;
            view.transform.localPosition = new Vector3(0, 1.72f, 0);
            view.transform.localRotation = Quaternion.identity;
            view.fieldOfView = 64;
            pitch = 0;
            viewYaw = 0;
            IsFlying = false;
            body.enabled = wasEnabled;
        }

        void ResetView() { if (NearMemoryRoom()) FocusTaskView(); }

        public void ToggleFlight()
        {
            if (!TaskLocked && (Controls == null || !Controls.BlocksNavigation)) IsFlying = !IsFlying;
        }

        bool PointerOverUI(Vector2 position)
        {
            if (EventSystem.current == null) return false;
            uiHits.Clear();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = position }, uiHits);
            return uiHits.Count > 0;
        }

        void Update()
        {
            var mouse = Mouse.current;
            var keys = Keyboard.current;
            if (mouse == null || keys == null) return;
            if (room != null && room.gameObject.activeInHierarchy && room.Session.Phase != previousPhase)
            {
                if (room.Session.Phase == SessionPhase.Running) FocusTaskView();
                previousPhase = room.Session.Phase;
            }
            bool near = NearMemoryRoom();
            bool helpOpen = Controls != null && Controls.HelpOpen;
            if (keys.escapeKey.wasPressedThisFrame && near)
            {
                if (helpOpen) Controls.CloseHelp();
                else if (room.ResetPending) room.CancelReset();
                else room.TogglePause();
            }
            if (keys.hKey.wasPressedThisFrame && near && !TaskLocked) Controls.ToggleHelp();
            if (!helpOpen && near && !room.ResetPending && !room.IsPaused)
            {
                if (keys.fKey.wasPressedThisFrame) FocusTaskView();
                if (keys.spaceKey.wasPressedThisFrame && (room.Session.Phase == SessionPhase.Idle || room.Session.Phase == SessionPhase.ReadyForNextRound)) room.StartOrContinue();
                if (room.Session.Phase == SessionPhase.Rating)
                    for (int i = 0; i < 5; i++)
                        if (keys[(Key)((int)Key.Digit1 + i)].wasPressedThisFrame || keys[(Key)((int)Key.Numpad1 + i)].wasPressedThisFrame) room.RateEffort(i + 1);
            }
            if (keys.gKey.wasPressedThisFrame) ToggleFlight();
            bool overUI = PointerOverUI(mouse.position.ReadValue());
            bool canNavigate = !TaskLocked && (Controls == null || !Controls.BlocksNavigation) && dragged == null;
            if (canNavigate && !overUI && mouse.rightButton.isPressed)
            {
                Vector2 delta = mouse.delta.ReadValue() * 0.12f;
                transform.Rotate(0f, delta.x, 0f);
                pitch = Mathf.Clamp(pitch - delta.y, -65f, 65f);
                view.transform.localEulerAngles = new Vector3(pitch, viewYaw, 0f);
            }
            Vector2 direction = new Vector2((keys.dKey.isPressed || keys.rightArrowKey.isPressed ? 1 : 0) - (keys.aKey.isPressed || keys.leftArrowKey.isPressed ? 1 : 0),
                (keys.wKey.isPressed || keys.upArrowKey.isPressed ? 1 : 0) - (keys.sKey.isPressed || keys.downArrowKey.isPressed ? 1 : 0));
            if (canNavigate)
            {
                Vector3 forward = IsFlying ? view.transform.forward : transform.forward;
                Vector3 move = transform.right * direction.x + forward * direction.y;
                if (IsFlying) move += Vector3.up * ((keys.eKey.isPressed ? 1 : 0) - (keys.qKey.isPressed ? 1 : 0));
                body.Move((move.normalized * moveSpeed + (IsFlying ? Vector3.zero : Vector3.down * 2f)) * Time.deltaTime);
            }
            if (dragged != null)
            {
                if (!mouse.leftButton.isPressed || TaskLocked || (Controls != null && Controls.BlocksNavigation)) { dragged.EndDrag(); dragged = null; }
                else dragged.Drag(view.ScreenPointToRay(mouse.position.ReadValue()));
            }
            MemoryCard target = null;
            if (!overUI && !mouse.rightButton.isPressed && dragged == null
                && (Controls == null || !Controls.BlocksNavigation)
                && mouse.position.x.ReadValue() >= 0 && mouse.position.y.ReadValue() >= 0
                && mouse.position.x.ReadValue() <= Screen.width && mouse.position.y.ReadValue() <= Screen.height)
            {
                Ray ray = view.ScreenPointToRay(mouse.position.ReadValue());
                if (Physics.Raycast(ray, out var hit, 20f))
                {
                    target = hit.collider.GetComponentInParent<MemoryCard>();
                    if (mouse.leftButton.wasPressedThisFrame)
                    {
                        var practice = hit.collider.GetComponentInParent<MemoryPracticeObject>();
                        if (practice != null && practice.BeginDrag(ray)) dragged = practice;
                    }
                }
            }
            if (hoveredCard != target) hoveredCard?.SetHover(false);
            hoveredCard = target;
            hoveredCard?.SetHover(true);
            if (mouse.leftButton.wasPressedThisFrame) target?.Select();
        }

        void OnDisable()
        {
            dragged?.EndDrag(); dragged = null;
            hoveredCard?.SetHover(false); hoveredCard = null;
        }

        void OnDestroy() { if (room != null) room.ResetPerformed -= ResetView; }
    }
}
