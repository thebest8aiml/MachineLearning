'''
Helper for csv to have one row per run, not episode.

The Unity recorder writes one row every time an agent finishes an episode, so a
run is thousands of rows. This script finds where each run starts and
summarises it in a single row.

A new run starts when:
  - a header line appears in the middle of the file (Unity writes the header
    every time you press Play), or
  - `currtime` (the stopwatch) jumps back, because it restarts at each Play.

Usage:
    'python scripts/clean_csv.py ml_algorithms/FoodCollectorData.csv'
    
    python clean_csv.py data.csv                 -> writes data_clean.csv
    python clean_csv.py data.csv out.csv         -> writes out.csv
    python clean_csv.py data.csv out.csv run1    -> also adds a run_id column
                                                    (only if the file is ONE run)
'''

import sys

import pandas as pd


LAST_FRACTION = 0.1
DEFAULT_PREFIX = "unlinked_"


CONSTANT = [
    "game", "trainer", "max_steps", "learning_rate", "beta", "epsilon", "lambda",
    "num_epoch", "gamma", "strength", "batch_size", "buffer_size", "hidden_units",
    "num_layers", "cpu_cores", "ram_mb", "gpu_model", "operating_system",
    "neural_network_type",
]

MAXIMUM = ["total_processor_time", "working_set_b", "private_memory_b", "peak_ram_b"]

TIMESPAN = ["total_processor_time", "currtime"]

PER_EPISODE = ["final_reward", "food_eaten", "poison_eaten", "final_size"]

TEXT = {"game", "trainer", "gpu_model", "operating_system", "neural_network_type",
        "date", "run_id"}


def timespan_to_seconds(text):
    if pd.isna(text) or not str(text).strip():
        return float("nan")
    text = str(text).strip()

    try:
        days = 0
        head, _, _ = text.partition(":")

        if "." in head:  # "1.02:03:04" -> days before the first dot
            day_part, text = text.split(".", 1)
            days = int(day_part)
        hours, minutes, seconds = text.split(":")

        return days * 86400 + int(hours) * 3600 + int(minutes) * 60 + float(seconds)
    
    except ValueError:
        return float("nan")


def load(path):
    df = pd.read_csv(path, dtype=str)

    # Header lines in the middle of the file mark a new Play. Count them, then drop them.
    is_header = df["id"] == "id"
    df["_header_count"] = is_header.cumsum()
    df = df[~is_header].copy()

    for col in TIMESPAN:
        if col in df:
            df[col] = df[col].map(timespan_to_seconds)
            
    for col in df.columns:
        if col not in TEXT and col != "_header_count" and col not in TIMESPAN:
            df[col] = pd.to_numeric(df[col], errors="coerce")

    if "run_id" in df.columns and df["run_id"].notna().any():
        # Newer data: the CSV says which run each row belongs to.
        df["session"] = pd.factorize(df["run_id"])[0]  # numbered in order of appearance

    else:
        # Older data: find where each run starts.
        header_appeared = df["_header_count"].diff().fillna(0) != 0
        clock_went_back = df["currtime"].diff() < -1
        df["session"] = (header_appeared | clock_went_back).cumsum()
    return df.drop(columns="_header_count")


def summarise(group):

    row = {}
    if "run_id" in group and group["run_id"].notna().any():
        row["run_id"] = group["run_id"].iloc[0]

    row["n_rows"] = len(group)
    if "agentID" in group:
        row["n_agents"] = group["agentID"].nunique()

    row["date_first"] = group["date"].iloc[0]
    row["date_last"] = group["date"].iloc[-1]

    for col in CONSTANT:
        if col in group:
            row[col] = group[col].iloc[0]

    for col in MAXIMUM:
        if col in group:
            row[col] = group[col].max()

    if "avg_ram_mb" in group:
        row["avg_ram_mb"] = group["avg_ram_mb"].mean()


    if "currtime" in group:
        row["training_time_sec"] = group["currtime"].max()

    if "time_sec" in group:
        row["time_sec_max"] = group["time_sec"].max()  

    n_last = max(1, int(len(group) * LAST_FRACTION))

    for col in PER_EPISODE:
        if col in group:
            row[f"{col}_mean"] = group[col].mean()
            row[f"{col}_last_mean"] = group[col].tail(n_last).mean()

    if "final_reward" in group:
        row["final_reward_max"] = group["final_reward"].max()

    return row


def clean(df, prefix=DEFAULT_PREFIX):
    result = pd.DataFrame([summarise(g) for _, g in df.groupby("session", sort=True)])

    if "run_id" not in result.columns:
        result.insert(0, "run_id", [f"{prefix}{i + 1}" for i in range(len(result))])

    return result


if __name__ == "__main__":
    
    if len(sys.argv) < 2:
        print(__doc__)
        sys.exit(1)

    src = sys.argv[1]
    dst = sys.argv[2] if len(sys.argv) > 2 else src.rsplit(".", 1)[0] + "_clean.csv"
    prefix = sys.argv[3] if len(sys.argv) > 3 else DEFAULT_PREFIX

    raw = load(src)
    result = clean(raw, prefix)

    result.to_csv(dst, index=False)
    print(f"Read {len(raw)} rows, found {len(result)} run(s) -> {dst}")
    print("Run ids:      ", result["run_id"].tolist())
    print("Rows per run: ", result["n_rows"].tolist())
    if "training_time_sec" in result:
        print("Training time (s):", [round(t) for t in result["training_time_sec"]])