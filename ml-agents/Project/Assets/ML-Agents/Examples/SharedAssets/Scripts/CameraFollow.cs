using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.MLAgentsExamples
{
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("The target to follow")] public Transform target;

        // List of other possible targets to follow, which the user can cycle between.
        [Tooltip("Other possible targets to follow")] public Transform[] otherTargets;
        private int t_CurrentTargetIndex = 0;
        public UnityEvent<Transform, int> onTargetChangedEvent;

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
            if (target is null && otherTargets is not null && otherTargets.Length > 0)
            {
                target = otherTargets[0];
            }
            if (target is null)
            {
                throw new Exception("CameraFollow: No target specified");
            }
            if (otherTargets is null || otherTargets.Length == 0)
            {
                otherTargets = new Transform[1];
                otherTargets[0] = target;
            }

            t_CurrentTargetIndex = Array.IndexOf(otherTargets, target);

            m_Offset = gameObject.transform.position - target.position;

            Vector3 e = transform.eulerAngles;
            yaw = e.y;
            pitch = e.x;

            // Init event
            onTargetChangedEvent?.Invoke(target, t_CurrentTargetIndex);
        }

        void FixedUpdate()
        {
            var newPosition = new Vector3(target.position.x + m_Offset.x, transform.position.y,
                target.position.z + m_Offset.z);

            gameObject.transform.position =
                Vector3.SmoothDamp(transform.position, newPosition, ref m_CamVelocity, smoothingTime, Mathf.Infinity,
                    Time.fixedDeltaTime);
        }

        static int Modulus(int x, int m) => x < 0 ? ((x % m) + m) % m : x % m;

        void CycleTarget(int direction)
        {
            if (otherTargets.Length <= 1)
            {
                return;
            }

            t_CurrentTargetIndex += direction;

            t_CurrentTargetIndex = Modulus(t_CurrentTargetIndex, otherTargets.Length);

            target = otherTargets[t_CurrentTargetIndex];
            onTargetChangedEvent?.Invoke(target, t_CurrentTargetIndex);
        }

        void LateUpdate()
        {
            // Check for arrow keys or space
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                CycleTarget(1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CycleTarget(-1);
            }

            // Adjust camera rotation based on mouse input
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            // Update camera position and rotation
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPos = target.position + rot * m_Offset;
            transform.position = desiredPos;
            transform.LookAt(target);
        }
    }
}
