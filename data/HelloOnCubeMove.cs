using UnityEngine;
using YamlDotNet.Serialization;

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
	    string[] header = {"id","game","date","group_name","trainer","max_steps","learning_rate", "beta", "epsilon","lamdb", "num_epoch","batch_size","buffer_size","hidden_units","num_layers","beta","cpu_cores","ram_gb","gpu_model","time_sec","peak_ram_mb","final_reward","avg_ram_mb","steps_to_90pct","food_eaten","poison_eaten","final_size","block_goal_reached","worm_length", "operating_system", "neural_network_type"};

	    Dictionary<string,string> entry = new Dictionary<string,string>();

	    entry.Add(SystemInfo.processorCount, "cpu_cores");
	    entry.Add(SystemInfo.systemMemorySize, "ram_gb");
	    entry.Add(SystemInfo.graphicsDeviceName, "gpu_model");
	    entry.Add(SystemInfo.operatingSystem, "operating_system");

	    //change the next few values manually. The alternative is to read the value directly from the yaml file which is a pain. To do it conveniently you'd need a library called YamlDotNet.
            //the default values are that of the worm. Find your agents corresponding config file in ml-agents/config.

	    // network settings
	    entry.Add("512", "hidden_units");
	    entry.Add("3", "num_layers");
	    entry.Add("ppo", "neural_network_type");
	    // hyperparameters
	    entry.Add("batch_size", "2024");
	    entry.Add("buffer_size", "20240");
	    entry.Add("learning_rate", "0.0003");
	    entry.Add("beta", "0.005");
	    entry.Add("epsilon", "0.2");
	    entry.Add("lambd", "0.95");
	    entry.Add("num_epoch", "3");
	    // reward signals
	    entry.Add("gamma", "0.995");
	    entry.Add("strength", "1.0");
	    entry.Add("max_steps", "7000000");
            // resource allocation statistics
	    
            Debug.Log("hello");
        }
    }
}
