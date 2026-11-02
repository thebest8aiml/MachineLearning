using System;
using UnityEngine;

namespace Unity.MLAgentsExamples
{

    /// FYI this bit is entirely AI idk how this works... im tired its 4 am im heading to sleep...

    /// <summary>
    /// Central place to notify listeners that a cube/target has moved.
    /// Add a listener component (for example <see cref="HelloOnCubeMove"/>)
    /// to any GameObject in the scene.
    /// </summary>
    public static class CubeMoveHook
    {
        /// <summary>
        /// Called with the cube's Transform whenever it moves.
        /// </summary>
        public static event Action<Transform> CubeMoved;

        // Clear stale subscriptions when entering Play mode with domain reload disabled.
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
