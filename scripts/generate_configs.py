import csv
import os

GAMES = {"pushblockcollab": "data/experiments/configs/pushblockCollab/PushBlockCollab_template.yaml",
         "foodcollector": "data/experiments/configs/foodCollector/foodCollector_template.yaml",
         "worm": "data/experiments/configs/worm/worm_template.yaml"}

OUTPUT_DIRS = {"pushblock": "data/experiments/configs/pushblockCollab/PushBlockCollab_generated",
               "foodcollector": "data/experiments/configs/foodCollector/foodCollector_generated",
               "worm": "data/experiments/configs/worm/worm_generated"}

CSV_FILE = "data/experiments/experiments.csv"


with open(CSV_FILE) as f:
    reader = csv.DictReader(f)
    for row in reader:
        game = row["game"]
        template_path = GAMES[game]
        output_dir = OUTPUT_DIRS[game]

        with open(template_path) as f:
            template = f.read()

        config = template
        config = config.replace("PLACEHOLDER_LR", row["learning_rate"])
        config = config.replace("PLACEHOLDER_HU", row["hidden_unites"])
        config = config.replace("PLACEHOLDER_STEPS", row["max_steps"])

        os.makedirs(output_dir, exist_ok=True)
        output_path = os.path.join(output_dir, f"{row['run_id']}.yaml")

        with open(output_path, "w") as output:
            output.write(config)

        print(f"Created {output_path}")