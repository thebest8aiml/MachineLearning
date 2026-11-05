using System;
using UnityEngine;

namespace Unity.MLAgentsExamples
{

    public class CubeMoveHook : MonoBehaviour
    {
        public static event Action<Transform> CubeMoved;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            CubeMoved = null;
        }

        public static void RaiseCubeMoved(Transform cube)
        {
            CubeMoved?.Invoke(cube);
        }
    }
}
