#!/bin/bash
sqlite3 data.db ".mode csv" ".import --skip 1 ./FoodCollectorData.csv data"