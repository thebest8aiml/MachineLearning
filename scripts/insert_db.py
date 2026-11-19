import sqlite3


COLUMNS = [
    "run_id",
    "game",
    "date",
    "group_name",
    "trainer",
    "max_steps",
    "learning_rate",
    "batch_size",
    "buffer_size",
    "hidden_units",
    "num_layers",
    "beta",
    "cpu_cores",
    "ram_gb",
    "gpu_model",
    "time_sec",
    "peak_ram_mb",
    "final_reward",
    "avg_ram_mb",
    "steps_to_90pct",
    "food_eaten",
    "poison_eaten",
    "final_size",
    "block_goal_reached",
    "worm_length",
]


def _normalize_value(val):
    if val is None:
        return None
    if isinstance(val, str) and val.strip() == "":
        return None
    return val


def insert_run(data:dict, db_path: str = "data/runs.db"):
    clean_data = {k.strip(): v for k, v in data.items()}

    placeholders = ", ".join(["?"] * len(COLUMNS)) # question mark placeholders
    col_sql = ", ".join(COLUMNS) # column list
    values = [_normalize_value(clean_data.get(col)) for col in COLUMNS] # final

    sql = f"INSERT OR REPLACE INTO runs ({col_sql}) VALUES ({placeholders})"

    conn = sqlite3.connect(db_path)
    try:
        with conn:
            conn.execute(sql, values)
    finally:
        conn.close()





