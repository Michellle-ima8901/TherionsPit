using UnityEngine;
using UnityEngine.InputSystem;

namespace yohansouls.showroom
{
    public class CamRotation : MonoBehaviour
    {
        public Transform target;

        [Header("Orbit Settings")]
        public float sensitivity = 0.3f;
        public float minVerticalAngle = -30f;
        public float maxVerticalAngle = 60f;

        private float currentX = 0f;
        private float currentY = 20f;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            currentX = angles.y;
            currentY = angles.x;
        }

        void LateUpdate()
        {
            // Simple Look Around code
            if (target == null) return;

            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();

                currentX += delta.x * sensitivity;
                currentY -= delta.y * sensitivity;
                currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
            }

            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
            float distance = Vector3.Distance(transform.position, target.position);

            transform.position = target.position - (rotation * Vector3.forward * distance);
            transform.LookAt(target.position);
        }
    }

}
