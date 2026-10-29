import sqlite3

def insert_run(data:dict):
    conn = sqlite3.connect("data/runs.db")
    cursor = conn.cursor
    conn.execute ('''
        INSERT INTO runs (
            run_id, game, date, group_name,
            trainer, max_steps, learning_rate, batch_size, buffer_size, hidden_units, num_layers, beta,
            cpu_cores, ram_gb, gpu_model,
            time_sec, peak_ram_mb, final_reward, avg_ram_mb,
            steps_to_90pct, food_eaten, poison_eaten, final_size, block_goal_reached, worm_length             
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    
    ''', (
        data["run_id"], data["game"], data["date"], data["group_name"],
        data["trainer"], data["max_steps"], data["learning_rate"], data["batch_size"], data["buffer_size"],
        data["hidden_units"], data["num_layers"], data["beta"],
        data["cpu_cores"], data["ram_gb"], data["gpu_model"],
        data["time_sec"], data["peak_ram_mb"], data["avg_ram_mb"], data["final_reward"],
        data["steps_to_90pct"], data["food_eaten"], data["poison_eaten"],
        data["final_size"], data["block_goal_reached"], data["worm_length"]
    )) 

    conn.commit()
    conn.close()