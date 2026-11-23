import sqlite3
import os

class Application:
    CONN: sqlite3.Connection
    CURSOR:  sqlite3.Cursor
    CSVFILEPATH = './FoodCollectorData.csv'
    def __init__(self, name: str) -> None:
        self.name = name
        self.CONN = sqlite3.connect('data.db')
        self.CURSOR = self.CONN.cursor()


    def run(self) -> int:
        print(f"Running {self.name}")
        # TODO: add your logic here
        # self.createtable()
        os.system("bash databasesetup.bash")
        self.outputcontents()
        self.flushdb()

        print(f"Finished running {self.name}")
        return 0

    def outputcontents(self):
        data = self.CURSOR.execute('''
            SELECT * FROM data;
        ''')

        for row in data:
            print(row) 

    def flushdb(self):
        self.CURSOR.execute('''
            DROP TABLE IF EXISTS data;
        ''')

    def insertdata(self):
        print('bruh')

    def createtable(self): # databasesetup.bash handles creation. Just use that.
        '''
        "area_id", "agent_id", "game", "date", "agentID", "final_reward", "trainer", "max_steps", "learning_rate",
        "beta", "epsilon", "lambda", "num_epoch", "gamma", "strength", "batch_size", "buffer_size",
        "hidden_units", "num_layers", "cpu_cores", "total_processor_time", "ram_mb", "working_set_b",
        "private_memory_b", "gpu_model", "time_sec", "peak_ram_b", "avg_ram_mb", "steps_to_90pct",
        "food_eaten", "poison_eaten", "final_size", "operating_system",
        "neural_network_type", "currtime"
        '''
        self.CURSOR.execute('''
            CREATE TABLE IF NOT EXISTS data(
                id INTEGER,
                game TEXT,
                date TEXT,
                agentID INTEGER,
                final_reward REAL,
                trainer TEXT,
                max_steps INTEGER,
                learning_rate REAL,
                beta REAL,
                epsilon REAL,
                lambda REAL,
                num_epoch INTEGER,
                gamma REAL,
                strength REAL,
                batch_size INTEGER,
                buffer_size INTEGER,
                hidden_units INTEGER,
                num_layers INTEGER,
                cpu_cores INTEGER,
                total_processor_time REAL,
                ram_mb REAL,
                working_set_b INTEGER,
                private_memory_b INTEGER,
                gpu_model TEXT,
                time_sec REAL,
                peak_ram_b INTEGER,
                avg_ram_mb REAL,
                steps_to_90pct INTEGER,
                food_eaten INTEGER,
                poison_eaten INTEGER,
                final_size INTEGER,
                operating_system TEXT,
                neural_network_type TEXT,
                currtime TEXT
            );''')


def main() -> int:
    app = Application(name="ml_algorithms")
    return app.run()


if __name__ == "__main__":
    raise SystemExit(main())
