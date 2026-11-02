using UnityEngine;

namespace Unity.MLAgentsExamples
{
    /// <summary>
    /// Prints "hello" every time a cube raises <see cref="CubeMoveHook.CubeMoved"/>.
    /// Add this component to any GameObject in the scene, or to the target cube.
    /// </summary>
    public class HelloOnCubeMove : MonoBehaviour
    {
        void OnEnable()
        {
            CubeMoveHook.CubeMoved += OnCubeMoved;
        }

        void OnDisable()
        {
            CubeMoveHook.CubeMoved -= OnCubeMoved;
        }

        void OnCubeMoved(Transform cube)
        {
	    Debug.Log($"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)");
            Debug.Log($"Total RAM: {SystemInfo.systemMemorySize} MB");
            Debug.Log($"GPU: {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsMemorySize} MB)");
            Debug.Log($"OS: {SystemInfo.operatingSystem}");
            Debug.Log("hello");
        }
    }
}
