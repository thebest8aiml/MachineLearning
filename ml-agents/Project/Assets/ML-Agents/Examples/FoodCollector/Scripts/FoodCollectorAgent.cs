using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;
using Stopwatch = System.Diagnostics.Stopwatch;
public class FoodCollectorAgent : Agent
{
    FoodCollectorSettings m_FoodCollecterSettings;
    public GameObject area;
    FoodCollectorArea m_MyArea;
    bool m_Frozen;
    bool m_Poisoned;
    bool m_Satiated;
    bool m_Shoot;
    float m_FrozenTime;
    float m_EffectTime;
    Rigidbody m_AgentRb;
    float m_LaserLength;
    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 2;
    public Material normalMaterial;
    public Material badMaterial;
    public Material goodMaterial;
    public Material frozenMaterial;
    public GameObject myLaser;
    public bool contribute;
    public bool useVectorObs;

    // Agent life
    public float baseMaxLife = 100, maxLife = 100, life;
    public bool dead = false;
    public GameObject model;
    public BoxCollider boxCollider;
    public float foodScaling = 1.1f;
    public float poisonScaling = 0.75f;
    public GameObject gridSensor;
    [Tooltip("Use only the frozen flag in vector observations. If \"Use Vector Obs\" " +
             "is checked, this option has no effect. This option is necessary for the " +
             "VisualFoodCollector scene.")]
    public bool useVectorFrozenFlag;
    // data
    public int agentID;
    private Stopwatch stopwatch = new Stopwatch();
    public int foodCount = 0, poisonCount = 0;
    public static int num = -21;
    float m_LastEpisodeReward;
    float m_EpisodeStartTime = 0;
    EnvironmentParameters m_ResetParams;

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if(!dead)
        {
            life -= 0.05f;
            //AddReward(0.01f);
            if (life <= 0) Die();
            MoveAgent(actionBuffers);
        }
        m_LastEpisodeReward = GetCumulativeReward();
    }
    public void Die()
    {
        gameObject.GetComponentInChildren<Renderer>().material = badMaterial;
        dead = true;
        AddReward(-10f);
        m_MyArea.CheckAllDead();
    }
    public void Start()
    {
        life = maxLife;
        boxCollider = GetComponent<BoxCollider>();
    }
    public override void Initialize()
    {
        stopwatch.Start();
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<FoodCollectorArea>();
        m_FoodCollecterSettings = FindObjectOfType<FoodCollectorSettings>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
            sensor.AddObservation(model.transform.localScale.x);
            sensor.AddObservation(life);
            sensor.AddObservation(localVelocity.x);
            sensor.AddObservation(localVelocity.z);
            sensor.AddObservation(m_Frozen);
            sensor.AddObservation(m_Shoot);
        }
        else if (useVectorFrozenFlag)
        {
            sensor.AddObservation(m_Frozen);
        }
    }

    public Color32 ToColor(int hexVal)
    {
        var r = (byte)((hexVal >> 16) & 0xFF);
        var g = (byte)((hexVal >> 8) & 0xFF);
        var b = (byte)(hexVal & 0xFF);
        return new Color32(r, g, b, 255);
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {
        m_Shoot = false;

        if (Time.time > m_FrozenTime + 4f && m_Frozen)
        {
            Unfreeze();
        }
        if (Time.time > m_EffectTime + 0.5f)
        {
            if (m_Poisoned)
            {
                Unpoison();
            }
            if (m_Satiated)
            {
                Unsatiate();
            }
        }

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;

        if (!m_Frozen)
        {
            var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
            var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
            var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

            dirToGo = transform.forward * forward;
            dirToGo += transform.right * right;
            rotateDir = -transform.up * rotate;

            var shootCommand = discreteActions[0] > 0;
            if (shootCommand)
            {
                m_Shoot = true;
                dirToGo *= 0.5f;
                m_AgentRb.velocity *= 0.75f;
            }
            m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
            transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        }

        if (m_AgentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            m_AgentRb.velocity *= 0.95f;
        }

        if (m_Shoot)
        {
            var myTransform = transform;
            myLaser.transform.localScale = new Vector3(1f, 1f, m_LaserLength);
            var rayDir = 25.0f * myTransform.forward;
            Debug.DrawRay(myTransform.position, rayDir, Color.red, 0f, true);
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 2f, rayDir, out hit, 25f))
            {
                if (hit.collider.gameObject.CompareTag("agent"))
                {
                    hit.collider.gameObject.GetComponent<FoodCollectorAgent>().Freeze();
                }
            }
        }
        else
        {
            myLaser.transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }

    void Freeze()
    {
        gameObject.tag = "frozenAgent";
        m_Frozen = true;
        m_FrozenTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = frozenMaterial;
    }

    void Unfreeze()
    {
        m_Frozen = false;
        gameObject.tag = "agent";
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    void Poison()
    {
        life -= 20;
        m_Poisoned = true;
        m_EffectTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = badMaterial;
        model.transform.localScale *= poisonScaling;
        maxLife = baseMaxLife * model.transform.localScale.x;
        gridSensor.transform.localPosition += boxCollider.size.x * (1 - poisonScaling) / 2 * new Vector3(0, 1, 0);
        boxCollider.size *= poisonScaling;
    }

    void Unpoison()
    {
        m_Poisoned = false;
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    void Satiate()
    {
        life = Mathf.Min(maxLife, life + 10);
        m_Satiated = true;
        m_EffectTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = goodMaterial;
        model.transform.localScale *= foodScaling;
        maxLife = baseMaxLife * model.transform.localScale.x;
        gridSensor.transform.localPosition -= boxCollider.size.x * (foodScaling - 1f) / 2 * new Vector3(0, 1, 0);
        boxCollider.size *= foodScaling;
    }

    void Unsatiate()
    {
        m_Satiated = false;
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[2] = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public override void OnEpisodeBegin()
    {
        num += 1;
        Debug.Log("yo " + num);
        if (num >= 0)
            WriteDataToCSV();

        Unfreeze();
        Unpoison();
        Unsatiate();
        life = maxLife;
        dead = false;
        m_Shoot = false;
        m_AgentRb.velocity = Vector3.zero;
        model.transform.localScale = Vector3.one;
        gridSensor.transform.localPosition = Vector3.zero;
        boxCollider.size = Vector3.one;
        myLaser.transform.localScale = new Vector3(0f, 0f, 0f);
        transform.position = new Vector3(Random.Range(-m_MyArea.range, m_MyArea.range),
            2f, Random.Range(-m_MyArea.range, m_MyArea.range))
            + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        foodCount = 0;
        poisonCount = 0;

        m_EpisodeStartTime = Time.time;

        SetResetParameters();
    }

    static readonly string[] Header = {
    "id", "game", "date", "agentID", "final_reward", "trainer", "max_steps", "learning_rate",
    "beta", "epsilon", "lambda", "num_epoch", "gamma", "strength", "batch_size", "buffer_size",
    "hidden_units", "num_layers", "cpu_cores", "total_processor_time", "ram_mb", "working_set_b",
    "private_memory_b", "gpu_model", "time_sec", "peak_ram_b", "avg_ram_mb", "steps_to_90pct",
    "food_eaten", "poison_eaten", "final_size", "operating_system",
    "neural_network_type", "currtime"
};
    void WriteDataToCSV()
    {
        Dictionary<string, string> entry = new Dictionary<string, string>();

        entry.Add("id", num.ToString());
        entry.Add("game", "FoodCollector");
        entry.Add("date", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        entry.Add("agentID", agentID.ToString());
        entry.Add("final_reward", m_LastEpisodeReward.ToString("F4", System.Globalization.CultureInfo.InvariantCulture));
        entry.Add("food_eaten", foodCount.ToString());
        entry.Add("poison_eaten", poisonCount.ToString());
        entry.Add("final_size", model.transform.localScale.x.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));

        entry.Add("cpu_cores", SystemInfo.processorCount.ToString());
        entry.Add("total_processor_time", System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime.ToString());
        entry.Add("ram_mb", SystemInfo.systemMemorySize.ToString());
        entry.Add("working_set_b", GetProcessMemoryMB(false).ToString()); //this is the total memory the whole unity project is taking up.
        entry.Add("private_memory_b", GetProcessMemoryMB(true).ToString()); //this is the memory that the memory that belongs to the process.
        entry.Add("gpu_model", SystemInfo.graphicsDeviceName);
        entry.Add("operating_system", SystemInfo.operatingSystem);
        entry.Add("time_sec", (Time.time - m_EpisodeStartTime).ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
        //change the next few values manually. The alternative is to read the value directly from the yaml file which is a pain. To do it conveniently you'd need a library called YamlDotNet.
        //the default values are that of the worm. Find your agents corresponding config file in ml-agents/config.


        // neural network settings
        entry.Add("hidden_units", "256");
        entry.Add("num_layers", "1");
        entry.Add("trainer", "ppo");                
        entry.Add("neural_network_type", "simple");
        // hyperparameters
        entry.Add("batch_size", "1024");
        entry.Add("buffer_size", "10240");
        entry.Add("learning_rate", "0.0003");
        entry.Add("beta", "0.005");
        entry.Add("epsilon", "0.2");
        entry.Add("lambda", "0.95");
        entry.Add("num_epoch", "3");
        // reward signals
        entry.Add("gamma", "0.99");
        entry.Add("strength", "1.0");
        entry.Add("max_steps", "2000000");
        entry.Add("currtime", stopwatch.Elapsed.ToString());

        var csvPath = System.IO.Path.Combine(Application.persistentDataPath, "FoodCollectorData.csv");
        bool newFile = !System.IO.File.Exists(csvPath);

        using (var writer = new System.IO.StreamWriter(csvPath, true))
        {
            if (newFile)
                writer.WriteLine(string.Join(",", Header));

            for (int i = 0; i < Header.Length; i++)
            {
                if (i > 0) writer.Write(",");

                string value;
                entry.TryGetValue(Header[i], out value);
                value = value ?? string.Empty;

                if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                    value = "\"" + value.Replace("\"", "\"\"") + "\"";

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("food"))
        {
            Satiate();
            collision.gameObject.GetComponent<FoodLogic>().OnEaten();
            foodCount += 1;
            AddReward(2f);
            if (contribute)
            {
                m_FoodCollecterSettings.totalScore += 1;
            }
        }
        if (collision.gameObject.CompareTag("badFood"))
        {
            Poison();
            collision.gameObject.GetComponent<FoodLogic>().OnEaten();
            poisonCount += 1;
            AddReward(-1f);
            if (contribute)
            {
                m_FoodCollecterSettings.totalScore -= 1;
            }
        }
    }

    public void SetLaserLengths()
    {
        m_LaserLength = m_ResetParams.GetWithDefault("laser_length", 1.0f);
    }

    public void SetAgentScale()
    {
        float agentScale = m_ResetParams.GetWithDefault("agent_scale", 1.0f);
        gameObject.transform.localScale = new Vector3(agentScale, agentScale, agentScale);
    }

    public void SetResetParameters()
    {
        SetLaserLengths();
        SetAgentScale();
    }
}
