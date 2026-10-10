using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace FXR.Memory
{
    [RequireComponent(typeof(Rigidbody), typeof(XRSimpleInteractable))]
    public sealed class MemoryCard : MonoBehaviour
    {
        public int index;
        public GameObject front;
        public GameObject[] symbols;
        public Renderer cardRenderer;
        public Material faceDownMaterial, faceUpMaterial;
        public MemoryRoomController room;
        public GameObject hoverFrame;
        public GameObject backDecoration;
        Vector3 homePosition;
        Quaternion homeRotation;
        Rigidbody body;
        XRSimpleInteractable interactable;

        void Awake()
        {
            homePosition = transform.localPosition;
            homeRotation = transform.localRotation;
            body = GetComponent<Rigidbody>();
            interactable = GetComponent<XRSimpleInteractable>();
            interactable.selectEntered.AddListener(_ => Select());
            interactable.hoverEntered.AddListener(_ => SetHover(true));
            interactable.hoverExited.AddListener(_ => SetHover(false));
        }

        public void Select() { if (room != null) room.SelectCard(index); }

        public void SetHover(bool active)
        {
            if (hoverFrame != null) hoverFrame.SetActive(active && room != null && room.CanSelectCard(index));
        }

        public void SetAvailable(bool available)
        {
            if (interactable != null) interactable.enabled = available;
            if (!available) SetHover(false);
        }

        public void ResetCard(int pairId)
        {
            if (body == null) body = GetComponent<Rigidbody>();
            if (!body.isKinematic)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
            body.isKinematic = true;
            transform.localPosition = homePosition;
            transform.localRotation = homeRotation;
            for (int i = 0; i < symbols.Length; i++) symbols[i].SetActive(i == pairId);
            ShowFace(false);
            SetHover(false);
        }

        public void ShowFace(bool visible)
        {
            front.SetActive(visible);
            if (backDecoration != null) backDecoration.SetActive(!visible);
            cardRenderer.sharedMaterial = visible ? faceUpMaterial : faceDownMaterial;
        }

        public void DropMatched()
        {
            body.isKinematic = false;
            body.useGravity = true;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            body.linearDamping = .8f;
            body.angularDamping = 2f;
            body.maxAngularVelocity = 1.2f;
            body.maxDepenetrationVelocity = 1.5f;
            // A small forward nudge clears the lower cards/rim without flinging cards at the viewer.
            body.linearVelocity = room.transform.TransformDirection(new Vector3(0, 0, -.25f));
            body.angularVelocity = room.transform.TransformDirection(new Vector3(.3f, .6f, .15f));
        }
    }
}
