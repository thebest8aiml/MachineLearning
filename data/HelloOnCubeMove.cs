using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Unity.MLAgentsExamples
{
    public class HelloOnCubeMove : MonoBehaviour
    {
		static int num = 0;
		private Stopwatch stopwatch = new Stopwatch();
        void OnEnable()
        {
            CubeMoveHook.CubeMoved += OnCubeMoved;
			stopwatch.Start();
        }

        void OnDisable()
        {
            CubeMoveHook.CubeMoved -= OnCubeMoved;
			stopwatch.Stop();
        }

        void OnCubeMoved(Transform cube)
        {
	    string[] header = {"id","game","date","group_name","trainer","max_steps","learning_rate", "beta", "epsilon","lambda", "num_epoch","gamma","strength","batch_size","buffer_size","hidden_units","num_layers","beta","cpu_cores","total_processor_time","ram_gb","working_set_b","private_memory_b","gpu_model","time_sec","peak_ram_b","final_reward","avg_ram_mb","steps_to_90pct","food_eaten","poison_eaten","final_size","block_goal_reached","worm_length", "operating_system", "neural_network_type", "currtime"};

	    Dictionary<string,string> entry = new Dictionary<string,string>();

		entry.Add("id",num.ToString());

	    entry.Add("cpu_cores", SystemInfo.processorCount.ToString());
	    entry.Add("total_processor_time", System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime.ToString());
	    entry.Add("ram_gb", SystemInfo.systemMemorySize.ToString());
	    entry.Add("working_set_mb", GetProcessMemoryMB(false).ToString()); //this is the total memory the whole unity project is taking up.
	    entry.Add("private_memory_mb", GetProcessMemoryMB(true).ToString()); //this is the memory that the memory that belongs to the process.
	    entry.Add("gpu_model", SystemInfo.graphicsDeviceName);
	    entry.Add("operating_system", SystemInfo.operatingSystem);

	    //change the next few values manually. The alternative is to read the value directly from the yaml file which is a pain. To do it conveniently you'd need a library called YamlDotNet.
            //the default values are that of the worm. Find your agents corresponding config file in ml-agents/config.
	    // neural network settings
	    entry.Add("hidden_units", "512");
	    entry.Add("num_layers", "3");
	    entry.Add("neural_network_type", "ppo");
	    // hyperparameters
	    entry.Add("batch_size", "2024");
	    entry.Add("buffer_size", "20240");
	    entry.Add("learning_rate", "0.0003");
	    entry.Add("beta", "0.005");
	    entry.Add("epsilon", "0.2");
	    entry.Add("lambda", "0.95");
	    entry.Add("num_epoch", "3");
	    // reward signals
	    entry.Add("gamma", "0.995");
	    entry.Add("strength", "1.0");
	    entry.Add("max_steps", "7000000");
		entry.Add("currtime", stopwatch.Elapsed.ToString());

        var csvPath = System.IO.Path.Combine(Application.persistentDataPath, "data.csv");
        using (var writer = new System.IO.StreamWriter(csvPath, true))
        {
            for (int i = 0; i < header.Length; i++)
            {
                if (i > 0)
                {
                    writer.Write(",");
                }

                string value;
                entry.TryGetValue(header[i], out value);
                value = value ?? string.Empty;

                if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                {
                    value = "\"" + value.Replace("\"", "\"\"") + "\"";
                }

                writer.Write(value);
            }

            writer.WriteLine();
        }
		}

        static long GetProcessMemoryMB(bool privateMemory)
        {
            try
            {
                using (var process = System.Diagnostics.Process.GetCurrentProcess())
                {
                    var bytes = privateMemory ? process.PrivateMemorySize64 : process.WorkingSet64;
                    return bytes;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
