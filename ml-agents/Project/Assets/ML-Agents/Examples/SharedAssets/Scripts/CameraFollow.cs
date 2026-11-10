using UnityEngine;

namespace Unity.MLAgentsExamples
{
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("The target to follow")] public Transform target;

        [Tooltip("The time it takes to move to the new position")]
        public float smoothingTime; //The time it takes to move to the new position

        private Vector3 m_Offset;
        private Vector3 m_CamVelocity; //Camera's velocity (used by SmoothDamp)

        // ***** Rotation ******
        private float yaw;
        private float pitch;
        public float mouseSensitivity = 3f;
        public float minPitch = -30f;
        public float maxPitch = 80f;

        // Use this for initialization
        void Start()
        {
            m_Offset = gameObject.transform.position - target.position;

            Vector3 e = transform.eulerAngles;
            yaw = e.y;
            pitch = e.x;
        }

        void FixedUpdate()
        {
            var newPosition = new Vector3(target.position.x + m_Offset.x, transform.position.y,
                target.position.z + m_Offset.z);

            gameObject.transform.position =
                Vector3.SmoothDamp(transform.position, newPosition, ref m_CamVelocity, smoothingTime, Mathf.Infinity,
                    Time.fixedDeltaTime);
        }

        void LateUpdate()
        {
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPos = target.position + rot * m_Offset;
            transform.position = desiredPos;
            transform.LookAt(target);
        }
    }
}
