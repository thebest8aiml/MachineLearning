using System;
using UnityEngine;

namespace Unity.MLAgentsExamples
{

    public class CubeMoveHook : MonoBehaviour
    {
		void Start() {
			var csvPath = System.IO.Path.Combine(Application.persistentDataPath, "data.csv");
			var writer = new System.IO.StreamWriter(csvPath, true);
			writer.WriteLine("id,game,date,group_name,trainer,max_steps,learning_rate,beta,epsilon,lambda,num_epoch,gamma,strength,batch_size,buffer_size,hidden_units,num_layers,beta,cpu_cores,total_processor_time,ram_gb,working_set_b,private_memory_b,gpu_model,time_sec,peak_ram_b,final_reward,avg_ram_mb,steps_to_90pct,food_eaten,poison_eaten,final_size,block_goal_reached,worm_length,operating_system,neural_network_type,currtime");
            writer.Close();
			Debug.Log($"Saving to: {Application.persistentDataPath}");
		}
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
