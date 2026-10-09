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
        }

        public void Select() { if (room != null) room.SelectCard(index); }

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
        }

        public void ShowFace(bool visible)
        {
            front.SetActive(visible);
            cardRenderer.sharedMaterial = visible ? faceUpMaterial : faceDownMaterial;
        }

        public void DropMatched()
        {
            body.isKinematic = false;
            body.useGravity = true;
            body.AddTorque(new Vector3(0.12f, 0.08f, 0.04f), ForceMode.Impulse);
        }
    }
}
