using UnityEngine;

namespace FXR.Memory
{
    // A separate practice object demonstrates distance manipulation without
    // adding drag difficulty to the measured matching task.
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class MemoryPracticeObject : MonoBehaviour
    {
        public MemoryRoomController room;
        public bool IsDragging { get; private set; }
        public bool CanDrag => room != null && room.Session != null && !room.ResetPending && !room.IsPaused
            && (room.Session.Phase == SessionPhase.Idle || room.Session.Phase == SessionPhase.Completed);
        Rigidbody body;
        Vector3 home, offset;
        Quaternion homeRotation;
        Plane dragPlane;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            home = transform.localPosition;
            homeRotation = transform.localRotation;
        }

        public bool BeginDrag(Ray ray)
        {
            if (!CanDrag) return false;
            dragPlane = new Plane(Vector3.up, transform.position);
            if (!dragPlane.Raycast(ray, out float distance)) return false;
            offset = transform.position - ray.GetPoint(distance);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
            IsDragging = true;
            return true;
        }

        public void Drag(Ray ray)
        {
            if (!IsDragging) return;
            if (!CanDrag) { EndDrag(); return; }
            if (!dragPlane.Raycast(ray, out float distance)) return;
            var local = transform.parent.InverseTransformPoint(ray.GetPoint(distance) + offset);
            local.x = Mathf.Clamp(local.x, home.x - .4f, home.x + .4f);
            local.z = Mathf.Clamp(local.z, home.z - .3f, home.z + .3f);
            local.y = home.y;
            body.MovePosition(transform.parent.TransformPoint(local));
        }

        public void EndDrag()
        {
            if (!IsDragging) return;
            IsDragging = false;
            body.isKinematic = false;
        }

        public void ResetObject()
        {
            if (body == null) return;
            IsDragging = false;
            body.isKinematic = false;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = transform.parent.TransformPoint(home);
            body.rotation = transform.parent.rotation * homeRotation;
        }

        void OnDisable() { EndDrag(); }
    }
}
