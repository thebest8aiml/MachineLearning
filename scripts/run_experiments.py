'''
Runs every YAML one after another

A run is finished when results/<run_id>/run_info.json exists with exit code 0. Finished runs are skipped next time and crashed/stopped ones are redone with --force to overwrite the previous ones.

For each run it saves results/<run_id>/run_info.json with the training time and the RAM usage.

-------------------------

How to use:

1. Open a terminal, activate venv
2. Open the game in unity, DO NOT START YET
3. Start the script: python scripts/run_experiments.py
4. For each run:
    - Terminal asks you for enter, press it
    - Trainer starts and prints it is waiting for unity
    - Press Play in Unity
    - Wait. When trainer prints that training is finished, stop the game. Script then asks you about next run.
5. Results: results/<run_id>/run_info.json

'''



import glob
import json
import os
import subprocess
import time
import psutil
import threading
import sys
import re

CONFIG_DIR = "data/experiments/configs/foodCollector/foodCollector_generated"
RESULTS_DIR = "results"

ENV_PATH = None
TIMEOUT_WAIT = 300

def process_tree_rss(process):
    total = 0

    try:
        parent = psutil.Process(process.pid)
        for p in [parent] + parent.children(recursive=True):
            try:
                total += p.memory_info().rss
            except psutil.Error:
                pass
    except psutil.Error:
        pass

    return total

def sort_yaml(path):
    name = os.path.basename(path)
    return [int(part) if part.isdigit() else part.lower() for part in re.split(r"(\d+)", name)]


def is_finished(run_dir):
    info_path = os.path.join(run_dir, "run_info.json")
    if not os.path.exists(info_path):
        return False

    try: 
        with open(info_path) as f:
            return json.load(f).get("exit_code") == 0
    except (OSError, ValueError):
        return False


def run_one(yaml_path):
    run_id = os.path.splitext(os.path.basename(yaml_path))[0]
    run_dir = os.path.join(RESULTS_DIR, run_id)

    if is_finished(run_dir):
        print(f"Skipping {run_id} (already finished)")
        return

    cmd = [sys.executable, "-m", "mlagents.trainers.learn", yaml_path, f"--run-id={run_id}", f"--timeout-wait={TIMEOUT_WAIT}"]
    if os.path.exists(run_dir):
        cmd.append("--force")

    if ENV_PATH:
        cmd += [f"--env={ENV_PATH}", "--no-graphics"]

    if not ENV_PATH:
        input(f"\n[{run_id}] Make sure Unity is NOT in play mode, then press Enter to start. " f"When you see 'Listening on port', press Play \n")


    print(f"Starting {run_id}: {' '.join(cmd)}")
    launch = time.time()
    process = subprocess.Popen(cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, bufsize=1) # seperate program to not freeze
    connected_at = []

    def read_output():
        for line in process.stdout:
            print(line, end="")
            if not connected_at and "Connected to Unity environment" in line:
                connected_at.append(time.time())

    reader = threading.Thread(target=read_output, daemon=True)
    reader.start()

    peak_bytes = 0
    samples = []

    while process.poll() is None:
        if psutil:
            rss = process_tree_rss(process)
            peak_bytes = max(peak_bytes, rss)
            samples.append(rss)
        time.sleep(2)


    reader.join(timeout=5)
    start = connected_at[0] if connected_at else launch

    elapsed = time.time() - start

    info = { #to mb
        "run_id":run_id,
        "time_sec": round(elapsed, 1),
        "peak_ram_mb": round(peak_bytes / 1024 / 1024, 1) if psutil else None,
        "avg_ram_mb": round(sum(samples) / len(samples) / 1024 / 1024, 1) if samples else None,
        "exit_code": process.returncode,
    }

    os.makedirs(run_dir, exist_ok=True)
    with open(os.path.join(run_dir, "run_info.json"), "w") as f:
        json.dump(info, f, indent=2)

    if process.returncode == 0:
        print(f"Finished {run_id} in {elapsed / 60:.1f} min")
    else:
        print(f"Warning: {run_id} FAILED or was stopped (exit code {process.returncode}). " f"It will be redone next time you start the script.")
    


if __name__ == "__main__":
    if psutil is None:
        print("psutil is not installed: peak RAM will not be recorded")

    yamls = sorted(glob.glob(os.path.join(CONFIG_DIR, "*.yaml")), key=sort_yaml)
    print(f"Found {len(yamls)} configs")
    for path in yamls:
        run_one(path)

    #yaml_path = "data/experiments/configs/foodCollector_generated/test.yaml"
    #run_one(yaml_path)