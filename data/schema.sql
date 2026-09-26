CREATE TABLE runs (
    -- Identifiers
    run_id TEXT PRIMARY KEY,
    game TEXT NOT NULL,
    date TEXT,
    group_name TEXT,

    -- Hyperparameters
    max_steps INTEGER,
    learning_rate REAL,
    batch_size INTEGER,
    buffer_size INTEGER,
    hidden_units INTEGER,
    num_layers INTEGER,
    beta REAL,

    -- Hardware
    cpu_cores INTEGER,
    ram_gb REAL,
    gpu_model TEXT,

    -- Outcomes
    time_sec REAL,
    peak_ram_mb REAL,
    final_reward REAL,
    steps_to_90pct INTEGER
);