import sqlite3
import os

os.makedirs("data", exist_ok= True)

conn = sqlite3.connect("data/runs.db")

with open("data/schema.sql") as f:
    conn.executescript(f.read())

conn.commit()
conn.close()