# Data dictionary (FoodCollector)

This file explains the columns in our FoodCollector data files.

## 1. Unity CSV (`FoodCollectorData.csv`)

One row gets added every time an agent finishes an episode.

Since there are 2 versions of this file, I differentiated between old version and new version

| Column | Meaning |
|---|---|
| `area_id` | Which area the agent is in (new version only) |
| `agent_id` | Agent number, 0 to 4 (new version only)|
| `id` | Row counter (old version only) |
| `agentID` | Agent number. Same as `agent_id` |
| `game` | We only got one game and you already know it's the one and only `FoodCollector` |
| `date` | When the row was written (`yyyy-MM-dd HH:mm:ss`) |
| `final_reward` | Reward the agent got in that episode |
| `food_eaten` | Food eaten in the episode |
| `poison_eaten` | Poison eaten in the episode |
| `final_size` | Size of the agent at the end of the episode |
| `time_sec` | How long the episode took in game seconds. NOT REAL TIME AND NOT TRAINING TIME |
| `currtime` | Stopwatch time as `hh:mm:ss`. Starts again at 0 every time you press Play |
| `total_processor_time` | CPU time Unity has used so far, as `hh:mm:ss` |
| `cpu_cores` | Number of logical CPU cores of the computer |
| `ram_mb` | RAM of the computer in MB |
| `gpu_model` | Name of the GPU. `Null Device` means there was no graphics |
| `trainer` | Training algorithm. Value in the code: ppo |
| `max_steps` | Number of training steps. Value in the code: 2000000 |
| `learning_rate` | Learning rate. Value in the code: 0.0003 |
| `batch_size` | Batch size. Value in the code: 1024 |
| `buffer_size` | Buffer size. Value in the code: 1024. |
| `hidden_units` | Hidden units per layer. Value in the code: 256 |
| `num_layers` | Number of hidden layers. Value in the code: 1 |
| `beta` | Entropy strength (PPO). Value in the code: 0.005 |
| `epsilon` | PPO clipping. Value in the code: 0.2 |
| `lambda` | PPO lambda. Value in the code: 0.95 |
| `num_epoch` | PPO epochs. Value in the code: 3 |
| `gamma` | Discount factor. Value in the code: 0.99 |
| `strength` | Strength of the reward signal. Value in the code: 1.0 |
| `neural_network_type` | Network type. Value in the code: simple |
| `working_set_b`, `private_memory_b` | Memory used by Unity in bytes. These are always 0 so they don't work!! |
| `peak_ram_b`, `avg_ram_mb`, `steps_to_90pct` | In the header but never filled in. These are always empty |

## 2. Cleaned CSV (`FoodCollector_clean.csv`)

 One row per session (one press of Play). It has the same settings columns as above (it takes the first value of the session). The new columns are:

| Column | Meaning |
|---|---|
| `n_rows` | Number of rows (episodes) in the session |
| `n_agents` | Number of different agents in the session |
| `date_first`, `date_last` | `date` of the first and last row of the session |
| `final_reward_mean`, `food_eaten_mean`, `poison_eaten_mean`, `final_size_mean` | Average over the whole session |
| `final_reward_last_mean`, `food_eaten_last_mean`, `poison_eaten_last_mean`, `final_size_last_mean` | Average over the last 10 % of the rows of the session |
| `final_reward_max` | Best reward in the session |

Some columns changed compared to the Unity CSV:
`time_sec` is the longest episode of the session. It is NOT the training time!!
`currtime` and `total_processor_time` are the biggest value of the session (seconds)

## 3. Sample file (`data/food_collector_data.csv`)

Small sample file (20 rows, 2 runs)

| Column | Meaning |
|---|---|
| `run_id` | We only got 2 runs in this sample file so its either run data 1 or 2 |
| `episode_id` | Episode number inside the run (0 to 9) |
| `area_id`, `agent_id` | Area and agent number |
| `start_step` | Training step where the episode started |
| `time_sec` | Longest episode of the session (I am not really sure) |
| `seed` | Random seed (11 or 12) |
| `num_envs` | Number of environments (1 or 2) |
| `time_scale` | Unity time scale (1 to 10) |
| `num_areas`, `agents_per_area` | Number of areas (2) and agents per area (5) |
| `food_per_area`, `poison_per_area` | Food (20 or 25) and poison (10 or 12) items per area |
| `num_obs` | Number of observations (5 or 6) |
| `batch_size`, `buffer_size`, `learning_rate`, `max_steps` | Hyperparameters of the run |
| `spawn_x`, `spawn_z` | Spawn point of the agent |
| `spawn_dist_nearest_food` | Distance from the start to the nearest food |
| `food_within_radius` | Food items close to the start (we don't know the radius) |
| `reward` | Reward of the episod. |
| `episode_length` | Always 1000 |
| `died` | 1 if the agent died, otherwise 0 |
| `food_eaten`, `poison_eaten` | Food and poison eaten |
| `final_size` | Size of the agent at the end |
| `times_frozen` | How many times the agent got frozen |
| `shots_fired`, `hits_landed` | Laser shots and how many of them hit |
| `ram_python_mb`, `ram_unity_mb_total` | Memory of the Python trainer and of Unity in MB |
| `cpu_pct_machine` | CPU usage of the computer in % |
| `cpu_model`, `physical_cores`, `total_ram_gb` | CPU name, number of physical cores, RAM in GB |


## 4. `run_info.json`

It is one file per training run, saved in `results/<run_id>/`.

| Field | Meaning |
|---|---|
| `run_id` | Run number . |
| `time_sec` | This is the training time in very real seconds |
| `peak_ram_mb` | Highest memory use of `mlagents-learn` and its child processes (checked every 2 seconds). Unity is not included |
| `avg_ram_mb` | Average of those memory checks |
| `exit_code` | 0 means the run finished just fine |

## 5. `experiments.csv` and the YAML files
This is an experimental table and `scripts/generate_configs.py` turns it into the YAML files.
Columns: `run_id` (1 to 27), `game`, `max_steps`, `learning_rate`, `hidden_units`. 
- `max_steps` is 100000, 500000 or 1000000
- `learning_rate` is 0.0001, 0.0003 or 0.001
- `hidden_units` is always 128

## 6. Database (`data/runs.db`, table `runs`)

SQLite database. `scripts/insert_db.py` adds rows to it.

| Column | Meaning |
|---|---|
| `id` | Automatic row number. |
| `run_id`, `game`, `date`, `group_name` | Which run it is, the game, the date and our group |
| `trainer`, `max_steps`, `learning_rate`, `batch_size`, `buffer_size`, `hidden_units`, `num_layers`, `beta` | Hyperparameters |
| `cpu_cores` | Logical CPU cores |
| `ram_gb` | RAM of the computer in GB (the Unity CSV uses MB) |
| `gpu_model` | Name of the GPU |
| `time_sec` | Training time in seconds (same as `time_sec` in `run_info.json`) |
| `peak_ram_mb`, `avg_ram_mb` | Same as in `run_info.json` |
| `final_reward`, `steps_to_90pct`, `food_eaten`, `poison_eaten`, `final_size` | Results of the run |
| `block_goal_reached`, `worm_length` | For the other games. Always empty for FoodCollector. |

## Some stuff I noticed 

1. `time_sec` means something different in each file 
2. The memory columns from Unity are always 0
3. The Unity CSV has no `run_id`

