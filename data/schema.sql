CREATE TABLE runs (
    -- Identifiers
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    run_id TEXT NOT NULL,
    game TEXT NOT NULL,
    date TEXT,
    group_name TEXT,

    -- Hyperparameters
    trainer TEXT,
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
    avg_ram_mb REAL,

    -- Specific for games
    steps_to_90pct INTEGER, 
    food_eaten INTEGER,         --food collector
    poison_eaten INTEGER,       --food collector
    final_size REAL,            --food collector
    block_goal_reached INTEGER, --Push block
    worm_length REAL,            -- Worm

    UNIQUE(run_id, group_name)
);