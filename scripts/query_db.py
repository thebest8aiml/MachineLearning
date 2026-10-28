import sqlite3
import pandas as pd

conn = sqlite3.connect("data/runs.db")
df = pd.read_sql("SELECT * FROM runs", conn)
conn.close()

print(df)
print(f"\n Total rows: {len(df)}")